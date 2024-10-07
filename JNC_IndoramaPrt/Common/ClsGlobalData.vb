Imports Common.ClsFunction
Public Class ClsGlobalData
  'Public Shared ReadOnly DB_DATASOURCE As String = "NN2205001\SQLEXPRESS"
  ''Public Shared ReadOnly DB_DATASOURCE As String = "(local)\SANPAIDX"
  'Public Shared ReadOnly DB_DEFAULTDATABASE As String = "SANPAI"
  'Public Shared ReadOnly DB_USERID As String = "sa"
  'Public Shared ReadOnly DB_PASSWORD As String = "495344"
  Public Shared ReadOnly DB_DATASOURCE As String = ReadConnectIniFile("DB_DATASOURCE", "VALUE")
  Public Shared ReadOnly DB_DEFAULTDATABASE As String = ReadConnectIniFile("DB_DEFAULTDATABASE", "VALUE")
  Public Shared ReadOnly DB_USERID As String = ReadConnectIniFile("DB_USERID", "VALUE")
  Public Shared ReadOnly DB_PASSWORD As String = ReadConnectIniFile("DB_PASSWORD", "VALUE")

  ' 得意先コードゼロ詰め
  Public Shared ReadOnly CUSTOMER_ZERO_PADDING As String = "000000"
  ' 直送先コードゼロ詰め
  Public Shared ReadOnly CHOKU_ZERO_PADDING As String = "00"

  ''' <summary>
  ''' 印刷帳票の保存先
  ''' </summary>
  Public Shared ReadOnly REPORT_FILENAME As String = "Report.accdb"
  ''' <summary>
  ''' 印刷プレビューフラグ
  ''' </summary>
  Public Shared ReadOnly PRINT_PREVIEW As Integer = 1     '0：プレビューしない、1：プレビューする
  ''' <summary>
  ''' 印刷プレビューフラグ
  ''' </summary>
  Public Shared ReadOnly PRINT_NON_PREVIEW As Integer = 0     '0：プレビューしない、1：プレビューする
  ''' <summary>
  ''' 印刷用Access元ファイル
  ''' </summary>
  ''' <remarks>
  '''  実行時は本ファイルを実行ファイルと同じフォルダにコピーして使用する
  ''' </remarks>
  Public Shared ReadOnly REPORT_ORG_FILEPATH As String = "C:\JNC\REPORT\Report_org.accdb"
    ''' <summary>
    ''' 印刷用Access元ファイル
    ''' </summary>
    ''' <remarks>
    '''  実行時は本ファイルを実行ファイルと同じフォルダにコピーして使用する
    ''' </remarks>
    Public Shared ReadOnly DATA_FILEPATH As String = "C:\JNC\DAT\Data.accdb"


End Class
