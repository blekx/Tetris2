using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris2
{
    public class LinesManager
    {
        private Game game;

        public LinesManager(Game game)
        {
            this.game = game;
        }

        public bool IsTheLineFull(int y)
        {
            int totalSquaresInThisLine = 0;
            for (int i = 0; i < game.DimensionX; i++)
                if (game.BoolField[i, y]) totalSquaresInThisLine++;
            return (game.DimensionX == totalSquaresInThisLine);
        }
    }
}
