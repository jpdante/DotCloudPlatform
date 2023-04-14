using DotCloudPlatform.Protocol.Connection;
using DotCloudPlatform.Protocol.Management;
using MessagePack;

namespace DotCloudPlatform.Protocol.Abstractions; 

[Union(0, typeof(PingPacket))]
[Union(1, typeof(NotAllowedPacket))]
[Union(2, typeof(ModulePacket))]
[MessagePackObject]
public abstract class BasePacket {
  
  [Key(0)]
  public short PacketId { get; set; }

}