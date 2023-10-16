using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace OpenTelemetryDemo.EF
{
    public static class CustomMetrics
    {
        public static readonly Meter Default = new Meter("OTel-Demo", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0");
        public static readonly Counter<long> StudentsCreated = Default.CreateCounter<long>("otel_demo_students_created",
            description: "Total number of students created");
        public static readonly Histogram<int> UpdateStudentDelay = Default.CreateHistogram<int>("otel_demo_student_update_delay", description: "The delay time in ms for updating a stundet");
        public static readonly ExplicitBucketHistogramConfiguration UpdateStudentDelayView = new ExplicitBucketHistogramConfiguration { Boundaries = new double[] { 25, 50, 60, 70, 80, 90, 100, 125 } };
    }
}
