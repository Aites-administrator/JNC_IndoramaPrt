Public Class ClsSqlServer

  Inherits ClsComDatabase
  Public Sub New()
    DataSource = ClsGlobalData.DB_DATASOURCE
    DefaultDatabase = ClsGlobalData.DB_DEFAULTDATABASE
    UserId = ClsGlobalData.DB_USERID
    Password = ClsGlobalData.DB_PASSWORD
  End Sub

End Class
