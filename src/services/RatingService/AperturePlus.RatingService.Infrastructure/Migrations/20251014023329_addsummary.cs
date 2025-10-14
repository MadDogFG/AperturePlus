using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AperturePlus.RatingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addsummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivitySummaries",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySummaries", x => x.ActivityId);
                });

            migrationBuilder.CreateTable(
                name: "UserSummaries",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSummaries", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivitySummaries");

            migrationBuilder.DropTable(
                name: "UserSummaries");
        }
    }
}
