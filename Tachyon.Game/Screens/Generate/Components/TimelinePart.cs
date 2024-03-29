using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Beatmaps;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class TimelinePart : TimelinePart<Drawable>
    {
    }

    /// <summary>
    /// Represents a part of the summary timeline..
    /// </summary>
    public class TimelinePart<T> : Container<T> where T : Drawable
    {
        protected readonly IBindable<WorkingBeatmap> Beatmap = new Bindable<WorkingBeatmap>();

        private readonly Container<T> content;

        protected override Container<T> Content => content;

        public TimelinePart(Container<T> content = null)
        {
            AddInternal(this.content = content ?? new Container<T> { RelativeSizeAxes = Axes.Both });

            Beatmap.ValueChanged += b =>
            {
                updateRelativeChildSize();
                LoadBeatmap(b.NewValue);
            };
        }

        [BackgroundDependencyLoader]
        private void load(IBindable<WorkingBeatmap> beatmap)
        {
            Beatmap.BindTo(beatmap);
        }

        private void updateRelativeChildSize()
        {
            // the track may not be loaded completely (only has a length once it is).
            if (!Beatmap.Value.Track.IsLoaded)
            {
                content.RelativeChildSize = Vector2.One;
                Schedule(updateRelativeChildSize);
                return;
            }

            content.RelativeChildSize = new Vector2((float)Math.Max(1, Beatmap.Value.Track.Length), 1);
        }

        protected virtual void LoadBeatmap(WorkingBeatmap beatmap)
        {
            content.Clear();
        }
    }
}