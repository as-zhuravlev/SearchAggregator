using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


using Newtonsoft.Json;

using SearchAggregator.Core.Entities;
using SearchAggregator.Infrastructure.Processing;
using SearchAggregator.Web.Stuff;

#pragma warning disable 1591 // disable comment requirement all public class/members


namespace SearchAggregator.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings;

        private readonly ISearchProcessor _searchProcessor;
        private readonly ILogger _logger;
        
        public SearchController(ISearchProcessor searchProcessor, ILogger<SearchController> logger)
        {
            _searchProcessor = searchProcessor;
            _logger = logger;
        }

        static SearchController()
        {
            var picr = new PropertyIgnoreContractResolver();
            picr.IgnoreProperty(typeof(BaseEntity), "Id");
            picr.IgnoreProperty(typeof(UrlSearchResult), "UrlId", "SearchResultId", "SearchResult");
            picr.IgnoreProperty(typeof(UrlSearchResultSearchEngine), "UrlSearchResultId", "UrlSearchResult", "SearchEngineId");
            picr.IgnoreProperty(typeof(Url), "UrlSearchResults");
            
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = picr,
            };
        }

        /// <summary>
        /// Get search results from several web search services. 
        /// </summary>
        /// <param name="request">string for web search</param>  
        /// <param name="onlyInDatabase">show only results already found</param>  
        [HttpGet()]
        public JsonResult Get([FromQuery] string request, [FromQuery] bool onlyInDatabase)
        {
            _logger.LogInformation($"Search request: {request}");
            try
            {
                var searchResult = _searchProcessor.Search(request, onlyInDatabase);
                var json = new JsonResult(searchResult, _jsonSerializerSettings);
                return json;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unhandled exception on search request {request}");
                throw;
            }
        }
    }
}
