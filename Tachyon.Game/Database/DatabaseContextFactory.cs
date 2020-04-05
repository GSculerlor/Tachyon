﻿using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using osu.Framework.Platform;
using osu.Framework.Statistics;

namespace Tachyon.Game.Database
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        private readonly Storage storage;

        private const string database_name = @"tachyon_client";

        private ThreadLocal<TachyonDbContext> threadContexts;

        private readonly object writeLock = new object();

        private bool currentWriteDidWrite;
        private bool currentWriteDidError;

        private int currentWriteUsages;

        private IDbContextTransaction currentWriteTransaction;

        public DatabaseContextFactory(Storage storage)
        {
            this.storage = storage;
            recycleThreadContexts();
        }

        private static readonly GlobalStatistic<int> reads = GlobalStatistics.Get<int>("Database", "Get (Read)");
        private static readonly GlobalStatistic<int> writes = GlobalStatistics.Get<int>("Database", "Get (Write)");
        private static readonly GlobalStatistic<int> commits = GlobalStatistics.Get<int>("Database", "Commits");
        private static readonly GlobalStatistic<int> rollbacks = GlobalStatistics.Get<int>("Database", "Rollbacks");

        public TachyonDbContext Get()
        {
            reads.Value++;
            return threadContexts.Value;
        }

        public DatabaseWriteUsage GetForWrite(bool withTransaction = true)
        {
            writes.Value++;
            Monitor.Enter(writeLock);
            TachyonDbContext context;

            try
            {
                if (currentWriteTransaction == null && withTransaction)
                {
                    if (threadContexts.IsValueCreated)
                        recycleThreadContexts();

                    context = threadContexts.Value;
                    currentWriteTransaction = context.Database.BeginTransaction();
                }
                else
                {
                    context = threadContexts.Value;
                }
            }
            catch
            {
                Monitor.Exit(writeLock);
                throw;
            }

            Interlocked.Increment(ref currentWriteUsages);

            return new DatabaseWriteUsage(context, usageCompleted) { IsTransactionLeader = currentWriteTransaction != null && currentWriteUsages == 1 };
        }

        private void usageCompleted(DatabaseWriteUsage usage)
        {
            int usages = Interlocked.Decrement(ref currentWriteUsages);

            try
            {
                currentWriteDidWrite |= usage.PerformedWrite;
                currentWriteDidError |= usage.Errors.Any();

                if (usages == 0)
                {
                    if (currentWriteDidError)
                    {
                        rollbacks.Value++;
                        currentWriteTransaction?.Rollback();
                    }
                    else
                    {
                        commits.Value++;
                        currentWriteTransaction?.Commit();
                    }

                    if (currentWriteDidWrite || currentWriteDidError)
                    {
                        usage.Context.Dispose();

                        recycleThreadContexts();
                    }

                    currentWriteTransaction = null;
                    currentWriteDidWrite = false;
                    currentWriteDidError = false;
                }
            }
            finally
            {
                Monitor.Exit(writeLock);
            }
        }

        private void recycleThreadContexts()
        {
            threadContexts?.Value.Dispose();
            threadContexts = new ThreadLocal<TachyonDbContext>(CreateContext, true);
        }

        protected virtual TachyonDbContext CreateContext() => new TachyonDbContext(storage.GetDatabaseConnectionString(database_name))
        {
            Database = { AutoTransactionsEnabled = false }
        };

        public void ResetDatabase()
        {
            lock (writeLock)
            {
                recycleThreadContexts();

                try
                {
                    storage.DeleteDatabase(database_name);
                }
                catch
                {
                    // for now we are not sure why file handles are kept open by EF, but this is generally only used in testing
                }
            }
        }
    }
}
