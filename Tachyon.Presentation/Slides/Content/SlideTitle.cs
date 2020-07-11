using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Screens;
using Tachyon.Presentation.Graphics;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlideTitle : TachyonScreen
    {
        private TachyonTextFlowContainer titleFlow;
        private TachyonTextFlowContainer dosbingFlow, deptFlow;
        
        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures)
        {
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 200,
                    Children = new Drawable[]
                    {
                        new Sprite
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fill,
                            Texture = textures.Get(@"Presentation/header_awal")
                        },
                    }
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 200,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Color4.Black.Opacity(0.4f)
                                },
                                new GridContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Content = new[]
                                    {
                                        new Drawable[]
                                        {
                                            dosbingFlow = new TachyonTextFlowContainer
                                            {
                                                Padding = new MarginPadding(30),
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                TextAnchor = Anchor.CentreLeft,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Spacing = new Vector2(0, 2),
                                            },
                                            deptFlow = new TachyonTextFlowContainer
                                            {
                                                Padding = new MarginPadding(30),
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                TextAnchor = Anchor.CentreRight,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Spacing = new Vector2(0, 2),
                                            },
                                        },
                                    },
                                    ColumnDimensions = new[]
                                    {
                                        new Dimension(),
                                        new Dimension()
                                    }
                                },
                            }
                        }, 
                        titleFlow = new TachyonTextFlowContainer
                        {
                            Padding = new MarginPadding(30),
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            TextAnchor = Anchor.CentreLeft,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Spacing = new Vector2(0, 2),
                        },
                    }
                }
            };
            
            titleFlow.AddText("Rancang Bangun Tachyon:", t => t.Font = TachyonFont.Default.With(size: 54, weight: FontWeight.SemiBold));
            titleFlow.NewParagraph();
            titleFlow.AddText("Multi-Platform Horizontal Scrolling Rhythm Game Berbasis osu!framework dan BASS Audio Library Sebagai Audio Decoder", t => t.Font = TachyonFont.Default.With(size: 40, weight: FontWeight.SemiBold));

            titleFlow.NewParagraph();
            titleFlow.NewParagraph();
            titleFlow.AddText("Tugas Akhir - IF184802", t => t.Font = TachyonFont.Default.With(size: 20, weight: FontWeight.SemiBold));
            
            titleFlow.NewParagraph();
            titleFlow.NewParagraph();
            titleFlow.AddText("Ganendra Afrasya Salsabilla - 05111640000071", t => t.Font = TachyonFont.Default.With(size: 26, weight: FontWeight.SemiBold));

            dosbingFlow.AddText("Dosen Pembimbing", t => t.Font = TachyonFont.Default.With(size: 24, weight: FontWeight.SemiBold));
            dosbingFlow.NewParagraph();
            dosbingFlow.NewParagraph();
            dosbingFlow.AddText("Dr.Eng. Darlis Herumurti, S.Kom., M.Kom.");
            dosbingFlow.NewParagraph();
            dosbingFlow.AddText("Hadziq Fabroyir, S.Kom., Ph.D");
            dosbingFlow.NewParagraph();
            
            deptFlow.AddText("DEPARTEMEN TEKNIK INFORMATIKA", t => t.Font = TachyonFont.Default.With(size: 24, weight: FontWeight.SemiBold));
            deptFlow.NewParagraph();
            deptFlow.AddText("Fakultas Teknologi Elektro Dan Informatika Cerdas");
            deptFlow.NewParagraph();
            deptFlow.AddText("Institut Teknologi Sepuluh Nopember");
            deptFlow.NewParagraph();
            deptFlow.AddText("Surabaya 2020");
        }
    }
}
