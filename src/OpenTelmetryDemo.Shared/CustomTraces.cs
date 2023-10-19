using System.Diagnostics;

namespace OpenTelemetryDemo.Shared
{
    public static class CustomTraces
    {
        public static readonly ActivitySource Default = new("OTel-App-Demo");
    }
}
