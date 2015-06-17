namespace Search
{
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using Nancy.Responses.Negotiation;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline(EnableCors);
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                var processors = new[]
                {
                    typeof(JsonProcessor)
                };

                return NancyInternalConfiguration.WithOverrides(x => x.ResponseProcessors = processors);
            }
        }

        private static void EnableCors(NancyContext ctx)
        {
            ctx.Response.WithHeader("Access-Control-Allow-Origin", "*");
            ctx.Response.WithHeader("Access-Control-Allow-Headers", "*");
            ctx.Response.WithHeader("Access-Control-Allow-Methods", "*");
            ctx.Response.WithHeader("Access-Control-Max-Age", "30758400");
        }
    }
}
