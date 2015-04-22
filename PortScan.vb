Imports System.Net
Imports System.Net.Sockets
Imports System.IO

Public Class PortScan

    Public IP As String
    Public PortBegin As Integer = 1
    Public PortEnd As Integer = 1000

    Public PortsAdd As New List(Of Integer)
    Public PortsRemove As New List(Of Integer)

    Public Verbose As Boolean = False

    Public DT As New DataTable
    Private FI As FileInfo

    Public Sub New(IP As String, Optional PortBegin As Integer = 1, Optional PortEnd As Integer = 1000)
        Me.IP = IP
        Me.PortBegin = PortBegin
        Me.PortEnd = PortEnd

        DT.Columns.Add("Port", GetType(Integer))
        DT.Columns.Add("Status", GetType(String))

    End Sub

    Public Function Scan() As DataTable
        Try
            If PortEnd < PortBegin Then Throw New Exception("PortBegin must be a lower number than PortEnd")
            For Port As Integer = PortBegin To PortEnd
                If PortsRemove.Contains(Port) Then Continue For
                FScan2(Port)
            Next
            If PortsAdd.Count > 0 Then
                For Each Port In PortsAdd
                    FScan2(Port)
                Next
            End If
            Return DT
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Function

    Private Sub FScan(Port As Integer)
        Try
            Dim HostAddress As IPAddress = Dns.GetHostEntry(IP).AddressList(0)
            Dim EPhost As New IPEndPoint(HostAddress, Port)
            Dim S As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            'S.SendTimeout = 2
            'S.ReceiveTimeout = 2
            Try
                S.Connect(EPhost)
            Catch ex As Exception
            End Try

            If S.Connected = False Then
                If Verbose = True Then
                    DT.Rows.Add(Port, "closed")
                End If
            Else
                DT.Rows.Add(Port, "open")
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Async Sub FScan2(Port As Integer)
        Dim Client As New TcpClient()
        Try
            'Client.SendTimeout = 2
            'Client.ReceiveTimeout = 2
            Await Client.ConnectAsync(IP, Port)
            DT.Rows.Add(Port, "open")
        Catch ex As SocketException
            'If Verbose = True Then
            '    DT.Rows.Add(Port, "closed")
            'End If
        Catch ex As ObjectDisposedException
            'If Verbose = True Then
            '    DT.Rows.Add(Port, "closed")
            'End If
        Finally
            Client.Close()
        End Try
    End Sub

    Public Sub SaveToFile(filepath As String, filename As String)
        FI = New FileInfo(filepath & "\" & filename)
        If FI.Exists = True Then
            Dim FH As StreamWriter = FI.CreateText
            FH.Write("")
            FH.Close()
        End If
        Threading.Thread.Sleep(5000)
        Dim Writer As StreamWriter = FI.AppendText
        For Each DTR As DataRow In DT.Rows
            Writer.WriteLine(DTR("port").ToString & vbTab & DTR("status").ToString)
        Next
        Writer.Close()
    End Sub
End Class
