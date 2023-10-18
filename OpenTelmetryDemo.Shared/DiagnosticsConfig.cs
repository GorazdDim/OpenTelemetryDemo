using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OpenTelemetryDemo.Shared
{
    public static class OpenTelemetryConfig
    {
        public const string ServiceName = "otel-app";
        public const string ServiceProxyName = "otel-proxy-app";
        public static Meter Meter = new(ServiceName);
        public static Histogram<double> SalesValue = Meter.CreateHistogram<double>("sales.value");
        public static Histogram<double> SalesMarkupValue = Meter.CreateHistogram<double>("sales.markup");
        public static Counter<long> SalesCount = Meter.CreateCounter<long>("sales.count");
        public static ActivitySource Source = new(ServiceName);
    }
}
