using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SocketServer
{
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private bool isRunning;

    public void StartServer(int port)
    {
        if (isRunning) return;
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        isRunning = true;
        Console.WriteLine($"Unity server started on port {port}");

        client = server.AcceptTcpClient();
        stream = client.GetStream();
        IPEndPoint clientEndPoint = (IPEndPoint) client.Client.RemoteEndPoint;
        string clientIP = clientEndPoint.Address.ToString();
        int clientPort = clientEndPoint.Port;
        Console.WriteLine($"Client connected! IP: {clientIP}, Port: {clientPort}");
    }

    public void StopServer()
    {
        if (!isRunning) return;
        Console.WriteLine("Stopping server");
        stream?.Close();
        client?.Close();
        server.Stop();
        Console.WriteLine("Server stopped");
    }

    public char? Recv()
    {
        if (stream == null) return null;
        byte[] buffer = new byte[1];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead == 0) return null;
        char msg = Encoding.UTF8.GetString(buffer, 0, bytesRead)[0];
        Console.WriteLine($"Received: {msg}");
        return msg;
    }

    public void Send(string msg)
    {
        if (stream == null) return;
        byte[] msg_byte = Encoding.UTF8.GetBytes(msg);
        stream.Write(msg_byte, 0, msg_byte.Length);
        Console.WriteLine($"Sent");
    }
}
