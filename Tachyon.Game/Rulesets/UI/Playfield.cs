﻿using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Rulesets.Objects.Drawables;

namespace Tachyon.Game.Rulesets.UI
{
    public abstract class Playfield : CompositeDrawable
    {
        /// <summary>
        /// The <see cref="DrawableHitObject"/> contained in this Playfield.
        /// </summary>
        public HitObjectContainer HitObjectContainer => hitObjectContainerLazy.Value;

        private readonly Lazy<HitObjectContainer> hitObjectContainerLazy;

        /// <summary>
        /// A function that converts gamefield coordinates to screen space.
        /// </summary>
        public Func<Vector2, Vector2> GamefieldToScreenSpace => HitObjectContainer.ToScreenSpace;

        /// <summary>
        /// A function that converts screen space coordinates to gamefield.
        /// </summary>
        public Func<Vector2, Vector2> ScreenSpaceToGamefield => HitObjectContainer.ToLocalSpace;

        /// <summary>
        /// All the <see cref="DrawableHitObject"/>s contained in this <see cref="Playfield"/> and all <see cref="NestedPlayfields"/>.
        /// </summary>
        public IEnumerable<DrawableHitObject> AllHitObjects => HitObjectContainer?.Objects.Concat(NestedPlayfields.SelectMany(p => p.AllHitObjects)) ?? Enumerable.Empty<DrawableHitObject>();

        /// <summary>
        /// All <see cref="Playfield"/>s nested inside this <see cref="Playfield"/>.
        /// </summary>
        public IEnumerable<Playfield> NestedPlayfields => nestedPlayfields.IsValueCreated ? nestedPlayfields.Value : Enumerable.Empty<Playfield>();

        private readonly Lazy<List<Playfield>> nestedPlayfields = new Lazy<List<Playfield>>();

        /// <summary>
        /// Whether judgements should be displayed by this and and all nested <see cref="Playfield"/>s.
        /// </summary>
        public readonly BindableBool DisplayJudgements = new BindableBool(true);

        /// <summary>
        /// Creates a new <see cref="Playfield"/>.
        /// </summary>
        protected Playfield()
        {
            RelativeSizeAxes = Axes.Both;

            hitObjectContainerLazy = new Lazy<HitObjectContainer>(CreateHitObjectContainer);
        }

        /// <summary>
        /// Performs post-processing tasks (if any) after all DrawableHitObjects are loaded into this Playfield.
        /// </summary>
        public virtual void PostProcess() => NestedPlayfields.ForEach(p => p.PostProcess());

        /// <summary>
        /// Adds a DrawableHitObject to this Playfield.
        /// </summary>
        /// <param name="h">The DrawableHitObject to add.</param>
        public virtual void Add(DrawableHitObject h) => HitObjectContainer.Add(h);

        /// <summary>
        /// Remove a DrawableHitObject from this Playfield.
        /// </summary>
        /// <param name="h">The DrawableHitObject to remove.</param>
        public virtual bool Remove(DrawableHitObject h) => HitObjectContainer.Remove(h);

        /// <summary>
        /// Registers a <see cref="Playfield"/> as a nested <see cref="Playfield"/>.
        /// This does not add the <see cref="Playfield"/> to the draw hierarchy.
        /// </summary>
        /// <param name="otherPlayfield">The <see cref="Playfield"/> to add.</param>
        protected void AddNested(Playfield otherPlayfield)
        {
            otherPlayfield.DisplayJudgements.BindTo(DisplayJudgements);
            nestedPlayfields.Value.Add(otherPlayfield);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // in the case a consumer forgets to add the HitObjectContainer, we will add it here.
            if (HitObjectContainer.Parent == null)
                AddInternal(HitObjectContainer);
        }

        /// <summary>
        /// Creates the container that will be used to contain the <see cref="DrawableHitObject"/>s.
        /// </summary>
        protected virtual HitObjectContainer CreateHitObjectContainer() => new HitObjectContainer();
    }
}