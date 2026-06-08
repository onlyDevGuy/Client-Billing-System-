Public Class frmClientBilling
    Private ReadOnly clock As New TimeClock
    Private ReadOnly billData As New BillingData
    Private validatedClient As String = String.Empty

    Private Sub frmClientBilling_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim clients As List(Of String) = billData.ClientColl

        cboClient.DropDownStyle = ComboBoxStyle.DropDownList
        cboClient.DataSource = clients
        cboClient.SelectedIndex = If(clients.Count > 0, 0, -1)

        btnStart.Enabled = clients.Count > 0
        btnStop.Enabled = False
        lblDuration.Visible = False
        lblStartTime.Text = String.Empty
        lblStopTime.Text = String.Empty

        If clients.Count = 0 Then
            MessageBox.Show("No clients are available to select. Please add client names to the clients file.")
        End If
    End Sub

    Private Sub cboClient_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboClient.SelectedIndexChanged
        ' Any new selection must be validated before billing can start for that client.
        validatedClient = String.Empty
    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        If Not ValidateSelectedClient() Then
            Return
        End If

        clock.ClientName = validatedClient
        Me.Text = "Client Billing - " & validatedClient
        clock.Start()

        lblStartTime.Text = clock.StartTime.ToShortTimeString()
        lblStopTime.Text = String.Empty
        lblDuration.Text = String.Empty
        lblDuration.Visible = True

        btnStart.Enabled = False
        btnStop.Enabled = True
        btnExit.Enabled = False
        cboClient.Enabled = False
    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        clock.StopClock()

        btnStart.Enabled = True
        btnStop.Enabled = False
        btnExit.Enabled = True
        cboClient.Enabled = True

        lblStopTime.Text = clock.StopTime.ToShortTimeString()
        lblDuration.Text = String.Format("{0}:{1:00}:{2:00}", clock.Elapsed.Hours, clock.Elapsed.Minutes, clock.Elapsed.Seconds)

        With clock
            billData.WriteBilling(
                Now.ToShortDateString() & ", " &
                .ClientName & ", " &
                .StartTime.ToShortTimeString() & ", " &
                .StopTime.ToShortTimeString() & ", " &
                .TotalElapsed.Hours & ", " &
                .TotalElapsed.Minutes & ", " &
                .TotalElapsed.Seconds
            )
        End With
    End Sub

    Private Function ValidateSelectedClient() As Boolean
        If cboClient.SelectedItem Is Nothing Then
            MessageBox.Show("Select a client from the list before starting billing.")
            Return False
        End If

        Dim selectedClient As String = cboClient.SelectedItem.ToString()

        If String.Equals(validatedClient, selectedClient, StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If

        Dim confirmation As DialogResult = MessageBox.Show(
            "You selected " & selectedClient & "." & Environment.NewLine &
            "Confirm that you want to access and bill this client's information.",
            "Confirm client access",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

        If confirmation <> DialogResult.Yes Then
            MessageBox.Show("Client access was cancelled.")
            Return False
        End If

        Dim validationText As String = InputBox(
            "For privacy, type your Windows user name before continuing.",
            "Validate client access"
        ).Trim()

        ' This is a lightweight desktop validation gate. Replace this with client PINs or login credentials if sensitive details are later added.
        If String.Equals(validationText, Environment.UserName, StringComparison.OrdinalIgnoreCase) Then
            validatedClient = selectedClient
            Return True
        End If

        MessageBox.Show("Validation failed. Client billing was not started.")
        Return False
    End Function

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub
End Class
