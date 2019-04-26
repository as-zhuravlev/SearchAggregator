﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SearchAggregator.Infrastructure.Data;

namespace SearchAggregator.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190425102541_Inital")]
    partial class Inital
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-rtm-30799")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SearchAggregator.Core.Entities.SearchEngine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("SearchEngines");
                });

            modelBuilder.Entity("SearchAggregator.Core.Entities.SearchResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SearchRequest");

                    b.HasKey("Id");

                    b.ToTable("SearchResults");
                });

            modelBuilder.Entity("SearchAggregator.Core.Entities.Url", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Snippet");

                    b.Property<string>("Title");

                    b.Property<string>("UrlString")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAlternateKey("UrlString");

                    b.ToTable("Urls");
                });

            modelBuilder.Entity("SearchAggregator.Core.Entities.UrlSearchResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("SearchResultId");

                    b.Property<Guid>("UrlId");

                    b.HasKey("Id");

                    b.HasAlternateKey("UrlId", "SearchResultId");

                    b.HasIndex("SearchResultId");

                    b.ToTable("UrlSearchResults");
                });

            modelBuilder.Entity("SearchAggregator.Core.Entities.UrlSearchResultSearchEngine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("SearchEngineId");

                    b.Property<Guid>("UrlSearchResultId");

                    b.HasKey("Id");

                    b.HasAlternateKey("UrlSearchResultId", "SearchEngineId");

                    b.HasIndex("SearchEngineId");

                    b.ToTable("UrlSearchResultSearchEngines");
                });

            modelBuilder.Entity("SearchAggregator.Core.Entities.UrlSearchResult", b =>
                {
                    b.HasOne("SearchAggregator.Core.Entities.SearchResult", "SearchResult")
                        .WithMany("UrlSearchResults")
                        .HasForeignKey("SearchResultId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SearchAggregator.Core.Entities.Url", "Url")
                        .WithMany("UrlSearchResults")
                        .HasForeignKey("UrlId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SearchAggregator.Core.Entities.UrlSearchResultSearchEngine", b =>
                {
                    b.HasOne("SearchAggregator.Core.Entities.SearchEngine", "SearchEngine")
                        .WithMany()
                        .HasForeignKey("SearchEngineId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SearchAggregator.Core.Entities.UrlSearchResult", "UrlSearchResult")
                        .WithMany("UrlSearchResultSearchEngines")
                        .HasForeignKey("UrlSearchResultId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
