namespace Tachyon.Game.Tests.Visual
{
    public abstract class RateAdjustedBeatmapTestScene : ScreenTestScene
    {
        protected override void Update()
        {
            base.Update();

            // note that this will override any mod rate application
            Beatmap.Value.Track.Tempo.Value = Clock.Rate;
        }
    }
}
