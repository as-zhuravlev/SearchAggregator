using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SearchAggregator.Core.Entities;

namespace SearchAggregator.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Url> Urls { get; set; }

        public DbSet<SearchEngine> SearchEngines { get; set;}

        public DbSet<SearchResult> SearchResults { get; set; }

        public DbSet<UrlSearchResult> UrlSearchResults { get; set; }

        public DbSet<UrlSearchResultSearchEngine> UrlSearchResultSearchEngines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Url>().HasAlternateKey(u => u.UrlString);
            
            modelBuilder.Entity<UrlSearchResult>().HasAlternateKey(usr => new { usr.UrlId, usr.SearchResultId });

            modelBuilder.Entity<UrlSearchResult>().HasOne(usr => usr.Url)
                                                         .WithMany(u => u.UrlSearchResults)
                                                         .HasForeignKey(usr => usr.UrlId)
                                                         .IsRequired(true);

            modelBuilder.Entity<UrlSearchResult>().HasOne(um => um.SearchResult)
                                                         .WithMany(sr => sr.UrlSearchResults)
                                                         .HasForeignKey(um => um.SearchResultId)
                                                         .IsRequired(true);

            modelBuilder.Entity<UrlSearchResultSearchEngine>().HasAlternateKey(usrse => new { usrse.UrlSearchResultId, usrse.SearchEngineId});

            modelBuilder.Entity<UrlSearchResultSearchEngine>().HasOne(usrse => usrse.UrlSearchResult)
                                                              .WithMany(usr => usr.UrlSearchResultSearchEngines)
                                                              .HasForeignKey(usrse => usrse.UrlSearchResultId)
                                                              .IsRequired(true);

            modelBuilder.Entity<UrlSearchResultSearchEngine>().HasOne(usrse => usrse.SearchEngine)
                                                              .WithMany()//se => se.UrlSearchResultSearchEngines)
                                                              .HasForeignKey(usrse => usrse.SearchEngineId)
                                                              .IsRequired(true);

            modelBuilder.Entity<UrlSearchResultSearchEngine>().Ignore(usrse => usrse.IsNew);

        }

    }
}
