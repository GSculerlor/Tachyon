using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osuTK;
using Tachyon.Game.Rulesets.UI.Scrolling;

namespace Tachyon.Game.Rulesets.Objects.Drawables
{
    public abstract class DrawableTachyonHitObject : DrawableHitObject<TachyonHitObject>
    {
        protected readonly IBindable<TachyonAction> Action = new Bindable<TachyonAction>();
        
        protected readonly Container Content;
        private readonly Container proxiedContent;

        private readonly Container nonProxiedContent;

        protected DrawableTachyonHitObject(TachyonHitObject hitObject)
            : base(hitObject)
        {
            AddRangeInternal(new[]
            {
                nonProxiedContent = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = Content = new Container { RelativeSizeAxes = Axes.Both }
                },
                proxiedContent = new ProxiedContentContainer { RelativeSizeAxes = Axes.Both }
            });
        }
        
        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] IBindable<TachyonAction> action)
        {
            if (action != null)
                Action.BindTo(action);
        }

        /// <summary>
        /// <see cref="proxiedContent"/> is proxied into an upper layer. We don't want to get masked away otherwise <see cref="proxiedContent"/> would too.
        /// </summary>
        protected override bool ComputeIsMaskedAway(RectangleF maskingBounds) => false;

        private bool isProxied;

        /// <summary>
        /// Moves <see cref="Content"/> to a layer proxied above the playfield.
        /// Does nothing if content is already proxied.
        /// </summary>
        protected void ProxyContent()
        {
            if (isProxied) return;

            isProxied = true;

            nonProxiedContent.Remove(Content);
            proxiedContent.Add(Content);
        }

        /// <summary>
        /// Moves <see cref="Content"/> to the normal hitobject layer.
        /// Does nothing is content is not currently proxied.
        /// </summary>
        protected void UnproxyContent()
        {
            if (!isProxied) return;

            isProxied = false;

            proxiedContent.Remove(Content);
            nonProxiedContent.Add(Content);
        }

        /// <summary>
        /// Creates a proxy for the content of this <see cref="DrawableTachyonHitObject"/>.
        /// </summary>
        public Drawable CreateProxiedContent() => proxiedContent.CreateProxy();

        public abstract bool OnPressed(TachyonAction action);

        public virtual void OnReleased(TachyonAction action)
        {
        }

        public override double LifetimeStart
        {
            get => base.LifetimeStart;
            set
            {
                base.LifetimeStart = value;
                proxiedContent.LifetimeStart = value;
            }
        }

        public override double LifetimeEnd
        {
            get => base.LifetimeEnd;
            set
            {
                base.LifetimeEnd = value;
                proxiedContent.LifetimeEnd = value;
            }
        }

        private class ProxiedContentContainer : Container
        {
            public override bool RemoveWhenNotAlive => false;
        }
    }
    
    public abstract class DrawableTachyonHitObject<TObject> : DrawableTachyonHitObject
        where TObject : TachyonHitObject
    {
        public override Vector2 OriginPosition => new Vector2(DrawHeight / 2);

        public new TObject HitObject;

        protected readonly Vector2 BaseSize;
        protected readonly CompositeDrawable MainPiece;

        protected DrawableTachyonHitObject(TObject hitObject)
            : base(hitObject)
        {
            HitObject = hitObject;
        
            Anchor = Anchor.CentreLeft;
            Origin = Anchor.Custom;
            
            Content.Add(MainPiece = CreateMainPiece());

            RelativeSizeAxes = Axes.Both;
            Size = BaseSize = new Vector2(TachyonHitObject.DEFAULT_SIZE);
        }
        
        protected abstract CompositeDrawable CreateMainPiece();
    }
}
