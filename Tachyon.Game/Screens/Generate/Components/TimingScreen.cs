using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Rulesets.Objects;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class TimingScreen : CompositeDrawable
    {
        //private TachyonButton deleteButton;
        private HitObjectTable table;

        private IBindableList<HitObject> controlHitObjects;

        [Resolved]
        private IFrameBasedClock clock { get; set; }

        [Resolved]
        protected IBindable<WorkingBeatmap> Beatmap { get; private set; }

        [Resolved]
        private Bindable<HitObject> selectedHitObject { get; set; }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            RelativeSizeAxes = Axes.Both;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                new TachyonScrollContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = table = new HitObjectTable(),
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            //selectedHitObject.BindValueChanged(selected => { deleteButton.Enabled.Value = selected.NewValue != null; }, true);

            controlHitObjects = new BindableList<HitObject>(Beatmap.Value.Beatmap.HitObjects);
            controlHitObjects.ItemsAdded += _ => createContent();
            controlHitObjects.ItemsRemoved += _ => createContent();

            createContent();
        }

        private void createContent() => table.HitObjects = controlHitObjects;

        /*private void delete()
        {
            if (selectedHitObject.Value == null)
                return;

            Beatmap.Value.Beatmap.ControlPointInfo.RemoveGroup(selectedHitObject.Value);

            selectedHitObject.Value = Beatmap.Value.Beatmap.ControlPointInfo.Groups.FirstOrDefault(g => g.Time >= clock.CurrentTime);
        }

        private void addNew()
        {
            selectedHitObject.Value = Beatmap.Value.Beatmap.ControlPointInfo.GroupAt(clock.CurrentTime, true);
        }*/
    }
}
