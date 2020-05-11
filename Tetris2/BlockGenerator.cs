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
        private static BlockPatternS[] Patterns =
        {
            new BlockPatternS(0,1,1,1,"10"),          // .
            new BlockPatternS(1,1,2,2,"1111" +
                                      "0"),       // 2x2
            new BlockPatternS(2,4,3,2,"111010" +  // T
                                      "101110" +
                                      "010111" +
                                      "011101" +
                                      "0100"),
            new BlockPatternS(3,2,3,2,"011110" +  // Z
                                      "101101" +
                                      "01"),
            new BlockPatternS(4,2,3,2,"110011" +  // S
                                      "011110" +
                                      "00"),
            new BlockPatternS(5,4,2,3,"111010" +  // L
                                      "100111" +
                                      "010111" +
                                      "111001" +
                                      "1000"),
            new BlockPatternS(6,4,2,3,"110101" +  // (L)R
                                      "111100" +
                                      "101011" +
                                      "001111" +
                                      "0010"),
            new BlockPatternS(7,2,1,4,"1111" + // I
                                      "1111" +
                                      "20"),
        };
        private static Color4B[] BlockColors =
        {
            new Color4B(100,255,255,255), // Gr  .
            new Color4B(255,127,0),   // O   2x2
            new Color4B(40,40,255),   // B   T
            new Color4B(0,255,255),   // C   Z
            new Color4B(127,0,255),   // P   S
            new Color4B(255,255,0),   // Y   L
            new Color4B(255,0,127),   // M  (L)R
            new Color4B(40,255,40),   // G   I
        };
        private static Random random = new Random();

        public static Block NewBlockRandomOrientation()
        {
            return RandomlyRotateBlock(NewBlockDefault());
        }

        /// <summary>  NEW BLOCK, default orientation, patternt 1..7 out of 0..7</summary>
        public static Block NewBlockDefault()
        {
            BlockPatternS p = Patterns[random.Next(1, Patterns.Length)];
            return new Block(p.ID, 0, p.Directions, p.X, p.Y, BlockColors[p.ID], p.Shape[0], p.X_Offsets[0]);
        }

        public static Block RandomlyRotateBlock(Block b)
        {
            return (RotateBlock(b, random.Next(b.Directions)));
        }

        /// <summary>  NEW BLOCK, (direction derived from entering parameter block) ...just counts the final orientation</summary>
        public static Block RotateBlock(Block b, int rotations)
        {
            int direction = b.Direction + rotations;
            while (direction < 0) { direction += b.Directions; }
            direction = direction % b.Directions;
            return RotateBlockSimply(b, direction);
        }

        /// <summary> NEW BLOCK (from: given B, orientation, and Pattern) </summary>
        private static Block RotateBlockSimply(Block b, int intoOrientation)
        {
            BlockPatternS p = Patterns[b.ShapeID];
            int previousOffset = b.RotationOffset;
            int dimX = (intoOrientation % 2 == 0) ? p.X : p.Y;
            int dimY = (intoOrientation % 2 == 0) ? p.Y : p.X;
            Block result = new Block(p.ID, intoOrientation, p.Directions, dimX, dimY, BlockColors[p.ID], p.Shape[intoOrientation], p.X_Offsets[intoOrientation]);
            result.CoordinatesX = b.ghostCoordX + result.RotationOffset - previousOffset;
            result.CoordinatesY = b.CoordinatesY;
            return result;
        }

        internal static Block Ghost(Block activeBlock, int height)
        {
            Block result = new Block(activeBlock.ShapeID, activeBlock.Direction, activeBlock.Directions,
                activeBlock.DimensionX, activeBlock.DimensionY, BlockColors[0], activeBlock.Shape);

            result.CoordinatesX = activeBlock.ghostCoordX;
            result.CoordinatesY = height;

            return result;
        }

        //    /// <summary>
        //    /// not proven------------------------------------
        //    /// </summary>
        //    /// <param name="b"></param>
        //    /// <returns></returns>
        //    public static Block CutBlock(Block b)
        //    {
        //        int minX = b.DimensionX;
        //        int minY = b.DimensionY;
        //        int maxX = 0;
        //        int maxY = 0;
        //        for (int i = 0; i < b.DimensionX; i++)
        //            for (int j = 0; j < b.DimensionY; j++)
        //            {
        //                if (b.Shape[i, j])
        //                {
        //                    if (i < minX) minX = i;
        //                    if (j < minY) minY = j;
        //                    if (i > maxX) maxX = i;
        //                    if (j > maxY) maxY = j;
        //                }
        //            }
        //        int dimX = maxX - minX + 1;
        //        int dimY = maxY - minY + 1;
        //        bool[,] newShape = new bool[dimX, dimY];
        //        for (int i = 0; i < dimX; i++)
        //            for (int j = 0; j < dimY; j++)
        //            {
        //                newShape[i, j] = b.Shape[i + minX, j + minY];
        //            }
        //        Block result = new Block(b.ShapeID, b.Direction, b.Directions, dimX, dimY, BlockColors[b.ShapeID], newShape);
        //        result.CoordinatesX = b.CoordinatesX + minX;
        //        result.CoordinatesY = b.CoordinatesY + minY;
        //        return result;
        //}
    }
}
