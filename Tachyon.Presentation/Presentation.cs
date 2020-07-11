using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK.Input;
using Tachyon.Game;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Presentation.Graphics;
using Tachyon.Presentation.Slides.Content;
using Tachyon.Presentation.Utils;

namespace Tachyon.Presentation
{
    public class Presentation : TachyonGameBase
    {
        private readonly Type[] slides =
        {
            //Title
            typeof(SlideTitle),
            
            //Pendahuluan
            typeof(SlideTitlePendahuluan),
            typeof(SlideLatarBelakang),
            typeof(SlideRumusanMasalah),
            typeof(SlideBatasanMasalah),
            typeof(SlideTujuan),
            typeof(SlideManfaat),

            //Analisis dan Perancangan Sistem
            typeof(SlideTitleAnalisis),
            typeof(SlidePerancanganArsitektur),
            typeof(SlidePerancanganLayout),
            typeof(SlidePerancanganGameplayRow),
            typeof(SlidePerancanganAutoGeneratorRandom),
            typeof(SlidePerancanganAutoGeneratorGanen),
            typeof(SlidePerancanganAutoGeneratorTachyonWaveform),
            
            //Implementasi Sistem
            /*typeof(SlideTitleImplementasi),
            typeof(SlideImplementasiGame),*/
            
            //Pengujian dan Evaluasi
            typeof(SlideTitlePengujian),
            typeof(SlideLingkunganUjiCoba),
            typeof(SlideHasilUjiCobaGameplay),
            typeof(SlideHasilUjiCobaPerformaUtama),
            typeof(SlideHasilUjiCobaPerformaTambahan),
            typeof(SlideHasilUjiCobaBeatmap),
            
            //Kesimpulan dan Saran
            typeof(SlideTitleKesimpulan),
            typeof(SlideKesimpulan),
            typeof(SlideSaran),
            typeof(SlideTerimaKasih),
        };
        
        private int current = -1;

        private ScreenStack stack;
        
        [Cached]
        protected readonly ColorUtils ColorUtils;

        public Presentation()
        {
            ColorUtils = new ColorUtils(OverlayColourScheme.Blue);
        }

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            TachyonTextFlowContainer title;
            
            Child = new DrawSizePreservingFillContainer
            {
                Children = new Drawable[]
                {
                    new TrianglesContainer
                    {
                        Masking = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    stack = new ScreenStack
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    title = new TachyonTextFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        TextAnchor = Anchor.CentreRight,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                    },
                }
            };

            title.AddText("osu!framework (running \"Presentasi Tugas Akhir\")", text => text.Font = TachyonFont.Default.With(size: 18, weight: FontWeight.SemiBold));

            next();
        }

        private void next()
        {
            if (current + 1 >= slides.Length)
                return;

            stack.Push((Screen)Activator.CreateInstance(slides[++current]));
        }

        private void prev()
        {
            if (stack.CurrentScreen == null) return;

            stack.CurrentScreen.Exit();
            current--;
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            if (e.Repeat) return false;

            switch (e.Key)
            {
                case Key.Left:
                    prev();
                    return true;

                case Key.Right:
                    next();
                    return true;

                case Key.Number0:
                    Host.Window.CursorState = CursorState.Hidden;
                    return true;
                
                case Key.Number9:
                    Host.Window.CursorState = CursorState.Default;
                    return true;
            }

            return base.OnKeyDown(e);
        }
    }
}
