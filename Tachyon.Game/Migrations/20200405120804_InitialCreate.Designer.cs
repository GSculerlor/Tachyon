﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tachyon.Game.Database;

namespace Tachyon.Game.Migrations
{
    [DbContext(typeof(TachyonDbContext))]
    [Migration("20200405120804_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("Tachyon.Game.Beatmaps.BeatmapDifficulty", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("ApproachRate");

                    b.Property<float>("CircleSize");

                    b.Property<float>("DrainRate");

                    b.Property<float>("OverallDifficulty");

                    b.Property<double>("SliderMultiplier");

                    b.Property<double>("SliderTickRate");

                    b.HasKey("ID");

                    b.ToTable("BeatmapDifficulty");
                });

            modelBuilder.Entity("Tachyon.Game.Beatmaps.BeatmapInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("AudioLeadIn");

                    b.Property<double>("BPM");

                    b.Property<int>("BaseDifficultyID");

                    b.Property<int>("BeatmapSetInfoID");

                    b.Property<bool>("Countdown");

                    b.Property<string>("Hash");

                    b.Property<double>("Length");

                    b.Property<string>("MD5Hash");

                    b.Property<int?>("MetadataID");

                    b.Property<int?>("OnlineBeatmapID");

                    b.Property<string>("Path");

                    b.Property<bool>("SpecialStyle");

                    b.Property<float>("StackLeniency");

                    b.Property<double>("StarDifficulty");

                    b.Property<string>("Version");

                    b.HasKey("ID");

                    b.HasIndex("BaseDifficultyID");

                    b.HasIndex("BeatmapSetInfoID");

                    b.HasIndex("Hash");

                    b.HasIndex("MD5Hash");

                    b.HasIndex("MetadataID");

                    b.HasIndex("OnlineBeatmapID")
                        .IsUnique();

                    b.ToTable("BeatmapInfo");
                });

            modelBuilder.Entity("Tachyon.Game.Beatmaps.BeatmapMetadata", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Artist");

                    b.Property<string>("ArtistUnicode");

                    b.Property<string>("AudioFile");

                    b.Property<string>("BackgroundFile");

                    b.Property<int>("PreviewTime");

                    b.Property<string>("Source");

                    b.Property<string>("Tags");

                    b.Property<string>("Title");

                    b.Property<string>("TitleUnicode");

                    b.HasKey("ID");

                    b.ToTable("BeatmapMetadata");
                });

            modelBuilder.Entity("Tachyon.Game.Beatmaps.BeatmapSetFileInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BeatmapSetInfoID");

                    b.Property<int>("FileInfoID");

                    b.Property<string>("Filename")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("BeatmapSetInfoID");

                    b.HasIndex("FileInfoID");

                    b.ToTable("BeatmapSetFileInfo");
                });

            modelBuilder.Entity("Tachyon.Game.Beatmaps.BeatmapSetInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("DateAdded");

                    b.Property<bool>("DeletePending");

                    b.Property<string>("Hash");

                    b.Property<int?>("MetadataID");

                    b.Property<int?>("OnlineBeatmapSetID");

                    b.HasKey("ID");

                    b.HasIndex("DeletePending");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.HasIndex("MetadataID");

                    b.HasIndex("OnlineBeatmapSetID")
                        .IsUnique();

                    b.ToTable("BeatmapSetInfo");
                });

            modelBuilder.Entity("Tachyon.Game.IO.FileInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Hash");

                    b.Property<int>("ReferenceCount");

                    b.HasKey("ID");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.HasIndex("ReferenceCount");

                    b.ToTable("FileInfo");
                });

            modelBuilder.Entity("Tachyon.Game.Beatmaps.BeatmapInfo", b =>
                {
                    b.HasOne("Tachyon.Game.Beatmaps.BeatmapDifficulty", "BaseDifficulty")
                        .WithMany()
                        .HasForeignKey("BaseDifficultyID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tachyon.Game.Beatmaps.BeatmapSetInfo", "BeatmapSet")
                        .WithMany("Beatmaps")
                        .HasForeignKey("BeatmapSetInfoID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tachyon.Game.Beatmaps.BeatmapMetadata", "Metadata")
                        .WithMany("Beatmaps")
                        .HasForeignKey("MetadataID");
                });

            modelBuilder.Entity("Tachyon.Game.Beatmaps.BeatmapSetFileInfo", b =>
                {
                    b.HasOne("Tachyon.Game.Beatmaps.BeatmapSetInfo")
                        .WithMany("Files")
                        .HasForeignKey("BeatmapSetInfoID");

                    b.HasOne("Tachyon.Game.IO.FileInfo", "FileInfo")
                        .WithMany()
                        .HasForeignKey("FileInfoID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Tachyon.Game.Beatmaps.BeatmapSetInfo", b =>
                {
                    b.HasOne("Tachyon.Game.Beatmaps.BeatmapMetadata", "Metadata")
                        .WithMany("BeatmapSets")
                        .HasForeignKey("MetadataID");
                });
#pragma warning restore 612, 618
        }
    }
}