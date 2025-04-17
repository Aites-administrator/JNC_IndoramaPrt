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
  Private OrgKey As String = String.Empty
  ''' <summary>
  ''' 連番フラグ
  ''' </summary>
  Private Const PALLET_NO_FLG As Boolean = True

  ''' <summary>
  ''' バッグ連番名
  ''' </summary>
  Private Const BAG_REN_NAME As String = "BAG#"

  ''' <summary>
  ''' パレット連番名
  ''' </summary>
  Private Const PALLET_REN_NAME As String = "PALLET#"

  ''' <summary>
  ''' バッグワークテーブル名
  ''' </summary>
  Private Const BAG_WK_TBL As String = "T_BAG"

  ''' <summary>
  ''' パレットレポート名
  ''' </summary>
  Private Const BAG_REPORT_NAME As String = "R_BAG"

  ''' <summary>
  ''' パレットワークテーブル名
  ''' </summary>
  Private Const PALLET_WK_TBL As String = "T_PALLET"

  ''' <summary>
  ''' パレットワークテーブル名
  ''' </summary>
  Private Const PALLET_REPORT_NAME As String = "R_PALLET"

  Private InjiNenGappi As String = ""

  ' 抽出条件
  Private Enum typBagPallet
    ''' <summary>バッグタイプ</summary>
    BAG_TYPE = 0
    ''' <summary>パレットタイプ</summary>
    PALLET_TYPE
  End Enum

#Region "イベントプロシージャ"
  ''' <summary>
  ''' 数値のみコントロールイベント
  ''' </summary>
  Private Sub TxtSeisanDate_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtSeisanDate.KeyPress, TxtQuantity.KeyPress, TxtFixNo.KeyPress, TxtRecNo.KeyPress, TxtBag.KeyPress, TxtPrtCnt.KeyPress, TxtSplice.KeyPress, TxtBag2.KeyPress, TxtPallet.KeyPress
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
      TxtFixNo.Text = TxtFixNo.Text.PadLeft(8, "0"c)
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
  Private Sub TxtLotNo_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TxtLotNo.Validating, TxtBag.Validating, TxtBag2.Validating, TxtPallet.Validating
    Dim txtBox As TextBox = DirectCast(sender, TextBox)
    Dim newValue As String = txtBox.Text

    InsFlg = True

    TxtBag.Text = TxtBag.Text.PadLeft(3, "0"c)
    TxtBag2.Text = If(String.IsNullOrWhiteSpace(TxtBag2.Text), "", TxtBag2.Text.PadLeft(3, "0"c))
    TxtPallet.Text = If(String.IsNullOrWhiteSpace(TxtPallet.Text), "", TxtPallet.Text.PadLeft(3, "0"c))

    If sender.name = TxtBag2.Name Then
      TxtBag2.Text = If(String.IsNullOrWhiteSpace(TxtBag2.Text), "", TxtBag2.Text.PadLeft(3, "0"c))
      Exit Sub
    End If

    If sender.name <> TxtLotNo.Name Then
      If RdoBagLayout.Checked And sender.name <> TxtBag.Name Then
        Exit Sub
      End If
      If RdoPalletLayout.Checked And sender.name <> TxtPallet.Name Then
        Exit Sub
      End If

    End If

    '手動モードで入力値が変更されたとき
    If RdoManual.Checked AndAlso OrgKey <> newValue Then
      'データ反映処理
      DataDisp(RdoPalletLayout.Checked)
    End If

    '採番処理
    NumberingProcess()

    TxtLotNoEdaNo.Text = TxtLotNo.Text & "-" & TxtBag.Text & If(RdoPalletLayout.Checked, "," & TxtBag2.Text, "")



  End Sub



  ''' <summary>
  ''' 印刷ボタン
  ''' </summary>
  Private Sub BtnPrint_Click(sender As Object, e As EventArgs) Handles BtnPrint.Click

    Try
      If RdoBagLayout.Checked And (TxtSplice.Text = "" Or TxtQuantity.Text = "" Or TxtQuantity.Text = "0") Then
        ComMessageBox("重量 または 継目が入力されていません。", "印刷処理", typMsgBox.MSG_WARNING, typMsgBoxButton.BUTTON_OK)
        Exit Sub
      End If

      '自動データ登録処理
      If RdoAuto.Checked Then
        InsertAutoData()
      End If

      ' Bagラベル印刷処理
      If ChkPrint(typBagPallet.BAG_TYPE) Then
        'DataDisp(Not PALLET_NO_FLG)
        BagPrint()
      End If

      ' Palletラベル印刷処理
      If ChkPrint(typBagPallet.PALLET_TYPE) Then
        'DataDisp(PALLET_NO_FLG)
        PalletPrint()
      End If

      '自動データ登録処理
      If RdoAuto.Checked Then
        '採番処理
        DataDisp(Not PALLET_NO_FLG)
        NumberingProcess()
        TxtQuantity.Text = ""
        TxtSplice.Text = ""

        If Integer.Parse(TxtBag.Text) Mod 2 = 0 Then
          TxtSeisanDate.Enabled = False
          TxtItemName.Enabled = False
          TxtMrms.Enabled = False
          TxtFixNo.Enabled = False
        Else
          TxtSeisanDate.Enabled = True
          TxtItemName.Enabled = True
          TxtMrms.Enabled = True
          TxtFixNo.Enabled = True
        End If

      End If

      If RdoManual.Checked Then
        ObjClear()
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

      DataLen = InAllData.Length

      '受信完了チェック
      If IsReceveComp(DataLen) Then
        '   イベント止める
        RemoveHandler SeriPort.DataReceived, AddressOf SerialPort_DataReceived
        '''''
        If Strings.Right(InAllData, 1) = ETX And Strings.Left(InAllData, 1) <> STX Then
          InAllData = ""
        Else
          '　　重量取得チェック
          If IsGetJyuryo(DataLen) Then

            ComWriteLog(InAllData, "重量取得直前データ.log")

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

          End If

          'データバッファクリア
          InAllData = ""
        End If
      Else
        Jyuryo = 0

      End If

    Catch ex As Exception
      ComWriteErrLog(ex)
    Finally
      '   イベント止める
      RemoveHandler SeriPort.DataReceived, AddressOf SerialPort_DataReceived
      '   イベント再開
      AddHandler SeriPort.DataReceived, AddressOf SerialPort_DataReceived

    End Try

  End Sub

  ''' <summary>
  ''' 受信完了チェック
  ''' <param name="prmDataLen">データ長</param>
  ''' </summary>

  Private Function IsReceveComp(prmDataLen As Integer) As Boolean
    Dim rtn As Boolean = True

    If prmDataLen > 0 Then
      ' ETXフラグがたっていたら
      rtn = InAllData.Substring(0, 1) <> STX _
                OrElse Strings.Right(InAllData, 1) = ENQ _
                OrElse Strings.Right(InAllData, 1) = ENQ _
                OrElse Strings.Right(InAllData, 1) = ACK _
                OrElse Strings.Right(InAllData, 1) = NAK _
                OrElse Strings.Right(InAllData, 1) = EOT _
                OrElse (prmDataLen > 1 And Strings.Right(InAllData, 1) = ETX)

    End If

    Return rtn

  End Function

  ''' <summary>   
  ''' 重量取得チェック
  ''' <param name="prmDataLen">データ長</param>
  ''' </summary>
  Private Function IsGetJyuryo(prmDataLen As Integer) As Boolean
    Return (prmDataLen = 31 Or prmDataLen = 32) _
            AndAlso (InAllData.Substring(5, 1) = 0 Or InAllData.Substring(5, 1) = 4) _
            AndAlso InAllData.Substring(6, 1) = 0

  End Function

  Private Sub RdoAuto_CheckedChanged(sender As Object, e As EventArgs) Handles RdoAuto.CheckedChanged, RdoManual.CheckedChanged
    Try
      TxtSeisanDate.Enabled = True
      TxtItemName.Enabled = True
      TxtMrms.Enabled = True
      TxtFixNo.Enabled = True

      Select Case True
        Case RdoAuto.Checked
          RdoBagLayout.Checked = True
          GrpLayout.Enabled = False
          ChkManuPriority.Enabled = True
          ChkManuPriority.Checked = False
          TxtQuantity.ReadOnly = False
          TxtLotNo.BackColor = Color.White
          TxtBag.BackColor = Color.White
          TxtPallet.BackColor = SystemColors.Control
          TxtBag2.BackColor = SystemColors.Control

          TxtRecNo.ReadOnly = True
          SerialPortsGet()

          'データクリア
          ObjClear()

          'TxtRecNo.Text = GetRecNo(REC_NO_FLG).PadLeft(6, "0"c)
          TxtRecNo.Text = (GetRenData(BAG_REN_NAME).Rows(0).Item("RenNo") + 1).ToString.PadLeft(6, "0"c)

          If Not String.IsNullOrWhiteSpace(TxtLotNo.Text) Then
            TxtBag.Text = GetRecNo().PadLeft(3, "0"c)
          End If

        Case RdoManual.Checked
          GrpLayout.Enabled = True
          RdoBagLayout.Checked = False
          RdoBagLayout.Checked = True
          ChkManuPriority.Enabled = False
          ChkManuPriority.Checked = False
          TxtLotNo.BackColor = Color.AliceBlue
          TxtRecNo.ReadOnly = False
          TxtRecNo.Text = ""
          'データクリア
          ObjClear()
          'TxtQuantity.ReadOnly = False
          If SeriPort.IsOpen = True Then   'ポートオープン済み
            SeriPort.Close()                         'ポートクローズ
          End If
      End Select

    Catch ex As Exception
      ComWriteErrLog(ex)
      ComMessageBox(ex.Message, "接続確認", typMsgBox.MSG_WARNING, typMsgBoxButton.BUTTON_OK)
    End Try
  End Sub


  Private Sub RdoLayout_CheckedChanged(sender As Object, e As EventArgs) Handles RdoBagLayout.CheckedChanged, RdoPalletLayout.CheckedChanged
    Try

      Select Case True
        Case RdoBagLayout.Checked
          TxtBag.ReadOnly = False
          TxtBag2.ReadOnly = True
          TxtBag2.BackColor = SystemColors.Control
          TxtPallet.ReadOnly = True
          TxtPallet.BackColor = SystemColors.Control
          TxtSplice.ReadOnly = False
          TxtSplice.BackColor = Color.White
          If GrpLayout.Enabled Then
            TxtBag.BackColor = Color.AliceBlue
          End If

          DataDisp(RdoPalletLayout.Checked)
        Case RdoPalletLayout.Checked
          TxtBag.ReadOnly = False
          TxtBag2.ReadOnly = False
          TxtBag2.BackColor = Color.White
          TxtPallet.ReadOnly = False
          TxtPallet.BackColor = Color.White
          TxtBag.BackColor = Color.White
          TxtSplice.ReadOnly = True
          TxtSplice.BackColor = SystemColors.Control
          If GrpLayout.Enabled Then
            TxtPallet.BackColor = Color.AliceBlue
          End If

          DataDisp(RdoPalletLayout.Checked)
      End Select

      '印刷数初期設定
      InitPrintCnt()

    Catch ex As Exception
      ComWriteErrLog(ex)
      ComMessageBox(ex.Message, "接続確認", typMsgBox.MSG_WARNING, typMsgBoxButton.BUTTON_OK)
    End Try
  End Sub


  Private Sub TxtPrtCnt_Validated(sender As Object, e As EventArgs) Handles TxtPrtCnt.Validated
    If Not IsNumeric(TxtPrtCnt.Text) Then
      InitPrintCnt()
    End If

    If TxtPrtCnt.Text = 0 Then
      InitPrintCnt()
    End If
  End Sub

  Private Sub TxtQuantity_Validated(sender As Object, e As EventArgs) Handles TxtQuantity.Validated
    '仮納品用
    If RdoAuto.Checked Then
      If Not ChkManuPriority.Checked Then
        TxtQuantity.Text = Jyuryo
      End If
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

  Private Sub TxtLotNo_Enter(sender As Object, e As EventArgs) Handles TxtLotNo.Enter, TxtBag.Enter, TxtBag2.Enter, TxtPallet.Enter
    Dim txtBox As TextBox = DirectCast(sender, TextBox)
    OrgKey = txtBox.Text ' 現在のテキストボックスの値を保存
  End Sub
