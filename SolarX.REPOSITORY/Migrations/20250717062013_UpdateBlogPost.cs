using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarX.REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Agencies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 17, 13, 20, 10, 907, DateTimeKind.Unspecified).AddTicks(4058), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 17, 13, 20, 10, 907, DateTimeKind.Unspecified).AddTicks(4090), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 17, 13, 20, 10, 907, DateTimeKind.Unspecified).AddTicks(4256), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 17, 13, 20, 10, 907, DateTimeKind.Unspecified).AddTicks(4259), new TimeSpan(0, 7, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "BlogPosts");

            migrationBuilder.UpdateData(
                table: "Agencies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 10, 14, 20, 9, 339, DateTimeKind.Unspecified).AddTicks(7480), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 10, 14, 20, 9, 339, DateTimeKind.Unspecified).AddTicks(7508), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 10, 14, 20, 9, 339, DateTimeKind.Unspecified).AddTicks(7643), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 10, 14, 20, 9, 339, DateTimeKind.Unspecified).AddTicks(7644), new TimeSpan(0, 7, 0, 0, 0)) });
        }
    }
}
