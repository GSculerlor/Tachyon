using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tachyon.Game.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeatmapDifficulty",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DrainRate = table.Column<float>(nullable: false),
                    CircleSize = table.Column<float>(nullable: false),
                    OverallDifficulty = table.Column<float>(nullable: false),
                    ApproachRate = table.Column<float>(nullable: false),
                    SliderMultiplier = table.Column<double>(nullable: false),
                    SliderTickRate = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapDifficulty", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BeatmapMetadata",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(nullable: true),
                    TitleUnicode = table.Column<string>(nullable: true),
                    Artist = table.Column<string>(nullable: true),
                    ArtistUnicode = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    Tags = table.Column<string>(nullable: true),
                    PreviewTime = table.Column<int>(nullable: false),
                    AudioFile = table.Column<string>(nullable: true),
                    BackgroundFile = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapMetadata", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FileInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hash = table.Column<string>(nullable: true),
                    ReferenceCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileInfo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BeatmapSetInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OnlineBeatmapSetID = table.Column<int>(nullable: true),
                    DateAdded = table.Column<DateTimeOffset>(nullable: false),
                    MetadataID = table.Column<int>(nullable: true),
                    Hash = table.Column<string>(nullable: true),
                    DeletePending = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapSetInfo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BeatmapSetInfo_BeatmapMetadata_MetadataID",
                        column: x => x.MetadataID,
                        principalTable: "BeatmapMetadata",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BeatmapInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OnlineBeatmapID = table.Column<int>(nullable: true),
                    BeatmapSetInfoID = table.Column<int>(nullable: false),
                    MetadataID = table.Column<int>(nullable: true),
                    BaseDifficultyID = table.Column<int>(nullable: false),
                    Length = table.Column<double>(nullable: false),
                    BPM = table.Column<double>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Hash = table.Column<string>(nullable: true),
                    MD5Hash = table.Column<string>(nullable: true),
                    AudioLeadIn = table.Column<double>(nullable: false),
                    Countdown = table.Column<bool>(nullable: false),
                    StackLeniency = table.Column<float>(nullable: false),
                    SpecialStyle = table.Column<bool>(nullable: false),
                    Version = table.Column<string>(nullable: true),
                    StarDifficulty = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapInfo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BeatmapInfo_BeatmapDifficulty_BaseDifficultyID",
                        column: x => x.BaseDifficultyID,
                        principalTable: "BeatmapDifficulty",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeatmapInfo_BeatmapSetInfo_BeatmapSetInfoID",
                        column: x => x.BeatmapSetInfoID,
                        principalTable: "BeatmapSetInfo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeatmapInfo_BeatmapMetadata_MetadataID",
                        column: x => x.MetadataID,
                        principalTable: "BeatmapMetadata",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BeatmapSetFileInfo",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileInfoID = table.Column<int>(nullable: false),
                    Filename = table.Column<string>(nullable: false),
                    BeatmapSetInfoID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeatmapSetFileInfo", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BeatmapSetFileInfo_BeatmapSetInfo_BeatmapSetInfoID",
                        column: x => x.BeatmapSetInfoID,
                        principalTable: "BeatmapSetInfo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BeatmapSetFileInfo_FileInfo_FileInfoID",
                        column: x => x.FileInfoID,
                        principalTable: "FileInfo",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapInfo_BaseDifficultyID",
                table: "BeatmapInfo",
                column: "BaseDifficultyID");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapInfo_BeatmapSetInfoID",
                table: "BeatmapInfo",
                column: "BeatmapSetInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapInfo_Hash",
                table: "BeatmapInfo",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapInfo_MD5Hash",
                table: "BeatmapInfo",
                column: "MD5Hash");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapInfo_MetadataID",
                table: "BeatmapInfo",
                column: "MetadataID");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapInfo_OnlineBeatmapID",
                table: "BeatmapInfo",
                column: "OnlineBeatmapID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapSetFileInfo_BeatmapSetInfoID",
                table: "BeatmapSetFileInfo",
                column: "BeatmapSetInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapSetFileInfo_FileInfoID",
                table: "BeatmapSetFileInfo",
                column: "FileInfoID");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapSetInfo_DeletePending",
                table: "BeatmapSetInfo",
                column: "DeletePending");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapSetInfo_Hash",
                table: "BeatmapSetInfo",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapSetInfo_MetadataID",
                table: "BeatmapSetInfo",
                column: "MetadataID");

            migrationBuilder.CreateIndex(
                name: "IX_BeatmapSetInfo_OnlineBeatmapSetID",
                table: "BeatmapSetInfo",
                column: "OnlineBeatmapSetID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileInfo_Hash",
                table: "FileInfo",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileInfo_ReferenceCount",
                table: "FileInfo",
                column: "ReferenceCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeatmapInfo");

            migrationBuilder.DropTable(
                name: "BeatmapSetFileInfo");

            migrationBuilder.DropTable(
                name: "BeatmapDifficulty");

            migrationBuilder.DropTable(
                name: "BeatmapSetInfo");

            migrationBuilder.DropTable(
                name: "FileInfo");

            migrationBuilder.DropTable(
                name: "BeatmapMetadata");
        }
    }
}
