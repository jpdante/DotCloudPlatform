using DotCloudPlatform.Daemon.Abstractions;

namespace DotCloudPlatform.Daemon.Telemetry; 

public class TelemetryDaemon : IDaemonModule {
  public string Name => "Telemetry";
  public string Version => "v1.0.0";
  
  public Task Start() {
    return Task.CompletedTask;
  }

  public Task Stop() {
    return Task.CompletedTask;
  }
  
  public void Dispose() {
    GC.SuppressFinalize(this);
  }
}