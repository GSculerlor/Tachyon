using Microsoft.EntityFrameworkCore.Migrations;

namespace Tachyon.Game.Migrations
{
    public partial class AddProtectedBeatmap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Protected",
                table: "BeatmapSetInfo",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Protected",
                table: "BeatmapSetInfo");
        }
    }
}
