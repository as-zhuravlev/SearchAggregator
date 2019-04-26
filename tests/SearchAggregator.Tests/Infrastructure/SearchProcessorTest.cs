using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

using SearchAggregator.Infrastructure.Processing;
using SearchAggregator.Infrastructure.Data;
using SearchAggregator.Tests.TestHelpers;
using SearchAggregator.Core.Interfaces;
using SearchAggregator.Core.Entities;

namespace SearchAggregator.Tests.Infrastructure
{
    public class SearchProcessorTest
    {

        [Fact, TestPriority(2)]
        public void SearchTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

                AppDbContextTest.SeedTestDataToDbAndTest(options);

                using (var appDbContext = new AppDbContext(options))
                {
                    var logger = Mock.Of<ILogger<SearchProcessor>>();

                    var bingMoq = new Mock<ISearchEngine>();
                    bingMoq.Setup(x => x.Name).Returns("Bing");
                    bingMoq.Setup(x => x.Search("cat")).Returns(new List<Url>
                    {
                        new Url() {UrlString = "cat.com"},
                        new Url() {UrlString = "cat.net"},
                        new Url() {UrlString = "cat.io"},
                    });

                    var googleMoq = new Mock<ISearchEngine>();
                    googleMoq.Setup(x => x.Name).Returns("Google");
                    googleMoq.Setup(x => x.Search("cat")).Returns(new List<Url>
                    {
                        new Url() {UrlString = "cat.com"},
                        new Url() {UrlString = "cat.net"},
                        new Url() {UrlString = "cat.ru"},
                    });


                    var repo = new EfRepository(appDbContext);

                    var processor = new SearchProcessor(new[] { bingMoq.Object, googleMoq.Object}, repo, logger);

                    Assert.Equal(1, processor.Search("cat", true).UrlSearchResults.Count);

                    var result1 = processor.Search("cat", false);

                    Assert.Equal(4, processor.Search("cat", true).UrlSearchResults.Count);
                    Assert.Equal(2, processor.Search("cat", true).UrlSearchResults.First().UrlSearchResultSearchEngines.Count);
                    Assert.Equal(1, processor.Search("cat", true).UrlSearchResults.Last().UrlSearchResultSearchEngines.Count);
                    
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
