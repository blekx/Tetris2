using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris2
{
    public class Block
    {
        public double CoordinatesX, CoordinatesY;
        public int ShapeID { get; private set; }
        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }
        public bool[,] Shape;
        public Color4B Color { get; private set; }

        public Block(int shapeID, int dimensionX, int dimensionY, Color4B color, bool[,] shape)
        {
            ShapeID = shapeID;
            DimensionX = dimensionX;
            DimensionY = dimensionY;
            Color = color;
            Shape = new bool[DimensionX, DimensionY];
            Shape = shape ?? throw new ArgumentNullException(nameof(shape));
        }

        public override string ToString()
        {
            string s = "";
            for (int y = DimensionY - 1; y >= 0; y--)
            {
                s += "Y" + y.ToString() + ": ";
                for (int x = 0; x < DimensionX; x++)
                    s += x.ToString() + ", ";
                if (y != 0) s += Environment.NewLine;
            }
            return String.Format("X, Y: {0}, {1}", CoordinatesX, CoordinatesY)
           + Environment.NewLine + String.Format("Size: {0} * {1}", DimensionX, DimensionY)
           + Environment.NewLine + String.Format("Shape ID, Color: {0}, {1}", ShapeID, Color)
           + Environment.NewLine + s;
        }
    }
}
