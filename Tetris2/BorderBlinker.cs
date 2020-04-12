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
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace Tetris2
{
    public class BorderBlinker
    {
        private MainWindow mw;
        private Grid gridMain;
        private Brush borderColor = new SolidColorBrush(Color.FromArgb(255, 200, 150, 0));
        private Brush colorTransparent = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
        private bool currentlyResizing;
        private ResizeWindowStartingParameters resizeFrom;
        private Point mouseMovedTo;

        private DispatcherTimer mouseHunterTimer;

        public BorderBlinker(MainWindow mw)//Grid gridMain)
        {
            //this.gridMain = gridMain;
            this.mw = mw;
            this.gridMain = mw.gridMain;

            mouseHunterTimer = new DispatcherTimer(DispatcherPriority.Render);
            mouseHunterTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            mouseHunterTimer.Tick += new EventHandler(MouseHunter_Tick);


            AddRectangle(DV.T, DH.L, 0, 0, 1, 1); ;
            AddRectangle(DV.T, DH.M, 0, 1, 1, gridMain.ColumnDefinitions.Count - 2);
            AddRectangle(DV.T, DH.R, 0, gridMain.ColumnDefinitions.Count - 1, 1, 1);
            AddRectangle(DV.M, DH.L, 1, 0, gridMain.RowDefinitions.Count - 2, 1);
            AddRectangle(DV.M, DH.R, 1, gridMain.ColumnDefinitions.Count - 1, gridMain.RowDefinitions.Count - 2, 1);
            AddRectangle(DV.B, DH.L, gridMain.RowDefinitions.Count - 1, 0, 1, 1);
            AddRectangle(DV.B, DH.M, gridMain.RowDefinitions.Count - 1, 1, 1, gridMain.ColumnDefinitions.Count - 2);
            AddRectangle(DV.B, DH.R, gridMain.RowDefinitions.Count - 1, gridMain.ColumnDefinitions.Count - 1, 1, 1);
        }

        private void AddRectangle(DV dirVer, DH dirHor, int row, int col, int rSpan, int cSpan)
        {
            Rectangle r = new Rectangle { Fill = colorTransparent, Name = "rectangle" + dirVer.ToString() + dirHor.ToString(), };
            r.MouseEnter += new MouseEventHandler(Border_MouseEnter);
            r.MouseLeave += new MouseEventHandler(Border_MouseLeave);
            r.MouseDown += new MouseButtonEventHandler(ResizeStart);
            //r1.MouseMove += new MouseEventHandler(PaintActive);
            r.MouseMove += new MouseEventHandler(Border_MouseMove);
            r.MouseUp += new MouseButtonEventHandler(ResizeStop);
            Grid.SetRow(r, row);
            Grid.SetColumn(r, col);
            Grid.SetColumnSpan(r, cSpan);
            Grid.SetRowSpan(r, rSpan);

            gridMain.Children.Add(r);
        }

        private void ResizeStart(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left
                    &&
                currentlyResizing == false)
            {
                resizeFrom = new ResizeWindowStartingParameters(mw, ((Rectangle)sender).Name, e.GetPosition(mw));
                currentlyResizing = true;
            }
        }

        private void ResizeStop(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left
                    &&
                currentlyResizing == true)
            {
                currentlyResizing = false;
                //mouseHunterTimer.Stop();
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentlyResizing)
            {
                ResizeWindow();
            }
        }

        private void MouseHunter_Tick(object sender, EventArgs e)
        {
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            mouseMovedTo = GetMousePosition();
            if (resizeFrom.VerticalDirection == DV.B)
            {
                mw.Height = resizeFrom.Height + (mouseMovedTo.Y - resizeFrom.MouseStart.Y);
            }
            if (resizeFrom.HorizontalDirection == DH.R)
            {
                mw.Width = resizeFrom.Width + (mouseMovedTo.X - resizeFrom.MouseStart.X);
            }
            if (resizeFrom.VerticalDirection == DV.T)
            {
                mw.Top = resizeFrom.Top + (mouseMovedTo.Y - resizeFrom.MouseStart.Y);
                mw.Height = resizeFrom.Height - (mouseMovedTo.Y - resizeFrom.MouseStart.Y);
            }
            if (resizeFrom.HorizontalDirection == DH.L)
            {
                mw.Left = resizeFrom.Left + (mouseMovedTo.X - resizeFrom.MouseStart.X);
                mw.Width = resizeFrom.Width - (mouseMovedTo.X - resizeFrom.MouseStart.X);
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePositionDLL()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        ///// <summary>
        ///// doesnt work out of window
        ///// </summary>
        ///// <returns></returns>
        //private Point GetMousePosition()
        //{
        //    Point relative = Mouse.GetPosition(mw);
        //    Point absolute = new Point(relative.X + mw.Left, relative.Y + mw.Top);
        //    return absolute;
        //}

        private Point GetMousePosition()
        {
            return GetMousePositionDLL();
        }

        private void Border_MouseLeave(object r, EventArgs e)//MouseButtonEventArgs e)
        {
            if (currentlyResizing)
            {
                //mouseHunterTimer.Start();
                ResizeWindow();
            }
            else
            {
                ((Rectangle)r).Fill = colorTransparent;
            }
        }

        //private void PaintActive(Rectangle r)//, System.EventArgs e)
        private void Border_MouseEnter(object r, EventArgs e)
        {
            if (currentlyResizing)
            {
                //mouseHunterTimer.Stop();
                ResizeWindow();
            }
            else
            {
                ((Rectangle)r).Fill = borderColor;
            }
            //throw new Exception(((Rectangle)r).ToString());
            //MessageBox.Show(((Rectangle)r).Name);
        }

        internal string Report()
        {
            return resizeFrom.ToString() + Environment.NewLine
                + GetMousePosition().ToString()
                + Environment.NewLine
                + "rectangle"[6] + "rectangle"[7] + "rectangle"[8];
        }
    }
}
