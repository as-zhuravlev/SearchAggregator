using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SearchAggregator.Core.Entities;
using SearchAggregator.Core.Interfaces;
using SearchAggregator.Core.Shared;

namespace SearchAggregator.Google
{
    public class GoogleSearchEngine : ISearchEngine
    {
        private readonly ILogger _logger;
        private readonly string _apiKey;
        private readonly string _apiCx;

        public GoogleSearchEngine(IConfiguration conf, ILogger<GoogleSearchEngine> logger)
        {
            _logger = logger;
            _apiKey = conf.GetSection("Google").GetValue<string>("ApiKey");
            _apiCx = conf.GetSection("Google").GetValue<string>("ApiCx");
        }
        
        public string Name => "Google";

        public IReadOnlyCollection<Url> Search(string request)
        {
            var result = new List<Url>();
            string response;

            try
            {
                response = TakeDataFromGoogle(request);
            }
            catch (WebException ex)
            {
                _logger.LogError(ex,"Can not get data from Google!");
                throw new BaseException("Can not get data from Google!", ex);
            }
            
            try
            {
                var json = JObject.Parse(response);
                var items = (JArray)json["items"];
                
                foreach (var i in items)
                {
                    result.Add(new Url()
                    {
                        UrlString = i["link"].ToObject<string>(),
                        Title = i["title"].ToObject<string>(),
                        Snippet = i["snippet"].ToObject<string>(),
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can not get items from google api response");
                throw new BaseException("Can not get data from Google!", ex);
            }

            return result;
        }


        string TakeDataFromGoogle(string request)
        {
            var _searchUrl = $"https://www.googleapis.com/customsearch/v1?key={_apiKey}&q={request}&cx={_apiCx}";

            var hwr = HttpWebRequest.Create(_searchUrl);
            hwr.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)hwr.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

    }
}
