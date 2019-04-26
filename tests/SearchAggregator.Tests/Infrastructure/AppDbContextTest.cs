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
    public class AppDbContextTest
    {
        [Fact, TestPriority(0)]
        public void AddAndSearchDbTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(connection)
                .Options;

                SeedTestDataToDbAndTest(options);
            }
            finally
            {
                connection.Close();
            }
        }
        
        internal static void SeedTestDataToDbAndTest(DbContextOptions<AppDbContext> options)
        {
            using (var context = new AppDbContext(options))
            {
                context.Database.EnsureCreated();
            }

            var searchEngineid = GuidHelper.Int(1);
            //-----------------------------------------------------------
            using (var context = new AppDbContext(options))
            {
                context.SearchEngines.Add(new SearchEngine() { Name = "Google", Id = searchEngineid });

                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                Assert.Equal(1, context.SearchEngines.Count());
                Assert.Equal("Google", context.SearchEngines.Single().Name);
                searchEngineid = context.SearchEngines.Single().Id;
            }

            //-----------------------------------------------------------
            using (var context = new AppDbContext(options))
            {
                context.Urls.Add(new Url() { UrlString = "cat.com", Id = GuidHelper.Int(10) });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                Assert.Equal(1, context.Urls.Count());
                Assert.Equal("cat.com", context.Urls.Single().UrlString);
            }

            //-----------------------------------------------------------
            using (var context = new AppDbContext(options))
            {
                context.SearchResults.Add(new SearchResult() { SearchRequest = "cat", Id = GuidHelper.Int(20) });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                Assert.Equal(1, context.SearchResults.Count());
                Assert.Equal("cat", context.SearchResults.Single().SearchRequest);
            }

            //----------------------------------------------------------
            using (var context = new AppDbContext(options))
            {
                context.UrlSearchResults.Add(new UrlSearchResult() { Id = GuidHelper.Int(30), UrlId = GuidHelper.Int(10), SearchResultId = GuidHelper.Int(20) });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                Assert.Equal(1, context.UrlSearchResults.Count());
                Assert.Equal(GuidHelper.Int(10), context.UrlSearchResults.Single().UrlId);
                Assert.Equal(GuidHelper.Int(20), context.UrlSearchResults.Single().SearchResultId);
            }

            //----------------------------------------------------------
            using (var context = new AppDbContext(options))
            {
                context.UrlSearchResultSearchEngines.Add(new UrlSearchResultSearchEngine() { UrlSearchResultId = GuidHelper.Int(30), SearchEngineId = searchEngineid });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                Assert.Equal(1, context.UrlSearchResultSearchEngines.Count());
                Assert.Equal(GuidHelper.Int(30), context.UrlSearchResultSearchEngines.Single().UrlSearchResultId);
                Assert.Equal(GuidHelper.Int(1), context.UrlSearchResultSearchEngines.Single().SearchEngineId);
            }
        }
    }
}
