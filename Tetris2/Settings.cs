using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Tetris2
{
    public static class Settings
    {
        // Game Grid
        public static int blockResolution = 10;
        public static int gameFieldX = 10;
        public static int gameFieldY = 24;
        public static int gameFrameWidth = 5;
        public static int gameFramePadding = 3;
        public static int gameUpperFrameHeight_Blocks = 3;
        public static int gameRightFrameWidth_Blocks = 4;
        public static Brush backgroundBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        public static double radialGradient_rX = 1.6;
        public static double radialGradient_rY = 1.2;
        public static Point radialGradient_origin = new Point(0.5, 0.5);
        public static Color4B radialGradient_c1 = new Color4B(112, 23, 28);
        public static Color4B radialGradient_c2 = new Color4B(205, 175, 0);
        public static double radialGradient_o1 = 0.15;
        public static double radialGradient_o2 = 0.6;
        public static double maxIgnoredLatency_ratioFromTick = 0.2;
        public static int preparedBlocks = 3;


        public static string TicksToReadable(long t)
        {
            //return string.Format("{0:00,000.00}ms {1:0000}", (double)(t % 1000000000) / 10000, t % 10000);
            return string.Format("{0:00,000.00}ms", (double)(t % 1000000000) / 10000);
        }
    }
}
