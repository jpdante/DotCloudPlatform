using System.Net;
using DotCloudPlatform.Net.Socket;
using DotCloudPlatform.Protocol.Abstractions;
using MessagePack;

namespace DotCloudPlatform.Net;

public class DCPClient {
  private readonly DCPClientConfig _config;
  private readonly MessagePackSerializerOptions _serializerOptions;

  private RedTcpClient? _client;
  private CancellationTokenSource _cancellationTokenSource;

  public delegate void ConnectedHandler(object sender);

  public delegate void DisconnectedHandler(object sender);

  public delegate void PacketReceivedHandler(object sender, BasePacket packet);

  public event ConnectedHandler OnConnected;
  public event DisconnectedHandler OnDisconnected;
  public event PacketReceivedHandler OnPacketReceived;

  public DCPClient(DCPClientConfig config) {
    _config = config;
    _serializerOptions = MessagePackSerializerOptions.Standard
      .WithSecurity(MessagePackSecurity.TrustedData)
      .WithCompression(MessagePackCompression.Lz4BlockArray);
  }

  public async Task Connect() {
    if (!IPAddress.TryParse(_config.Host, out var ipAddress))
      ipAddress = (await Dns.GetHostEntryAsync(_config.Host)).AddressList[0];
    _client = new RedTcpClient(_config.Host, _config.CheckCertificateRevocation);
    await _client.ConnectAsync(new IPEndPoint(ipAddress, _config.Port));
  }

  public async Task Disconnect() {
    if (_client == null) return;
    await _client.DisconnectAsync();
    _client = null;
  }

  private Task SendPacket(BasePacket packet, CancellationToken cancellationToken = default) {
    if (_client == null && _client?.SocketStatus == SocketStatus.Connected)
      throw new InvalidOperationException("Client is not connected.");
    return MessagePackSerializer.SerializeAsync(_client!.Stream!, packet, _serializerOptions, cancellationToken);
  }

  private async Task PacketListener(CancellationToken cancellationToken) {
    while (!cancellationToken.IsCancellationRequested) {
      if (_client == null && _client?.SocketStatus == SocketStatus.Connected)
        throw new InvalidOperationException("Client is not connected.");
      var packet =
        await MessagePackSerializer.DeserializeAsync<BasePacket>(_client!.Stream!, _serializerOptions,
          cancellationToken);
      try {
        await HandlePacket(packet);
      } catch (Exception e) {
        Console.WriteLine(e);
      }
    }
  }

  private async Task HandlePacket(BasePacket basePacket) {
    switch (basePacket) {
      
    }
  }
}