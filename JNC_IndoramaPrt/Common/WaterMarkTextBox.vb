Option Strict On
Imports System
Imports System.Windows.Forms
Imports System.Drawing
Public Class WaterMarkTextBox
    Inherits TextBox

    Private oldFont As Font = Nothing
    Private waterMarkTextEnabled As Boolean = False

#Region "Attributes"
    Private _waterMarkColor As Color = Drawing.Color.Gray

    Public Property WaterMarkColor() As Color
        Get
            Return _waterMarkColor
        End Get
        Set(ByVal value As Color)
            _waterMarkColor = value
            Invalidate()
        End Set
    End Property

    Private _waterMarkText As String = "Water Mark"

    Public Property WaterMarkText() As String
        Get
            Return _waterMarkText
        End Get
        Set(ByVal value As String)
            _waterMarkText = value
            Invalidate()
        End Set
    End Property
#End Region

    Public Sub New()
        JoinEvents(True)
    End Sub

    Private Sub JoinEvents(ByVal join As Boolean)
        If join Then
            AddHandler(TextChanged), AddressOf WaterMark_Toggle
            AddHandler(LostFocus), AddressOf WaterMark_Toggle
            AddHandler(FontChanged), AddressOf WaterMark_FontChanged
        End If
    End Sub

    Private Sub WaterMark_Toggle(ByVal sender As Object, ByVal args As EventArgs)
        If Text.Length <= 0 Then
            EnableWaterMark()
        Else
            DisableWaterMark()
        End If
    End Sub

    Private Sub WaterMark_FontChanged(ByVal sender As Object, ByVal args As EventArgs)
        If waterMarkTextEnabled Then
            oldFont = New Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit)
            Refresh()
        End If
    End Sub

    Private Sub EnableWaterMark()
        oldFont = New Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit)

        SetStyle(ControlStyles.UserPaint, True)
        waterMarkTextEnabled = True

        Refresh()

    End Sub

    Private Sub DisableWaterMark()
        waterMarkTextEnabled = False
        SetStyle(ControlStyles.UserPaint, False)

        If oldFont IsNot Nothing Then
            Font = New Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit)
        End If
    End Sub

    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        WaterMark_Toggle(Nothing, Nothing)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        Dim drawFont As New Font(Font.FontFamily, Font.Size, Font.Style, Font.Unit)
        Dim drawBrush As New SolidBrush(WaterMarkColor)
        e.Graphics.DrawString(IIf(waterMarkTextEnabled, WaterMarkText, Text).ToString(), drawFont, drawBrush, New Point(0, 0))
        MyBase.OnPaint(e)
    End Sub

End Class
