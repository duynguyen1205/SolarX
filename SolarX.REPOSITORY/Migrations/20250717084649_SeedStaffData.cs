using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarX.REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class SeedStaffData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AgencyId", "CreatedAt", "Email", "FullName", "IsDeleted", "Password", "PhoneNumber", "Role", "UpdateAt", "UserName" },
                values: new object[] { new Guid("66666666-6666-6666-6666-666666666666"), new Guid("44444444-4444-4444-4444-444444444444"), new DateTimeOffset(new DateTime(2025, 7, 17, 15, 46, 47, 168, DateTimeKind.Unspecified).AddTicks(211), new TimeSpan(0, 7, 0, 0, 0)), "Staff@gmail.com", "Staff SolarX", false, "nnsY/SJvZg/iBkxHXS/l9g==:0KN/XMNuKmz3nhL40RxTMNcMC9Xe5UE6XuYZ6bZQ1YU=", "0952252587", 1, new DateTimeOffset(new DateTime(2025, 7, 17, 15, 46, 47, 168, DateTimeKind.Unspecified).AddTicks(211), new TimeSpan(0, 7, 0, 0, 0)), "staffSolarX" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

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
    }
}
