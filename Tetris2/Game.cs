using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tetris2
{
    public class Game
    {
        //public Image Image;
        //private WriteableBitmap bitmap;
        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }
        public double Gravity { get; private set; }
        /// <summary>
        /// [0,0] = Left, Bottom
        /// </summary>
        private bool[,] boolField;
        private List<Block> AllFieldBlocks = new List<Block>();
        private List<Block> FallingBlocks = new List<Block>();
        public Viewbox ParentControlElement { get; private set; }
        //private Viewbox parent;
        private Grid gameGrid;
        private Canvas canvasPlayground;
        private Canvas canvasUp;
        private Canvas canvasRight;


        //public Game(WriteableBitmap bitmap, int dimensionX, int dimensionY, double gravity)
        public Game(object parentControl, int dimensionX, int dimensionY, double gravity)
        {
            //this.bitmap = bitmap;
            //Image = image;
            ParentControlElement = parentControl as Viewbox;
            DimensionX = dimensionX;
            DimensionY = dimensionY;
            Gravity = gravity;
            boolField = new bool[DimensionX, DimensionY * 2];

            CreateOwnEnvironment();
        }

        //public Game(WriteableBitmap bitmap)
        //    : this(bitmap, 10, 20, 1.5) { }
        //public Game(Image image)
        //            : this((WriteableBitmap)image.Source, 10, 20, 1.5) { }
        public Game(object parentControl)
               : this(parentControl, 10, 20, 1.5) { }
        public Game(Image image)
               : this(image, 10, 20, 1.5) { }
        //public Game(WriteableBitmap image)
        //          : this((Image)image.Source, 10, 20, 1.5) { }

        private void CreateOwnEnvironment()
        {

            gameGrid = new Grid
            {
                Width = Settings.blockResolution * (DimensionX + 4)
                        + Settings.gameFrameWidth * 3
                        + Settings.gameFramePadding * 4,
                Height = Settings.blockResolution * (DimensionY + Settings.gameUpperFrameHeight_Blocks)
                        + Settings.gameFrameWidth * 3
                        + Settings.gameFramePadding * 4,
                Background = new RadialGradientBrush
                {
                    RadiusX = 1.6,
                    RadiusY = 1.2,
                    GradientOrigin = new System.Windows.Point(0.5, .5),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop
                        {
                            Color=                            Color.FromArgb(255,112,23,28),
                            Offset=0.15,
                        },
                        new GradientStop
                        {
                            Color=                            Color.FromArgb(255,205,175,0),
                            Offset=0.6,
                        }
                    }
                },
            };
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(Settings.gameFrameWidth, System.Windows.GridUnitType.Pixel) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = new System.Windows.GridLength(Settings.gameFramePadding * 2 + Settings.gameUpperFrameHeight_Blocks, System.Windows.GridUnitType.Pixel) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(Settings.gameFrameWidth, System.Windows.GridUnitType.Pixel) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition
            { Width = new System.Windows.GridLength(Settings.gameFramePadding * 2 + Settings.blockResolution * DimensionY, System.Windows.GridUnitType.Pixel) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(Settings.gameFrameWidth, System.Windows.GridUnitType.Pixel) });
            //                                                                                          , System.Windows.GridUnitType.Pixel) });
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(Settings.gameFrameWidth) });
            gameGrid.RowDefinitions.Add(new RowDefinition
            { Height = new System.Windows.GridLength(Settings.gameFramePadding * 2 + Settings.blockResolution * DimensionX) });
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(Settings.gameFrameWidth) });
            gameGrid.RowDefinitions.Add(new RowDefinition
            { Height = new System.Windows.GridLength(Settings.gameFramePadding * 2 + Settings.blockResolution * 4) });
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(Settings.gameFrameWidth) });

            Rectangle background1 = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                Width = Settings.gameFramePadding * 2 + Settings.blockResolution * DimensionX,
                Height = Settings.gameFramePadding * 2 + Settings.blockResolution * DimensionX,
            };
            gameGrid.Children.Add(background1);
            Grid.SetColumn(background1, 1);
            Grid.SetRow(background1, 3);
            canvasPlayground = new Canvas();
            canvasPlayground.Background = new SolidColorBrush(Color.FromArgb(200, 0, 200, 255));
            Grid.SetColumn(canvasPlayground, 1);
            Grid.SetRow(canvasPlayground, 3);
            gameGrid.Children.Add(canvasPlayground);

            ParentControlElement.Child = gameGrid;
        }

        public void Update()
        {
            //CheckBlocks();
            //CountFallingBlocks();
            Redraw();
        }

        private void Redraw()
        {
            throw new NotImplementedException();
        }

        private void CountFallingBlocks()
        {
            throw new NotImplementedException();
        }

        private void CheckBlocks()
        {
            throw new NotImplementedException();
        }

        public void HelloBlock()
        {
            Block b = BlockGenerator.NewBlock();
            AllFieldBlocks.Add(b);
        }
        public string ShowHelloBlock()
        {
            AllFieldBlocks.Clear();
            HelloBlock();
            return AllFieldBlocks[0].ToString();
        }

        public Block GiveHelloBlock()
        {
            return AllFieldBlocks[0];
        }
    }
}
