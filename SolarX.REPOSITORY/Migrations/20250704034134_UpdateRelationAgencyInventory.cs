using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarX.REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationAgencyInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventories_AgencyId",
                table: "Inventories");

            migrationBuilder.UpdateData(
                table: "Agencies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 4, 10, 41, 33, 205, DateTimeKind.Unspecified).AddTicks(6505), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 4, 10, 41, 33, 205, DateTimeKind.Unspecified).AddTicks(6534), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 4, 10, 41, 33, 205, DateTimeKind.Unspecified).AddTicks(6681), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 4, 10, 41, 33, 205, DateTimeKind.Unspecified).AddTicks(6682), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_AgencyId",
                table: "Inventories",
                column: "AgencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventories_AgencyId",
                table: "Inventories");

            migrationBuilder.UpdateData(
                table: "Agencies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 4, 9, 24, 34, 612, DateTimeKind.Unspecified).AddTicks(6903), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 4, 9, 24, 34, 612, DateTimeKind.Unspecified).AddTicks(6935), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 4, 9, 24, 34, 612, DateTimeKind.Unspecified).AddTicks(7058), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 4, 9, 24, 34, 612, DateTimeKind.Unspecified).AddTicks(7059), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_AgencyId",
                table: "Inventories",
                column: "AgencyId",
                unique: true);
        }
    }
}
