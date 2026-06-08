Public Class TimeClock
    Public Property ClientName As String
    Private startApp As DateTime
    Private stopApp As DateTime
    Private isRunning As Boolean = False

    Public ReadOnly Property StartTime As DateTime
        Get
            Return startApp
        End Get
    End Property

    Public ReadOnly Property StopTime As DateTime
        Get
            Return stopApp
        End Get
    End Property

    Public ReadOnly Property Elapsed As TimeSpan
        Get
            If isRunning Then
                Return Now.Subtract(startApp)
            Else
                Return stopApp.Subtract(startApp)
            End If
        End Get
    End Property

    Public ReadOnly Property TotalElapsed As TimeSpan
        Get
            If isRunning Then
                Return Now.Subtract(startApp)
            Else
                Return stopApp.Subtract(startApp)
            End If
        End Get
    End Property

    Public Sub Start()
        startApp = Now
        isRunning = True
    End Sub

    Public Sub StopClock()
        If isRunning Then
            stopApp = Now
            isRunning = False
        End If
    End Sub

    Public Sub Reset()
        startApp = DateTime.MinValue
        stopApp = DateTime.MinValue
        isRunning = False
    End Sub
End Class