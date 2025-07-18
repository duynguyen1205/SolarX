using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarX.REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class AddCatetoBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Agencies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 18, 15, 56, 12, 38, DateTimeKind.Unspecified).AddTicks(5261), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 18, 15, 56, 12, 38, DateTimeKind.Unspecified).AddTicks(5287), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 18, 15, 56, 12, 38, DateTimeKind.Unspecified).AddTicks(5455), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 18, 15, 56, 12, 38, DateTimeKind.Unspecified).AddTicks(5458), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 18, 15, 56, 12, 38, DateTimeKind.Unspecified).AddTicks(5461), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 18, 15, 56, 12, 38, DateTimeKind.Unspecified).AddTicks(5462), new TimeSpan(0, 7, 0, 0, 0)) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "BlogPosts");

            migrationBuilder.UpdateData(
                table: "Agencies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 17, 15, 46, 47, 168, DateTimeKind.Unspecified).AddTicks(43), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 17, 15, 46, 47, 168, DateTimeKind.Unspecified).AddTicks(74), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 17, 15, 46, 47, 168, DateTimeKind.Unspecified).AddTicks(204), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 17, 15, 46, 47, 168, DateTimeKind.Unspecified).AddTicks(205), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 17, 15, 46, 47, 168, DateTimeKind.Unspecified).AddTicks(211), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 17, 15, 46, 47, 168, DateTimeKind.Unspecified).AddTicks(211), new TimeSpan(0, 7, 0, 0, 0)) });
        }
    }
}
