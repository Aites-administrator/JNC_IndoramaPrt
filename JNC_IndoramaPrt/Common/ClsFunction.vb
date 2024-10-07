Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports Common.ClsComDatabase



Public Class ClsFunction
#Region " P/Invoke "
    <DllImport("user32.dll")>
    Private Shared Function SetForegroundWindow(hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll")>
    Public Shared Function SendMessage(hWnd As HandleRef, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    ' ウィンドウにメッセージを送信。この関数は、指定したウィンドウのウィンドウプロシージャが処理を終了するまで制御を返さない
    <DllImport("user32.DLL", CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(
    ByVal hWnd As IntPtr,
    ByVal wMsg As Integer,
    ByVal wParam As Integer,
    ByVal lParam As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function FindWindowEx(
    ByVal parentHandle As IntPtr,
    ByVal childAfter As IntPtr,
    ByVal lclassName As String,
    ByVal windowTitle As String) As IntPtr
    End Function

    Private Const EC_LEFTMARGIN As Integer = &H1    ' 左マージンの設定
    Private Const EC_RIGHTMARGIN As Integer = &H2   ' 右マージンの設定
    Private Const EM_SETMARGINS As Integer = &HD3   ' 左右マージンの設定

#End Region

    ' 抽出条件
    Public Enum typExtraction
        ''' <summary>等しい</summary>
        EX_EQ = 0
        ''' <summary>等しくない</summary>
        EX_NEQ
        ''' <summary>より小さい</summary>
        EX_GT
        ''' <summary>より小さいか等しい</summary>
        EX_GTE
        ''' <summary>より大きい</summary>
        EX_LT
        ''' <summary>より大きいか等しい</summary>
        EX_LTE
        ''' <summary>部分一致検索</summary>
        EX_LIK
        ''' <summary>前方一致検索</summary>
        EX_LIKF
        ''' <summary>後方一致検索</summary>
        EX_LIKB
    End Enum

    ' データタイプ
    Public Enum typColumnKind
        CK_Text = 0
        CK_Number
        CK_Date
    End Enum
    ' メッセージボックスタイプ
    Public Enum typMsgBox
        MSG_NORMAL = 0
        MSG_WARNING
        MSG_ERROR
    End Enum

    ' メッセージボックスボタンタイプ
    Public Enum typMsgBoxButton
        BUTTON_OK = 0
        BUTTON_ABORTRETRYIGNORE
        BUTTON_OKCANCEL
        BUTTON_YESNOCANCEL
        BUTTON_YESNO
        BUTTON_RETRYCANCEL
        BUTTON_YESNO_DEFAULT_YES
    End Enum

    ' メッセージボックス戻り値タイプ
    Public Enum typMsgBoxResult
        RESULT_NONE = 0
        RESULT_OK
        RESULT_CANCEL
        RESULT_ABORT
        RESULT_RETRY
        RESULT_IGNORE
        RESULT_YES
        RESULT_NO
    End Enum

    ' 日付フォーマットタイプ
    Public Enum typDateFormat
        FORMAT_DATE = 0
        FORMAT_STRING
    End Enum


    Public Shared DirName As String
    Public Shared LogDir As String
    Public Shared LogFileName As String

    Public Shared ErrorLogPath As String = ReadSettingIniFile("ERROR_LOG_PATH", "VALUE")
    Public Shared SqlLogPath As String = ReadSettingIniFile("SQL_LOG_PATH", "VALUE")

    ''' <summary>
    ''' スレッドの引数
    ''' </summary>
    Class prmReport
        Public ReadOnly printPreview As String      'プレビューフラグ
        Public ReadOnly strReportName As String     'レポートファイル名

        Sub New(preview As String, fileName As String)
            Me.printPreview = preview
            Me.strReportName = fileName
        End Sub
    End Class


    Public Shared Function DateTypeCheck(Day As String)
        Dim CheckResult As Boolean
        Dim yearPart As String = String.Empty
        Dim monthPart As String = String.Empty
        Dim dayPart As String = String.Empty

        Select Case Day.Length
            Case 6
                yearPart = Day.Substring(0, 2)
                monthPart = Day.Substring(2, 2)
                dayPart = Day.Substring(4, 2)
            Case 8
                yearPart = Day.Substring(0, 4)
                monthPart = Day.Substring(4, 2)
                dayPart = Day.Substring(6, 2)
        End Select

        If Not IsValidDate(yearPart, monthPart, dayPart) Then
            CheckResult = False
        Else
            CheckResult = True
        End If

        Return CheckResult
    End Function

    Public Shared Function IsValidDate(year As String, month As String, day As String) As Boolean
        Dim dt As Date
        If year.Length = 2 Then
            year = "20" & year
        End If

        If Date.TryParseExact($"{year}-{month}-{day}", "yyyy-MM-dd", Nothing, Globalization.DateTimeStyles.None, dt) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function DateTxt2DateTxt(Day As String)
        Dim formattedDate As String = String.Empty
        Dim yearPart As String
        Dim monthPart As String
        Dim dayPart As String

        Select Case Day.Length
            Case 6
                yearPart = Day.Substring(0, 2)
                monthPart = Day.Substring(2, 2)
                dayPart = Day.Substring(4, 2)
                formattedDate = $"20{yearPart}/{monthPart}/{dayPart}"
            Case 8
                yearPart = Day.Substring(0, 4)
                monthPart = Day.Substring(4, 2)
                dayPart = Day.Substring(6, 2)
                formattedDate = $"{yearPart}/{monthPart}/{dayPart}"
        End Select

        Return formattedDate
    End Function

    Public Shared Function ComGetProcDay() As String
        Return Date.Parse(ComGetProcTime()).ToString("dd")
    End Function

    Public Shared Function ComGetProcMonth() As String
        Return Date.Parse(ComGetProcTime()).ToString("MM")
    End Function

    Public Shared Function ComGetProcYear() As String
        Return Date.Parse(ComGetProcTime()).ToString("yyyy")
    End Function

    Public Shared Function ComGetProcYearMonth() As String
        Return Date.Parse(ComGetProcTime()).ToString("yyyy/MM")
    End Function

    'エラーログファイルを出力(文字列出力)
    Public Shared Sub ComWriteErrLog(ByVal strMsg As Exception,
                                     Optional ByVal filePath As String = "")
        If filePath = "" Then filePath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "err" & ComGetProcYear() & ComGetProcMonth() & ".log")
        Call ComWriteLog(strMsg.Message, filePath)
    End Sub

    Public Shared Function ComGetProcTime() As String
        Dim dtNow As Date = Date.Now
        Return dtNow
    End Function

    Public Shared Sub ComWriteLog(ByVal desc As String, ByVal filePath As String)
        On Error Resume Next
        Dim strWr As New System.IO.StreamWriter(filePath, True)
        strWr.WriteLine(desc)
        strWr.Close()
        strWr.Dispose()
        strWr = Nothing
        On Error GoTo 0
    End Sub

    Public Shared Sub WriteDetail(dt As DataTable, dgView As DataGridView,
    Optional ByVal columnCtl As Boolean = False)
        dgView.Rows.Clear()
        Try
            For i As Integer = 0 To dt.Rows.Count - 1
                dgView.Rows.Add()
                For j As Integer = 0 To dt.Columns.Count - 1
                    'フィールドの取得

                    'Dim field = dt.Rows(i).Item(j).ToString()

                    Dim field = dt.Rows(i)(j).ToString()

                    If field Is DBNull.Value Then
                        If columnCtl Then
                            dgView.Rows(i).Cells(j + 1).Value = field.ToString()
                        Else
                            dgView.Rows(i).Cells(j).Value = field.ToString()
                        End If
                    Else
                        If columnCtl Then
                            dgView.Rows(i).Cells(j + 1).Value = field
                        Else
                            dgView.Rows(i).Cells(j).Value = field
                        End If
                    End If
                Next
            Next
            '書き込むファイルを開く
        Catch ex As Exception
            OutLog(ex.Message, "明細表示処理")
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Shared Sub OutLog(msg As String, dataKind As String)

        Dim sw As New StreamWriter(DirName & LogDir & LogFileName, True, System.Text.Encoding.GetEncoding("shift_jis"))
        Try
            sw.WriteLine(DateTime.Now.ToString & " " & dataKind & "：" & msg)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "ログ出力処理")
        End Try
        sw.Close()

    End Sub

    Public Shared Function CountChar(ByVal s As String, ByVal c As Char) As String
        Return s.Length - s.Replace(c.ToString(), "").Length
    End Function

    Public Shared Sub ClearTextBox(ByVal hParent As Control)
        For Each cControl As Control In hParent.Controls
            If cControl.HasChildren Then
                ClearTextBox(cControl)
            End If
            If TypeOf cControl Is TextBoxBase Then
                cControl.Text = String.Empty
            End If
        Next cControl
    End Sub

    ' ------------------------------------------------------
    ' ini ファイル読み込み
    ' ------------------------------------------------------
    '指定のIniファイルの指定キーの値を取得(文字列)
    ' AUTO版 GetPrivateProfileString
    Private Declare Auto Function GetPrivateProfileString Lib "kernel32" _
        (ByVal lpAppName As String,
            ByVal lpKeyName As String,
                ByVal lpDefault As String,
                    ByVal lpReturnedString As StringBuilder,
                        ByVal nSize As Integer,
                            ByVal lpFileName As String) As Integer

    'プロファイル文字列書込み  
    Private Declare Function WritePrivateProfileString Lib "kernel32" _
        Alias "WritePrivateProfileStringA" _
        (ByVal lpApplicationName As String, ByVal lpKeyName As String,
         ByVal lpString As String, ByVal lpFileName As String) As Long

    ''' <summary>
    ''' iniファイルから取得する
    ''' </summary>
    ''' <param name="lpAppName"></param>
    ''' <param name="lpKeyName"></param>
    ''' <param name="strPath"></param>
    ''' <returns></returns>
    Public Shared Function GetIniString(ByVal lpAppName As String, ByVal lpKeyName As String, strPath As String) As String
        Dim sb As New StringBuilder(256)
        Try
            Call GetPrivateProfileString(lpAppName, lpKeyName, "", sb, Convert.ToUInt32(sb.Capacity), strPath)
            Return sb.ToString
        Catch ex As Exception
            Return sb.ToString
        End Try
    End Function

    ''' <summary>
    ''' iniファイルに書き込む
    ''' </summary>
    ''' <param name="lpAppName"></param>
    ''' <param name="lpKeyName"></param>
    ''' <param name="lpValue"></param>
    ''' <param name="strPath"></param>
    ''' <returns></returns>
    Public Shared Function PutIniString(ByVal lpAppName As String, lpKeyName As String, ByVal lpValue As String, ByVal strPath As String) As Boolean
        Try
            Dim result As Long = WritePrivateProfileString(lpAppName, lpKeyName, lpValue, strPath)
            Return result <> 0
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Shared Sub OpenForm(StrKey As String)
        Dim exePath As String = ReadMenuIniFile(StrKey)
        Dim psi As New ProcessStartInfo(exePath)
        System.Diagnostics.Process.Start(psi)
    End Sub
    Public Shared Function ReadMenuIniFile(strKey As String)
        Dim strPath As String = "C:\COUNTING_DX\INI\menu.ini"
        Dim exeName As String = GetIniString(strKey, "EXE", strPath)
        Return exeName
    End Function

    Public Shared Function ReadSettingIniFile(strKey As String, keyName As String)
        Dim strPath As String = "C:\COUNTING_DX\INI\setting.ini"
        Dim stringValue As String = GetIniString(strKey, keyName, strPath)
        Return stringValue
    End Function

    Public Shared Function ReadConnectIniFile(strKey As String, keyName As String)
        Dim strPath As String = "C:\COUNTING_DX\INI\connect.ini"
        Dim stringValue As String = GetIniString(strKey, keyName, strPath)
        Return stringValue
    End Function

    'エラーログファイルを出力
    Public Shared Sub ComWriteErrLog(ByVal prmSource As String,
                                   ByVal prmTargetSite As String,
                                   ByVal prmMessage As String)

        Dim tmpExeFileName As String = System.IO.Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)

        Call ComWriteLog("【Date】" & DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") _
                       & "【Exe】" & tmpExeFileName _
                       & "【Source】" & prmSource _
                       & "【Method】" & prmTargetSite _
                       & "【Description】" & prmMessage, ErrorLogPath)
    End Sub
    ''' <summary>
    ''' SQL実行ログ保存
    ''' </summary>
    ''' <param name="prmSql">SQL文</param>
    Public Shared Sub WriteExecuteLog(ByVal prmSource As String, ByVal prmTargetSite As String, prmSql As String)
        Try
            Dim tmpProcTime As String = ComGetProcTime()
            Dim tmpExeFileName As String = System.IO.Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)
            Dim logText As String = "【Date】" & tmpProcTime _
                                  & "【PC】" & System.Net.Dns.GetHostName() _
                                  & "【Exe】" & tmpExeFileName _
                                  & "【Source】" & prmSource _
                                  & "【Method】" & prmTargetSite _
                                  & "【Sql】" & prmSql
            ComWriteLog(logText, SqlLogPath)
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' 文字列が数値かどうか判定
    ''' </summary>
    ''' <param name="str">数値かどうか判定する文字列</param>
    ''' <returns>
    ''' True :数値である
    ''' False:数値でない
    ''' </returns>
    Public Shared Function ComChkNumeric(str As String) As Boolean

        Dim flag As Boolean = True
        Dim c As Char

        For Each c In str
            '数字以外の文字が含まれているか調べる
            If c < "0"c OrElse "9"c < c Then
                flag = False
                Exit For
            End If
        Next

        Return flag

    End Function

    ''' <summary>
    ''' 文字列からInteger型にデータ変換
    ''' </summary>
    ''' <param name="strVal">元の文字列</param>
    ''' <returns>変換後データ</returns>
    Public Shared Function StringToInt(ByVal strVal As String) As Integer
        Try
            Dim intVal As Integer
            ' TryParseでInteger型に変換
            Dim result As Boolean = Int32.TryParse(strVal, intVal)

            ' ParseでInteger型に変換
            Return intVal

        Catch ex As Exception
            ' 例外発生の場合、「0」で返す
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' 左のマージンを設定する
    ''' </summary>
    ''' <param name="prmHandle">コントロールハンドル</param>
    ''' <param name="prmMargin">マージン幅</param>
    Public Shared Sub SetLeftMargin(prmHandle As IntPtr,
                                  prmMargin As Integer)

        SendMessage(prmHandle, EM_SETMARGINS, EC_LEFTMARGIN, prmMargin)

    End Sub

    ''' <summary>
    ''' メッセージボックスを表示
    ''' </summary>
    ''' <param name="msg">表示するメッセージ</param>
    ''' <param name="title">表示するタイトル</param>
    ''' <param name="type">表示する種類（通常・警告・異常）</param>
    ''' <param name="typeButton">表示するボタン</param>
    ''' <returns>押下したボタン情報</returns>
    Public Shared Function ComMessageBox(msg As String _
                                       , title As String _
                                       , type As typMsgBox _
                                       , Optional typeButton As typMsgBoxButton = typMsgBoxButton.BUTTON_OK _
                                       , Optional prmDefaultButton As MessageBoxDefaultButton? = Nothing) As typMsgBoxResult

        Dim typMsgBtn As MessageBoxButtons
        Dim numButton As Integer

        'メッセージボックスに表示するボタンを定義する定数を指定
        Select Case (typeButton)
            Case typMsgBoxButton.BUTTON_OK
                typMsgBtn = MessageBoxButtons.OK
                numButton = 1
            Case typMsgBoxButton.BUTTON_ABORTRETRYIGNORE
                typMsgBtn = MessageBoxButtons.AbortRetryIgnore
                numButton = 3
            Case typMsgBoxButton.BUTTON_OKCANCEL
                typMsgBtn = MessageBoxButtons.OKCancel
                numButton = 2
            Case typMsgBoxButton.BUTTON_YESNOCANCEL
                typMsgBtn = MessageBoxButtons.YesNoCancel
                numButton = 3
            Case typMsgBoxButton.BUTTON_YESNO
                typMsgBtn = MessageBoxButtons.YesNo
                numButton = 2
            Case typMsgBoxButton.BUTTON_YESNO_DEFAULT_YES
                typMsgBtn = MessageBoxButtons.YesNo
                numButton = 1
            Case typMsgBoxButton.BUTTON_RETRYCANCEL
                typMsgBtn = MessageBoxButtons.RetryCancel
                numButton = 2
        End Select

        '始めにフォーカスのあるボタンの設定
        Dim defaultFocus As MessageBoxDefaultButton
        If prmDefaultButton IsNot Nothing Then
            defaultFocus = prmDefaultButton
        Else
            Select Case (numButton)
                Case 1
                    defaultFocus = MessageBoxDefaultButton.Button1
                Case 2
                    defaultFocus = MessageBoxDefaultButton.Button2
                Case 3
                    defaultFocus = MessageBoxDefaultButton.Button3
            End Select
        End If

        Dim result As DialogResult

        'メッセージボックスを表示
        Select Case (type)
      '通常
            Case typMsgBox.MSG_NORMAL
                result = MessageBox.Show(msg,
                                 title,
                                 typMsgBtn,
                                 MessageBoxIcon.Information,
                                 defaultFocus)
      '警告
            Case typMsgBox.MSG_WARNING
                result = MessageBox.Show(msg,
                                 title,
                                 typMsgBtn,
                                 MessageBoxIcon.Warning,
                                 defaultFocus)
      '異常　
            Case typMsgBox.MSG_ERROR
                result = MessageBox.Show(msg,
                                 title,
                                 typMsgBtn,
                                 MessageBoxIcon.Error,
                                 defaultFocus)
        End Select

        'メッセージボックスで押したボタンの戻り値を設定
        Dim typMsgResult As typMsgBoxResult = typMsgBoxResult.RESULT_NONE
        Select Case result
            Case DialogResult.None
                typMsgResult = typMsgBoxResult.RESULT_NONE
            Case DialogResult.OK
                typMsgResult = typMsgBoxResult.RESULT_OK
            Case DialogResult.Cancel
                typMsgResult = typMsgBoxResult.RESULT_CANCEL
            Case DialogResult.Abort
                typMsgResult = typMsgBoxResult.RESULT_ABORT
            Case DialogResult.Retry
                typMsgResult = typMsgBoxResult.RESULT_RETRY
            Case DialogResult.Ignore
                typMsgResult = typMsgBoxResult.RESULT_IGNORE
            Case DialogResult.Yes
                typMsgResult = typMsgBoxResult.RESULT_YES
            Case DialogResult.No
                typMsgResult = typMsgBoxResult.RESULT_NO
        End Select

        Return typMsgResult

    End Function


    Public Shared Function DateFormatChange(prmMode As Integer, prmDate As String) As String
        Dim rtn As String = ""

        Select Case prmMode
            Case typDateFormat.FORMAT_DATE
                rtn = prmDate.Substring(0, 4) + "/" + prmDate.Substring(4, 2) + "/" + prmDate.Substring(6, 2)
            Case typDateFormat.FORMAT_STRING
                rtn = prmDate.Substring(0, 4) + prmDate.Substring(5, 2) + prmDate.Substring(8, 2)
            Case Else
                rtn = prmDate
        End Select

        Return rtn
    End Function



    ''' <summary>
    ''' 読み取り専用の属性を解除するメソッド
    ''' </summary>
    ''' <param name="strFileName">ファイル名</param>
    Public Shared Sub ReleaseReadOnly(strFileName As String)

        Try
            '対象ファイルの属性をオブジェクト化
            Dim fas As FileAttributes = File.GetAttributes(strFileName)
            ' 読み取り専用かどうか確認
            If (fas And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
                ' ファイル属性から読み取り専用を削除
                fas = fas And Not FileAttributes.ReadOnly

                ' ファイル属性を設定
                File.SetAttributes(strFileName, fas)
            End If

        Catch ex As Exception
            Throw
        End Try

    End Sub

    Public Shared Function CreateInsSql(prmTableName As String, prmKeyVal As Dictionary(Of String, String)) As String
        Dim sql As String = String.Empty
        Dim tmpDst As String = String.Empty
        Dim tmpVal As String = String.Empty

        For Each tmpKey As String In prmKeyVal.Keys
            tmpDst = tmpDst & tmpKey & " ,"
            tmpVal = tmpVal & "'" & prmKeyVal(tmpKey) & "' ,"
        Next
        tmpDst = tmpDst.Substring(0, tmpDst.Length - 1)
        tmpVal = tmpVal.Substring(0, tmpVal.Length - 1)

        sql &= "INSERT INTO " & prmTableName & "(" & tmpDst & ")"
        sql &= "          VALUES(" & tmpVal & ")"

        Console.WriteLine(sql)

        Return sql
    End Function

    Public Shared Function CreateUpdSql(prmTableName As String, prmKeyVal As Dictionary(Of String, String)) As String
        Dim sql As String = String.Empty
        Dim tmpDst As String = String.Empty
        Dim tmpVal As String = String.Empty

        For Each tmpKey As String In prmKeyVal.Keys
            tmpDst = tmpDst & tmpKey & " = '" & prmKeyVal(tmpKey) & "',"
        Next

        tmpDst = tmpDst.Substring(0, tmpDst.Length - 1)

        sql &= "UPDATE " & prmTableName
        sql &= " SET " & tmpDst

        Console.WriteLine(sql)

        Return sql
    End Function

    Public Shared Function ComSearchType2Text(SearchType As typExtraction) As String
        Dim Extraction As String = String.Empty

        Select Case SearchType
            Case typExtraction.EX_EQ
                Extraction = " = "
            Case typExtraction.EX_GT
                Extraction = " > "
            Case typExtraction.EX_GTE
                Extraction = " >= "
            Case typExtraction.EX_LT
                Extraction = " < "
            Case typExtraction.EX_LTE
                Extraction = " <= "
            Case typExtraction.EX_NEQ
                Extraction = " <> "
            Case typExtraction.EX_LIK _
      , typExtraction.EX_LIKF _
      , typExtraction.EX_LIKB
                Extraction = " LIKE "
        End Select

        Return Extraction
    End Function


    Public Shared Function ComGetLiteralChar(DataType As typColumnKind _
                                  , Provider As TypProvider) As String
        Dim LiteralChar As String = String.Empty

        Select Case DataType
            Case typColumnKind.CK_Date
                ' 日付形式
                Select Case Provider
                    Case TypProvider.sqlServer
                        LiteralChar = "'"
                    Case TypProvider.Accdb
                        LiteralChar = "#"
                    Case TypProvider.Mdb
                        LiteralChar = "#"
                End Select
            Case typColumnKind.CK_Number
                ' 数値
                LiteralChar = ""
            Case typColumnKind.CK_Text
                ' 文字列
                LiteralChar = "'"
        End Select

        Return LiteralChar
    End Function

    ''' <summary>
    ''' INSERTのSQL文作成
    ''' </summary>
    ''' <param name="prmtable">テーブル名</param>
    ''' <param name="prmKeyVal">編集コントロールの値を保持する連想配列</param>
    ''' <returns></returns>

    Public Shared Function SqlInsert(prmtable As String,
                                   prmKeyVal As Dictionary(Of String, String)) As String

        Dim sql As String = String.Empty
        Dim tmpDst As String = String.Empty
        Dim tmpVal As String = String.Empty

        For Each tmpKey As String In prmKeyVal.Keys
            tmpDst = tmpDst & tmpKey & " ,"
            tmpVal = tmpVal & prmKeyVal(tmpKey) & " ,"
        Next
        tmpDst = tmpDst.Substring(0, tmpDst.Length - 1)
        tmpVal = tmpVal.Substring(0, tmpVal.Length - 1)

        sql &= "INSERT INTO " & prmtable
        sql &= "           (" & tmpDst & ")"
        sql &= "    VALUES (" & tmpVal & ")"

        Return sql

    End Function
End Class
