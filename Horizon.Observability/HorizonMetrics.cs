using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Horizon.Observability;

public static class HorizonMetrics
{
    private static readonly Meter Meter = new("Horizon.Observability", "1.0.0");
    
    // Example: Track total successful checkouts
    private static readonly Counter<long> CheckoutsCounter = Meter.CreateCounter<long>(
        "horizon_checkouts_total", 
        unit: "{checkout}", 
        description: "Total number of successful checkouts");

    public static void RecordCheckout(long count = 1, string? store = null)
    {
        var tags = new TagList();
        if (!string.IsNullOrEmpty(store)) tags.Add("store", store);
        
        CheckoutsCounter.Add(count, tags);
    }
}
