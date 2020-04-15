using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace Tetris2
{
    public static class Painter
    {
        private const int resolution = 10;
        private const int borderWidth = 1;
        private static SolidColorBrush BorderBrush = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));


        public static Canvas PaintBlock(Block b)
        {
            Canvas c = new Canvas
            {
                Width = b.DimensionX * resolution,
                Height = b.DimensionY * resolution,
            };
            for (int y = 0; y < b.DimensionY; y++)
                for (int x = 0; x < b.DimensionX; x++)
                {
                    if (b.Shape[x, y])
                    {
                        Rectangle r = new Rectangle
                        {
                            Height = resolution,
                            Width = resolution,
                            Fill = new SolidColorBrush(Color.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B)),
                        };
                        Canvas.SetLeft(r, x * resolution);
                        Canvas.SetTop(r, (b.DimensionY - 1 - y) * resolution);
                        c.Children.Add(r);
                        c.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        c.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;


                        //if (x == 0 || (x > 0 && b.Shape[x - 1, y] == false))
                        //{//Left
                        //    Rectangle border = new Rectangle
                        //    {
                        //        Height = resolution,
                        //        Width = borderWidth,
                        //        Fill = BorderBrush,
                        //    };
                        //    Canvas.SetLeft(border, x * resolution);
                        //    Canvas.SetTop(border, (b.DimensionY - 1 - y) * resolution);
                        //    c.Children.Add(border);
                        //}
                        //if (x == b.DimensionX - 1 || (x < b.DimensionX && b.Shape[x + 1, y] == false))
                        //{//Right
                        //    Rectangle border = new Rectangle
                        //    {
                        //        Height = resolution,
                        //        Width = borderWidth,
                        //        Fill = BorderBrush,
                        //    };
                        //    Canvas.SetLeft(border, x * resolution + resolution - borderWidth);
                        //    Canvas.SetTop(border, (b.DimensionY - 1 - y) * resolution);
                        //    c.Children.Add(border);
                        //}
                        //if (y == b.DimensionY - 1 || (y < b.DimensionY && b.Shape[x, y + 1] == false))
                        //{//Top
                        //    Rectangle border = new Rectangle
                        //    {
                        //        Height = borderWidth,
                        //        Width = resolution,
                        //        Fill = BorderBrush,
                        //    };
                        //    Canvas.SetLeft(border, x * resolution);
                        //    Canvas.SetTop(border, (b.DimensionY - 1 - y) * resolution);
                        //    c.Children.Add(border);
                        //}
                        //if (y == 0 || (y > 0 && b.Shape[x, y - 1] == false))
                        //{//Bottom
                        //    Rectangle border = new Rectangle
                        //    {
                        //        Height = borderWidth,
                        //        Width = resolution,
                        //        Fill = BorderBrush,
                        //    };
                        //    Canvas.SetLeft(border, x * resolution);
                        //    Canvas.SetTop(border, (b.DimensionY - 1 - y) * resolution + resolution - borderWidth);
                        //    c.Children.Add(border);
                        //}

                        //Left:
                        if (x == 0) AddEdge(b, c, D4.L, x, y);
                        else if (b.Shape[x - 1, y] == false) AddEdge(b, c, D4.L, x, y);
                        //Right:
                        if (x == b.DimensionX - 1) AddEdge(b, c, D4.R, x, y);
                        else if (b.Shape[x + 1, y] == false) AddEdge(b, c, D4.R, x, y);
                        //Top:
                        if (y == b.DimensionY - 1) AddEdge(b, c, D4.T, x, y);
                        else if (b.Shape[x, y + 1] == false) AddEdge(b, c, D4.T, x, y);
                        //Bottom:
                        if (y == 0) AddEdge(b, c, D4.B, x, y);
                        else if (b.Shape[x, y - 1] == false) AddEdge(b, c, D4.B, x, y);
                    }
                }
            return c;
        }

        private static bool IsEdgeVertical(D4 d)
        {
            if (d == D4.L || d == D4.R) return true;
            else return false;
        }

        private static void AddEdge(Block b, Canvas c, D4 d, int x, int y)
        {
            Rectangle border = new Rectangle
            {
                Height = IsEdgeVertical(d) ? resolution : borderWidth,
                Width = IsEdgeVertical(d) ? borderWidth : resolution,
                Fill = BorderBrush,
            };
            int horizontalOffset = (d == D4.R) ? resolution - borderWidth : 0;
            int verticalOffset = (d == D4.B) ? resolution - borderWidth : 0;
            Canvas.SetLeft(border, x * resolution + horizontalOffset);
            Canvas.SetTop(border, (b.DimensionY - 1 - y) * resolution + verticalOffset);
            c.Children.Add(border);
        }
    }
}
