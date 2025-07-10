using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolarX.REPOSITORY.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConsultingRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "ConsultingRequests");

            migrationBuilder.DropColumn(
                name: "PreferredTime",
                table: "ConsultingRequests");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "ConsultingRequests",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "ConsultingRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestType",
                table: "ConsultingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ConsultingRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Message",
                table: "ConsultingRequests");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "ConsultingRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ConsultingRequests");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ConsultingRequests",
                newName: "Note");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "ConsultingRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PreferredTime",
                table: "ConsultingRequests",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Agencies",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 10, 11, 31, 7, 450, DateTimeKind.Unspecified).AddTicks(4229), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 10, 11, 31, 7, 450, DateTimeKind.Unspecified).AddTicks(4259), new TimeSpan(0, 7, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdateAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 7, 10, 11, 31, 7, 450, DateTimeKind.Unspecified).AddTicks(4406), new TimeSpan(0, 7, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 7, 10, 11, 31, 7, 450, DateTimeKind.Unspecified).AddTicks(4407), new TimeSpan(0, 7, 0, 0, 0)) });
        }
    }
}
