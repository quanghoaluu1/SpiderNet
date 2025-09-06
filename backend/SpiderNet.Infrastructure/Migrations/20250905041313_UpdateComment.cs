using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiderNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GifPublicId",
                table: "Comments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GifUrl",
                table: "Comments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Comments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Comments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MediaType",
                table: "Comments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoPublicId",
                table: "Comments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Comments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_MediaType",
                table: "Comments",
                column: "MediaType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_MediaType",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "GifPublicId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "GifUrl",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "VideoPublicId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Comments");
        }
    }
}
