using System;

namespace Tetris2
{
    public struct BlockPattern
    {
        public int ID;
        public int Directions;
        public int X, Y;
        //public bool[][,] Shape;
        public int[][,] Shape;

        public BlockPattern(int iD, int directions, int x, int y, string shape)
        {
            ID = iD;
            Directions = directions;
            X = x;
            Y = y;
            Shape = new int[4][,];
            for (int s = 0; s < 4; s++)
                Shape[s] = new int[x, y];
            for (int r = 0; r < 4; r++)
                for (int j = 0; j < y; j++)
                    for (int i = 0; i < x; i++)
                    {
                        //Shape[r][i, j] = (shape[r * x * y + i * y + j] == '0') ? false : true;
                        Shape[r][i, j] = Convert.ToInt32(shape[r * x * y + j * y + i].ToString());
                    }
        }
    }

    public struct Color4B
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color4B(byte red, byte green, byte blue, byte alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        public override string ToString()
        {
            return string.Format("RGBa( {0}, {1}, {2}, {3})", R, G, B, A);
        }
    }
}
