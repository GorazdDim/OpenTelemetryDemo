using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OpenTelemetryDemo
{
    public static class DiagnosticsConfig
    {
        public const string ServiceName = "demo-otl";
        public static Meter Meter = new Meter(ServiceName);
        public static Histogram<double> SalesValue = Meter.CreateHistogram<double>("sales.value");
        public static Histogram<double> SalesMarkupValue = Meter.CreateHistogram<double>("sales.markup");
        public static Counter<long> SalesCount = Meter.CreateCounter<long>("sales.count");
        public static ActivitySource Source = new ActivitySource(ServiceName);
        public static Uri JaeggerEndpoint = new Uri("http://localhost:4317");
    }
}
