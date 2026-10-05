using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBook.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddFavoriteClientId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ClientId",
            table: "Favorites",
            type: "TEXT",
            maxLength: 64,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateIndex(
            name: "IX_Favorites_ClientId",
            table: "Favorites",
            column: "ClientId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Favorites_ClientId",
            table: "Favorites");

        migrationBuilder.DropColumn(
            name: "ClientId",
            table: "Favorites");
    }
}
