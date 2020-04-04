﻿using System;

namespace Tachyon.Game.Database
{
    /// <summary>
    /// Represents a model manager that publishes events when <typeparamref name="TModel"/>s are added or removed.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    public interface IModelManager<out TModel>
        where TModel : class
    {
        event Action<TModel> ItemAdded;

        event Action<TModel> ItemRemoved;
    }
}
