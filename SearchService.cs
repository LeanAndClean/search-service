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

        List<Doc> docs = new List<Doc>();
        CircuitBreaker breaker = new CircuitBreaker();
        string CATALOG_SERVICE_URL = Environment.GetEnvironmentVariable("CATALOG_SERVICE_URL") ?? "NONE";

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
            if(number == 0) number = 10;

            Console.WriteLine("LOG: where - " + where);
            Console.WriteLine("LOG: page - " + page);
            Console.WriteLine("LOG: number - " + number);

            return docs
                    .AsQueryable()
                    .Where(where)
                    .Skip(page * number)
                    .Take(number)
                    .Select(x=>(dynamic)x)
                    .ToList();
        }

        public dynamic Replicate()
        {            
            dynamic result = new { message = "" };

            try
            {
                breaker.ExecuteWithRetries(() => {
                    Console.WriteLine("Try to replicate from: " + CATALOG_SERVICE_URL);

                    var client = new RestClient(CATALOG_SERVICE_URL);
                    var request = new RestRequest("/products/_all_docs?include_docs=true", Method.GET);
                    var res = client.Execute<Search>(request);
                    if(res.Data == null) throw new Exception();

                    docs = new List<Doc>();
                    docs.AddRange(res.Data.Rows.Select(x=>x.Doc).ToList() ?? new List<Doc>());

                    result = new {message = docs.Count() + " doc(s) replicated"};
                    Console.WriteLine(result);
                    docs.ForEach(x => Console.Write("{0};\r\n", x));

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
