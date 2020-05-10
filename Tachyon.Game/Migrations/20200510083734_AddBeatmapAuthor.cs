using Microsoft.EntityFrameworkCore.Migrations;

namespace Tachyon.Game.Migrations
{
    public partial class AddBeatmapAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "BeatmapMetadata",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "BeatmapMetadata");
        }
    }
}
