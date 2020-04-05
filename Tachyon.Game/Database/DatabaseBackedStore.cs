﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using osu.Framework.Platform;

namespace Tachyon.Game.Database
{
    public abstract class DatabaseBackedStore
    {
        protected readonly Storage Storage;

        protected readonly IDatabaseContextFactory ContextFactory;

        protected virtual void Refresh<T>(ref T obj, IQueryable<T> lookupSource = null) where T : class, IHasPrimaryKey
        {
            using (var usage = ContextFactory.GetForWrite())
            {
                var context = usage.Context;

                if (context.Entry(obj).State != EntityState.Detached) return;

                var id = obj.ID;
                var foundObject = lookupSource?.SingleOrDefault(t => t.ID == id) ?? context.Find<T>(id);
                if (foundObject != null)
                    obj = foundObject;
                else
                    context.Add(obj);
            }
        }

        protected DatabaseBackedStore(IDatabaseContextFactory contextFactory, Storage storage = null)
        {
            ContextFactory = contextFactory;
            Storage = storage;
        }

        public virtual void Cleanup()
        {
        }
    }
}
