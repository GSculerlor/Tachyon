using System;
using System.Diagnostics;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Timing;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Configuration;
using Tachyon.Game.Generator.Patterns;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Screens.Generate;
using Tachyon.Game.Utils;

namespace Tachyon.Game.Generator
{
    public class BeatmapGenerator : Component, IBeatSnapProvider
    {
        [Resolved]
        private BeatmapManager beatmapManager { get; set; }

        private TachyonRuleset ruleset;
        
        private Bindable<GenerationType> generationType = new Bindable<GenerationType>();
        private readonly BindableBeatDivisor beatDivisor = new BindableBeatDivisor();

        private Bindable<WorkingBeatmap> working;
        private IBeatmap playableBeatmap;

        private EditorBeatmap editorBeatmap;
        private EditorClock clock;

        [BackgroundDependencyLoader]
        private void load(Bindable<WorkingBeatmap> working, TachyonRuleset ruleset, TachyonConfigManager config)
        {
            this.ruleset = ruleset;
            this.working = working;
            
            generationType = config.GetBindable<GenerationType>(TachyonSetting.GenerationType);

            beatDivisor.Value = 4;
            
            var sourceClock = (IAdjustableClock) working.Value.Track ?? new StopwatchClock();
            clock = new EditorClock(working.Value, beatDivisor) { IsCoupled = false };
            clock.ChangeSource(sourceClock);
            clock.Stop();
        }

        protected override void Update()
        {
            base.Update();
            clock?.ProcessFrame();
        }

        private void createEditorBeatmap()
        {
            try
            {
                playableBeatmap = working.Value.GetPlayableBeatmap(ruleset.RulesetInfo);
                editorBeatmap = new EditorBeatmap(playableBeatmap);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Could not load beatmap successfully!");
            }
        }

        private void saveBeatmap() => beatmapManager.Save(playableBeatmap.BeatmapInfo, editorBeatmap);

        public void Save() => saveBeatmap();

        public void Generate()
        {
            createEditorBeatmap();
            
            if (editorBeatmap.HitObjects.Count > 0)
            {
                Logger.Log("Can't generate beatmap that already have hitobjects!");
                
                return;
            }
            
            Debug.Assert(editorBeatmap != null);
            Logger.Log($"Generating beatmap using {generationType.Value.GetDescription()}");
            
            if (generationType.Value == GenerationType.Random)
                generateRandomBeatmap();
            else
                generateEpicBeatmap();
        }

        private void generateRandomBeatmap()
        {
            var timingpoints = working.Value.Beatmap.ControlPointInfo.TimingPoints.ToList();
            clock.Seek(timingpoints[0].Time);

            while (clock.CurrentTime < working.Value.Track.Length)
            {
                var sliderType = EnumUtils.GetRandom<PatternType>();

                switch (sliderType)
                {
                    case PatternType.Triplet:
                        createTriplet(clock.CurrentTime);
                        break;
                        
                    case PatternType.Quintuplet:
                        createQuintuplet(clock.CurrentTime);
                        break;

                    default: 
                        createLowerNote(clock.CurrentTime);

                        seek(4);
                        break;
                }
            }
            
            Save();
        }

        private void generateEpicBeatmap()
        {
            Save();
        }

        private void createUpperNote(double startTime)
        {
            var upperNote = new Note();
            upperNote.StartTime = startTime;
            upperNote.Samples.Add(new HitSampleInfo
            {
                Name = HitSampleInfo.HIT_CLAP
            });
            
            addHitObject(upperNote);
        }
        
        private void createLowerNote(double startTime)
        {
            var lowerNote = new Note();
            lowerNote.StartTime = startTime;
            lowerNote.Samples.Add(new HitSampleInfo
            {
                Name = HitSampleInfo.HIT_NORMAL
            });
            
            addHitObject(lowerNote);
        }

        private void createTriplet(double startTime)
        {
            createLowerNote(startTime);
            seek(2);
            
            createUpperNote(clock.CurrentTime);
            seek(2);
            
            createUpperNote(clock.CurrentTime);
            seek(2);
        }
        
        private void createQuintuplet(double startTime)
        {
            createUpperNote(startTime);
            seek(2);
            
            createLowerNote(clock.CurrentTime);
            seek(2);
            
            createLowerNote(clock.CurrentTime);
            seek(2);
            
            createUpperNote(clock.CurrentTime);
            seek(2);
            
            createLowerNote(clock.CurrentTime);
            seek(2);
        }

        private void addHitObject(HitObject hitObject) => editorBeatmap.Add(hitObject);

        private void seek(double amount)
        {
            clock.SeekForward(true, amount);
        }
        
        public double SnapTime(double time, double? referenceTime) => editorBeatmap.SnapTime(time, referenceTime);

        public double GetBeatLengthAtTime(double referenceTime) => editorBeatmap.GetBeatLengthAtTime(referenceTime);
        
        public int BeatDivisor => beatDivisor?.Value ?? 4;
    }
}
