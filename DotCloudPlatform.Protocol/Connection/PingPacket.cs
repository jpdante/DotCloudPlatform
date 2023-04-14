using MessagePack;

namespace DotCloudPlatform.Protocol.Connection;

[MessagePackObject]
public class PingPacket {
  
  [Key(0)]
  public Guid Id { get; set; }
  
}