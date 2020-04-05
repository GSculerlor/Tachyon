﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using osu.Framework.Platform;

namespace Tachyon.Game.Database
{
    public abstract class MutableDatabaseBackedStore<T> : DatabaseBackedStore
        where T : class, IHasPrimaryKey, ISoftDelete
    {
        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;

        protected MutableDatabaseBackedStore(IDatabaseContextFactory contextFactory, Storage storage = null)
            : base(contextFactory, storage)
        {
        }
        
        public IQueryable<T> ConsumableItems => AddIncludesForConsumption(ContextFactory.Get().Set<T>());

        public void Add(T item)
        {
            using (var usage = ContextFactory.GetForWrite())
            {
                var context = usage.Context;
                context.Attach(item);
            }

            ItemAdded?.Invoke(item);
        }

        public void Update(T item)
        {
            using (var usage = ContextFactory.GetForWrite())
                usage.Context.Update(item);

            ItemRemoved?.Invoke(item);
            ItemAdded?.Invoke(item);
        }

        public bool Delete(T item)
        {
            using (ContextFactory.GetForWrite())
            {
                Refresh(ref item);
                
                if (item.DeletePending) return false;

                item.DeletePending = true;
            }

            ItemRemoved?.Invoke(item);
            return true;
        }

        public bool Undelete(T item)
        {
            using (ContextFactory.GetForWrite())
            {
                Refresh(ref item, ConsumableItems);
                
                if (!item.DeletePending) return false;

                item.DeletePending = false;
            }

            ItemAdded?.Invoke(item);
            return true;
        }

        protected virtual IQueryable<T> AddIncludesForConsumption(IQueryable<T> query) => query;

        protected virtual IQueryable<T> AddIncludesForDeletion(IQueryable<T> query) => query;

        protected virtual void Purge(List<T> items, TachyonDbContext context) => context.RemoveRange(items);

        public override void Cleanup()
        {
            base.Cleanup();
            PurgeDeletable();
        }

        public void PurgeDeletable(Expression<Func<T, bool>> query = null)
        {
            using (var usage = ContextFactory.GetForWrite())
            {
                var context = usage.Context;

                var lookup = context.Set<T>().Where(s => s.DeletePending);

                if (query != null) lookup = lookup.Where(query);

                lookup = AddIncludesForDeletion(lookup);

                var purgeable = lookup.ToList();

                if (!purgeable.Any()) return;

                Purge(purgeable, context);
            }
        }
    }
}
