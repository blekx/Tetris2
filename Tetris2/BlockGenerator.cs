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

        private static List<Block> SplitOneAreaIntoBlocks(bool[,] shape)
        {
            List<Block> r = new List<Block>();
            throw new NotImplementedException();
            return r;
        }

        public static List<Block> LowerCutRemains(Block b, int yLine, int heightAbove)
        {
            throw new NotImplementedException();
        }
        public static List<Block> UpperCutRemains(Block b, int yLine, int heightAbove)
        {
            if ((b.Shape.GetLength(0) != b.DimensionX) ||
                 (b.Shape.GetLength(1) != b.DimensionY))
                System.Windows.MessageBox.Show("Array Dimensions Error" + b.ToString());
            throw new NotImplementedException();
            return SplitOneAreaIntoBlocks(b.Shape);
            //return r;
        }

        public static Block NewlyCutBlocks(Block b, List<int> lines)
        {
            throw new NotImplementedException();
        }

        internal static List<Block> GiveRemainingBlockies(Block b, List<int> lines)
        {
            //throw new NotImplementedException();
            int x = b.Shape.GetLength(0);
            int y = b.Shape.GetLength(1);
            List<Block> result = new List<Block>();

            //copy bf:
            bool[,] newBoolfield = new bool[x, y];
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    newBoolfield[i, j] = b.Shape[i, j];

            foreach (int l in lines)
            {
                ///c = cleared line of one block
                int c = l - (int)(b.CoordinatesY);
                if (c >= 0 && c < b.DimensionY)
                    for (int i = 0; i < x; i++)
                        newBoolfield[i, c] = false;
            } // ^ delete the completed lines

            bool done = false;
            while (!done)
            {
                done = true;
                int i;
                int j = 0;
                for (i = 0; i < x; i++)
                {
                    for (j = 0; j < y; j++)
                        if (newBoolfield[i, j])
                        {
                            #region //exploring new shape
                            bool[,] newShapeUncut = new bool[x, y];
                            int minX = i;
                            int minY = j;
                            int maxX = i;
                            int maxY = j;
                            //bool keepExploring = true;
                            //loc
                            List<Coords> todo = new List<Coords> { new Coords(i, j) };
                            newBoolfield[i, j] = false;
                            newShapeUncut[i, j] = true;
                            while (todo.Count > 0)
                            {
                                Coords current = todo[0];
                                if (current.X + 1 < x)
                                    if (newBoolfield[current.X + 1, current.Y])
                                    {
                                        todo.Add(new Coords(current.X + 1, current.Y));
                                        if (current.X + 1 > maxX) maxX = current.X + 1;
                                        newBoolfield[current.X + 1, current.Y] = false;
                                        newShapeUncut[current.X + 1, current.Y] = true;
                                    }
                                if (current.X - 1 >= 0)
                                    if (newBoolfield[current.X - 1, current.Y])
                                    {
                                        todo.Add(new Coords(current.X - 1, current.Y));
                                        if (current.X - 1 < minX) minX = current.X - 1;
                                        newBoolfield[current.X - 1, current.Y] = false;
                                        newShapeUncut[current.X - 1, current.Y] = true;
                                    }
                                if (current.Y + 1 < y)
                                    if (newBoolfield[current.X, current.Y + 1])
                                    {
                                        todo.Add(new Coords(current.X, current.Y + 1));
                                        if (current.Y + 1 > maxY) maxY = current.Y + 1;
                                        newBoolfield[current.X, current.Y + 1] = false;
                                        newShapeUncut[current.X, current.Y + 1] = true;
                                    }
                                if (current.Y - 1 >= 0)
                                    if (newBoolfield[current.X, current.Y - 1])
                                    {
                                        todo.Add(new Coords(current.X, current.Y - 1));
                                        if (current.Y - 1 < minY) minY = current.Y - 1;
                                        newBoolfield[current.X, current.Y - 1] = false;
                                        newShapeUncut[current.X, current.Y - 1] = true;
                                    }
                                todo.RemoveAt(0);
                                //List of coords
                                //look around
                                //remove this one
                            }
                            result.Add(CreateRemains(b, newShapeUncut, minX, minY, maxX, maxY));
                            #endregion
                            done = false; //found some remains, will look for another ones
                            break;
                        }
                    if (done == false)
                        break;
                }
                if (i == x - 1 && j == y - 1) done = true;
            }
            return result;
        }

        private static Block CreateRemains(Block b, bool[,] newShapeUncut, int minX, int minY, int maxX, int maxY)
        {
            bool[,] newShapeCut = new bool[maxX + 1 - minX, maxY + 1 - minY];
            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    newShapeCut[x - minX, y - minY] = newShapeUncut[x, y];
            Block result = new Block(0, 0, 1, maxX + 1 - minX, maxY + 1 - minY, b.Color, newShapeCut);
            result.CoordinatesX = b.CoordinatesX + minX;
            result.ghostCoordX = (int)result.CoordinatesX;
            result.CoordinatesY = b.CoordinatesY + minY;
            return result;
            throw new NotImplementedException();
        }

        public struct Coords
        {
            public int X;
            public int Y;

            public Coords(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }



    }
}
