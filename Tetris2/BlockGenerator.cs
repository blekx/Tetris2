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
            new BlockPattern(0,1,1,1,"1"),          // .
            new BlockPattern(1,1,2,2,"1111"),       // 2x2
            new BlockPattern(2,4,3,3,"111010000" +  // T
                                     "010011010" +
                                     "010111000" +
                                     "010110010"),
            new BlockPattern(3,2,3,3,"011110000" +  // Z
                                     "010011001"),
            new BlockPattern(4,2,3,3,"110011000" +  // S
                                     "010110100"),
            new BlockPattern(5,4,3,3,"011010010" +  // L
                                     "100111000" +
                                     "010010110" +
                                     "111100000"),
            new BlockPattern(6,4,3,3,"110010010" +  // (L)R
                                     "111100000" +
                                     "010010011" +
                                     "001111000"),
            new BlockPattern(7,2,4,4,"0010001000100010" + // I
                                     "1111000000000000"),
        };
        private static Color4B[] BlockColors =
        {
            new Color4B(100,100,100), // Gr  .
            new Color4B(255,127,0),   // O   2x2
            new Color4B(40,40,255),   // B   T
            new Color4B(0,255,255),   // C   Z
            new Color4B(127,0,255),   // P   S
            new Color4B(255,255,0),   // Y   L
            new Color4B(255,0,127),   // M  (L)R
            new Color4B(40,255,40),   // G   I
        };
        private static Random random = new Random();

        public static Block NewBlock()
        {
            return RotatedBlock(RandomDefaultBlock());
        }

        private static Block RandomDefaultBlock()
        {
            BlockPattern p = Patterns[random.Next(Patterns.Length)];
            return new Block(p.ID, 0, p.Directions, p.X, p.Y, BlockColors[p.ID], p.Shape[0]);
        }

        private static Block RotatedBlock(Block b)
        {
            return RotateBlockSimply(b, random.Next(b.Directions));
        }

        private static Block RotateBlockSimply(Block b, int intoPosition)
        {
            return new Block(b.ShapeID, intoPosition, b.Directions,
                b.DimensionX, b.DimensionY, b.Color, Patterns[b.ShapeID].Shape[intoPosition]);
        }
    }
}
