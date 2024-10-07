Imports System.IO.Ports
Imports ClsPrintingProcess.ClsPrintingProcess
Imports Common
Imports Common.ClsFunction
Imports Common.ClsGlobalData

Public Class Frm_IndoramaPrt
    Private SeriPort As New SerialPort
    Private STX As String = ChrW(2)
    Private ETX As String = ChrW(3)
    Private ENQ As String = ChrW(5)
    Private ACK As String = ChrW(6)
    Private NAK As String = ChrW(&H15)
    Private EOT As String = ChrW(4)
    Private IzData As String = String.Empty
    Private Jyuryo As String
    Private InAllData As String = ""
    Private InsFlg As Boolean = True

    ''' <summary>
    ''' マスタMDBファイルパス
    ''' </summary>
    Private Const REC_NO_FLG As Boolean = True

    ''' <summary>
    ''' マスタMDBファイルパス
    ''' </summary>
    Private Const MST_ACCESS_PATH As String = "D:\JNC\DAT\JISSEKI.mdb"

    Private InjiNenGappi As String = ""

#Region "イベントプロシージャ"
    ''' <summary>
    ''' 数値のみコントロールイベント
    ''' </summary>
    Private Sub TxtSeisanDate_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtSeisanDate.KeyPress, TxtQuantity.KeyPress, TxtFixNo.KeyPress, TxtRecNo.KeyPress, TxtBag.KeyPress, TxtPrtCnt.KeyPress
        If (e.KeyChar < "0"c OrElse "9"c < e.KeyChar) AndAlso
            e.KeyChar <> ControlChars.Back Then
            e.Handled = True
        End If
        If e.KeyChar = ChrW(Windows.Forms.Keys.Enter) Then
            e.Handled = True
        End If
    End Sub

    ''' <summary>
    ''' Enterキーコントロールイベント
    ''' </summary>
    Private Sub TxtItemName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtItemName.KeyPress, TxtMrms.KeyPress, TxtLotNo.KeyPress

        If e.KeyChar = ChrW(Windows.Forms.Keys.Enter) Then
            e.Handled = True
        End If
    End Sub

    ''' <summary>
    ''' チェックディジット計算コントロールイベント
    ''' </summary>
    Private Sub CheckDigitContorol_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TxtSeisanDate.Validating, TxtFixNo.Validating, TxtRecNo.Validating
        Try
            TxtRecNo.Text = TxtRecNo.Text.PadLeft(6, "0"c)
            'チェックディジット計算
            TxtCheckDigit.Text = AddCheckDigit(TxtFixNo.Text & TxtSeisanDate.Text & TxtRecNo.Text)

        Catch ex As Exception
            ComWriteErrLog(ex)
            ComMessageBox(ex.Message, "チェックディジット計算処理", typMsgBox.MSG_WARNING, typMsgBoxButton.BUTTON_OK)
        End Try
    End Sub

    ''' <summary>
    ''' ロット番号、BAG番号変更時イベント
    ''' </summary>
    Private Sub TxtLotNo_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TxtLotNo.Validating, TxtBag.Validating
        Dim tmpDt As New DataTable
        InsFlg = True

        TxtBag.Text = TxtBag.Text.PadLeft(3, "0"c)

        TxtLotNoEdaNo.Text = TxtLotNo.Text & "-" & TxtBag.Text

        '再発行データ取得処理
        tmpDt = GetTrnData()


        If tmpDt.Rows.Count = 1 Then

            TxtSeisanDate.Text = tmpDt.Rows(0).Item("PRODUCT_DATE").ToString
            TxtItemName.Text = tmpDt.Rows(0).Item("ITEM").ToString
            TxtMrms.Text = tmpDt.Rows(0).Item("MRMS").ToString
            TxtQuantity.Text = tmpDt.Rows(0).Item("QUANTITY").ToString
            TxtLotNo.Text = tmpDt.Rows(0).Item("LOT").ToString
            TxtBag.Text = tmpDt.Rows(0).Item("BAG").ToString
            TxtFixNo.Text = tmpDt.Rows(0).Item("FIX_NO").ToString
            TxtRecNo.Text = tmpDt.Rows(0).Item("REC_NO").ToString

            InsFlg = False
        End If

        If RdoAuto.Checked Then
            If tmpDt.Rows.Count = 0 Then
                TxtBag.Text = GetRecNo().PadLeft(3, "0"c)
                TxtRecNo.Text = GetRecNo(REC_NO_FLG).PadLeft(6, "0"c)
            End If
        End If

        'チェックディジット計算
        TxtCheckDigit.Text = AddCheckDigit(TxtFixNo.Text & TxtSeisanDate.Text & TxtRecNo.Text.PadLeft(6, "0"c))

    End Sub



    ''' <summary>
    ''' 印刷ボタン
    ''' </summary>
    Private Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click

        Try
            '自動データ登録処理
            If RdoAuto.Checked Then
                If InsFlg Then
                    InsertAutoData()
                End If
            End If

            ' 印刷処理
            Print()

            '自動データ登録処理
            If RdoAuto.Checked Then
                TxtRecNo.Text = GetRecNo(REC_NO_FLG).PadLeft(6, "0"c)
                TxtBag.Text = GetRecNo().PadLeft(3, "0"c)
            End If
        Catch ex As Exception
            ComWriteErrLog(ex)
            ComMessageBox(ex.Message, "印刷処理", typMsgBox.MSG_WARNING, typMsgBoxButton.BUTTON_OK)
        End Try
    End Sub


    ''' <summary>
    ''' シリアルポート受信取得イベント
    ''' </summary>
    Private Sub SerialPort_DataReceived(sender As Object, e As SerialDataReceivedEventArgs)
        ''読み込んだ値を操作する。
        Dim sp As SerialPort = CType(sender, SerialPort)
        Dim indata As String = String.Empty
        Dim DataLen As Integer = 0
        Try
            indata = sp.ReadExisting()
            InAllData += indata
            ComWriteLog("すぐ！" & InAllData, "test.log")

            DataLen = InAllData.Length

            ' ETXフラグがたっていたら
            If InAllData.Substring(0, 1) <> STX Then
                ComWriteLog("STXではない時：" & InAllData, "..\file.log")
                InAllData = ""
                Exit Sub
            End If

            If Strings.Right(InAllData, 1) = ENQ Then
                InAllData = indata
            End If

            If Strings.Right(InAllData, 1) = ACK Then
                InAllData = indata
            End If

            If Strings.Right(InAllData, 1) = NAK Then
                InAllData = indata
            End If

            If Strings.Right(InAllData, 1) = EOT Then
                InAllData = indata
            End If

            If Strings.Right(InAllData, 1) = ETX Then
                ComWriteLog(Strings.Right(InAllData, 1) & "Strings.Right(InAllData, 1) = ETX", "test.log")
                InAllData = ""
                Exit Sub
            End If


            ComWriteLog("あと！" & InAllData, "test.log")

            ComWriteLog(DataLen, "変数確認.log")
            If DataLen <> 31 And DataLen <> 32 Then
                ComWriteLog(DataLen & " " & "Not (DataLen = 32 And DataLen = 33)", "変数確認.log")
                Exit Sub
            End If

            'ComWriteLog(InAllData, "変数確認.log")
            'ComWriteLog(InAllData.Substring(5, 1) & " " & InAllData.Substring(5, 1), "変数確認.log")
            'If InAllData.Substring(5, 1) <> 6 And InAllData.Substring(5, 1) <> 4 Then
            '    ComWriteLog("Not (InAllData.Substring(6, 1) = 6 And InAllData.Substring(6, 1) = 4)", "変数確認.log")
            '    Exit Sub
            'End If

            ComWriteLog(InAllData.Substring(6, 1) & " " & InAllData.Substring(6, 1), "変数確認.log")
            If InAllData.Substring(6, 1) <> 0 Then
                ComWriteLog("Not InAllData.Substring(7, 1) = 0", "変数確認.log")
                Exit Sub
            End If



            Dim Bunbo As Integer = 0
            Dim Juryo As String = InAllData.Substring(15, 6)
            Dim Fugou As String = InAllData.Substring(21, 1)
            Dim sisu As String = InAllData.Substring(22, 1)

            If Fugou = "0" Then
                Bunbo = 1000
            ElseIf Fugou = "F" Then
                Bunbo = 1
            End If
            '
            If sisu <> 0 Then
                Bunbo = Bunbo * (10 ^ sisu)
            End If

            Jyuryo = Val(Juryo) / Bunbo
            Jyuryo = Fix(Jyuryo)


        Catch ex As Exception
            ComWriteErrLog(ex)
        End Try

    End Sub

    Private Sub RdoAuto_CheckedChanged(sender As Object, e As EventArgs) Handles RdoAuto.CheckedChanged, RdoManual.CheckedChanged
        Try
            Select Case True
                Case RdoAuto.Checked
                    TxtQuantity.ReadOnly = False
                    SerialPortsGet()
                    TxtRecNo.ReadOnly = True
                    TxtRecNo.Text = GetRecNo(REC_NO_FLG).PadLeft(6, "0"c)
                    If Not String.IsNullOrWhiteSpace(TxtLotNo.Text) Then
                        TxtBag.Text = GetRecNo().PadLeft(3, "0"c)
                    End If
                Case RdoManual.Checked
                    'TxtQuantity.ReadOnly = False
                    If SeriPort.IsOpen = True Then   'ポートオープン済み
                        SeriPort.Close()                         'ポートクローズ
                    End If
                    TxtRecNo.ReadOnly = False
                    TxtRecNo.Text = ""
            End Select

        Catch ex As Exception
            ComWriteErrLog(ex)
            ComMessageBox(ex.Message, "接続確認", typMsgBox.MSG_WARNING, typMsgBoxButton.BUTTON_OK)
        End Try
    End Sub

    Private Sub TxtPrtCnt_Validated(sender As Object, e As EventArgs) Handles TxtPrtCnt.Validated
        If Not IsNumeric(TxtPrtCnt.Text) Then
            TxtPrtCnt.Text = 2
        End If

        If TxtPrtCnt.Text = 0 Then
            TxtPrtCnt.Text = 2
        End If
    End Sub

    Private Sub TxtQuantity_Validated(sender As Object, e As EventArgs) Handles TxtQuantity.Validated
        '仮納品用
        If RdoAuto.Checked Then
            TxtQuantity.Text = Jyuryo
        End If
    End Sub

    Private Sub Frm_IndoramaPrt_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim forward As Boolean

        If e.KeyCode = Keys.Enter Then
            ' Shiftキーが押されているかの判定
            forward = e.Modifiers <> Keys.Shift
            ' タブオーダー順で次のコントロールにフォーカスを移動
            Me.SelectNextControl(Me.ActiveControl, forward, True, True, True)
            e.Handled = True
        End If

    End Sub
