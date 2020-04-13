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
                        if (x == 0 || (x > 0 && b.Shape[x - 1, y] == false))
                        {//Left
                            Rectangle border = new Rectangle
                            {
                                Height = resolution,
                                Width = borderWidth,
                                Fill = BorderBrush,
                            };
                            Canvas.SetLeft(r, x * resolution);
                            Canvas.SetTop(r, (b.DimensionY - 1 - y) * resolution);
                            c.Children.Add(border);
                        }
                        if (x == b.DimensionX || (x < b.DimensionX && b.Shape[x + 1, y] == false))
                        {//Right
                            Rectangle border = new Rectangle
                            {
                                Height = resolution,
                                Width = borderWidth,
                                Fill = BorderBrush,
                            };
                            Canvas.SetLeft(r, x * resolution + resolution - borderWidth);
                            Canvas.SetTop(r, (b.DimensionY - 1 - y) * resolution);
                            c.Children.Add(border);
                        }
                        if (y == b.DimensionY || (y < b.DimensionY && b.Shape[x, y + 1] == false))
                        {//Top
                            Rectangle border = new Rectangle
                            {
                                Height = borderWidth,
                                Width = resolution,
                                Fill = BorderBrush,
                            };
                            Canvas.SetLeft(r, x * resolution);
                            Canvas.SetTop(r, (b.DimensionY - 1 - y) * resolution);
                            c.Children.Add(border);
                        }
                        if (y == 0 || (y > 0 && b.Shape[x, y - 1] == false))
                        {//Bottom
                            Rectangle border = new Rectangle
                            {
                                Height = borderWidth,
                                Width = resolution,
                                Fill = BorderBrush,
                            };
                            Canvas.SetLeft(r, x * resolution);
                            Canvas.SetTop(r, (b.DimensionY - 1 - y) * resolution + resolution - borderWidth);
                            c.Children.Add(border);
                        }
                    }
                }
            return c;
        }
    }
}
