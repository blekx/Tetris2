using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tetris2
{
    public class BlockGenerator
    {
        private Random random = new Random();
        private BlockPattern[] Patterns =
        {
            new BlockPattern(0,1,1,"1111"),
            new BlockPattern(1,3,3,"111010000" +
                                   "010011010" +
                                   "010111000" +
                                   "010110010"),
        };
        private Color4B[] BlockColors =
        {
            new Color4B(100,100,100,255),// ID 0
            new Color4B(255,255,0,255),  // ID 1
            new Color4B(0,0,255,255),    // ID 2
        };

        public Block NewBlock()
        {
            //Block b = new Block(3, 2, 2, new Color4B(206, 0, 0, 255), new bool[2, 2] { { true, false }, { true, true } });
            //return b;
            //return new Block(1, 2, 2, new Color4B(200, 0, 0, 255), new bool[2, 2] { { true, true }, { true, true } });
            return  RotatedBlock(RandomDefaultBlock());
        }

        private Block RandomDefaultBlock()
        {
            BlockPattern p = Patterns[random.Next(Patterns.Length)];
            return null;//new Block(p.ID, p.X, p.Y, BlockColors[p.ID], p.Shape[0]);
        }

        private Block RotatedBlock(Block b)
        {
            return RotateBlockSimply(b, random.Next(4));
        }

        private Block RotateBlockSimply(Block b, int intoPosition)
        {
            //b.Shape=
            return new Block(b.DimensionX, b.DimensionY, b.ShapeID, b.Color, Patterns[b.ShapeID].Shape[intoPosition]);
        }

        //public static bool[,] String
    }
}
