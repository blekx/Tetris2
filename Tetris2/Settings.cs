using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Tetris2
{
    public static class Settings
    {
        // Game Fyzix (physics)
        // default units: square lenght, second
        public static int gameTimerInterval_ms = 30;
        public static int gameMaxLag_ms = 5 * gameTimerInterval_ms;
        public static double gameDefaultGravity_T = 2;
        public static bool isGameSmooth_NotTicking = true;
        public static double g_TickingToSmooth_coef = 4;
        public static double gameSpeedHoriz_PerTick = 0.1;
        public static double gameSpeedHoriz_LastFewSteps = 5;
        public static double gameSpeedHoriz_TotalTicks = 10;
        public static double gameAB_HorizAcceleration = 40;
        public static double gameAB_HorizDecceleration = 80;
        public static double gameAB_HorAgressivenessOfStopping = 3;
        public static int defaultTimetoSidestep_ms = 100;

        // Game Grid
        public static int blockResolution = 10;
        public static int gameFieldX = 10;
        public static int gameFieldY = 30;//24
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

        // Key controls
        public static Key player1_Left = Key.Left;
        public static Key player1_Right = Key.Right;
        public static Key player1_LandBlock = Key.Space;
        public static Key player1_RotateRight = Key.D;
        public static Key player1_RotateLeft = Key.A;
        public static Key player1_RotateTwice = Key.S;
        public static int keyTimerInterval_ms =10;
        public static int gameRepeatingKeyInterval_ms = 80;

        public static string TicksToReadable(long t)
        {
            //return string.Format("{0:00,000.00}ms {1:0000}", (double)(t % 1000000000) / 10000, t % 10000);
            return string.Format("{0:00,000.00}ms", (double)(t % 1000000000) / 10000);
        }

        public static string BoolfieldToString(bool[,] b)
        {
            string s = "";
            string result = "";
            string t = "▓▓ ";
            string f = "░░ ";
            for (int y = b.GetLength(1) / 2; y > -1; y--)
            {
                s = "Y" + y.ToString() + ": ";
                for (int x = 0; x < b.GetLength(0); x++)
                    s += (b[x, y]) ? t : f;
                result += s + Environment.NewLine;
            }
            return result;
        }
    }
}
