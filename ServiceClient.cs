namespace Search
{
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net.Http;
    using System;
    using Newtonsoft.Json;

    public class ServiceInfo
    {
        public string Address { get; set; }
        public string ServicePort { get; set; }
    }

    public class ServiceClient
    {
        private readonly ConcurrentDictionary<string, string[]> serviceUrls = new ConcurrentDictionary<string, string[]>();
        private string[] discoveryServiceUrls;

        public ServiceClient(string[] discoveryServiceUrls)
        {
            this.discoveryServiceUrls = discoveryServiceUrls;
        }

        public T Get<T>(string serviceName, string resource)
        {
            var serviceUrls = GetServiceUrls(serviceName);
            return ExecuteRequest<T>(serviceUrls.Select(url => "http://" + url + resource), HttpMethod.Get);
        }

        private string[] GetServiceUrls(string serviceName)
        {
            string[] result;
            if (serviceUrls.TryGetValue(serviceName, out result)) return result;

            var infos = ExecuteRequest<ServiceInfo[]>(discoveryServiceUrls.Select(url => url + "/v1/catalog/service/" + serviceName), HttpMethod.Get);
            var urls = infos.Select(info => info.Address + ':' + info.ServicePort).ToArray();
            serviceUrls[serviceName] = urls;
            return urls;
        }

        private T ExecuteRequest<T>(IEnumerable<string> urls, HttpMethod method)
        {
            HttpResponseMessage response = null;

            using (var httpClient = new HttpClient())
            using (var urlEnumerator = urls.GetEnumerator())
            {
                while (response == null && urlEnumerator.MoveNext())
                {
                    var url = urlEnumerator.Current;
                    try
                    {
                        var request = new HttpRequestMessage(method, url);
                        response = httpClient.SendAsync(request).Result;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("URL {0} failed with exception {1}.", url, exception.Message);
                    }
                }
            }

            try
            {
                if (response == null) throw new Exception("No service was reachable.");
                response.EnsureSuccessStatusCode();
                
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
    }
}
