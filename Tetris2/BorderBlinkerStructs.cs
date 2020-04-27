using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace Tetris2
{
    /// <summary>
    /// Directions (Top,Middle,Bottom< / >Left,Middle,Right)
    /// </summary>
    public enum D8
    {
        TL,
        TM,
        TR,
        ML,
        MR,
        BL,
        BM,
        BR,
    }

    /// <summary>
    /// Directions (Top, Right, Bottom, Left)
    /// </summary>
    public enum D4
    {
        T,
        R,
        B,
        L,
    }

    /// <summary>
    /// Vertical Direction ( Top, Middle, Bottom )
    /// </summary>
    public enum DV
    {
        T,
        M,
        B,
    }

    /// <summary>
    /// Horizontal Direction ( Left, Middle, Right )
    /// </summary>
    public enum DH
    {
        L,
        M,
        R,
    }

    /// <summary>
    /// a struct for BorderBlinker
    /// </summary>
    public struct ResizeWindowStartingParameters
    {
        public double Left, Top, Width, Height;
        public DV VerticalDirection;
        public DH HorizontalDirection;
        public Point MouseStart;

        /// <summary>
        /// (constructor of) a struct for BorderBlinker
        /// </summary>
        /// <param name="mw"></param>
        /// <param name="directions">rectangleTL</param>
        /// <param name="relative"></param>
        public ResizeWindowStartingParameters(MainWindow mw, string directions, Point relative)
        {
            Left = mw.Left;
            Top = mw.Top;
            Width = mw.Width;
            Height = mw.Height;
            if (directions[9] == 'T') VerticalDirection = DV.T;
            else if (directions[9] == 'B') VerticalDirection = DV.B;
            else VerticalDirection = DV.M;
            if (directions[10] == 'L') HorizontalDirection = DH.L;
            else if (directions[10] == 'R') HorizontalDirection = DH.R;
            else HorizontalDirection = DH.M;
            MouseStart = new Point(relative.X + mw.Left, relative.Y + mw.Top);
        }

        public override string ToString()
        {
            return String.Format("LTWH: {0}, {1}, {2}, {3}", Left, Top, Width, Height, Width)
                + Environment.NewLine + String.Format("vert, hor: {0}, {1}", VerticalDirection.ToString(), HorizontalDirection.ToString())
                + Environment.NewLine + String.Format("mouse X, Y: {0}, {1}", MouseStart.X, MouseStart.Y);
        }
    }

    public struct KeyHappened
    {

    }
}
