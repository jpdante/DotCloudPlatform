namespace DotCloudPlatform.Daemon.Abstractions; 

public interface IDaemonModule : IDisposable {
  
  public string Name { get; }
  public string Version { get; }
  
  public Task Start();
  
  public Task Stop();

}