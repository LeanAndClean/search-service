namespace Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using ReliabilityPatterns;
    using RestSharp;

    public class SearchService : ISearchService
    {
        private List<Doc> docs = new List<Doc>();
        private CircuitBreaker breaker = new CircuitBreaker();
        private string CATALOG_SERVICE_URL = Environment.GetEnvironmentVariable("CATALOG_SERVICE_URL") ?? "NONE";
        private string COVER_SERVICE_URL = Environment.GetEnvironmentVariable("COVER_SERVICE_URL") ?? "NONE";

        public SearchService()
        {
            Replicate();
        }

        public IList<dynamic> Find(dynamic filter)
        {
            var where = (string)filter.where ?? "true";
            var number = 0;
            var page = 0;

            Int32.TryParse(filter.number, out number);
            Int32.TryParse(filter.page, out page);
            if (number == 0) number = 10;

            Console.WriteLine("LOG: where - " + where);
            Console.WriteLine("LOG: page - " + page);
            Console.WriteLine("LOG: number - " + number);

            return docs
                    .AsQueryable()
                    .Where(where)
                    .Skip(page * number)
                    .Take(number)
                    .Select(x => (dynamic)x)
                    .ToList();
        }

        public dynamic Replicate()
        {            
            dynamic result = new { message = "" };

            try
            {
                breaker.ExecuteWithRetries(() => {
                    var catalogServiceClient = new RestClient(CATALOG_SERVICE_URL);
                    var coverServiceClient = new RestClient(COVER_SERVICE_URL);
                    
                    Console.WriteLine("Try to replicate from: " + CATALOG_SERVICE_URL);                    
                    var catalogRequest = new RestRequest("/products/_all_docs?include_docs=true", Method.GET);
                    var catalogResponse = catalogServiceClient.Execute<Search>(catalogRequest);
                    if (catalogResponse.Data == null) throw new Exception();

                    docs = catalogResponse.Data.Rows.Select(x => x.Doc).ToList() ?? new List<Doc>();

                    docs.ForEach(doc => {
                        Console.WriteLine("Try to get cover for MBID " + doc.MbId + " from " + COVER_SERVICE_URL);                        
                        var coverRequest = new RestRequest("/images/" + doc.MbId, Method.GET);
                        var coverResponse = coverServiceClient.Execute<List<string>>(coverRequest);
                        
                        doc.Covers = coverResponse.Data ?? new List<string>(0);
                    });

                    result = new { message = docs.Count + " doc(s) replicated" };
                    Console.WriteLine(result);
                    docs.ForEach(Console.WriteLine);

                }, 3, TimeSpan.FromSeconds(2));

                return result;
            }
            catch(Exception error)
            {
                result = new {message = "Replication error", error = error.Message};
                Console.WriteLine(result);
                return result;
            }
        }
    }
}
