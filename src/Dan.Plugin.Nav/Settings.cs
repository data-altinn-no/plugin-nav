namespace Dan.Plugin.Nav;

public class Settings
{
    public int DefaultCircuitBreakerOpenCircuitTimeSeconds { get; set; }
    public int DefaultCircuitBreakerFailureBeforeTripping { get; set; }
    public int SafeHttpClientTimeout { get; set; }

    public string EndpointUrl { get; set; }
}
