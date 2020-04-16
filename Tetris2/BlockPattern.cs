using System;
using System.Windows.Media;

namespace Tetris2
{
    public struct BlockPatternS
    {
        public int ID;
        public int Directions;
        public int X, Y;
        public bool[][,] Shape;
        public int[] X_Offsets;

        private void SwapXY()
        { int c = X; X = Y; Y = c; }

        public BlockPatternS(int iD, int directions, int x, int y, string shape)
        {
            ID = iD;
            Directions = directions;
            X = x;
            Y = y;
            Shape = new bool[Directions][,];
            X_Offsets = new int[Directions];
            for (int s = 0; s < Directions; s++)
            {
                Shape[s] = new bool[X, Y];
                SwapXY();
                string offset = shape[Directions * x * y + s].ToString();
                X_Offsets[s] = Convert.ToInt32(offset);
            }
            if (Directions % 2 == 1) SwapXY();
            for (int r = 0; r < Directions; r++)
            {
                for (int j = 0; j < Y; j++)
                    for (int i = 0; i < X; i++)
                        Shape[r][i, j] = (shape[r * x * y + j * X + i] == '0') ? false : true;
                SwapXY();
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
            return Color.FromArgb(A, R, G, B);
        }
    }
}
