using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace Tetris2
{
    class BorderBlinker
    {
        private Grid gridMain;
        private Brush borderColor = new SolidColorBrush(Color.FromArgb(255, 200, 150, 0));
        private Brush colorTransparent = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));

        public BorderBlinker(Grid gridMain)
        {
            this.gridMain = gridMain;

            AddRectangle(0, 0, 1, 1);
            AddRectangle(0, 1, 1, gridMain.ColumnDefinitions.Count-2);
            AddRectangle(0, gridMain.ColumnDefinitions.Count-1, 1, 1);
            AddRectangle(1, 0, gridMain.RowDefinitions.Count - 2, 1);
            AddRectangle(1, gridMain.ColumnDefinitions.Count - 1, gridMain.RowDefinitions.Count - 2, 1);
            AddRectangle(gridMain.RowDefinitions.Count - 1, 0, 1, 1);
            AddRectangle(gridMain.RowDefinitions.Count - 1, gridMain.ColumnDefinitions.Count - 1, 1, 1);
            AddRectangle(gridMain.RowDefinitions.Count - 1, 1, 1, gridMain.ColumnDefinitions.Count-2);
        }

        private void AddRectangle(int row,int col,int rSpan, int cSpan)
        {
            Rectangle r = new Rectangle { Fill = colorTransparent, };
            r.MouseEnter += new MouseEventHandler(PaintActive);
            //r1.MouseMove += new MouseEventHandler(PaintActive);
            r.MouseLeave += new MouseEventHandler(PaintINactive);
            Grid.SetRow(r, row);
            Grid.SetColumn(r, col);
            Grid.SetColumnSpan(r, cSpan);
            Grid.SetRowSpan(r, rSpan);

            gridMain.Children.Add(r);
        }

        private void PaintINactive(object r, EventArgs e)//MouseButtonEventArgs e)
        {
            ((Rectangle)r).Fill = colorTransparent;
        }

        //private void PaintActive(Rectangle r)//, System.EventArgs e)
        private void PaintActive(object r, EventArgs e)
        {
            //throw new Exception(((Rectangle)r).ToString());
            ((Rectangle)r).Fill = borderColor;
        }
    }
}
