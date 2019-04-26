using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SearchAggregator.Infrastructure.Migrations
{
    public partial class Inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SearchEngines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchEngines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SearchRequest = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urls",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UrlString = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Snippet = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urls", x => x.Id);
                    table.UniqueConstraint("AK_Urls_UrlString", x => x.UrlString);
                });

            migrationBuilder.CreateTable(
                name: "UrlSearchResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UrlId = table.Column<Guid>(nullable: false),
                    SearchResultId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlSearchResults", x => x.Id);
                    table.UniqueConstraint("AK_UrlSearchResults_UrlId_SearchResultId", x => new { x.UrlId, x.SearchResultId });
                    table.ForeignKey(
                        name: "FK_UrlSearchResults_SearchResults_SearchResultId",
                        column: x => x.SearchResultId,
                        principalTable: "SearchResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UrlSearchResults_Urls_UrlId",
                        column: x => x.UrlId,
                        principalTable: "Urls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UrlSearchResultSearchEngines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UrlSearchResultId = table.Column<Guid>(nullable: false),
                    SearchEngineId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlSearchResultSearchEngines", x => x.Id);
                    table.UniqueConstraint("AK_UrlSearchResultSearchEngines_UrlSearchResultId_SearchEngineId", x => new { x.UrlSearchResultId, x.SearchEngineId });
                    table.ForeignKey(
                        name: "FK_UrlSearchResultSearchEngines_SearchEngines_SearchEngineId",
                        column: x => x.SearchEngineId,
                        principalTable: "SearchEngines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UrlSearchResultSearchEngines_UrlSearchResults_UrlSearchResultId",
                        column: x => x.UrlSearchResultId,
                        principalTable: "UrlSearchResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrlSearchResults_SearchResultId",
                table: "UrlSearchResults",
                column: "SearchResultId");

            migrationBuilder.CreateIndex(
                name: "IX_UrlSearchResultSearchEngines_SearchEngineId",
                table: "UrlSearchResultSearchEngines",
                column: "SearchEngineId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlSearchResultSearchEngines");

            migrationBuilder.DropTable(
                name: "SearchEngines");

            migrationBuilder.DropTable(
                name: "UrlSearchResults");

            migrationBuilder.DropTable(
                name: "SearchResults");

            migrationBuilder.DropTable(
                name: "Urls");
        }
    }
}
