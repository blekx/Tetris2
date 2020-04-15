using System;
using System.Windows.Media;

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

        public Color4B(byte alpha, byte red, byte green, byte blue)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }
        public Color4B(byte red, byte green, byte blue) : this(255, red, green, blue) { }

        public override string ToString()
        {
            return (A == 255) ? string.Format("(a)RGB(___, {0:3}, {1:3}, {2:3})", R, G, B) : string.Format("(a)RGB({3:3}, {0:3}, {1:3}, {2:3})", R, G, B, A);
        }

        public static Color ToColor(Color4B c)
        {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }
        public Color ToColor()
        {
            return Color.FromArgb(A,R, G, B);
        }
    }
}
