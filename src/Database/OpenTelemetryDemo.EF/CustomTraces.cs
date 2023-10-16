using System.Diagnostics;

namespace OpenTelemetryDemo.EF.Entities
{
    public static class CustomTraces
    {
        public static readonly ActivitySource Default = new ActivitySource("OTel-Demo");
    }
}
