using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Tetris2
{
    public class Block
    {
        public int Direction;
        public int ShapeID { get; private set; }
        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }
        public bool[,] Shape;
        public int Directions { get; private set; }
        public Color4B Color { get; private set; }
        public Canvas Canvas { get; private set; }
        public int RotationOffset;

        public double CoordinatesX, CoordinatesY;
        public DateTime CoordinatesT;
        public double CoordinatesV;
        public int ghostCoordX;

        public double fallingDistance_Helper;

        private static int id;
        public int ID { get; }

        public Block()
            : this(0, 0, 1, 1, 1, new Color4B(200, 200, 200), new bool[1, 1] { { true } })
        { }
        public Block(int shapeID, int direction, int directions, int dimensionX, int dimensionY, Color4B color, bool[,] shape) 
           : this(shapeID, direction, directions, dimensionX, dimensionY, color, shape, 0)
        { }
        public Block(int shapeID, int direction, int directions, int dimensionX, int dimensionY, Color4B color, bool[,] shape, int offset)
        {
            ShapeID = shapeID;
            Direction = direction;
            Directions = directions;
            DimensionX = dimensionX;
            DimensionY = dimensionY;
            Color = color;
            Shape = new bool[DimensionX, DimensionY];
            Shape = shape ?? throw new ArgumentNullException(nameof(shape));
            this.Canvas = Painter.PaintBlock(this);
            ID = id++;
        }

        /// <summary>
        /// Boolean to String
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public string BTS(bool b)
        {
            if (b) return "W";
            else return "....";
        }

        public override string ToString()
        {
            string s = "";
            for (int y = DimensionY - 1; y >= 0; y--)
            {
                s += "Y" + y.ToString() + ": ";
                for (int x = 0; x < DimensionX; x++)
                {
                    //s += Shape[x, y].ToString() + ", ";
                    s += BTS(Shape[x, y]) + ", ";
                }
                if (y != 0) s += Environment.NewLine;
            }
            return String.Format("ID:{0},", ID)
           + Environment.NewLine + String.Format("X, Y: {0}, {1}", CoordinatesX, CoordinatesY)
           + Environment.NewLine + String.Format(" Size: {0} * {1}", DimensionX, DimensionY)
           + Environment.NewLine + String.Format(" Color: {0}", Color)
           + Environment.NewLine + "Shape ID, Direction (x of n):"
           + Environment.NewLine + String.Format("{0}, ( {1} / {2} )", ShapeID, Direction, Directions)
           + Environment.NewLine + s;
        }
    }
}
