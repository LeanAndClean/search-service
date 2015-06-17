namespace Search
{
    using System;
    using Nancy;

    public class HealthCheckModule : NancyModule
    {
        public HealthCheckModule()
        {
          Get["/healthcheck"] = _ => {
            Console.WriteLine("LOG: OK - Health check");
            return new { Message = "OK" };
          };
        }
    }
}
