
Imports System.Data.OracleClient
Imports System.Threading

Public Class MainForm

#Region "variable and constant"
    Private da As OracleDataAdapter
    Private ds As DataSet
    Private intRowCount As Integer
    Private selectRowCount As Integer = 0
    Public Delegate Sub processData()
    Private threadGetData As Thread
#End Region

#Region "struct"

    Enum DataGridViewArray
        Checkbox
        Username
        Email
        Department
        Status
        CreatedAt
        ID
        Password
        MaxIndex
    End Enum

#End Region

#Region "init"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dgvResult.Columns(5).DefaultCellStyle.Format = "dd-MM-yyyy"
        lblDateTime.Text = DateTime.Now.ToString("yyyy/MM/dd")
        LoadDataGrid()
        btnEdit.Enabled = False
    End Sub

#End Region

#Region "method"

    Private Sub LoadDataGrid()
        Dim objRowData(DataGridViewArray.MaxIndex) As Object
        Dim tbQuery As String
        tbQuery = "SELECT USERS.ID, " _
                           + "USERS.USERNAME, " _
                           + "USERS.PASSWORD, " _
                           + "USERS.EMAIL, " _
                           + "DEPARTMENTS.DEPARTMENT_NAME, " _
                           + "USERS.STATUS, USERS.CREATED_AT " _
                           + "FROM USERS JOIN DEPARTMENTS " _
                           + "ON USERS.DEPARTMENT_CODE = DEPARTMENTS.DEPARTMENT_CODE ORDER BY USERS.ID ASC"
        Dim cmd As New OracleCommand(tbQuery, oracleConnection)
        cmd.CommandType = CommandType.Text

        oracleConnection.Open()
        cmd = New OracleCommand(tbQuery, oracleConnection)
        cmd.CommandType = CommandType.Text

        da = New OracleDataAdapter(cmd)
        ds = New DataSet()

        da.Fill(ds)
        dgvResult.Rows.Clear()
        intRowCount = 0

        For Each dr As DataRow In ds.Tables(0).Rows

            objRowData(DataGridViewArray.Checkbox) = False

            objRowData(DataGridViewArray.Username) = dr("USERNAME").ToString()

            objRowData(DataGridViewArray.Email) = dr("EMAIL").ToString()

            objRowData(DataGridViewArray.Department) = dr("DEPARTMENT_NAME").ToString()

            objRowData(DataGridViewArray.Status) = dr("STATUS").ToString()

            objRowData(DataGridViewArray.CreatedAt) = Convert.ToDateTime(dr("CREATED_AT").ToString())

            objRowData(DataGridViewArray.ID) = dr("ID").ToString()

            objRowData(DataGridViewArray.Password) = dr("PASSWORD").ToString()

            dgvResult.Rows.Insert(intRowCount, objRowData)

            intRowCount += 1

        Next

        oracleConnection.Close()
    End Sub

    Private Sub DatagridviewToExcel(ByVal DGV As DataGridView)
        Try
            Dim DTB = New DataTable, RWS As Integer, CLS As Integer

            For CLS = 0 To DGV.ColumnCount - 3 ' COLUMNS OF DTB
                DTB.Columns.Add(DGV.Columns(CLS).Name.ToString)
            Next

            Dim DRW As DataRow

            For RWS = 1 To DGV.Rows.Count - 1 ' FILL DTB WITH DATAGRIDVIEW
                DRW = DTB.NewRow

                For CLS = 1 To DGV.ColumnCount - 3
                    Try
                        DRW(DTB.Columns(CLS).ColumnName.ToString) = DGV.Rows(RWS).Cells(CLS).Value.ToString
                    Catch ex As Exception

                    End Try
                Next

                DTB.Rows.Add(DRW)
            Next

            DTB.AcceptChanges()

            Dim DST As New DataSet
            DST.Tables.Add(DTB)
            Dim FLE As String = "D:\Software\xml.xml" ' PATH AND FILE NAME WHERE THE XML WIL BE CREATED (EXEMPLE: C:\REPS\XML.xml)
            DTB.WriteXml(FLE)
            Dim EXL As String = "C:\Program Files\Microsoft Office\Office16\EXCEL.EXE" ' PATH OF/ EXCEL.EXE IN YOUR MICROSOFT OFFICE
            Shell(Chr(34) & EXL & Chr(34) & " " & Chr(34) & FLE & Chr(34), vbNormalFocus) ' OPEN XML WITH EXCEL

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

#End Region

#Region "thread"

    Public Sub XuLy()
        Try
            While True
                If start = True Then
                    Me.BeginInvoke(New processData(AddressOf LoadDataGrid))
                End If
                start = False
                Thread.Sleep(100)
            End While
        Catch ex As Exception
            MessageBox.Show("Error!", "Warning!")
        End Try

    End Sub

    Public Sub start_threading()
        threadGetData = New Thread(AddressOf XuLy)
        threadGetData.IsBackground = True
        threadGetData.Start()
    End Sub


#End Region

