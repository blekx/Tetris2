using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris2
{
    public static class Settings
    {
        // Game Grid
        public static int blockResolution = 10;
        public static int gameFrameWidth = 5;
        public static int gameFramePadding = 3;
        public static int gameUpperFrameHeight_Blocks = 3;
        public static int gameRightFrameWidth_Blocks = 4;
        public static Brush backgroundBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
    }
}
