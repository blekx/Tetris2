using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tetris2
{
    public static class BlockGenerator
    {
        private static BlockPattern[] Patterns =
        {
            new BlockPattern(0,1,1,"1111"),
            new BlockPattern(1,3,3,"111010000" +
                                   "010011010" +
                                   "010111000" +
                                   "010110010")
        };
        private static Color4B[] BlockColors =
        {
            new Color4B(100,100,100,255),
            new Color4B(255,255,0,255),
            new Color4B(0,0,255,255),
        };
        private static Random random = new Random();

        public static Block NewBlock()
        {
            //return new Block(1, 2, 2, new Color4B(200, 0, 0, 255), new bool[2, 2] { { true, true }, { true, true } });
            return  RotatedBlock(RandomDefaultBlock());
        }

        private static Block RandomDefaultBlock()
        {
            BlockPattern p = Patterns[random.Next(Patterns.Length)];
            return new Block(p.ID, p.X, p.Y, BlockColors[p.ID], p.Shape[0]);
        }

        private static Block RotatedBlock(Block b)
        {
            return RotateBlockSimply(b, random.Next(4));
        }

        private static Block RotateBlockSimply(Block b, int intoPosition)
        {
            //b.Shape=
            return new Block(b.DimensionX, b.DimensionY, b.ShapeID, b.Color, Patterns[b.ShapeID].Shape[intoPosition]);
        }

        //public static bool[,] String
    }
}
