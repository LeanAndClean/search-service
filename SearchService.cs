namespace Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using ReliabilityPatterns;

    public class SearchService : ISearchService
    {
        private List<Doc> docs = new List<Doc>();
        private CircuitBreaker breaker = new CircuitBreaker();
        private string[] DISCOVERY_SERVICE_URLS = (Environment.GetEnvironmentVariable("DISCOVERY_SERVICE_URLS") ?? "NONE").Split(',', ';');

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
                    var serviceClient = new ServiceClient(DISCOVERY_SERVICE_URLS);
                    
                    Console.WriteLine("Try to replicate from catalog service");
                    var search = serviceClient.Get<Search>("couchdb", "/products/_all_docs?include_docs=true");
                    if (search == null) throw new Exception();

                    docs = search.Rows.Select(x => x.Doc).ToList() ?? new List<Doc>();

                    docs.ForEach(doc => {
                        Console.WriteLine("Try to get cover for MBID " + doc.MbId + " from cover service");
                        var covers = serviceClient.Get<List<string>>("cover-service", "/images/" + doc.MbId);
                                                
                        doc.Covers = covers ?? new List<string>(0);
                    });

                    result = new { message = docs.Count + " doc(s) replicated" };
                    Console.WriteLine(result);
                    docs.ForEach(Console.WriteLine);

                }, 3, TimeSpan.FromSeconds(2));

                return result;
            }
            catch (Exception error)
            {
                result = new {message = "Replication error", error = error.Message};
                Console.WriteLine(result);
                return result;
            }
        }
    }
}
