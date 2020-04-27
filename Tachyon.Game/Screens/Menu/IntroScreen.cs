using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.IO.Archives;
using Tachyon.Game.Screens.Backgrounds;
using Tachyon.Game.Screens.Select;

namespace Tachyon.Game.Screens.Menu
{
    public class IntroScreen : TachyonScreen
    {
        public bool DidLoadMenu { get; private set; }

        private Box gradientBox;
        private FillFlowContainer textContainer;
        private TachyonScreen songSelection;
        
        private WorkingBeatmap introBeatmap;
        private LeasedBindable<WorkingBeatmap> beatmap;
        private Track track;

        private readonly BindableDouble exitingVolumeFade = new BindableDouble(1);
        
        [Resolved]
        private AudioManager audio { get; set; }

        [BackgroundDependencyLoader]
        private void load(BeatmapManager beatmaps, osu.Framework.Game game)
        {
            AddRangeInternal(new Drawable[]
            {
                gradientBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = ColourInfo.GradientVertical(Color4Extensions.FromHex("000").Opacity(0.2f), Color4Extensions.FromHex("000").Opacity(0.8f))
                },
                textContainer = new FillFlowContainer
                {
                    Origin = Anchor.BottomCentre,
                    Anchor = Anchor.BottomCentre,
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Bottom = 100 },
                    Spacing = new Vector2(0, 20),
                    Children = new Drawable[]
                    {
                        new TachyonSpriteText
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Spacing = new Vector2(10, 0),
                            Text = "Touch screen to start",
                            Font = TachyonFont.Default.With(size: 30, weight: FontWeight.SemiBold),    
                        },
                        new FillFlowContainer
                        {
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(10),
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            AutoSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new SpriteIcon
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Size = new Vector2(60),
                                    Icon = FontAwesome.Solid.Meteor
                                },
                                new TachyonSpriteText
                                {
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Spacing = new Vector2(20, 0),
                                    Margin = new MarginPadding { Bottom = 4, Left = 20 },
                                    Font = TachyonFont.GetFont(size: 60, weight: FontWeight.Bold),
                                    Text = "tachyon"
                                },
                            }
                        },
                    }
                }
            });            
            
            //textFlow.AddParagraph("Touch or click anywhere to continue", t => t.Font = TachyonFont.Default.With(size: 30, weight: FontWeight.Regular));

            beatmap = Beatmap.BeginLease(false);
            
            BeatmapSetInfo setInfo = beatmaps.Import(new ZipArchiveReader(game.Resources.GetStream("Tracks/blue_haven.osz"), "blue_haven.osz")).Result;
            beatmaps.Update(setInfo);
            
            introBeatmap = beatmaps.GetWorkingBeatmap(setInfo.Beatmaps[0]);
            track = introBeatmap.Track;
        }
        
        public override bool AllowBackButton => false;

        private void prepareMainMenu() => LoadComponentAsync(songSelection = new TachyonSongSelect());
        
        protected override BackgroundScreen CreateBackground() => new TextureBackgroundScreen();

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);
            
            Scheduler.AddDelayed(delegate
            {
                startTrack();

                prepareMainMenu();
            }, 500);
        }

        public override bool OnExiting(IScreen next) => !DidLoadMenu;
        
        public override void OnResuming(IScreen last)
        {
            this.FadeIn(300);

            double fadeOutTime = 500;

            audio.AddAdjustment(AdjustableProperty.Volume, exitingVolumeFade);
            this.TransformBindableTo(exitingVolumeFade, 0, fadeOutTime).OnComplete(_ => this.Exit());

            Game.FadeTo(0.01f, fadeOutTime);

            base.OnResuming(last);
        }

        public override void OnSuspending(IScreen next)
        {
            base.OnSuspending(next);
            track = null;
        }
        
        private void startTrack()
        {
            track.Restart();
        }
        
        private void loadMenu()
        {
            beatmap.Return();

            DidLoadMenu = true;
            this.Push(songSelection);
        }

        protected override bool OnClick(ClickEvent e)
        {
            Scheduler.AddDelayed(delegate
            {
                textContainer.FadeOutFromOne(500, Easing.OutQuart);
                gradientBox.FadeColour(Color4.Black, 500, Easing.OutQuart);

                Scheduler.AddDelayed(loadMenu, 500);
            }, 500);
            
            return true;
        }
    }
}