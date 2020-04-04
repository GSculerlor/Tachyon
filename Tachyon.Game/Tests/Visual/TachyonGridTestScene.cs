using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Tachyon.Game.Tests.Visual
{
    public abstract class TachyonGridTestScene : TachyonTestScene
    {
        private readonly Drawable[,] cells;

        protected readonly int Rows;
        protected readonly int Cols;
        
        protected TachyonGridTestScene(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;

            GridContainer testContainer;
            Add(testContainer = new GridContainer { RelativeSizeAxes = Axes.Both });

            cells = new Drawable[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                    cells[r, c] = new Container { RelativeSizeAxes = Axes.Both };
            }

            testContainer.Content = cells.ToJagged();
        }

        protected Container Cell(int index) => (Container)cells[index / Cols, index % Cols];
        protected Container Cell(int row, int col) => (Container)cells[row, col];
    }
}