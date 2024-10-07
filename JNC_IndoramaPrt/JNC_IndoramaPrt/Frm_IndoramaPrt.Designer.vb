<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Frm_IndoramaPrt
  Inherits System.Windows.Forms.Form

  'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
  <System.Diagnostics.DebuggerNonUserCode()> _
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    Try
      If disposing AndAlso components IsNot Nothing Then
        components.Dispose()
      End If
    Finally
      MyBase.Dispose(disposing)
    End Try
  End Sub

  'Windows フォーム デザイナーで必要です。
  Private components As System.ComponentModel.IContainer

  'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
  'Windows フォーム デザイナーを使用して変更できます。  
  'コード エディターを使って変更しないでください。
  <System.Diagnostics.DebuggerStepThrough()> _
  Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtSeisanDate = New System.Windows.Forms.TextBox()
        Me.TxtItemName = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtLotNoEdaNo = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtLotNo = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtQuantity = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TxtFixNo = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TxtRecNo = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TxtCheckDigit = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtPrtCnt = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.BtnPrint = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TxtMrms = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TxtBag = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.RdoAuto = New System.Windows.Forms.RadioButton()
        Me.RdoManual = New System.Windows.Forms.RadioButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(22, 73)
        Me.Label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(104, 19)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "生産年月日"
        '
        'TxtSeisanDate
        '
        Me.TxtSeisanDate.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.TxtSeisanDate.Location = New System.Drawing.Point(215, 65)
        Me.TxtSeisanDate.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtSeisanDate.MaxLength = 6
        Me.TxtSeisanDate.Name = "TxtSeisanDate"
        Me.TxtSeisanDate.Size = New System.Drawing.Size(204, 26)
        Me.TxtSeisanDate.TabIndex = 1
        Me.TxtSeisanDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TxtItemName
        '
        Me.TxtItemName.Location = New System.Drawing.Point(215, 115)
        Me.TxtItemName.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtItemName.MaxLength = 18
        Me.TxtItemName.Name = "TxtItemName"
        Me.TxtItemName.Size = New System.Drawing.Size(204, 26)
        Me.TxtItemName.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(22, 118)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 19)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "品種名"
        '
        'TxtLotNoEdaNo
        '
        Me.TxtLotNoEdaNo.Location = New System.Drawing.Point(215, 344)
        Me.TxtLotNoEdaNo.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtLotNoEdaNo.MaxLength = 15
        Me.TxtLotNoEdaNo.Name = "TxtLotNoEdaNo"
        Me.TxtLotNoEdaNo.Size = New System.Drawing.Size(204, 26)
        Me.TxtLotNoEdaNo.TabIndex = 3
        Me.TxtLotNoEdaNo.Visible = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(22, 347)
        Me.Label3.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(86, 19)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "LOT BAG"
        Me.Label3.Visible = False
        '
        'TxtLotNo
        '
        Me.TxtLotNo.Location = New System.Drawing.Point(215, 255)
        Me.TxtLotNo.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtLotNo.MaxLength = 11
        Me.TxtLotNo.Name = "TxtLotNo"
        Me.TxtLotNo.Size = New System.Drawing.Size(204, 26)
        Me.TxtLotNo.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(22, 258)
        Me.Label4.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(53, 19)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "LOT#"
        '
        'TxtQuantity
        '
        Me.TxtQuantity.Location = New System.Drawing.Point(215, 210)
        Me.TxtQuantity.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtQuantity.MaxLength = 4
        Me.TxtQuantity.Name = "TxtQuantity"
        Me.TxtQuantity.ReadOnly = True
        Me.TxtQuantity.Size = New System.Drawing.Size(204, 26)
        Me.TxtQuantity.TabIndex = 5
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(22, 213)
        Me.Label5.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(47, 19)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "重量"
        '
        'TxtFixNo
        '
        Me.TxtFixNo.Location = New System.Drawing.Point(603, 112)
        Me.TxtFixNo.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtFixNo.MaxLength = 5
        Me.TxtFixNo.Name = "TxtFixNo"
        Me.TxtFixNo.Size = New System.Drawing.Size(204, 26)
        Me.TxtFixNo.TabIndex = 8
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(467, 115)
        Me.Label6.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(85, 19)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "固定番号"
        '
        'TxtRecNo
        '
        Me.TxtRecNo.Location = New System.Drawing.Point(603, 159)
        Me.TxtRecNo.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtRecNo.MaxLength = 6
        Me.TxtRecNo.Name = "TxtRecNo"
        Me.TxtRecNo.ReadOnly = True
        Me.TxtRecNo.Size = New System.Drawing.Size(204, 26)
        Me.TxtRecNo.TabIndex = 9
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(467, 162)
        Me.Label7.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(47, 19)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "連番"
        '
        'TxtCheckDigit
        '
        Me.TxtCheckDigit.Location = New System.Drawing.Point(603, 210)
        Me.TxtCheckDigit.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtCheckDigit.MaxLength = 1
        Me.TxtCheckDigit.Name = "TxtCheckDigit"
        Me.TxtCheckDigit.ReadOnly = True
        Me.TxtCheckDigit.Size = New System.Drawing.Size(204, 26)
        Me.TxtCheckDigit.TabIndex = 10
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(467, 210)
        Me.Label8.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(126, 19)
        Me.Label8.TabIndex = 14
        Me.Label8.Text = "チェックディジット"
        '
        'TxtPrtCnt
        '
        Me.TxtPrtCnt.Location = New System.Drawing.Point(732, 344)
        Me.TxtPrtCnt.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtPrtCnt.MaxLength = 1
        Me.TxtPrtCnt.Name = "TxtPrtCnt"
        Me.TxtPrtCnt.Size = New System.Drawing.Size(54, 26)
        Me.TxtPrtCnt.TabIndex = 11
        Me.TxtPrtCnt.Text = "2"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(599, 347)
        Me.Label9.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(123, 19)
        Me.Label9.TabIndex = 16
        Me.Label9.Text = "印刷枚数設定"
        '
        'BtnPrint
        '
        Me.BtnPrint.Location = New System.Drawing.Point(686, 383)
        Me.BtnPrint.Margin = New System.Windows.Forms.Padding(5)
        Me.BtnPrint.Name = "BtnPrint"
        Me.BtnPrint.Size = New System.Drawing.Size(157, 70)
        Me.BtnPrint.TabIndex = 18
        Me.BtnPrint.Text = "印刷"
        Me.BtnPrint.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("MS UI Gothic", 20.0!)
        Me.Label10.Location = New System.Drawing.Point(14, 19)
        Me.Label10.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(174, 27)
        Me.Label10.TabIndex = 19
        Me.Label10.Text = "印刷設定画面"
        '
        'TxtMrms
        '
        Me.TxtMrms.Location = New System.Drawing.Point(215, 159)
        Me.TxtMrms.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtMrms.MaxLength = 8
        Me.TxtMrms.Name = "TxtMrms"
        Me.TxtMrms.Size = New System.Drawing.Size(204, 26)
        Me.TxtMrms.TabIndex = 4
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(22, 162)
        Me.Label11.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(184, 19)
        Me.Label11.TabIndex = 22
        Me.Label11.Text = "IRMS/GCAS#、MRMS"
        '
        'TxtBag
        '
        Me.TxtBag.Location = New System.Drawing.Point(603, 56)
        Me.TxtBag.Margin = New System.Windows.Forms.Padding(5)
        Me.TxtBag.MaxLength = 3
        Me.TxtBag.Name = "TxtBag"
        Me.TxtBag.Size = New System.Drawing.Size(204, 26)
        Me.TxtBag.TabIndex = 7
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(467, 65)
        Me.Label12.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(56, 19)
        Me.Label12.TabIndex = 24
        Me.Label12.Text = "BAG#"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(429, 217)
        Me.Label13.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(29, 19)
        Me.Label13.TabIndex = 26
        Me.Label13.Text = "Kg"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(796, 351)
        Me.Label14.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(28, 19)
        Me.Label14.TabIndex = 26
        Me.Label14.Text = "枚"
        '
        'RdoAuto
        '
        Me.RdoAuto.AutoSize = True
        Me.RdoAuto.Checked = True
        Me.RdoAuto.Location = New System.Drawing.Point(15, 21)
        Me.RdoAuto.Name = "RdoAuto"
        Me.RdoAuto.Size = New System.Drawing.Size(65, 23)
        Me.RdoAuto.TabIndex = 27
        Me.RdoAuto.TabStop = True
        Me.RdoAuto.Text = "自動"
        Me.RdoAuto.UseVisualStyleBackColor = True
        '
        'RdoManual
        '
        Me.RdoManual.AutoSize = True
        Me.RdoManual.Location = New System.Drawing.Point(86, 21)
        Me.RdoManual.Name = "RdoManual"
        Me.RdoManual.Size = New System.Drawing.Size(65, 23)
        Me.RdoManual.TabIndex = 28
        Me.RdoManual.Text = "手動"
        Me.RdoManual.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.RdoManual)
        Me.GroupBox1.Controls.Add(Me.RdoAuto)
        Me.GroupBox1.Location = New System.Drawing.Point(246, 7)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(173, 50)
        Me.GroupBox1.TabIndex = 29
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "印刷モード設定"
        '
        'Frm_IndoramaPrt
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 19.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(857, 467)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.TxtBag)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.TxtMrms)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.BtnPrint)
        Me.Controls.Add(Me.TxtPrtCnt)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.TxtCheckDigit)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.TxtRecNo)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.TxtFixNo)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.TxtQuantity)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.TxtLotNo)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TxtLotNoEdaNo)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtItemName)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TxtSeisanDate)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("MS UI Gothic", 14.0!)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.Name = "Frm_IndoramaPrt"
        Me.Text = "印刷設定画面"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents TxtSeisanDate As TextBox
    Friend WithEvents TxtItemName As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtLotNoEdaNo As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TxtLotNo As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents TxtQuantity As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TxtFixNo As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents TxtRecNo As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents TxtCheckDigit As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents TxtPrtCnt As TextBox
    Friend WithEvents Label9 As Label
    Friend WithEvents BtnPrint As Button
    Friend WithEvents Label10 As Label
    Friend WithEvents TxtMrms As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents TxtBag As TextBox
    Friend WithEvents Label12 As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents RdoAuto As RadioButton
    Friend WithEvents RdoManual As RadioButton
    Friend WithEvents GroupBox1 As GroupBox
End Class
