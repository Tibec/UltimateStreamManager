using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

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
        protected Logger Log { get; private set; }


        public WebApi()
        {
            Log = LogManager.GetLogger(this.GetType().ToString());
        }

        public void AsynchRequest(string url, Delegate reportProgress)
        {
            JsonSerializer serializer = new JsonSerializer();
        }

        virtual public string Request(string requestUri, HttpMethod method = null)
        {
            Log.Info("Requesting {0}", requestUri);

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
                        task.Wait();
                        HttpResponseMessage response = task.Result;/*
                        task.Result.Content.ToString();
                        response.EnsureSuccessStatusCode();*/
                        string result = response.Content.ReadAsStringAsync().Result;
                        Log.Trace(result);
                        return result;
                    }
                    catch (Exception)
                    {

                    }
                }
                return "";
            }
        }
    }
}
