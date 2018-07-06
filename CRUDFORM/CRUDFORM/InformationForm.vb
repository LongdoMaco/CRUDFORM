Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Threading

Public Class InformationForm

#Region "variable and constant"
    Private da As OracleDataAdapter
    Private ds As DataSet
    Private idUser As String
    Public Delegate Sub sendDataHandle()
    Private th_GoiDuLieu As Thread
    Private blnFinishEdit As Boolean
    Private transaction As OracleTransaction
#End Region

#Region "init"

    Public Sub New(ByVal id As String)
        InitializeComponent()
        idUser = id
    End Sub

    Private Sub InformationForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitCombobox()
    End Sub

    Private Sub InitCombobox()
        Dim tbQuery As String
        tbQuery = "SELECT DEPARTMENT_CODE, DEPARTMENT_NAME FROM DEPARTMENTS"
        Dim cmd As New OracleCommand(tbQuery, oracleConnection)
        cmd.CommandType = CommandType.Text

        oracleConnection.Open()
        cmd = New OracleCommand(tbQuery, oracleConnection)
        cmd.CommandType = CommandType.Text

        da = New OracleDataAdapter(cmd)
        ds = New DataSet()

        da.Fill(ds)

        ccbDepartment.DataSource = ds.Tables(0)
        ccbDepartment.ValueMember = "DEPARTMENT_CODE"
        ccbDepartment.DisplayMember = "DEPARTMENT_NAME"

        oracleConnection.Close()
    End Sub

#End Region

#Region "thread"
    Public Sub startThreading()
        th_GoiDuLieu = New Thread(AddressOf handlingSendData)
        th_GoiDuLieu.IsBackground = True
        th_GoiDuLieu.Start()
    End Sub


    Public Sub handlingSendData()
        While True
            If blnFinishEdit = True Then
                Me.BeginInvoke(New sendDataHandle(AddressOf sendData))
            End If
            Thread.Sleep(300)
            Exit Sub
        End While
    End Sub

    Public Sub sendData()
        start = True
    End Sub

#End Region

#Region "button function"

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim tbQuery As String
        Dim strDate As String = DateTime.Now.ToString("dd - MMM - yy", CultureInfo.CreateSpecificCulture("en-US"))

        Try
            'check validate null
            If checkValidateUsernameAndPassword() = False Then
                MessageBox.Show("Please enter username and password!", "Warning")
                Exit Sub
            End If

            'check validate null and number
            If checkValidateStatus() = False Then
                MessageBox.Show("Status must be a number!", "Warning")
                Exit Sub
            End If

            If String.IsNullOrEmpty(idUser) Then
                tbQuery = "INSERT INTO USERS (ID, USERNAME, PASSWORD, EMAIL, DEPARTMENT_CODE, STATUS, CREATED_AT) " _
                            + "VALUES(SEQ_USERS.NEXTVAL, '" _
                            + txtUsername.Text.ToString + "', '" _
                            + txtPassword.Text.ToString + "', '" _
                            + txtEmail.Text.ToString + "', " _
                            + ccbDepartment.SelectedValue.ToString _
                            + "," + txtStatus.Text.ToString + ",'" _
                            + strDate + "')"
            Else
                tbQuery = "UPDATE USERS SET USERNAME= '" + txtUsername.Text.ToString _
                            + "',PASSWORD= '" + txtPassword.Text.ToString _
                            + "',EMAIL='" + txtEmail.Text.ToString _
                            + "', DEPARTMENT_CODE =" + ccbDepartment.SelectedValue.ToString _
                            + ", STATUS=" + txtStatus.Text.ToString + ", UPDATED_AT='" _
                            + strDate + "'  WHERE ID = '" + idUser + "'"
            End If

            oracleConnection.Open()
            Dim cmd As New OracleCommand(tbQuery, oracleConnection)
            cmd.CommandType = CommandType.Text
            cmd.Connection.Open()
            cmd.ExecuteNonQuery()
            blnFinishEdit = True
            startThreading()

            oracleConnection.Close()

        Catch ex As Exception
            MessageBox.Show("Error!", "Warning!")
        Finally
            oracleConnection.Close()
        End Try

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

#End Region

#Region "validate"

    Private Function checkValidateUsernameAndPassword() As Boolean
        If String.IsNullOrEmpty(txtUsername.Text) OrElse String.IsNullOrEmpty(txtPassword.Text) Then
            Return False
        End If
        Return True
    End Function

    Private Function checkValidateStatus() As Boolean
        If String.IsNullOrEmpty(txtStatus.Text) OrElse Not IsNumeric(txtStatus.Text) Then
            Return False
        End If
        Return True
    End Function

    ''todo
#End Region

End Class