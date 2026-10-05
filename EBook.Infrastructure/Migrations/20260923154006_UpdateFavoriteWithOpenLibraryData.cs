using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBook.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFavoriteWithOpenLibraryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Favorites",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Favorites",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Availability",
                table: "Favorites",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "Favorites",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Favorites",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Favorites",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Isbn",
                table: "Favorites",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Favorites",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Favorites",
                type: "TEXT",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "Availability",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "Isbn",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Favorites");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Favorites",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
