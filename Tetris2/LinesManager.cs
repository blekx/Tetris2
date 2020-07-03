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

        public bool IsBlockLocatedInLine(Block b, int line) // contains some unnecessary checking
        {
            bool result = false;
            if (line < b.CoordinatesY || line >= b.CoordinatesY + b.DimensionY)
                return result;
            else
                for (int x = 0; x < b.DimensionX; x++)
                    if (b.Shape[x, line - (int)b.CoordinatesY])
                        return true;
            return result;
        }

        public void OldCutTheBlock(Block b, int line) // contains some unnecessary checking
        {
            ///y-coord of the erased line, in the internal Block-coordinates
            int yLine = line - (int)(b.CoordinatesY);
            int heightAbove = b.DimensionY - 1 - yLine;
            int heightBelow = yLine;
            if (heightAbove > 0)
            {//create remainiing upper blocks
                List<Block> upperBlocks = BlockGenerator.UpperCutRemains(b, yLine, heightAbove);
            }
            if (heightBelow > 0)
            {//create remaining lower blocks
                List<Block> lowerBlocks = BlockGenerator.LowerCutRemains(b, yLine, heightBelow);
            }


            killBlock(b);
        }

        public void Old2CutTheBlock(Block b, int line) // contains some unnecessary checking
        {
            ///y-coord of the erased line, in the internal Block-coordinates
            int yLine = line - (int)(b.CoordinatesY);
            int heightAbove = b.DimensionY - 1 - yLine;
            int heightBelow = yLine;
            if (heightAbove > 0)
            {//create remainiing upper blocks
                List<Block> upperBlocks = BlockGenerator.UpperCutRemains(b, yLine, heightAbove);
            }
            if (heightBelow > 0)
            {//create remaining lower blocks
                List<Block> lowerBlocks = BlockGenerator.LowerCutRemains(b, yLine, heightBelow);
            }


            killBlock(b);
        }

        public void CutAllTheBlocks(List<int> lines) // contains some unnecessary checking
        {
            if (game.fallingBlocks.Count > 0)
            {
                throw new Exception("Can not cut falling blocks");
            }
            List<Block> blocksWhichMayBeCut = new List<Block>(game.allFieldBlocks);
            List<Block> blocksToCut = new List<Block>();
            foreach (Block b in game.allFieldBlocks)
                foreach (int lY in lines)
                {
                    ///y-coord of the erased line, in the internal Block-coordinates
                    ///lYB_interBlockCoordsSystem
                    int lYB = lY - (int)b.CoordinatesY;
                    if (lYB >= 0 && lYB < b.DimensionY)
                    {
                        blocksToCut.Add(b);
                        blocksWhichMayBeCut.Remove(b);
                        break;
                    }
                }

            List<Block> newlyCutBlocks = new List<Block>();
            foreach (Block b in blocksToCut)
            {
                newlyCutBlocks.Add(BlockGenerator.NewlyCutBlocks(b, lines));
                killBlock(b);
            }
            game.AddNewlyCutBlocks(newlyCutBlocks);
        }



        public void killBlock(Block b)
        {
            game.gameGrid.Children.Remove(b.Canvas);
            game.allFieldBlocks.Remove(b);
        }
    }
}
