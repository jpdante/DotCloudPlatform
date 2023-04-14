using DotCloudPlatform.Protocol.Abstractions;
using MessagePack;

namespace DotCloudPlatform.Protocol;

[MessagePackObject]
public class ModulePacket : BasePacket {
  
  [Key(0)]
  public short ModuleId { get; set; }

  [Key(1)]
  public byte[] Data { get; set; }
  
}