﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using osu.Framework.Logging;
using osu.Framework.Statistics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.IO;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Tachyon.Game.Database
{
    public class TachyonDbContext : DbContext
    {
        public DbSet<BeatmapInfo> BeatmapInfo { get; set; }
        public DbSet<BeatmapDifficulty> BeatmapDifficulty { get; set; }
        public DbSet<BeatmapMetadata> BeatmapMetadata { get; set; }
        public DbSet<BeatmapSetInfo> BeatmapSetInfo { get; set; }
        public DbSet<FileInfo> FileInfo { get; set; }

        private readonly string connectionString;

        private static readonly Lazy<TachyonDbLoggerFactory> logger = new Lazy<TachyonDbLoggerFactory>(() => new TachyonDbLoggerFactory());

        private static readonly GlobalStatistic<int> contexts = GlobalStatistics.Get<int>("Database", "Contexts");

        static TachyonDbContext()
        {
            // required to initialise native SQLite libraries on some platforms.
            SQLitePCL.Batteries_V2.Init();

            // https://github.com/aspnet/EntityFrameworkCore/issues/9994#issuecomment-508588678
            SQLitePCL.raw.sqlite3_config(2 /*SQLITE_CONFIG_MULTITHREAD*/);
        }

        public TachyonDbContext()
            : this("DataSource=:memory:")
        {
            // required for tooling (see https://wildermuth.com/2017/07/06/Program-cs-in-ASP-NET-Core-2-0).

            Migrate();
        }

        public TachyonDbContext(string connectionString)
        {
            this.connectionString = connectionString;

            var connection = Database.GetDbConnection();

            try
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA journal_mode=WAL;";
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                connection.Close();
                throw;
            }

            contexts.Value++;
        }

        ~TachyonDbContext()
        {
            // DbContext does not contain a finalizer (https://github.com/aspnet/EntityFrameworkCore/issues/8872)
            // This is used to clean up previous contexts when fresh contexts are exposed via DatabaseContextFactory
            Dispose();
        }

        private bool isDisposed;

        public override void Dispose()
        {
            if (isDisposed) return;

            isDisposed = true;

            base.Dispose();

            contexts.Value--;
            GC.SuppressFinalize(this);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning))
                .UseSqlite(connectionString, sqliteOptions => sqliteOptions.CommandTimeout(10))
                .UseLoggerFactory(logger.Value);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BeatmapInfo>().HasIndex(b => b.OnlineBeatmapID).IsUnique();
            modelBuilder.Entity<BeatmapInfo>().HasIndex(b => b.MD5Hash);
            modelBuilder.Entity<BeatmapInfo>().HasIndex(b => b.Hash);

            modelBuilder.Entity<BeatmapSetInfo>().HasIndex(b => b.OnlineBeatmapSetID).IsUnique();            modelBuilder.Entity<BeatmapSetInfo>().HasIndex(b => b.DeletePending);
            modelBuilder.Entity<BeatmapSetInfo>().HasIndex(b => b.DeletePending);
            modelBuilder.Entity<BeatmapSetInfo>().HasIndex(b => b.Hash).IsUnique();

            modelBuilder.Entity<FileInfo>().HasIndex(b => b.Hash).IsUnique();
            modelBuilder.Entity<FileInfo>().HasIndex(b => b.ReferenceCount);

            modelBuilder.Entity<BeatmapInfo>().HasOne(b => b.BaseDifficulty);
        }

        private class TachyonDbLoggerFactory : ILoggerFactory
        {
            #region Disposal

            public void Dispose()
            {
            }

            #endregion

            public ILogger CreateLogger(string categoryName) => new TachyonDbLogger();

            public void AddProvider(ILoggerProvider provider)
            {
                // no-op. called by tooling.
            }

            private class TachyonDbLogger : ILogger
            {
                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    if (logLevel < LogLevel.Information)
                        return;

                    osu.Framework.Logging.LogLevel frameworkLogLevel;

                    switch (logLevel)
                    {
                        default:
                            frameworkLogLevel = osu.Framework.Logging.LogLevel.Debug;
                            break;

                        case LogLevel.Warning:
                            frameworkLogLevel = osu.Framework.Logging.LogLevel.Important;
                            break;

                        case LogLevel.Error:
                        case LogLevel.Critical:
                            frameworkLogLevel = osu.Framework.Logging.LogLevel.Error;
                            break;
                    }

                    Logger.Log(formatter(state, exception), LoggingTarget.Database, frameworkLogLevel);
                }

                public bool IsEnabled(LogLevel logLevel)
                {
#if DEBUG_DATABASE
                    return logLevel > LogLevel.Debug;
#else
                    return logLevel > LogLevel.Information;
#endif
                }

                public IDisposable BeginScope<TState>(TState state) => null;
            }
        }

        public void Migrate() => Database.Migrate();
    }
}
