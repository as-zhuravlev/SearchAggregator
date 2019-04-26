using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using SearchAggregator.Core.Entities;
using SearchAggregator.Web.Controllers;
using SearchAggregator.Infrastructure.Processing;

namespace SearchAggregator.Tests.Web
{
    public class SearchControllerTest
    {
        [Fact]
        public void HideIdsTest()
        {
            var logger = Mock.Of<ILogger<SearchController>>();

            var moqSearchProcessor = new Mock<ISearchProcessor>();
            moqSearchProcessor.Setup(proc => proc.Search("hello", false)).Returns(GetTestSearchResult());

            var controller = new SearchController(moqSearchProcessor.Object, logger);
            Microsoft.AspNetCore.Mvc.JsonResult jsonResult = controller.Get("hello", false);
            var json = JsonConvert.SerializeObject(jsonResult.Value, jsonResult.SerializerSettings);

            Assert.True(!json.Contains("Id") && !json.Contains("id"));
        }


        SearchResult GetTestSearchResult()
        {
            return new SearchResult()
            {
                Id = Guid.NewGuid(),
                SearchRequest = "hello",
                UrlSearchResults = new List<UrlSearchResult>()
                {
                    new UrlSearchResult()
                    {
                        Id = Guid.NewGuid(),
                        SearchResultId = Guid.NewGuid(),
                        Url = new Url()
                        {
                            Id = Guid.NewGuid(),
                            Snippet = "Hello com site",
                            Title = "Hello",
                            UrlString = "hello.com"
                        },

                        UrlSearchResultSearchEngines = new List<UrlSearchResultSearchEngine>()
                        {
                            new UrlSearchResultSearchEngine()
                            {
                                IsNew = true,
                                SearchEngine = new SearchEngine()
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Google",
                                }
                            }
                        }
                    }
                }
            };

        }
    }
}
