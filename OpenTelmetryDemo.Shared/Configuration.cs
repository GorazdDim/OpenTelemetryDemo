using Microsoft.Extensions.Configuration;

namespace OpenTelemetryDemo.Shared
{
    public class Configuration
    {
        public static Uri GetJaegerEndpoint(IConfiguration configuration)
        {
            var agentHost = configuration.GetSection("Jaeger:AgentHost").Get<string>();
            var agentPort = configuration.GetSection("Jaeger:AgentPort").Get<int>();
            return new Uri("http://" + agentHost + ":" + agentPort);
        }

        public static Uri GetServiceEndpoint(IConfiguration configuration)
        {
            var serviceName = configuration.GetSection("OpenTelApp:ServiceName").Get<string>();
            var servicePort = configuration.GetSection("OpenTelApp:ServicePort").Get<int>();
            return new Uri("http://" + serviceName + ":" + servicePort + "/");
        }
    }
}
