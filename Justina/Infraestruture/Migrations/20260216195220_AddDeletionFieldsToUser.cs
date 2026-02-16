using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infraestruture.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletionFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeletionRequested",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionRequestedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDeletionDate",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletionRequested",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletionRequestedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ScheduledDeletionDate",
                table: "Users");
        }
    }
}