#End Region

    'チェックディジット計算処理
    Private Function AddCheckDigit(prmText As String) As String
        Dim len As Integer = 0
        Dim ret As String = prmText
        Dim NoEvenSum As Integer = 0
        Dim EvenSum As Integer = 0
        Dim sum As Integer = 0
        len = prmText.Length


        If String.IsNullOrWhiteSpace(prmText) Then
            Return ret
        End If

        If prmText.Length = 17 Then

            For Dataindex = 1 To len
                If Not (Dataindex Mod 2 = 0) Then
                    EvenSum = EvenSum + (Mid(prmText, Dataindex, 1))
                Else
                    NoEvenSum = NoEvenSum + (Mid(prmText, Dataindex, 1))
                End If
            Next
        End If

        EvenSum = EvenSum * 3
        NoEvenSum = NoEvenSum * 1

        sum = EvenSum + NoEvenSum

        Dim NotMuch As Integer = 0

        NotMuch = sum Mod 10

        '// チェックディジット付与
        If NotMuch = 0 Then
            ret = 0
        Else
            ret = (10 - NotMuch)
        End If


        Return ret
    End Function

    ''' <summary>
    ''' シリアル接続処理
    ''' </summary>
    Private Sub SerialPortsGet()
        Dim SerialPort As New SerialPort

        '受信メッセージ
        Dim ReceiveMsg As String = String.Empty

        Try

            If Not SeriPort.IsOpen Then
                With SeriPort
                    .BaudRate = 9600
                    .DataBits = 7
                    .StopBits = StopBits.Two
                    .Parity = Parity.Even
                    .PortName = "COM1"
                    .ReadTimeout = 10000
                    .WriteTimeout = 10000
                End With


                AddHandler SeriPort.DataReceived, AddressOf SerialPort_DataReceived
                SeriPort.Open()
                ComWriteLog("Port接続しました。", "..\file.log")

            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        Finally
        End Try

    End Sub

    ''' <summary>
    ''' 印刷処理
    ''' </summary>
    Private Sub Print()
        Dim tmpInsData As New Dictionary(Of String, String)

        Try
            tmpInsData.Clear()
            tmpInsData.Add("SUPPLIER_PRODUCT", "'" & TxtLotNoEdaNo.Text & "'")
                tmpInsData.Add("MRMS", "'" & TxtMrms.Text & "'")
            tmpInsData.Add("PRODUCT_DATE", "'" & TxtSeisanDate.Text & "'")
            tmpInsData.Add("PALLET_TYPE", "'" & TxtBag.Text & "'")
            tmpInsData.Add("ITEM", "'" & TxtItemName.Text & "'")
            tmpInsData.Add("QUANTITY", "'" & TxtQuantity.Text & "'")
            tmpInsData.Add("LOT", "'" & TxtLotNo.Text & "'")
            tmpInsData.Add("BARCODE_SRC", "'" & "(00)" & TxtFixNo.Text & TxtSeisanDate.Text & TxtRecNo.Text.PadLeft(6, "0"c) & TxtCheckDigit.Text & "'")
            tmpInsData.Add("OUTCOUNT", "'" & TxtPrtCnt.Text & "'")

            Dim PrtCnt As Integer = Integer.Parse(TxtPrtCnt.Text)
            For i As Integer = 0 To PrtCnt - 1
                PrintProcess(1, tmpInsData)
            Next

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try


    End Sub

    ''' <summary>
    ''' 自動データ登録処理
    ''' </summary>
    Private Sub InsertAutoData()
        Dim tmpDb As New ClsAccess(DATA_FILEPATH)
        Dim tmpInsData As New Dictionary(Of String, String)

        Try
            If String.IsNullOrWhiteSpace(TxtLotNo.Text) Then
                Exit Sub
            End If

            tmpInsData.Clear()
            tmpInsData.Add("PRODUCT_DATE", "'" & TxtSeisanDate.Text & "'")
            tmpInsData.Add("ITEM", "'" & TxtItemName.Text & "'")
            tmpInsData.Add("MRMS", "'" & TxtMrms.Text & "'")
            tmpInsData.Add("QUANTITY", "'" & TxtQuantity.Text & "'")
            tmpInsData.Add("LOT", "'" & TxtLotNo.Text & "'")
            tmpInsData.Add("BAG", "'" & TxtBag.Text & "'")
            tmpInsData.Add("FIX_NO", "'" & TxtFixNo.Text & "'")
            tmpInsData.Add("REC_NO", "'" & TxtRecNo.Text.PadLeft(6, "0"c) & "'")

            tmpDb.Execute(SqlInsert("TRN_Data", tmpInsData))

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 連番取得処理
    ''' </summary>
    Private Function GetRecNo(Optional prmRecNo As Boolean = False) As String
        Dim tmpDb As New ClsAccess(DATA_FILEPATH)
        Dim tmpDt As New DataTable
        Dim RecNo As String = String.Empty
        Try
            tmpDb.GetResult(tmpDt, SqlGetRecNo(prmRecNo))

            If tmpDt.Rows.Count = 0 Then
                RecNo = 0
            Else
                If Not String.IsNullOrWhiteSpace(tmpDt.Rows(0).Item("RecNo").ToString()) Then
                    RecNo = tmpDt.Rows(0).Item("RecNo").ToString
                Else
                    RecNo = 0
                End If

            End If

            If Not prmRecNo Then
                If String.IsNullOrWhiteSpace(RecNo) OrElse
                    String.IsNullOrWhiteSpace(TxtLotNo.Text) Then
                    RecNo = 0
                End If

            End If


        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        RecNo += 1
        Return RecNo
    End Function

    ''' <summary>
    ''' 再発行用データ取得処理
    ''' </summary>
    Private Function GetTrnData() As DataTable
        Dim tmpDb As New ClsAccess(DATA_FILEPATH)
        Dim tmpDt As New DataTable

        Try
            tmpDb.GetResult(tmpDt, SqlGetTrnData)

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return tmpDt
    End Function

#Region "SQL"
    ''' <summary>
    ''' 連番取得SQL
    ''' </summary>
    Private Function SqlGetRecNo(Optional prmRecNo As Boolean = False)
        Dim sql As String

        Try
            sql = ""
            If prmRecNo Then
                sql += " SELECT Max(REC_NO) as RecNo "
            Else
                sql += " SELECT Max(BAG) as RecNo "
            End If
            sql += " FROM TRN_Data "
            If Not prmRecNo Then
                sql += " WHERE Lot = '" & TxtLotNo.Text & "'"
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
        ComWriteLog(sql, System.AppDomain.CurrentDomain.BaseDirectory & "sql" & ComGetProcYear() & ComGetProcMonth() & ".log")
        Return sql
    End Function

    ''' <summary>
    ''' 再発行用データ取得SQL
    ''' </summary>
    Private Function SqlGetTrnData() As String
        Dim sql As String

        sql = ""
        sql += "SELECT  PRODUCT_DATE "
        sql += "    ,   ITEM "
        sql += "    ,   MRMS "
        sql += "    ,   QUANTITY "
        sql += "    ,   LOT "
        sql += "    ,   BAG "
        sql += "    ,   FIX_NO "
        sql += "    ,   REC_NO "
        sql += " FROM TRN_Data "
        sql += " WHERE lot = '" & TxtLotNo.Text & "'"
        sql += " And BAG = '" & TxtBag.Text & "'"

        ComWriteLog(sql, System.AppDomain.CurrentDomain.BaseDirectory & "sql" & ComGetProcYear() & ComGetProcMonth() & ".log")

        Return sql

    End Function

#End Region
End Class
