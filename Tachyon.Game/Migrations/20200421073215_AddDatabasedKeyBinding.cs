using Microsoft.EntityFrameworkCore.Migrations;

namespace Tachyon.Game.Migrations
{
    public partial class AddDatabasedKeyBinding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KeyBinding",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RulesetID = table.Column<int>(nullable: true),
                    Variant = table.Column<int>(nullable: true),
                    Keys = table.Column<string>(nullable: true),
                    Action = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyBinding", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyBinding_Action",
                table: "KeyBinding",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_KeyBinding_RulesetID_Variant",
                table: "KeyBinding",
                columns: new[] { "RulesetID", "Variant" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyBinding");
        }
    }
}
