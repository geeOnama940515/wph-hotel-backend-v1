using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WPHBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removed_user_id_on_booking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "GuestName",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "GuestName",
                value: "Juan Dela Cruz");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestName",
                table: "Bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "UserId",
                value: new Guid("12345678-90ab-cdef-1234-567890abcdef"));
        }
    }
}
