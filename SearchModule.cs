namespace Search
{
    using System;
    using Nancy;
    using Nancy.Responses;
    using Nancy.Responses.Negotiation;
    using System.Collections.Generic;

    public class SearchModule : NancyModule
    {
        public SearchModule(ISearchService searchService)
        {
            Get["/"] = _ => searchService.Find(this.Request.Query);
            Get["/replicate"] = _ => searchService.Replicate();
        }
    }
}
