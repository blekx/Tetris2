using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris2
{
    class Boolfield
    {
        private bool[,] boolField;

        public Boolfield(int x,int y)
        {
            boolField = new bool[x, y];
        }

        public override string ToString()
        {
            string s = "";
            string result = "";
            string t = "▓▓ ";
            string f = "░░ ";
            for (int y = boolField.GetLength(1) / 2; y > -1; y--)
            {
                s = "Y" + y.ToString() + ": ";
                for (int x = 0; x < boolField.GetLength(0); x++)
                    s += (boolField[x, y]) ? t : f;
                result += s + Environment.NewLine;
            }
            return result;
        }
    }
}
