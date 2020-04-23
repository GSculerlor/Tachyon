using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Drawables;

namespace Tachyon.Game.Rulesets.UI.Scrolling
{
    public class DualRow : ScrollingPlayfield
    {
        public const float ROW_SPACING = 1;
        
        public IReadOnlyList<Row> Rows => rowFlow.Children;
        private readonly FillFlowContainer<Row> rowFlow;
        
        private readonly Container topLevelContainer;
        
        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => Rows.Any(c => c.ReceivePositionalInputAt(screenSpacePos));

        private readonly int firstColumnIndex;

        public DualRow(int firstColumnIndex)
        {
            this.firstColumnIndex = firstColumnIndex;
            
            Name = "Dual row";
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        rowFlow = new FillFlowContainer<Row>
                        {
                            Name = "Columns",
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Padding = new MarginPadding { Left = ROW_SPACING, Right = ROW_SPACING },
                        },
                        topLevelContainer = new Container { RelativeSizeAxes = Axes.Both }
                    }
                }
            };
            
            for (int i = 0; i < 2; i++)
            {
                var row = new Row(firstColumnIndex + i);

                addRow(row);
            }
        }

        private void addRow(Row row)
        {
            rowFlow.Add(row);
            AddNested(row);
        }
        
        public override void Add(DrawableHitObject h)
        {
            var hitObject = (TachyonHitObject)h.HitObject;

            int columnIndex = -1;

            hitObject.RowBindable.BindValueChanged(_ =>
            {
                if (columnIndex != -1)
                    Rows.ElementAt(columnIndex).Remove(h);

                columnIndex = hitObject.Row - firstColumnIndex;
                Rows.ElementAt(columnIndex).Add(h);
            }, true);
        }

        public override bool Remove(DrawableHitObject h)
        {
            var maniaObject = (TachyonHitObject)h.HitObject;
            int columnIndex = maniaObject.Row - firstColumnIndex;
            Rows.ElementAt(columnIndex).Remove(h);

            return true;
        }
    }
}
