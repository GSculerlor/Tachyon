using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
    public class TachyonCheckbox : Checkbox
    {
        public Color4 CheckedColor { get; set; } = Color4.Cyan;
        public Color4 UncheckedColor { get; set; } = Color4.White;
        public int FadeDuration { get; set; }

        public string LabelText
        {
            set
            {
                if (labelText != null)
                    labelText.Text = value;
            }
        }

        public MarginPadding LabelPadding
        {
            get => labelText?.Padding ?? new MarginPadding();
            set
            {
                if (labelText != null)
                    labelText.Padding = value;
            }
        }

        protected readonly Nub Nub;

        private readonly TachyonTextFlowContainer labelText;
        private SampleChannel sampleChecked;
        private SampleChannel sampleUnchecked;

        public TachyonCheckbox()
        {
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;

            const float nub_padding = 5;

            Children = new Drawable[]
            {
                labelText = new TachyonCheckboxLabel
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Padding = new MarginPadding { Right = Nub.EXPANDED_SIZE + nub_padding },
                },
                Nub = new Nub
                {
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Margin = new MarginPadding { Right = nub_padding },
                },
            };

            Nub.Current.BindTo(Current);

            Current.DisabledChanged += disabled => labelText.Alpha = Nub.Alpha = disabled ? 0.3f : 1;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            sampleChecked = audio.Samples.Get(@"UI/check-on");
            sampleUnchecked = audio.Samples.Get(@"UI/check-off");
        }

        protected override bool OnHover(HoverEvent e)
        {
            Nub.Glowing = true;
            Nub.Expanded = true;
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            Nub.Glowing = false;
            Nub.Expanded = false;
            base.OnHoverLost(e);
        }

        protected override void OnUserChange(bool value)
        {
            base.OnUserChange(value);
            if (value)
                sampleChecked?.Play();
            else
                sampleUnchecked?.Play();
        }
        
        private class TachyonCheckboxLabel : TachyonTextFlowContainer {
            
            protected override SpriteText CreateSpriteText() => new TachyonSpriteText
            {
                Font = TachyonFont.Default.With(size: 20, weight: FontWeight.SemiBold),
            };
        }
    }
}
