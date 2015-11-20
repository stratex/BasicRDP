Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Sockets
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Threading

Namespace Networking

    Public NotInheritable Class SocketHandler


        Private Sub New()
        End Sub
        Public Class Client

            Public Delegate Sub OnConnectAsyncCallback(sender As Client, success As Boolean)
            Public Delegate Sub OnDisconnectCallback(sender As Client, ER As SocketError)
            Public Delegate Sub OnDataRetrievedCallback(sender As Client, data As Object())


            Public Event OnConnect As OnConnectAsyncCallback
            Public Event OnDisconnect As OnDisconnectCallback
            Public Event OnDataRetrieved As OnDataRetrievedCallback

            Private _globalSocket As Socket
            Private _BufferSize As Integer = 1000000
            Public Property Connected() As Boolean
                Get
                    Return m_Connected
                End Get
                Private Set
                    m_Connected = Value
                End Set
            End Property
            Private m_Connected As Boolean
            Public Property PacketBuffer() As Byte()
                Get
                    Return m_PacketBuffer
                End Get
                Private Set
                    m_PacketBuffer = Value
                End Set
            End Property
            Private m_PacketBuffer As Byte()
            Public Property BufferSize() As Integer
                Get
                    Return _BufferSize
                End Get
                Set
                    If Connected Then
                        Throw New Exception("Can not change buffer size while connected")
                    End If
                    If Value < 1 Then
                        Throw New ArgumentOutOfRangeException("BufferSize")
                    End If
                    _BufferSize = Value
                End Set
            End Property
            Public Sub New()
                _globalSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                Connected = False
            End Sub

            Public Sub New(SocketAddressFamily As AddressFamily)
                Me.New()
                _globalSocket = New Socket(SocketAddressFamily, SocketType.Stream, ProtocolType.Tcp)
            End Sub
            Public Sub New(SocketAddressFamily As AddressFamily, pType As ProtocolType, sType As SocketType)
                Me.New()
                _globalSocket = New Socket(SocketAddressFamily, sType, pType)
            End Sub

            Public Function Connect(IP As String, port As Integer) As Boolean
                Try
                    _globalSocket.Connect(IP, port)
                    OnConnected()
                    Return True
                Catch
                    Return False
                End Try
            End Function

            Public Function Connect(endpoint As IPEndPoint) As Boolean
                Try
                    _globalSocket.Connect(endpoint)
                    OnConnected()
                    Return True
                Catch
                    Return False
                End Try
            End Function

            Public Sub ConnectAsync(IP As String, port As Integer)
                _globalSocket.BeginConnect(IP, port, AddressOf OnConnectAsync, Nothing)
            End Sub

            Public Sub ConnectAsync(endpoint As IPEndPoint)
                _globalSocket.BeginConnect(endpoint, AddressOf OnConnectAsync, Nothing)
            End Sub

            Private Sub OnConnectAsync(AR As IAsyncResult)
                Try
                    _globalSocket.EndConnect(AR)
                    RaiseEvent OnConnect(Me, True)
                    OnConnected()
                Catch
                    RaiseEvent OnConnect(Me, False)
                End Try
            End Sub

            Private Sub OnConnected()
                PacketBuffer = New Byte(_BufferSize - 1) {}
                _globalSocket.BeginReceive(PacketBuffer, 0, PacketBuffer.Length, SocketFlags.None, AddressOf EndRetrieve, Nothing)
            End Sub

            Public Sub Send(ParamArray data As Object())
                Dim serilizedData As Byte() = Formatter.Serialize(data)

                Dim Packet As Byte() = Nothing

                Using packetStream As New MemoryStream()
                    Using packetWriter As New BinaryWriter(packetStream)
                        packetWriter.Write(serilizedData.Length)
                        packetWriter.Write(serilizedData)
                        Packet = packetStream.ToArray()
                    End Using
                End Using

                _globalSocket.BeginSend(Packet, 0, Packet.Length, SocketFlags.None, AddressOf EndSend, Nothing)
            End Sub

            Public Sub SendWait(ParamArray data As Object())
                SyncLock Me
                    Dim serilizedData As Byte() = Formatter.Serialize(data)
                    Dim Packet As Byte() = Nothing

                    Using packetStream As New MemoryStream()
                        Using packetWriter As New BinaryWriter(packetStream)
                            packetWriter.Write(serilizedData.Length)
                            packetWriter.Write(serilizedData)
                            Packet = packetStream.ToArray()
                        End Using
                    End Using

                    _globalSocket.Send(Packet)
                    Thread.Sleep(10)
                End SyncLock
            End Sub

            Private Sub EndSend(AR As IAsyncResult)
                Dim SE As SocketError
                _globalSocket.EndSend(AR, SE)
            End Sub

            Private Sub EndRetrieve(AR As IAsyncResult)
                Dim SE As SocketError
                Dim packetLength As Integer = _globalSocket.EndReceive(AR, SE)
                If SE <> SocketError.Success Then
                    RaiseEvent OnDisconnect(Me, SE)
                    Return
                End If
                Dim PacketCluster As Byte() = New Byte(packetLength - 1) {}
                Buffer.BlockCopy(PacketBuffer, 0, PacketCluster, 0, packetLength)


                Using bufferStream As New MemoryStream(PacketCluster)
                    Using packetReader As New BinaryReader(bufferStream)
                        Try
                            While bufferStream.Position < bufferStream.Length
                                Dim length As Integer = packetReader.ReadInt32()
                                Dim Packet As Byte() = Nothing
                                If length > bufferStream.Length - bufferStream.Position Then
                                    Using recievePacketChunks As New MemoryStream(length)
                                        Dim buffer__1 As Byte() = New Byte(bufferStream.Length - bufferStream.Position - 1) {}

                                        buffer__1 = packetReader.ReadBytes(buffer__1.Length)
                                        recievePacketChunks.Write(buffer__1, 0, buffer__1.Length)

                                        While recievePacketChunks.Position <> length
                                            packetLength = _globalSocket.Receive(PacketBuffer)
                                            buffer__1 = New Byte(packetLength - 1) {}
                                            Buffer.BlockCopy(PacketBuffer, 0, buffer__1, 0, packetLength)
                                            recievePacketChunks.Write(buffer__1, 0, buffer__1.Length)
                                        End While
                                        Packet = recievePacketChunks.ToArray()
                                    End Using
                                Else
                                    Packet = packetReader.ReadBytes(length)
                                End If


                                Dim data As Object() = Formatter.Deserialize(Of Object())(Packet)
                                If data IsNot Nothing Then
                                    RaiseEvent OnDataRetrieved(Me, data)
                                End If

                                _globalSocket.BeginReceive(PacketBuffer, 0, PacketBuffer.Length, SocketFlags.None, AddressOf EndRetrieve, Nothing)
                            End While
                        Catch
                        End Try
                    End Using
                End Using


            End Sub
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
