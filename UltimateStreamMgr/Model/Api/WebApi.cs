using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

        public virtual string Request(string requestUri, HttpMethod method = null, string content = "")
        {
            Log.Info("Requesting {0}", requestUri);

            int maxTries = 3;

            if (method == null)
                method = HttpMethod.Get;


            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(_baseUrl);
                HttpRequestMessage request = new HttpRequestMessage(method, requestUri);

                if (!string.IsNullOrEmpty(content))
                    request.Content = new  StringContent(content, Encoding.UTF8, "application/json");
                
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
