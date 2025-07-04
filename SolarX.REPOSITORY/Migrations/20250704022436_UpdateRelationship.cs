using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarX.REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventories_AgencyId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_AgencyWallets_AgencyId",
                table: "AgencyWallets");

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

            migrationBuilder.CreateIndex(
                name: "IX_AgencyWallets_AgencyId",
                table: "AgencyWallets",
                column: "AgencyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventories_AgencyId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_AgencyWallets_AgencyId",
                table: "AgencyWallets");

            migrationBuilder.UpdateData(
                table: "Agencies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 3, 16, 59, 26, 133, DateTimeKind.Unspecified).AddTicks(4265), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 3, 16, 59, 26, 133, DateTimeKind.Unspecified).AddTicks(4294), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 3, 16, 59, 26, 133, DateTimeKind.Unspecified).AddTicks(4414), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 3, 16, 59, 26, 133, DateTimeKind.Unspecified).AddTicks(4415), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_AgencyId",
                table: "Inventories",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyWallets_AgencyId",
                table: "AgencyWallets",
                column: "AgencyId");
        }
    }
}
