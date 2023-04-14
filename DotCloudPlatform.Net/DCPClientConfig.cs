namespace DotCloudPlatform.Net; 

public class DCPClientConfig {

  public string Host { get; set; } = "localhost";

  public int Port { get; set; } = 4443;

  public bool CheckCertificateRevocation { get; set; } = true;

  public bool UseSSL { get; set; } = true;
  
}