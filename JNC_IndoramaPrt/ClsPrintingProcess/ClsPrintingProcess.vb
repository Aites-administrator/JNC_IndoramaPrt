Imports Common
Imports Common.ClsFunction

Public Class ClsPrintingProcess
    ' ワークテーブル名
    Private Const WK_TBL As String = "T_SAMPLE"

    ' １つ目のプロセスＩＤ
    Private Shared ryoProcesID_01 As System.Diagnostics.Process
    ' プロセスＩＤ
    Private Shared procesID As System.Diagnostics.Process
    ' 納品書用ワークテーブル
    Private tmpNouhinshoDT As New DataTable
    ' SQLサーバー操作オブジェクト
    Private _SqlServer As ClsSqlServer
    Public Sub New()
    End Sub


    Private ReadOnly Property SqlServer As ClsSqlServer
        Get
            If _SqlServer Is Nothing Then
                _SqlServer = New ClsSqlServer
            End If
            Return _SqlServer
        End Get
    End Property

    ''' <summary>
    ''' ACCESSファイルを開く
    ''' </summary>
    ''' <param name="printPreview">プレビューフラグ</param>
    ''' <param name="strReportName">レポートファイル名</param>
    ''' <param name="prmWaitFlag">待機フラグ</param>
    ''' <returns>
    ''' True :ファイルオープン成功
    ''' False:ファイルオープン失敗
    ''' </returns>
    Public Shared Function AccessRun(printPreview As Integer, strReportName As String, Optional prmWaitFlag As Boolean = False) As Boolean
        Try

            ' Threadオブジェクトを作成する
            Dim MultiProgram_run = New System.Threading.Thread(AddressOf DoRyoSomething01)
            ' １つ目のスレッドを開始する
            MultiProgram_run.Start(New prmReport(printPreview.ToString, strReportName))

            If (prmWaitFlag) Then
                MultiProgram_run.Join()
            End If

        Catch ex As Exception
            Call ComWriteErrLog(ex)
            Return False
        End Try

        Return True

    End Function

    ''' <summary>
    ''' １つ目の印刷スレッド
    ''' </summary>
    ''' <param name="arg"></param>
    Private Shared Sub DoRyoSomething01(arg As Object)

        Try
            Dim prm As prmReport = DirectCast(arg, prmReport)
            Dim myPath As String = System.IO.Path.Combine(My.Application.Info.DirectoryPath, ClsGlobalData.REPORT_FILENAME)

            Dim strPrintPrwview As String
            If (prm.printPreview.Equals("1")) Then
                strPrintPrwview = "1"
            Else
                strPrintPrwview = "0"
            End If

            'ファイルを開く
            ryoProcesID_01 = System.Diagnostics.Process.Start(myPath, " /runtime /cmd " & strPrintPrwview & prm.strReportName)
            If ryoProcesID_01 IsNot Nothing Then
                '終了するまで待機する
                ryoProcesID_01.WaitForExit()
                ryoProcesID_01 = Nothing
            End If
        Catch ex As Exception
            Call ComWriteErrLog(ex, False)   ' Error出力（＋画面表示）
        End Try

    End Sub

    ''' <summary>
    ''' ACCESSファイルを開く
    ''' </summary>
    ''' <param name="printPreview">プレビューフラグ</param>
    ''' <param name="strReportName">レポートファイル名</param>
    ''' <returns>
    ''' True :ファイルオープン成功
    ''' False:ファイルオープン失敗
    ''' </returns>
    Public Shared Function ComAccessRun(printPreview As Integer, strReportName As String) As Boolean
        Try

            ' Threadオブジェクトを作成する
            Dim MultiProgram_run = New System.Threading.Thread(AddressOf DoSomething01)
            ' １つ目のスレッドを開始する
            MultiProgram_run.Start(New prmReport(printPreview.ToString, strReportName))

        Catch ex As Exception
            Call ComWriteErrLog(ex)
            Return False
        End Try

        Return True

    End Function


    ''' <summary>
    ''' 印刷スレッド
    ''' </summary>
    ''' <param name="arg"></param>
    Private Shared Sub DoSomething01(arg As Object)

        Dim prm As prmReport = DirectCast(arg, prmReport)
        Dim myPath As String = System.IO.Path.Combine(My.Application.Info.DirectoryPath, ClsGlobalData.REPORT_FILENAME)

        Dim strPrintPrwview As String
        If (prm.printPreview.Equals("1")) Then
            strPrintPrwview = "1"
        Else
            strPrintPrwview = "0"
        End If

        'ファイルを開く
        procesID = System.Diagnostics.Process.Start(myPath, " /runtime /cmd " & strPrintPrwview & prm.strReportName)
        If procesID IsNot Nothing Then
            '終了するまで待機する
            procesID.WaitForExit()
            procesID = Nothing
        End If

    End Sub

    ''' <summary>
    ''' プロセスの終了
    ''' </summary>
    Public Shared Sub ProcessKill()

        If procesID IsNot Nothing Then
            ' 起動した１つ目のプロセスの終了
            procesID.Kill()
            procesID = Nothing
        End If

    End Sub

    ''' <summary>
    ''' プロセス状態確認
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function ProcessStatus() As Boolean

        Dim ret As Boolean = False

        If procesID IsNot Nothing Then
            ret = True
        End If

        Return ret

    End Function

    Public Shared Sub PrintProcess(prmPreview As Integer, prmInsData As Dictionary(Of String, String), Optional ByRef prmDenpyoNo As String = "", Optional ByRef prmNohinPRTFlg As Integer = 0)
        Dim tmpDt As New DataTable
        Try

            '対象データ取得
            'SqlServer.GetResult(tmpDt, SqlGetPrintData(prmDenpyoNo, prmNohinPRTFlg))

            'ComMessageBox("対象データ取得", "確認", typMsgBox.MSG_NORMAL, typMsgBoxButton.BUTTON_OK)

            'If (tmpDt.Rows.Count = 0) Then
            '  Exit Sub
            'End If
            'ComMessageBox("件数確認", "確認", typMsgBox.MSG_NORMAL, typMsgBoxButton.BUTTON_OK)


            'ワークテーブル作成
            UpdateReportNohinSet(tmpDt, SqlInsert(WK_TBL, prmInsData))

            '印刷処理
            AccessRun(prmPreview, "R_SAMPLE", True)

            'For Each tmpRow As DataRow In tmpDt.Rows
            '  SqlServer.Execute(SqlUpdPrintFlg(tmpRow))
            'Next
        Catch ex As Exception
            ComWriteErrLog(ex, False)
        End Try

    End Sub

    ''' <summary>
    ''' 量目表（セット）ワークテーブル削除と新規作成
    ''' </summary>
    ''' <returns>
    '''  True   -   成功
    '''  False  -   失敗
    ''' </returns>
    Private Shared Function UpdateReportNohinSet(prmDt As DataTable, prmSql As String) As Boolean

        Dim tmpDb As New ClsReport(ClsGlobalData.REPORT_FILENAME)
        Dim dt As DateTime = DateTime.Parse(ComGetProcTime())

        ' 実行
        With tmpDb

            Try
                ' SQL文の作成
                .Execute("DELETE FROM " & WK_TBL)

            Catch ex As Exception
                Call ComWriteErrLog(ex)
                Throw New Exception("量目表（セット）ワークテーブルの削除に失敗しました")

            End Try

            Try
                Dim sql As String
                ' トランザクション開始
                .TrnStart()

                ' データテーブルから追加SQL文を作成
                '   For Each row As DataRow In prmDt.Rows
                sql = prmSql
                If String.IsNullOrWhiteSpace(sql) = False Then

                    .Execute(sql)
                End If
                '        Next

                ' 更新成功
                .TrnCommit()

            Catch ex As Exception
                Call ComWriteErrLog(ex)
                .TrnRollBack()
                Throw New Exception("量目表（セット）ワークテーブルの書き込みに失敗しました")
            End Try

            .Dispose()

        End With

        Return True

    End Function



    Private Function SqlGetPrintData(prmDenpyoNo As String, prmNohinPRTFlg As Integer) As String
        Dim sql As String = String.Empty

        sql &= "SELECT	NohinDay "
        sql &= "	,	DenNO "
        sql &= "	,	GyoNo "
        sql &= "	,	TokuiCD "
        sql &= "	,	TokuiNm "
        sql &= "	,	ShohinCD "
        sql &= "	,	ShohinNM "
        sql &= "	,	Irisu "
        sql &= "	,	Suryo "
        sql &= "	,	UriTanka "
        sql &= "	,	UriageKin "
        sql &= "FROM trn_jisseki "
        sql &= "WHERE 1=1 "
        If prmNohinPRTFlg <> -1 Then
            sql &= "AND NohinPRTFLG = " & prmNohinPRTFlg
        End If
        If Not String.IsNullOrWhiteSpace(prmDenpyoNo) Then
            sql &= "AND DenNO = '" & prmDenpyoNo & "'"
        End If
        sql &= "ORDER BY DenNO,GyoNo "

        Return sql
    End Function



    Private Function SqlUpdPrintFlg(prmDb As DataRow) As String
        Dim sql As String = String.Empty

        sql &= " Update TRN_JISSEKI "
        sql &= " SET NohinPRTFLG = 1"
        sql &= " WHERE DenNo = '" & prmDb.Item("DenNO").ToString & "'"
        sql &= " AND GyoNo = '" & prmDb.Item("GyoNO").ToString & "'"




        Return sql
    End Function
End Class
