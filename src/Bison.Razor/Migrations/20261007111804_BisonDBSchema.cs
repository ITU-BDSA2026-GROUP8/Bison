using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bison.Razor.Migrations
{
    /// <inheritdoc />
    public partial class BisonDBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.AuthorId);
                });

            migrationBuilder.CreateTable(
                name: "taxons",
                columns: table => new
                {
                    TaxonId = table.Column<string>(type: "TEXT", nullable: false),
                    DanishVernacularName = table.Column<string>(type: "TEXT", nullable: false),
                    parentTaxonId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taxons", x => x.TaxonId);
                    table.ForeignKey(
                        name: "FK_taxons_taxons_parentTaxonId",
                        column: x => x.parentTaxonId,
                        principalTable: "taxons",
                        principalColumn: "TaxonId");
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    ObservationId = table.Column<int>(type: "INTEGER", nullable: true),
                    TaxonId = table.Column<string>(type: "TEXT", nullable: true),
                    Proposal_ObservationId = table.Column<int>(type: "INTEGER", nullable: true),
                    Proposal_TaxonId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "AuthorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Post_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Post_Proposal_ObservationId",
                        column: x => x.Proposal_ObservationId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_taxons_Proposal_TaxonId",
                        column: x => x.Proposal_TaxonId,
                        principalTable: "taxons",
                        principalColumn: "TaxonId");
                    table.ForeignKey(
                        name: "FK_Post_taxons_TaxonId",
                        column: x => x.TaxonId,
                        principalTable: "taxons",
                        principalColumn: "TaxonId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_AuthorId",
                table: "Post",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ObservationId",
                table: "Post",
                column: "ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Proposal_ObservationId",
                table: "Post",
                column: "Proposal_ObservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Proposal_TaxonId",
                table: "Post",
                column: "Proposal_TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_TaxonId",
                table: "Post",
                column: "TaxonId");

            migrationBuilder.CreateIndex(
                name: "IX_taxons_parentTaxonId",
                table: "taxons",
                column: "parentTaxonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "taxons");
        }
    }
}
