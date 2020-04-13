using System;

namespace Tetris2
{
    public struct BlockPattern
    {
        public int ID;
        public int Directions;
        public int X, Y;
        public bool[][,] Shape;

        public BlockPattern(int iD, int directions, int x, int y, string shape)
        {
            ID = iD;
            Directions = directions;
            X = x;
            Y = y;
            Shape = new bool[Directions][,];
            for (int s = 0; s < Directions; s++)
                Shape[s] = new bool[x, y];
            for (int r = 0; r < Directions; r++)
                for (int j = 0; j < y; j++)
                    for (int i = 0; i < x; i++)
                    {
                        Shape[r][i, j] = (shape[r * x * y + j * y + i] == '0') ? false : true;
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
