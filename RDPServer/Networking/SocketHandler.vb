Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text
Imports System.Threading

Namespace Networking

    Public NotInheritable Class SocketHandler


        Private Sub New()
        End Sub

        Public Class Server


            Public Delegate Sub OnClientConnectCallback(sender As Server, client As SocketClient)
            Public Delegate Sub OnClientDisconnectCallback(sender As Server, client As SocketClient, ER As SocketError)
            Public Delegate Sub OnDataRetrievedCallback(sender As Server, client As SocketClient, data As Object())

            Public Event OnClientConnect As OnClientConnectCallback
            Public Event OnClientDisconnect As OnClientDisconnectCallback
            Public Event OnDataRetrieved As OnDataRetrievedCallback

            Private _globalSocket As Socket
            Private _BufferSize As Integer = 8192
            Public Property BufferSize() As Integer
                Get
                    Return _BufferSize
                End Get
                Set
                    If Value < 1 Then
                        Throw New ArgumentOutOfRangeException("BufferSize")
                    End If
                    If IsRunning Then
                        Throw New Exception("Cannot set buffer size while server is running.")
                    End If
                    _BufferSize = Value
                End Set
            End Property
            Public Property IsRunning() As Boolean
                Get
                    Return m_IsRunning
                End Get
                Private Set
                    m_IsRunning = Value
                End Set
            End Property
            Private m_IsRunning As Boolean

            Public Sub New()
                _globalSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                IsRunning = False
            End Sub
            Public Sub New(SocketaddressFamily As AddressFamily)
                Me.New()
                _globalSocket = New Socket(SocketaddressFamily, SocketType.Stream, ProtocolType.Tcp)
            End Sub

            Public Function Start(port As Integer) As Boolean
                If IsRunning Then
                    Throw New Exception("Server is already running.")
                End If
                Try
                    _globalSocket.Bind(New IPEndPoint(IPAddress.Any, port))
                    _globalSocket.Listen(100)
                    _globalSocket.BeginAccept(AddressOf AcceptCallback, Nothing)

                    IsRunning = True
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                    IsRunning = False
                End Try
                Return IsRunning
            End Function

            Public Function Start(port As Integer, backlog As Integer) As Boolean
                If IsRunning Then
                    Throw New Exception("Server is already running.")
                End If
                Try
                    _globalSocket.Bind(New IPEndPoint(IPAddress.Any, port))
                    _globalSocket.Listen(backlog)
                    _globalSocket.BeginAccept(AddressOf AcceptCallback, Nothing)
                    IsRunning = True
                Catch
                    Return False
                End Try
                Return IsRunning
            End Function

            Public Sub [Stop]()
                IsRunning = False
                _globalSocket.Close()

            End Sub

            Private Sub AcceptCallback(AR As IAsyncResult)
                If Not IsRunning Then
                    Return
                End If
                Dim cSock As Socket = _globalSocket.EndAccept(AR)
                Dim _client As New SocketClient(cSock, BufferSize)

                RaiseEvent OnClientConnect(Me, _client)
                _client.NetworkSocket.BeginReceive(_client.Buffer, 0, _client.Buffer.Length, SocketFlags.None, AddressOf RetrieveCallback, _client)
                _globalSocket.BeginAccept(AddressOf AcceptCallback, Nothing)
            End Sub

            Private Sub RetrieveCallback(AR As IAsyncResult)
                If Not IsRunning Then
                    Return
                End If
                Dim _client As SocketClient = DirectCast(AR.AsyncState, SocketClient)
                If _client.NetworkSocket Is Nothing OrElse Not _client.NetworkSocket.Connected Then
                    RaiseEvent OnClientDisconnect(Me, _client, SocketError.Disconnecting)
                    _client.Dispose()
                    Return
                End If
                Dim SE As SocketError
                Dim packetLength As Integer = _client.NetworkSocket.EndReceive(AR, SE)
                If SE <> SocketError.Success Then
                    RaiseEvent OnClientDisconnect(Me, _client, SE)
                    _client.Dispose()
                    Return
                End If
                Dim PacketCluster As Byte() = New Byte(packetLength - 1) {}
                Buffer.BlockCopy(_client.Buffer, 0, PacketCluster, 0, packetLength)

                Dim Packet As Byte() = Nothing
                Using bufferStream As New MemoryStream(PacketCluster)
                    Using packetReader As New BinaryReader(bufferStream)
                        Try
                            While bufferStream.Position < bufferStream.Length
                                Dim length As Integer = packetReader.ReadInt32()
                                If length > bufferStream.Length - bufferStream.Position Then
                                    Using recievePacketChunks As New MemoryStream(length)
                                        Dim buffer__1 As Byte() = New Byte(bufferStream.Length - bufferStream.Position - 1) {}

                                        buffer__1 = packetReader.ReadBytes(buffer__1.Length)
                                        recievePacketChunks.Write(buffer__1, 0, buffer__1.Length)

                                        While recievePacketChunks.Position <> length
                                            packetLength = _client.NetworkSocket.Receive(_client.Buffer)
                                            buffer__1 = New Byte(packetLength - 1) {}
                                            Buffer.BlockCopy(_client.Buffer, 0, buffer__1, 0, packetLength)
                                            recievePacketChunks.Write(buffer__1, 0, buffer__1.Length)
                                        End While
                                        Packet = recievePacketChunks.ToArray()
                                    End Using
                                Else
                                    Packet = packetReader.ReadBytes(length)
                                End If

                                Dim RetrievedData = Formatter.Deserialize(Of Object())(Packet)
                                If RetrievedData IsNot Nothing Then
                                    RaiseEvent OnDataRetrieved(Me, _client, RetrievedData)
                                End If

                                _client.NetworkSocket.BeginReceive(_client.Buffer, 0, _client.Buffer.Length, SocketFlags.None, AddressOf RetrieveCallback, _client)
                            End While

                        Catch
                        End Try
                    End Using
                End Using
            End Sub

            Public Class SocketClient
                Implements IDisposable
                Public Property Buffer() As Byte()
                    Get
                        Return m_Buffer
                    End Get
                    Set
                        m_Buffer = Value
                    End Set
                End Property
                Private m_Buffer As Byte()
                Public Property Tag() As Object
                    Get
                        Return m_Tag
                    End Get
                    Set
                        m_Tag = Value
                    End Set
                End Property
                Private m_Tag As Object
                Public Property NetworkSocket() As Socket
                    Get
                        Return m_NetworkSocket
                    End Get
                    Private Set
                        m_NetworkSocket = Value
                    End Set
                End Property
                Private m_NetworkSocket As Socket

                Public Sub New(cSock As Socket)
                    NetworkSocket = cSock
                    Buffer = New Byte(8191) {}
                End Sub
                Public Sub New(cSock As Socket, bufferSize As Integer)
                    NetworkSocket = cSock
                    Buffer = New Byte(bufferSize - 1) {}
                End Sub

                Public Sub Disconnect()
                    Try
                        NetworkSocket.Close()

                    Catch
                    End Try
                End Sub

                Public Sub Send(dataType As PacketHeaders.HeaderType, ParamArray args As Object())
                    Try
                        Dim serilisedData As Byte() = Formatter.Serialize(args)

                        Dim dataTypeByte As Byte() = Encoding.UTF8.GetBytes(dataType)
                        Dim Packet As Byte() = Nothing

                        Using packetStream As New MemoryStream()
                            Using packetWriter As New BinaryWriter(packetStream)
                                packetWriter.Write(serilisedData.Length)
                                packetWriter.Write(serilisedData)
                                Packet = packetStream.ToArray()
                            End Using
                        End Using

                        NetworkSocket.BeginSend(Packet, 0, Packet.Length, SocketFlags.None, AddressOf EndSend, Nothing)
                        'Not connected
                    Catch
                    End Try
                End Sub
                Private Sub EndSend(AR As IAsyncResult)
                    Dim SE As SocketError
                    NetworkSocket.EndSend(AR, SE)
                End Sub

                Public Sub Dispose()
                    If NetworkSocket IsNot Nothing AndAlso NetworkSocket.Connected Then
                        NetworkSocket.Shutdown(SocketShutdown.Both)
                    End If
                End Sub

                Private Sub IDisposable_Dispose() Implements IDisposable.Dispose
                    Throw New NotImplementedException()
                End Sub
            End Class
        End Class

        Public NotInheritable Class Formatter
            Private Sub New()
            End Sub

            Shared Sub New()
            End Sub
            Public Shared Function Serialize(input As Object) As Byte()

                Dim bf As New BinaryFormatter()
                Using ms As New MemoryStream()
                    bf.Serialize(ms, input)
                    Return Compress(ms.ToArray())
                End Using

            End Function

            Public Shared Function Deserialize(Of t)(input As Byte()) As t
                Try

                    Dim bf As New BinaryFormatter()
                    Using ms As New MemoryStream(Decompress(input))
                        Return DirectCast(bf.Deserialize(ms), t)

                    End Using
                Catch ex As Exception
                    Console.WriteLine("[Error] {0}", ex)
                    Return Nothing
                End Try
            End Function

            Public Shared Function Compress(input As Byte()) As Byte()
                Using ms As New MemoryStream()
                    Using _gz As New GZipStream(ms, CompressionMode.Compress)
                        _gz.Write(input, 0, input.Length)
                    End Using
                    Return ms.ToArray()
                End Using
            End Function

            Public Shared Function Decompress(input As Byte()) As Byte()
                Using decompressed As New MemoryStream()
                    Using ms As New MemoryStream(input)
                        Using _gz As New GZipStream(ms, CompressionMode.Decompress)
                            Dim Bytebuffer As Byte() = New Byte(1023) {}
                            Dim bytesRead As Integer = 0
                            While (InlineAssignHelper(bytesRead, _gz.Read(Bytebuffer, 0, Bytebuffer.Length))) > 0
                                decompressed.Write(Bytebuffer, 0, bytesRead)
                            End While
                        End Using
                        Return decompressed.ToArray()
                    End Using
                End Using
            End Function

            Private Class DataPacket
                Public Sub New(d As Object)
                    Data = d
                End Sub
                Public Data As Object
            End Class
            Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class
    End Class
End Namespace
