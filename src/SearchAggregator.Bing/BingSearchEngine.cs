using System;
using System.Collections.Generic;

using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using SearchAggregator.Core.Entities;
using SearchAggregator.Core.Interfaces;
using SearchAggregator.Core.Shared;

namespace SearchAggregator.Bing
{
    public class BingSearchEngine : ISearchEngine
    {
        private readonly ILogger _logger;
        private readonly string _apiKey;

        public BingSearchEngine(IConfiguration configuration, ILogger<ISearchEngine> logger)
        {
            _apiKey = configuration.GetSection("Bing").GetValue<string>("ApiKey");
            _logger = logger;
        }
        
        public string Name => "Bing";

        public IReadOnlyCollection<Url> Search(string request)
        {
            var client = new WebSearchClient(new ApiKeyServiceClientCredentials(_apiKey));

            try
            {
                var urls = new List<Url>();

                var webData = client.Web.SearchAsync(query: request).Result;

                if (webData?.WebPages?.Value?.Count == 0)
                    return urls;

                foreach (var wp in webData.WebPages.Value)
                {
                    urls.Add(new Url()
                    {
                        Snippet = wp.Snippet,
                        Title = wp.Name,
                        UrlString = wp.Url,
                    });
                }

                return urls;
            }
            catch (AggregateException ex)
            {
                _logger.LogError(ex, "Can not get data from Bing");
                throw new BaseException("Can not get data from Bing", ex);
            }   
        }
    }
}
