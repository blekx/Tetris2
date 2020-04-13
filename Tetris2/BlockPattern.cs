using System;

namespace Tetris2
{
    public struct BlockPattern
    {
        public int ID;
        public int X, Y;
        public bool[][,] Shape;


        public BlockPattern(int iD, int x, int y, string shape)
        {
            ID = iD;
            X = x;
            Y = y;
            Shape = new bool[4][,];
            for (int s = 0; s < 4; s++)
                Shape[s] = new bool[x, y];
            for (int r = 0; r < 4; r++)
                for (int i = 0; i < x; i++)
                    for (int j = 0; j < y; j++)
                    {
                        Shape[r][i, j] = (shape[r * x * y + i * y + j] == '0') ? false : true;
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
