using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Model.Api
{
    public class WebApi
    {
        protected string _baseUrl;
        protected Dictionary<string, string> _headers = new Dictionary<string, string>();
        protected int _refreshDelay = 0;
        protected DateTime lastRequestDate;
        protected string lastRequestContent;
        public string ApiName { get; protected set; } = "Anonymous API";

        public WebApi()
        {

        }

        public void AsynchRequest(string url, Delegate reportProgress)
        {
            JsonSerializer serializer = new JsonSerializer();
        }

        public string Request(string requestUri, HttpMethod method = null)
        {
            int maxTries = 3;

            if (method == null)
                method = HttpMethod.Get;
            // TODO : tester si la date de la derniere requete + delay < date actuelle, 
            // si oui continuer la requete
            // sinon retourner l'ancien contenu
            //

            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(_baseUrl);
                HttpRequestMessage request = new HttpRequestMessage(method, requestUri);
                foreach (var header in _headers)
                    request.Headers.Add(header.Key, header.Value);

                for (int i = 0 ; i < maxTries; ++i)
                {
                    try
                    {
                        Task<HttpResponseMessage> task = client.SendAsync(request);
                        task.Wait(10000);
                        HttpResponseMessage response = task.Result;
                        response.EnsureSuccessStatusCode();
                        string result = response.Content.ReadAsStringAsync().Result;
                        return result;
                    }
                    catch (Exception e)
                    {

                    }
                }
                return "";
            }
        }
    }
}
