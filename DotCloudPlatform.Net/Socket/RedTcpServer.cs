using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace DotCloudPlatform.Net.Socket;

public class RedTcpServer {
  private readonly System.Net.Sockets.Socket _socket;
  private readonly IPEndPoint _endPoint;
  private readonly bool _isEncrypted;
  private readonly string? _targetHost;
  private readonly X509Certificate? _serverCertificate;
  private readonly int _backLog;
  private bool _isListening;

  public delegate void NewClientHandler(object sender, RedTcpClient client);

  public event NewClientHandler? NewClientConnected;

  public delegate void AuthorizesIPHandler(object sender, AuthorizesIPEventArgs e);

  public event AuthorizesIPHandler? AuthorizesIP;

  public RedTcpServer(IPEndPoint address, int backLog = 1) {
    _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    _endPoint = address;
    _isEncrypted = false;
    _backLog = backLog;
  }

  public RedTcpServer(IPEndPoint address, string targetHost, X509Certificate serverCertificate, int backLog = 1) {
    _socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    _endPoint = address;
    _isEncrypted = true;
    _targetHost = targetHost;
    _serverCertificate = serverCertificate;
    _backLog = backLog;
  }

  public void Start() {
    _isListening = true;
    _socket.Bind(_endPoint);
    _socket.Listen(_backLog);
    _socket.BeginAccept(BeginAcceptAsyncResult, null);
  }

  public void Stop() {
    _isListening = false;
    _socket.Shutdown(SocketShutdown.Send);
  }

  private async void BeginAcceptAsyncResult(IAsyncResult ar) {
    try {
      var clientSocket = _socket.EndAccept(ar);
      var args = new AuthorizesIPEventArgs(clientSocket.RemoteEndPoint);
      AuthorizesIP?.Invoke(this, args);
      if (args.Authorize == false) {
        clientSocket.Send(new byte[] { 0x15 });
        clientSocket.Close();
        clientSocket.Dispose();
        goto NextSocket;
      }

      clientSocket.NoDelay = true;
      var redClient = new RedTcpClient(clientSocket, _targetHost, _serverCertificate, _isEncrypted);
      await redClient.InitializeSocketAsServer();
      if (redClient.SocketStatus == SocketStatus.Connected) {
        NewClientConnected?.Invoke(this, redClient);
      }
    } catch (Exception) {
      // ignored
    }

    NextSocket:
    if (!_isListening) return;
    _socket.BeginAccept(BeginAcceptAsyncResult, null);
  }
}

public class AuthorizesIPEventArgs : EventArgs {
  public EndPoint? EndPoint { get; }
  public bool Authorize { get; set; }

  public AuthorizesIPEventArgs(EndPoint? endPoint) {
    EndPoint = endPoint;
  }
}