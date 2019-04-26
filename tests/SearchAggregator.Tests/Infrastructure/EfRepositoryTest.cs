using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

using SearchAggregator.Core.Interfaces;
using SearchAggregator.Infrastructure.Data;
using SearchAggregator.Core.Entities;
using SearchAggregator.Tests.TestHelpers;

namespace SearchAggregator.Tests.Infrastructure
{
    public class EfRepositoryTest
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
                    var repo = new EfRepository(appDbContext);
                    var result = repo.FindSearchResult("cat");
                    
                    Assert.Equal(1, result.UrlSearchResults.Count);
                    Assert.Equal("cat.com", result.UrlSearchResults.First().Url.UrlString);
                    Assert.Equal(1, result.UrlSearchResults.First().UrlSearchResultSearchEngines.Count);
                    Assert.False(result.UrlSearchResults.First().UrlSearchResultSearchEngines.First().IsNew);
                    Assert.Equal(appDbContext.SearchEngines.First( x => x.Name == "Google").Name, result.UrlSearchResults.First().UrlSearchResultSearchEngines.First().SearchEngine.Name);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
