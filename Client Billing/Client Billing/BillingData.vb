Imports System.IO

Public Class BillingData
    Private ReadOnly clientCollection As New List(Of String)

    Public ReadOnly Property ClientColl As List(Of String)
        Get
            Return New List(Of String)(clientCollection)
        End Get
    End Property

    Public Sub WriteBilling(record As String)
        Dim billingPath As String = Path.Combine(Application.StartupPath, "billing.txt")

        Using billingFile As New StreamWriter(billingPath, True)
            billingFile.WriteLine(record)
        End Using
    End Sub

    Public Sub CloseBilling()
        ' Billing records are written and closed immediately so the file is never left locked.
    End Sub

    Sub New()
        LoadClients()
    End Sub

    Private Sub LoadClients()
        Dim clientPath As String = FindClientFile()

        If clientPath = String.Empty Then
            MessageBox.Show("Clients file not found. Add clients.txt or clients.txt.txt to the application folder.")
            Return
        End If

        Using clientFile As New StreamReader(clientPath)
            While clientFile.Peek <> -1
                Dim clientLine As String = clientFile.ReadLine()
                Dim clients() As String = clientLine.Split(New Char() {","c})

                For Each client As String In clients
                    AddClient(client)
                Next
            End While
        End Using

        clientCollection.Sort()
    End Sub

    Private Sub AddClient(clientName As String)
        Dim cleanName As String = clientName.Trim()

        If cleanName = String.Empty Then
            Return
        End If

        If Not clientCollection.Exists(Function(existingClient) String.Equals(existingClient, cleanName, StringComparison.OrdinalIgnoreCase)) Then
            clientCollection.Add(cleanName)
        End If
    End Sub

    Private Function FindClientFile() As String
        ' Some project copies used clients.txt.txt, so support both names while preferring clients.txt.
        Dim clientFiles() As String = {
            Path.Combine(Application.StartupPath, "clients.txt"),
            Path.Combine(Application.StartupPath, "clients.txt.txt"),
            Path.Combine(Directory.GetCurrentDirectory(), "clients.txt"),
            Path.Combine(Directory.GetCurrentDirectory(), "clients.txt.txt")
        }

        For Each clientPath As String In clientFiles
            If File.Exists(clientPath) Then
                Return clientPath
            End If
        Next

        Return String.Empty
    End Function
End Class
