using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;

namespace Tachyon.Game.Rulesets.Audio
{
    public class HitSound : CompositeDrawable, IKeyBindingHandler<TachyonAction>
    {
        private SampleChannel sampleHover;

        public HitSound()
        {
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            sampleHover = audio.Samples.Get(@"hitsound");
        }

        public bool OnPressed(TachyonAction action)
        {
            var drumSample = sampleHover;
            drumSample.Play();

            return false;
        }

        public void OnReleased(TachyonAction action)
        {
        }
    }
}
