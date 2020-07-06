using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Tetris2
{
    public partial class Game
    {
        internal void GameField_AddBlock(Block b)
        {
            VisualiseCanvas(b);

            allFieldBlocks.Add(b);
            fallingBlocks.Add(b);
            blocksToRedraw.Add(b);
            //b.ghostCoordX = (int)(b.CoordinatesX);

            //CreateNewGhostBlock();

        }
        private void VisualiseCanvas(Block b)
        {
            Grid.SetColumn(b.Canvas, 1);
            Grid.SetRow(b.Canvas, 3);
            Game.SetCanvasPosition(b);
            gameGrid.Children.Add(b.Canvas);
        }



    }
}
