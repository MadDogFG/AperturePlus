using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AperturePlus.RatingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendingRatings",
                columns: table => new
                {
                    PendingRatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RateByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RateToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RatedUserRole = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRatings", x => x.PendingRatingId);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RateByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RateToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RatedUserRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingRatings");

            migrationBuilder.DropTable(
                name: "Ratings");
        }
    }
}
