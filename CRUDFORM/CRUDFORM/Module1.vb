Imports System.Data.OracleClient

Module Module1
    Public start As Boolean

    Public Function oracleConnection() As OracleConnection
        Dim conn As New OracleConnection("SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.98)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=Training2)));uid=SYSTEM;pwd=123456")
        Return conn
    End Function

End Module
