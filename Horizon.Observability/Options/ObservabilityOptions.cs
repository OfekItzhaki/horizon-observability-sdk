namespace Horizon.Observability.Options;

public class ObservabilityOptions
{
    public const string SectionName = "HorizonObservability";

    public string ServiceName { get; set; } = string.Empty;
    public string SeqUrl { get; set; } = "http://localhost:5341";
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";
    public bool EnableConsoleLogging { get; set; } = true;
    public double TracingSampleRate { get; set; } = 1.0; // 100% by default
}
