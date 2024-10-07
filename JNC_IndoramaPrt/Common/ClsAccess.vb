Public Class ClsAccess

    Inherits ClsComDatabase
    Public Sub New(prmFileName As String)
        Me.DataSource = prmFileName
        Me.Provider = TypProvider.Accdb
    End Sub

End Class
