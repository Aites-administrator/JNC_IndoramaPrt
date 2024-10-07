Imports System.Net
Imports Common.ClsFunction

Public Class ClsComDatabase
    Implements IDisposable

    '-------------------------------------
    '       パブリックEnum
    '-------------------------------------
    Public Enum TypProvider
        ''' <summary>
        ''' SQL-Server
        ''' </summary>
        sqlServer = 0
        ''' <summary>
        ''' Access(MDB)
        ''' </summary>
        Mdb
        ''' <summary>
        ''' Access(Accdb)
        ''' </summary>
        Accdb
    End Enum

#Region "メンバ"

#Region "プライベート"
    Private _ObjCon As OleDb.OleDbConnection  ' ADOコネクション
    Private _ObjCmd As OleDb.OleDbCommand
    Private _ObjDa As OleDb.OleDbDataAdapter
    Private _ObjTran As OleDb.OleDbTransaction

    Private _Provider As TypProvider
    Private _DataSource As String
    Private _DefaultDatabase As String
    Private _UserId As String
    Private _Password As String
    Private _TrnRunnig As Boolean              ' トランザクション状態フラグ（True：トランザクション開始中）
    Private _QueryTimeOut As Integer
    Private _SqlHistory As String
#End Region

#End Region

#Region "プロパティー"

#Region "プライベート"
    Private ReadOnly Property DbConnection() As OleDb.OleDbConnection
        Get
            If IsConnected() = False Then DbConnect()
            Return _ObjCon
        End Get
    End Property
#End Region

#Region "パブリック"
    ' プロバイダー
    Public Property Provider As TypProvider
        Get
            Return _Provider
        End Get
        Set(value As TypProvider)
            _Provider = value
        End Set
    End Property

    ' データベースサーバー
    Public WriteOnly Property DataSource As String
        Set(value As String)
            _DataSource = value
        End Set
    End Property

    ' データベース
    Public WriteOnly Property DefaultDatabase As String
        Set(value As String)
            _DefaultDatabase = value
        End Set
    End Property

    ' 接続ID
    Public WriteOnly Property UserId As String
        Set(value As String)
            _UserId = value
        End Set
    End Property

    ' パスワード
    Public WriteOnly Property Password As String
        Set(value As String)
            _Password = value
        End Set
    End Property

    ''' <summary>
    ''' クエリータイムアウト時間
    ''' </summary>
    ''' <returns></returns>
    Public Property QueryTimeOut() As Integer
        Get
            Return _QueryTimeOut
        End Get
        Set(value As Integer)
            _QueryTimeOut = value
        End Set
    End Property


#End Region

#End Region

#Region "コンストラクタ"
    Public Sub New()
        ' トランザクションフラグ解除
        _TrnRunnig = False

        ' クエリータイムアウトデフォルト
        _QueryTimeOut = 180

        ' SQL実行履歴初期化
        _SqlHistory = String.Empty

    End Sub

#End Region

#Region "メソッド"

#Region "プライベート"
    ' データベース接続確認
    Private Function IsConnected() As Boolean
        Dim bConnected As Boolean

        ' 存在確認
        bConnected = Not (_ObjCon Is Nothing)
        ' 接続確認
        If bConnected Then bConnected = (_ObjCon.State = ConnectionState.Open)
        Return bConnected
    End Function

    ' プロバイダ文字列取得
    Private Function GetProviderName() As String
        Dim ProviderName As String = String.Empty

        Select Case _Provider
            Case TypProvider.sqlServer
                ProviderName = "SQLOLEDB"
            Case TypProvider.Mdb
                ProviderName = "Microsoft.Jet.OLEDB.4.0"
            Case TypProvider.Accdb
                ProviderName = "Microsoft.ACE.OLEDB.12.0"
            Case Else
                Call Err.Raise(9999, "GetProviderName", "適切なプロバイダーが設定されていません")
        End Select

        Return ProviderName
    End Function

    Private Sub ReleaseTrnRb()
        If _ObjTran IsNot Nothing Then
            _ObjTran.Rollback()
            _ObjTran.Dispose()
            _ObjTran = Nothing
            _SqlHistory = String.Empty
        End If
    End Sub

    Private Sub ReleaseTrnCmt()
        If _ObjTran IsNot Nothing Then
            _ObjTran.Commit()
            _ObjTran.Dispose()
            _ObjTran = Nothing
            ' SQLSERVERのみログを残す
            If (_Provider = TypProvider.sqlServer) Then
                'Call WriteExecuteLog(_SqlHistory)
            End If
            _SqlHistory = String.Empty
        End If
    End Sub
#End Region

#Region "パブリック"

    ' DB接続
    Public Sub DbConnect()
        Dim ConnectionString As String

        ' 接続中なら切断
        If IsConnected() Then
            Call DbDisconnect()
            _ObjCon = Nothing
        End If

        ' 接続オブジェクト初期化
        _ObjCon = New OleDb.OleDbConnection

        ' 接続文字列作成
        ConnectionString = ""
        ConnectionString = ConnectionString & "Provider=" & GetProviderName() & ";"
        ConnectionString = ConnectionString & "Data Source=" & _DataSource & ";"
        'ConnectionString = ConnectionString & "Initial Catalog=" & _DefaultDatabase & ";"
        'ConnectionString = ConnectionString & "Trusted_Connection=Yes"

        '' 以下、SQL-Server接続時のみ設定
        If _Provider = TypProvider.sqlServer Then
            ConnectionString = ConnectionString & "Initial Catalog=" & _DefaultDatabase & ";"
            ConnectionString = ConnectionString & "User ID=" & _UserId & ";"
            ConnectionString = ConnectionString & "Password=" & _Password & ";"
        End If

        _ObjCon.ConnectionString = ConnectionString

        ' 接続
        _ObjCon.Open()

    End Sub

    ' DB切断
    Public Sub DbDisconnect()
        Try
            If IsConnected() Then
                _ObjCon.Close()
                _SqlHistory = String.Empty
            End If
        Catch ex As Exception
            'Call ComWriteErrLog(ex)
        End Try
    End Sub

    Public Function GetResult(ByRef dt As DataTable _
                                  , sql As String, Optional prmLog As Boolean = False)

        '指定したSQL文が空白の場合、クエリーを実行しない
        If String.IsNullOrWhiteSpace(sql) Then
            Return dt
            Exit Function
        End If

        Try
            _ObjCmd = DbConnection.CreateCommand
            With _ObjCmd
                .CommandTimeout = _QueryTimeOut
                .CommandText = sql
                '.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED " & sql
                If _TrnRunnig Then
                    .Transaction = _ObjTran
                End If
            End With

            If (prmLog = False) Then
                If (_Provider = TypProvider.sqlServer) Then
                    'Call WriteExecuteLog(sql)
                End If
            End If

            '---
            _ObjDa = New OleDb.OleDbDataAdapter With {
                .SelectCommand = _ObjCmd
            }
            If dt Is Nothing Then
                dt = New DataTable
            Else
                dt.Clear()
            End If

            _ObjDa.Fill(dt)

            Return dt
            '---
        Catch ex As Exception
            Call ComWriteErrLog(ex)
            Throw New Exception("SQL実行時エラー:" & sql)
        Finally
            If _ObjDa IsNot Nothing Then
                _ObjDa.Dispose()
                _ObjDa = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' 更新系クエリー実行
    ''' </summary>
    ''' <param name="sql">実行するSQL文</param>
    ''' <returns></returns>
    Public Function Execute(sql As String) As Integer

        Dim AffectedCount As Integer

        Try
            'トランザクション開始
            If _ObjTran Is Nothing Then _ObjTran = DbConnection.BeginTransaction()

            _ObjCmd = DbConnection.CreateCommand()

            With _ObjCmd
                .CommandTimeout = _QueryTimeOut
                .Transaction = _ObjTran
                .CommandText = sql
                _SqlHistory &= sql & ";;;"
                AffectedCount = .ExecuteNonQuery()
            End With

            If _TrnRunnig = False Then
                ReleaseTrnCmt()
            End If

        Catch ex As Exception

            'Call ComWriteErrLog(ex)   ' Error出力

            If _TrnRunnig = False Then
                ReleaseTrnRb()
            End If

            Throw New Exception("SQL実行時エラー:" & sql)
        End Try

        Return AffectedCount
    End Function

    ' トランザクション開始
    Public Sub TrnStart()
        If _TrnRunnig Then
            Throw New Exception("トランザクションは既に開始されています。")
        Else
            _ObjTran = DbConnection.BeginTransaction()
            _TrnRunnig = True
        End If
    End Sub

    ' トランザクションコミット
    Public Sub TrnCommit()
        If _TrnRunnig Then
            ReleaseTrnCmt()
            _TrnRunnig = False
        End If
    End Sub

    ' トランザクションロールバック
    Public Sub TrnRollBack()
        If _TrnRunnig Then
            ReleaseTrnRb()
            _TrnRunnig = False
        End If
    End Sub
#End Region

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 重複する呼び出しを検出するには

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                DbDisconnect()

                If _ObjCmd IsNot Nothing Then
                    _ObjCmd.Dispose()
                    _ObjCmd = Nothing
                End If

                If _ObjDa IsNot Nothing Then
                    _ObjDa.Dispose()
                    _ObjDa = Nothing
                End If

                If _ObjTran IsNot Nothing Then
                    _ObjTran.Dispose()
                    _ObjTran = Nothing
                End If

                If _ObjCon IsNot Nothing Then
                    _ObjCon.Dispose()
                    _ObjCon = Nothing
                End If

            End If
        End If
        disposedValue = True
    End Sub

    ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        DbConnection.Close()
    End Sub
#End Region
End Class
