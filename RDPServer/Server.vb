Imports RDPServer.Networking
Imports System
Imports System.Net.Sockets
Imports System.Threading

Public Class Server

    Shared ReadOnly screenThread As New Thread(New ThreadStart(AddressOf SendDesktop))
    Shared ReadOnly dataHandlerThread As New Thread(New ThreadStart(AddressOf DataHandler))
    Shared ReadOnly _server As New SocketHandler.Server
    Shared _clientConnected As Boolean = False
    Shared _client As SocketHandler.Server.SocketClient
    Shared _bounds As Rectangle = Screen.PrimaryScreen.Bounds
    Shared dataObject As Object
    Shared dataObjectOld As Object

    Private Sub cmdListen_Click(sender As Object, e As EventArgs) Handles cmdListen.Click
        AddHandler _server.OnDataRetrieved, Sub(sender1, client, data) _server_OnDataRetrieved(sender1, client, data)
        AddHandler _server.OnClientDisconnect, Sub(sender1, client, er) _server_OnClientDisconnect(sender1, client, er)
        AddHandler _server.OnClientConnect, Sub(sender1, client) _server_OnClientConnect(sender1, client)
        _server.Start(8080)
        Console.WriteLine("Started on 8080...")
    End Sub
    Private Shared Sub _server_OnDataRetrieved(sender As SocketHandler.Server, client As SocketHandler.Server.SocketClient, data As Object())
        Console.WriteLine("Message: {0}", data(0))
        dataObject = data
    End Sub

    Private Shared Sub DataHandler()
        While _clientConnected
            If Not dataObject Is Nothing Then
                If Not dataObjectOld Is dataObject Then
                    dataObjectOld = dataObject
                End If
                Dim dataPieces = dataObjectOld(0).ToString().Split(":")
                If (dataPieces(0) = "M") Then
                    HandleMouse(Integer.Parse(dataPieces(1).Split(".")(0)), Integer.Parse(dataPieces(2).Split(".")(0)))
                End If
            End If
        End While
    End Sub

    Private Shared Sub HandleMouse(X As Integer, Y As Integer)
        Cursor.Position = New Point(X, Y)
    End Sub
    Private Shared Sub _server_OnClientDisconnect(sender As SocketHandler.Server, client As SocketHandler.Server.SocketClient, ER As SocketError)
        screenThread.Abort()
        dataHandlerThread.Abort()
        Console.WriteLine("Disconnected: {0}", client.NetworkSocket.RemoteEndPoint.ToString())
        _clientConnected = False
    End Sub

    Private Shared Sub _server_OnClientConnect(sender As SocketHandler.Server, client As SocketHandler.Server.SocketClient)
        Console.WriteLine("Connected: {0}", client.NetworkSocket.RemoteEndPoint.ToString())
        _clientConnected = True
        _client = client
        screenThread.IsBackground = True
        screenThread.Start()
        dataHandlerThread.IsBackground = True
        dataHandlerThread.Start()
    End Sub

    Private Shared Sub SendDesktop()
        While _clientConnected
            Using desktop = New Bitmap(_bounds.Width, _bounds.Height, Imaging.PixelFormat.Format32bppArgb)
                Using gfx As Graphics = Graphics.FromImage(desktop)
                    gfx.CopyFromScreen(_bounds.X, _bounds.Y, 0, 0, _bounds.Size, CopyPixelOperation.SourceCopy)
                    _client.Send(PacketHeaders.HeaderType.Desktop, desktop)
                End Using
            End Using
        End While
    End Sub
End Class