#End Region

  ''' <summary>
  ''' 採番処理
  ''' </summary>
  Private Sub InitPrintCnt()
    Select Case True
      Case RdoBagLayout.Checked
        TxtPrtCnt.Text = 1
      Case RdoPalletLayout.Checked
        TxtPrtCnt.Text = 2
      Case Else
        TxtPrtCnt.Text = 2
    End Select


  End Sub

  ''' <summary>
  ''' 採番処理
  ''' </summary>
  Private Sub NumberingProcess()
    Dim tmpDb As New ClsAccess(DATA_FILEPATH)

    Try
      If RdoAuto.Checked Then
        If Not String.IsNullOrWhiteSpace(TxtLotNo.Text) Then


          '連番の採番
          TxtRecNo.Text = (GetRenData(BAG_REN_NAME).Rows(0).Item("RenNo") + 1).ToString.PadLeft(6, "0"c)

          'Bagの採番
          TxtBag.Text = GetRecNo().PadLeft(3, "0"c)

          'Palletの採番
          TxtPallet.Text = GetRecNo(PALLET_NO_FLG).PadLeft(3, "0"c)
        End If
      End If
      'チェックディジット計算
      TxtCheckDigit.Text = AddCheckDigit(TxtFixNo.Text & TxtSeisanDate.Text & TxtRecNo.Text)

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try
  End Sub
  'チェックディジット計算処理
  Private Function AddCheckDigit(prmText As String) As String
    Dim len As Integer = 0
    Dim ret As String = prmText
    Dim NoEvenSum As Integer = 0
    Dim EvenSum As Integer = 0
    Dim sum As Integer = 0
    Dim EvenKetaLength As Integer = 0
    len = prmText.Length

    If String.IsNullOrWhiteSpace(prmText) Then
      Return ret
    End If

    If prmText.Length = TxtFixNo.MaxLength + TxtSeisanDate.MaxLength + TxtRecNo.MaxLength Then
      EvenKetaLength = prmText.Length Mod 2
      For Dataindex = 1 To len
        If Dataindex Mod 2 = EvenKetaLength Then
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
  ''' 印刷処理
  ''' </summary>
  Private Function ChkPrint(prmBagPallet As typBagPallet) As Boolean
    Dim rtn As Boolean
    '自動のときは、バッグは出力する。パレットはバッグが偶数のときのみ。
    '手動のときは、バッグレイアウトならバッグを出力。パレットレイアウトならパレットを出力。


    If RdoAuto.Checked Then
      '自動モード
      Select Case prmBagPallet
        Case typBagPallet.BAG_TYPE
          'バッグは無条件で出力
          rtn = True
        Case typBagPallet.PALLET_TYPE
          'パレットはBag#が偶数のときに出力
          rtn = (Integer.Parse(TxtBag.Text) Mod 2 = 0)
      End Select
    Else
      '手動モード
      Select Case prmBagPallet
        Case typBagPallet.BAG_TYPE
          'バッグレイアウト選択時にバッグを出力
          rtn = RdoBagLayout.Checked
        Case typBagPallet.PALLET_TYPE
          'パレットレイアウト選択時にパレットを出力
          rtn = RdoPalletLayout.Checked
      End Select
    End If

    Return rtn
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
  ''' データ反映処理
  ''' </summary>
  Private Sub DataDisp(prmPalletMode As Boolean)
    Dim tmpDt As New DataTable

    '再発行データ取得処理
    tmpDt = GetTrnData(prmPalletMode)

    '正常に再発行ﾃﾞｰﾀを取得
    If tmpDt.Rows.Count = 1 Then

      TxtSeisanDate.Text = tmpDt.Rows(0).Item("PRODUCT_DATE").ToString
      TxtItemName.Text = tmpDt.Rows(0).Item("ITEM").ToString
      TxtMrms.Text = tmpDt.Rows(0).Item("MRMS").ToString
      TxtQuantity.Text = tmpDt.Rows(0).Item("QUANTITY").ToString
      TxtLotNo.Text = tmpDt.Rows(0).Item("LOT").ToString
      TxtBag.Text = tmpDt.Rows(0).Item("BAG").ToString.PadLeft(3, "0"c)
      TxtBag2.Text = If(prmPalletMode, tmpDt.Rows(0).Item("BAG2").ToString.PadLeft(3, "0"c), "")
      TxtFixNo.Text = tmpDt.Rows(0).Item("FIX_NO").ToString.PadLeft(5, "0"c)
      TxtRecNo.Text = tmpDt.Rows(0).Item("REC_NO").ToString.PadLeft(6, "0"c)
      TxtPallet.Text = tmpDt.Rows(0).Item("PALLET").ToString.PadLeft(3, "0"c)
      TxtSplice.Text = If(prmPalletMode, "", tmpDt.Rows(0).Item("SPLICE").ToString)

      TxtLotNoEdaNo.Text = TxtLotNo.Text & "-" & TxtBag.Text & If(prmPalletMode, "," & TxtBag2.Text, "")
      InsFlg = False

      If RdoManual.Checked Then
        LblHis.Text = "履"
      End If

    Else
      LblHis.Text = ""
    End If
    'チェックディジット計算
    TxtCheckDigit.Text = AddCheckDigit(TxtFixNo.Text & TxtSeisanDate.Text & TxtRecNo.Text.PadLeft(6, "0"c))



  End Sub

  ''' <summary>
  ''' Bagラベル印刷処理
  ''' </summary>
  Private Sub BagPrint()
    Dim tmpInsData As New Dictionary(Of String, String)
    Dim tmpDb As New ClsAccess(DATA_FILEPATH)

    Try

      tmpInsData.Clear()
      tmpInsData.Add("SUPPLIER_PRODUCT", "'" & TxtLotNoEdaNo.Text & "'")
      tmpInsData.Add("MRMS", "'" & TxtMrms.Text & "'")
      tmpInsData.Add("PRODUCT_DATE", "'" & TxtSeisanDate.Text & "'")
      tmpInsData.Add("BAG", "'" & TxtBag.Text & "'")
      tmpInsData.Add("ITEM", "'" & TxtItemName.Text & "'")
      tmpInsData.Add("QUANTITY", "'" & TxtQuantity.Text & "'")
      tmpInsData.Add("LOT", "'" & TxtLotNo.Text & "'")
      tmpInsData.Add("BARCODE_SRC", "'" & "(00)" & TxtFixNo.Text & TxtSeisanDate.Text & TxtRecNo.Text.PadLeft(6, "0"c) & TxtCheckDigit.Text & "'")
      tmpInsData.Add("OUTCOUNT", "'" & TxtPrtCnt.Text & "'")
      tmpInsData.Add("SPLICE", "'" & TxtSplice.Text & "'")

      Dim PrtCnt As Integer = Integer.Parse(TxtPrtCnt.Text)
      For i As Integer = 0 To PrtCnt - 1
        PrintProcess(BAG_WK_TBL, BAG_REPORT_NAME, 0, tmpInsData)
      Next

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try


  End Sub

  ''' <summary>
  ''' Palletラベル印刷処理
  ''' </summary>
  Private Sub PalletPrint()
    Dim tmpInsData As New Dictionary(Of String, String)
    Dim tmpDb As New ClsAccess(DATA_FILEPATH)
    Dim tmpDt As New DataTable

    Try

      tmpInsData.Clear()
      tmpInsData.Add("SUPPLIER_PRODUCT", "'" & TxtLotNoEdaNo.Text & "'")
      tmpInsData.Add("MRMS", "'" & TxtMrms.Text & "'")
      tmpInsData.Add("PRODUCT_DATE", "'" & TxtSeisanDate.Text & "'")
      tmpInsData.Add("BAG", "'" & TxtBag.Text & "'")
      tmpInsData.Add("ITEM", "'" & TxtItemName.Text & "'")
      tmpInsData.Add("QUANTITY", "'" & TxtQuantity.Text & "'")
      tmpInsData.Add("LOT", "'" & TxtLotNo.Text & "'")
      tmpInsData.Add("BARCODE_SRC", "'" & "(00)" & TxtFixNo.Text & TxtSeisanDate.Text & TxtRecNo.Text.PadLeft(6, "0"c) & TxtCheckDigit.Text & "'")
      tmpInsData.Add("OUTCOUNT", "'" & TxtPrtCnt.Text & "'")
      tmpInsData.Add("BAG2", "'" & TxtBag2.Text & "'")
      tmpInsData.Add("SPLICE", "'" & TxtSplice.Text & "'")
      tmpInsData.Add("PALLET", "'" & TxtPallet.Text & "'")

      Dim PrtCnt As Integer = Integer.Parse(TxtPrtCnt.Text)

      If RdoAuto.Checked Then
        PrtCnt = PrtCnt * 2
      End If

      For i As Integer = 0 To PrtCnt - 1
        PrintProcess(PALLET_WK_TBL, PALLET_REPORT_NAME, 0, tmpInsData)
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
    Dim tmpDt As New DataTable

    Try
      If String.IsNullOrWhiteSpace(TxtLotNo.Text) Then
        Exit Sub
      End If

      tmpDb.TrnStart()

      'Bagデータ登録
      tmpInsData.Clear()
      tmpInsData.Add("PRODUCT_DATE", "'" & TxtSeisanDate.Text & "'")
      tmpInsData.Add("ITEM", "'" & TxtItemName.Text & "'")
      tmpInsData.Add("MRMS", "'" & TxtMrms.Text & "'")
      tmpInsData.Add("QUANTITY", "'" & If(TxtQuantity.Text = "", "0", TxtQuantity.Text) & "'")
      tmpInsData.Add("LOT", "'" & TxtLotNo.Text & "'")
      tmpInsData.Add("BAG", "'" & TxtBag.Text & "'")
      tmpInsData.Add("FIX_NO", "'" & TxtFixNo.Text & "'")
      '連番採番
      TxtRecNo.Text = (GetRenData(BAG_REN_NAME).Rows(0).Item("RenNo") + 1).ToString.PadLeft(6, "0"c)
      tmpDb.Execute(SqlUpdData(BAG_REN_NAME, GetRenData(BAG_REN_NAME).Rows(0).Item("RenNo") + 1))
      tmpInsData.Add("REC_NO", "'" & TxtRecNo.Text.PadLeft(6, "0"c) & "'")
      tmpInsData.Add("PALLET", "'" & TxtPallet.Text & "'")
      tmpInsData.Add("SPLICE", "'" & TxtSplice.Text & "'")

      tmpDb.Execute(SqlInsert("TRN_Bag", tmpInsData))

      'Palletデータ登録
      If Integer.Parse(TxtBag.Text) Mod 2 = 0 Then
        tmpInsData.Clear()
        '一つ前のBagを取得する？
        tmpDb.GetResult(tmpDt, SqlGetPlletData(TxtLotNo.Text, TxtPallet.Text))

        tmpInsData.Add("LOT", "'" & TxtLotNo.Text & "'")
        tmpInsData.Add("PALLET", "'" & TxtPallet.Text & "'")
        tmpInsData.Add("BAG", "'" & tmpDt.Rows(0).Item("MinBag").ToString & "'")
        tmpInsData.Add("BAG2", "'" & TxtBag.Text.PadLeft(3, "0"c) & "'")
        tmpInsData.Add("SUM_QUANTITY", "'" & tmpDt.Rows(0).Item("SumQuantity").ToString & "'")
        '連番採番
        TxtRecNo.Text = (GetRenData(PALLET_REN_NAME).Rows(0).Item("RenNo") + 1).ToString.PadLeft(6, "0"c)
        tmpDb.Execute(SqlUpdData(PALLET_REN_NAME, GetRenData(PALLET_REN_NAME).Rows(0).Item("RenNo") + 1))
        tmpInsData.Add("REC_NO", "'" & TxtRecNo.Text.PadLeft(6, "0"c) & "'")

        tmpDb.Execute(SqlInsert("TRN_Pallet", tmpInsData))

      End If

      tmpDb.TrnCommit()
    Catch ex As Exception
      tmpDb.TrnRollBack()
      Throw New Exception(ex.Message)
    End Try
  End Sub

  ''' <summary>
  ''' 連番取得処理
  ''' </summary>
  Private Function GetRecNo(Optional prmPalletNo As Boolean = False) As String
    Dim tmpDb As New ClsAccess(DATA_FILEPATH)
    Dim tmpDt As New DataTable
    Dim RecNo As String = String.Empty
    Try
      tmpDb.GetResult(tmpDt, SqlGetRecNo(prmPalletNo))

      If tmpDt.Rows.Count = 0 Then
        RecNo = 0
      Else
        If prmPalletNo Then
          RecNo = tmpDt.Rows(0).Item("PalletRecNo").ToString
        Else
          RecNo = tmpDt.Rows(0).Item("BagRecNo").ToString
        End If

        'If Not String.IsNullOrWhiteSpace(tmpDt.Rows(0).Item("RecNo").ToString()) Then
        '    RecNo = tmpDt.Rows(0).Item("RecNo").ToString
        'Else
        '    RecNo = 0
        'End If

      End If

      'If Not prmPalletNo Then
      '    If String.IsNullOrWhiteSpace(RecNo) OrElse
      '        String.IsNullOrWhiteSpace(TxtLotNo.Text) Then
      '        RecNo = 0
      '    End If

      'End If

      If String.IsNullOrWhiteSpace(RecNo) Then
        RecNo = 0
      End If

      RecNo += 1

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

    Return RecNo
  End Function

  ''' <summary>
  ''' 再発行用データ取得処理
  ''' </summary>
  Private Function GetTrnData(prmPalletMode As Boolean) As DataTable
    Dim tmpDb As New ClsAccess(DATA_FILEPATH)
    Dim tmpDt As New DataTable

    Try
      tmpDb.GetResult(tmpDt, SqlGetTrnData(prmPalletMode))

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

    Return tmpDt
  End Function


  ''' <summary>
  ''' 連番データ取得処理
  ''' </summary>
  Private Function GetRenData(prmRenName As String) As DataTable
    Dim tmpDb As New ClsAccess(DATA_FILEPATH)
    Dim tmpDt As New DataTable

    Try
      tmpDb.GetResult(tmpDt, SqlGetRenData(prmRenName))

    Catch ex As Exception
      Throw New Exception(ex.Message)
    End Try

    Return tmpDt
  End Function

  Private Sub ObjClear()
    TxtLotNo.Text = ""
    TxtSeisanDate.Text = ""
    TxtItemName.Text = ""
    TxtMrms.Text = ""
    TxtQuantity.Text = ""
    TxtFixNo.Text = ""
    TxtRecNo.Text = ""
    TxtSplice.Text = ""
    TxtBag2.Text = ""
    TxtBag.Text = ""
    TxtPallet.Text = ""
    LblHis.Text = ""
    'If RdoPalletLayout.Checked Or RdoAuto.Checked Then
    '    TxtBag.Text = ""
    'End If

    'If RdoBagLayout.Checked Or RdoAuto.Checked Then
    '    TxtPallet.Text = ""
    'End If

    'チェックディジット計算
    TxtCheckDigit.Text = AddCheckDigit(TxtFixNo.Text & TxtSeisanDate.Text & TxtRecNo.Text.PadLeft(6, "0"c))

  End Sub

#Region "SQL"
  ''' <summary>
  ''' 連番取得SQL
  ''' </summary>
  Private Function SqlGetRecNo(Optional prmPalletNo As Boolean = False)
    Dim sql As String

    Try
      sql = ""
      sql += " SELECT Max(Trn_Pallet.PALLET) As PalletRecNo "
      sql += " ,Max(Trn_Bag.Bag) As BagRecNo "
      sql += " FROM Trn_Bag "
      sql += " LEFT JOIN TRN_Pallet "
      sql += " On Trn_Pallet.LOT = Trn_Bag.LOT "
      sql += " AND Trn_Pallet.PALLET = Trn_Bag.PALLET "
      sql += " WHERE Trn_Bag.Lot = '" & TxtLotNo.Text & "'"
      sql += " Group By Trn_Bag.LOT "
      If prmPalletNo Then
        sql += " HAVING MIN(Trn_Bag.BAG) Mod 2 = 1  "
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
  Private Function SqlGetTrnData(prmPalletMode As Boolean) As String
    Dim sql As String

    sql = ""
    sql += "SELECT  DISTINCT "
    sql += "        Trn_Bag.PRODUCT_DATE "
    sql += "    ,   Trn_Bag.ITEM "
    sql += "    ,   Trn_Bag.MRMS "
    sql += "    ,   Trn_Bag.LOT "
    sql += "    ,   Trn_Bag.FIX_NO "
    sql += "    ,   Trn_Pallet.PALLET "
    'パレットレイアウトのとき
    If prmPalletMode Then
      sql += "    ,   Trn_Pallet.SUM_QUANTITY As QUANTITY"
      sql += "    ,   Trn_Pallet.REC_NO "
      sql += "    ,   Trn_Pallet.BAG "
      sql += "    ,   Trn_Pallet.BAG2 "
    Else
      sql += "    ,   Trn_Bag.SPLICE "
      sql += "    ,   Trn_Bag.QUANTITY "
      sql += "    ,   Trn_Bag.Bag "
      sql += "    ,   Trn_Bag.REC_NO "
    End If
    sql += " FROM Trn_Bag "
    sql += " LEFT JOIN Trn_Pallet "
    sql += " On  Trn_Pallet.LOT = Trn_Bag.LOT "
    sql += " And Trn_Pallet.PALLET = Trn_Bag.PALLET "
    sql += " WHERE Trn_Bag.LOT = '" & TxtLotNo.Text & "'"
    If prmPalletMode Then
      sql += " And Trn_Pallet.Pallet = '" & TxtPallet.Text & "'"
    Else
      sql += " And Trn_Bag.BAG = '" & TxtBag.Text & "'"
    End If

    ComWriteLog(sql, System.AppDomain.CurrentDomain.BaseDirectory & "sql" & ComGetProcYear() & ComGetProcMonth() & ".log")

    Return sql

  End Function

  ''' <summary>
  ''' 連番データ取得SQL
  ''' <param name="prmRenNoName">連番名</param>>
  ''' </summary>
  Private Function SqlGetRenData(prmRenNoName As String) As String
    Dim sql As String

    sql = ""
    sql += "SELECT  RenNo "
    sql += " FROM DenNoTbl "
    sql += " WHERE RenName = '" & prmRenNoName & "'"

    ComWriteLog(sql, System.AppDomain.CurrentDomain.BaseDirectory & "sql" & ComGetProcYear() & ComGetProcMonth() & ".log")

    Return sql

  End Function

  ''' <summary>
  ''' 連番データ取得SQL
  ''' <param name="prmRenNoName">連番名</param>>
  ''' </summary>
  Private Function SqlGetPlletData(prmLotNo As String, prmPallet As String) As String
    Dim sql As String

    sql = ""
    sql += " SELECT Trn_Bag.LOT "
    sql += "    ,   Trn_Bag.Pallet "
    sql += "    ,   Min(Trn_Bag.Bag) as MinBag "
    sql += "    ,   Sum(Quantity) As SumQuantity "
    sql += " FROM   Trn_Bag "
    sql += " WHERE  Trn_Bag.LOT = '" & prmLotNo & "'"
    sql += " AND    Trn_Bag.Pallet ='" & prmPallet & "'"
    sql += " GROUP BY   "
    sql += "        Trn_Bag.LOT"
    sql += "    ,   Trn_Bag.Pallet"

    ComWriteLog(sql, System.AppDomain.CurrentDomain.BaseDirectory & "sql" & ComGetProcYear() & ComGetProcMonth() & ".log")

    Return sql

  End Function

  ''' <summary>
  ''' 連番データ取得SQL
  ''' <param name="prmRenNoName">連番名</param>>
  ''' </summary>
  Private Function SqlUpdData(prmRenNoName As String, prmRenNo As Integer) As String
    Dim sql As String

    sql = ""
    sql += "UPDATE DenNoTbl "
    sql += "Set RenNo = " & prmRenNo
    sql += " WHERE RenName = '" & prmRenNoName & "'"

    ComWriteLog(sql, System.AppDomain.CurrentDomain.BaseDirectory & "sql" & ComGetProcYear() & ComGetProcMonth() & ".log")

    Return sql

  End Function



#End Region
End Class