#Region "event"

    Private Sub dgvResult_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvResult.CellContentClick
        dgvResult.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Private Sub dgvResult_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvResult.CellValueChanged
        Try
            selectRowCount = 0
            If e.ColumnIndex = 0 Then
                For intIndex As Integer = 0 To dgvResult.Rows.Count - 1 Step 1
                    If Convert.ToBoolean(dgvResult.Rows(intIndex).Cells(DataGridViewArray.Checkbox).Value) = True Then
                        selectRowCount = selectRowCount + 1
                    End If
                Next

            End If
            If selectRowCount = 1 Then
                btnEdit.Enabled = True
            Else
                btnEdit.Enabled = False
            End If
        Catch ex As Exception
            MessageBox.Show("Error!", "Warning!")
        End Try

    End Sub

    Private Sub txtSearch_GotFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearch.GotFocus
        txtSearch.Text = ""
    End Sub

    Private Sub txtSearch_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearch.LostFocus
        If txtSearch.Text = "" Then
            txtSearch.Text = "Search by username or email"
        End If
    End Sub

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.F1) Then
            btnAddNew.PerformClick()
        End If
        If (e.KeyCode = Keys.F2) Then
            btnEdit.PerformClick()
        End If
        If (e.KeyCode = Keys.F3) Then
            btnDelete.PerformClick()
        End If
        If (e.KeyCode = Keys.F4) Then
            btnExport.PerformClick()
        End If
        If (e.KeyCode = Keys.F5) Then
            Me.Close()
        End If

    End Sub

#End Region

#Region "button function"
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim objRowData(DataGridViewArray.MaxIndex) As Object
        Dim tbQuery As String

        Try
            tbQuery = "SELECT USERS.ID, " _
                           + "USERS.USERNAME, " _
                           + "USERS.PASSWORD, " _
                           + "USERS.EMAIL, " _
                           + "DEPARTMENTS.DEPARTMENT_NAME, " _
                           + "USERS.STATUS, " _
                           + "USERS.CREATED_AT " _
                           + "FROM USERS JOIN DEPARTMENTS ON USERS.DEPARTMENT_CODE=DEPARTMENTS.DEPARTMENT_CODE " _
                           + "WHERE " _
                           + "USERS.USERNAME Like '%" + txtSearch.Text + "%'" _
                           + "OR USERS.EMAIL LIKE '%" + txtSearch.Text + "%'"

            Dim cmd As New OracleCommand(tbQuery, oracleConnection)
            cmd.CommandType = CommandType.Text

            oracleConnection.Open()

            da = New OracleDataAdapter(cmd)
            ds = New DataSet()

            da.Fill(ds)
            dgvResult.Rows.Clear()
            intRowCount = 0
            For Each dr As DataRow In ds.Tables(0).Rows

                objRowData(DataGridViewArray.Checkbox) = False

                objRowData(DataGridViewArray.Username) = dr("USERNAME").ToString()

                objRowData(DataGridViewArray.Email) = dr("EMAIL").ToString()

                objRowData(DataGridViewArray.Department) = dr("DEPARTMENT_NAME").ToString()

                objRowData(DataGridViewArray.Status) = dr("STATUS").ToString()

                objRowData(DataGridViewArray.CreatedAt) = Convert.ToDateTime(dr("CREATED_AT").ToString())

                objRowData(DataGridViewArray.ID) = dr("ID").ToString()

                objRowData(DataGridViewArray.Password) = dr("PASSWORD").ToString()

                dgvResult.Rows.Insert(intRowCount, objRowData)

                intRowCount += 1

            Next
            oracleConnection.Close()
        Catch ex As Exception
            MessageBox.Show("Error!", "Warning!")
        End Try

    End Sub

    Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
        start_threading()
        Dim form As New InformationForm("")
        form.Show()
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        start_threading()
        For intIndex As Integer = 0 To dgvResult.Rows.Count - 1 Step 1

            If Convert.ToBoolean(dgvResult.Rows(intIndex).Cells(DataGridViewArray.Checkbox).Value) = True Then
                Dim form As New InformationForm(dgvResult.Rows(intIndex).Cells(DataGridViewArray.ID).Value.ToString)
                form.txtUsername.Text = dgvResult.Rows(intIndex).Cells(DataGridViewArray.Username).Value.ToString
                form.txtEmail.Text = dgvResult.Rows(intIndex).Cells(DataGridViewArray.Email).Value.ToString
                form.ccbDepartment.SelectedText = dgvResult.Rows(intIndex).Cells(DataGridViewArray.Department).Value.ToString
                form.txtStatus.Text = dgvResult.Rows(intIndex).Cells(DataGridViewArray.Status).Value.ToString
                form.txtPassword.Text = dgvResult.Rows(intIndex).Cells(DataGridViewArray.Password).Value.ToString
                form.Show()

            End If

        Next
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim tbQuery As String
        Dim transaction As OracleTransaction
        Dim myCommand As New OracleCommand

        Using conn = oracleConnection()
            conn.Open()
            transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted)
            myCommand.Transaction = transaction
            myCommand.Connection = conn

            Try
                For intIndex As Integer = dgvResult.Rows.Count - 1 To 0 Step -1

                    If Convert.ToBoolean(dgvResult.Rows(intIndex).Cells(DataGridViewArray.Checkbox).Value) = True Then
                        tbQuery = "DELETE FROM USERS WHERE ID= '" + dgvResult.Rows(intIndex).Cells(DataGridViewArray.ID).Value.ToString + "'"
                        myCommand.CommandText = tbQuery
                        myCommand.ExecuteNonQuery()
                    End If

                Next

                transaction.Commit()
                LoadDataGrid()
                selectRowCount = 0
                btnEdit.Enabled = False

            Catch ex As Exception
                transaction.Rollback()
                MessageBox.Show("Error!", "Warning!")
            Finally
                oracleConnection.Close()
            End Try

        End Using

    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        DatagridviewToExcel((dgvResult))
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#End Region

End Class
