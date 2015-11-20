Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports RDP.Networking

Public Class Client
    Shared _client As New SocketHandler.Client()
    Shared _clientConnected As Boolean = False
    Shared clientWidth As Integer
    Shared clientHeight As Integer

    Private Sub cmdConnect_Click(sender As Object, e As EventArgs) Handles cmdConnect.Click
        AddHandler _client.OnDataRetrieved, Sub(sender1, data) _client_OnDataRetrieved(sender1, data)
        AddHandler _client.OnDisconnect, Sub(sender1, er) _client_OnDisconnect(sender1, er)
        If _client.Connect(txtIP.Text, 8080) Then
            Console.WriteLine("Connected.")
            _clientConnected = True
        Else
            Console.WriteLine("Cannot Connect")
            _clientConnected = False
        End If
    End Sub

    Private Shared Sub _client_OnDisconnect(sender As SocketHandler.Client, ER As SocketError)
        Console.WriteLine("Disconnected.")
        _clientConnected = False
    End Sub

    Private Sub _client_OnDataRetrieved(sender As SocketHandler.Client, data As Object())
        Console.WriteLine("Data Received: {0}", data(0))
        picDesktop.Image = data(0)
        clientWidth = picDesktop.Image.Width
        clientHeight = picDesktop.Image.Height
    End Sub

    Private Sub picDesktop_MouseMove(sender As Object, e As MouseEventArgs) Handles picDesktop.MouseMove
        Console.WriteLine("X: {0}, Y: {1}", e.X, e.Y)
        If _clientConnected Then
            _client.Send(String.Format("M:{0}:{1}", ((e.X / picDesktop.Size.Width) * clientWidth).ToString(), ((e.Y / picDesktop.Size.Height) * clientHeight).ToString()))
        End If
    End Sub
End Class
