using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Tetris2
{
    public class Game
    {
        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }
        private double gravity;
        private int Tick_ms;
        private int maxIgnoredLatency;
        /// <summary>
        /// [0,0] = Left, Bottom
        /// </summary>
        private bool[,] boolField;
        private List<Block> allFieldBlocks = new List<Block>();
        private List<Block> fallingBlocks = new List<Block>();
        private List<Block> blocksToRedraw = new List<Block>();

        public Viewbox ParentControlElement { get; private set; }
        //private Viewbox parent;
        private Grid gameGrid;
        private Canvas canvasPlayground;
        private Canvas canvasUp;
        private Canvas canvasRight;

        private DispatcherTimer gameTimer;
        /// <summary>
        /// sheduled, unreal value
        /// </summary>
        //private DateTime nextGameUpdate = DateTime.Now;
        public DateTime nextGameUpdate = DateTime.Now;

        #region Constructing
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
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            gameTimer.Tick += new EventHandler(GameTimer_Tick);
            gameTimer.Start();
        }
        public double Gravity
        {
            get => gravity;
            private set
            {
                gravity = value;
                Tick_ms = (int)(1000 / gravity);
                maxIgnoredLatency = (int)(Tick_ms * Settings.maxIgnoredLatency_ratioFromTick);
            }
        }

        //public Game(WriteableBitmap bitmap)
        //    : this(bitmap, 10, 20, 1.5) { }
        //public Game(Image image)
        //            : this((WriteableBitmap)image.Source, 10, 20, 1.5) { }
        public Game(object parentControl)
               : this(parentControl, 10, 20, 1) { }
        public Game(Image image)
               : this(image, 10, 20, 1) { }
        //public Game(WriteableBitmap image)
        //          : this((Image)image.Source, 10, 20, 1.5) { }

        private void CreateOwnEnvironment()
        {
            gameGrid = new Grid
            {
                Width = Settings.blockResolution * (DimensionX + Settings.gameRightFrameWidth_Blocks) + Settings.gameFrameWidth * 3 + Settings.gameFramePadding * 4,
                Height = Settings.blockResolution * (DimensionY + Settings.gameUpperFrameHeight_Blocks) + Settings.gameFrameWidth * 3 + Settings.gameFramePadding * 4,
                Background = new RadialGradientBrush
                {
                    RadiusX = Settings.radialGradient_rX,
                    RadiusY = Settings.radialGradient_rY,
                    GradientOrigin = Settings.radialGradient_origin,
                    GradientStops = new GradientStopCollection {
                    new GradientStop { Color = (Settings.radialGradient_c1).ToColor(), Offset = Settings.radialGradient_o1, }, new GradientStop { Color = (Settings.radialGradient_c2).ToColor(), Offset = Settings.radialGradient_o2, } }
                },
            };
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(Settings.gameFrameWidth, System.Windows.GridUnitType.Pixel) });
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(Settings.gameFramePadding * 2 + Settings.blockResolution * Settings.gameUpperFrameHeight_Blocks) });
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(Settings.gameFrameWidth, System.Windows.GridUnitType.Pixel) });
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(Settings.gameFramePadding * 2 + Settings.blockResolution * DimensionY) });
            gameGrid.RowDefinitions.Add(new RowDefinition { Height = new System.Windows.GridLength(Settings.gameFrameWidth) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(Settings.gameFrameWidth) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(Settings.gameFramePadding * 2 + Settings.blockResolution * DimensionX) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(Settings.gameFrameWidth) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(Settings.gameFramePadding * 2 + Settings.blockResolution * Settings.gameRightFrameWidth_Blocks) });
            gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new System.Windows.GridLength(Settings.gameFrameWidth) });

            Add4Backgrounds();

            canvasPlayground = new Canvas { Background = new SolidColorBrush(Color.FromArgb(200, 0, 200, 255)), Margin = new System.Windows.Thickness(Settings.gameFramePadding) };
            Grid.SetColumn(canvasPlayground, 1);
            Grid.SetRow(canvasPlayground, 3);
            gameGrid.Children.Add(canvasPlayground);

            ParentControlElement.Child = gameGrid;
        }

        private void Add4Backgrounds()
        {
            Rectangle[] bg = new Rectangle[4];
            for (int i = 0; i < 4; i++)
            {
                bg[i] = new Rectangle { Fill = Settings.backgroundBrush };
                //Width = Settings.gameFramePadding * 2 + Settings.blockResolution * DimensionX,
                //Height = Settings.gameFramePadding * 2 + Settings.blockResolution * DimensionY,
                gameGrid.Children.Add(bg[i]);
            }
            Grid.SetColumn(bg[0], 1);
            Grid.SetColumn(bg[1], 3);
            Grid.SetColumn(bg[2], 1);
            Grid.SetColumn(bg[3], 3);
            Grid.SetRow(bg[0], 3);
            Grid.SetRow(bg[1], 3);
            Grid.SetRow(bg[2], 1);
            Grid.SetRow(bg[3], 1);
        }
        #endregion

        private void HelperTextOut() => HelperTextOut("nGU: " + Settings.TicksToReadable(nextGameUpdate.Ticks));
        private void HelperTextOut(string s)
        {
            TextBlock tb = new TextBlock
            {
                Text = s,
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.DarkBlue),
            };
            (ParentControlElement.Child as Grid).Children.Add(tb);
            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, 3);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (nextGameUpdate.Ticks < DateTime.Now.Ticks)
            {
                Update();
                DateTime now = DateTime.Now;
                nextGameUpdate = nextGameUpdate.Ticks + 10000 * maxIgnoredLatency < now.Ticks
                    ? now.AddMilliseconds(Tick_ms - maxIgnoredLatency)
                    : nextGameUpdate.AddMilliseconds(Tick_ms);
            }
            HelperTextOut();
        }

        //private void GameTimer_Tick_tests(object sender, EventArgs e)
        //{
        //    //bool itIsTime = false;
        //    if (nextGameUpdate.Ticks < DateTime.Now.Ticks)
        //    //itIsTime = true;
        //    //if (itIsTime)
        //    {
        //        /*
        //        Update();
        //        DateTime now = DateTime.Now;
        //        long t = nextGameUpdate.Ticks + 10000 * (Tick_ms + maxIgnoredLatency);
        //        nextGameUpdate = t < now.Ticks
        //            //? now.Subtract(new TimeSpan(0, 0, 0, 0, maxIgnoredLatency))
        //            ? now.AddMilliseconds(-maxIgnoredLatency)
        //            : nextGameUpdate.AddMilliseconds(Tick_ms);
        //        */
        //        //Tick_ms = 1000;
        //        //maxIgnoredLatency = 3;
        //        Update();
        //        DateTime now = DateTime.Now;
        //        long o = nextGameUpdate.Ticks;
        //        long n = now.Ticks;
        //        long t = nextGameUpdate.Ticks + 10000 * (Tick_ms - Tick_ms + maxIgnoredLatency);
        //        long m = now.Ticks - t;
        //        nextGameUpdate = 0 < now.Ticks - t
        //        //? now.Subtract(new TimeSpan(0, 0, 0, 0, maxIgnoredLatency))
        //            ? now.AddMilliseconds(Tick_ms - maxIgnoredLatency)
        //            : nextGameUpdate.AddMilliseconds(Tick_ms);
        //        TextBlock tb = new TextBlock
        //        {
        //            //Text = (nextGameUpdate.Ticks % 1000000000).ToString(),
        //            Text = "nGU: " + Settings.TicksToReadable(o) +
        //            Environment.NewLine + "now: " + Settings.TicksToReadable(n) +
        //            Environment.NewLine + "t--:  " + Settings.TicksToReadable(t) +
        //            Environment.NewLine + "n-t:  " + Settings.TicksToReadable(m) +
        //            Environment.NewLine + "nGU: " + Settings.TicksToReadable(nextGameUpdate.Ticks),
        //            Background = new SolidColorBrush(Colors.White),
        //            Foreground = new SolidColorBrush(Colors.DarkBlue),
        //        };
        //        (ParentControlElement.Child as Grid).Children.Add(tb);
        //        Grid.SetColumn(tb, 1);
        //        Grid.SetRow(tb, 3);
        //        //MessageBox.Show((nextGameUpdate.Ticks % 1000000000).ToString());
        //    }
        //}

        private void Update()
        {
            //CheckBlocks();
            //CountFallingBlocks();
            Redraw();
        }

        private void Redraw()
        {
            foreach (Block b in blocksToRedraw)
            {
                b.Canvas.Margin = new System.Windows.Thickness(
                    Settings.blockResolution * b.CoordinatesX, 0, 0, Settings.blockResolution * b.CoordinatesY);
            }
            blocksToRedraw.Clear();
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
            allFieldBlocks.Add(b);
        }
        public string ShowHelloBlock()
        {
            allFieldBlocks.Clear();
            HelloBlock();
            return allFieldBlocks[0].ToString();
        }

        public Block GiveHelloBlock()
        {
            return allFieldBlocks[0];
        }
    }
}
