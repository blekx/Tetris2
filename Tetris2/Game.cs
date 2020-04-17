﻿using System;
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
        /// [0,0] = Left, Bottom;  False=Free
        /// </summary>
        private bool[,] boolField;
        private List<Block> allFieldBlocks = new List<Block>();
        private List<Block> fallingBlocks = new List<Block>();
        private List<Block> nomore_fallingBlocks = new List<Block>();
        private List<Block> blocksToRedraw = new List<Block>();
        private List<Block> preparedBlocks = new List<Block>();
        private Block activeBlock;

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

        //public Game(Image image)
        //            : this((WriteableBitmap)image.Source, 10, 20, 1.5) { }
        public Game(object parentControl)
               : this(parentControl, Settings.gameFieldX, Settings.gameFieldY, Settings.gameDefaultGravity) { }


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

        #region Helpers: ( WriteLN: HelperTextOut(string s) )
        private void HelperTextOut() => HelperTextOut("nGU: " + Settings.TicksToReadable(nextGameUpdate.Ticks));
        public void HelperTextOut(string s)
        {
            TextBlock tb = new TextBlock
            {
                Text = s,
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.DarkBlue),
                TextWrapping = TextWrapping.Wrap,
            };
            (ParentControlElement.Child as Grid).Children.Add(tb);
            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, 3);
        }

        public string WriteBoolField()
        {
            return Settings.BoolfieldToString(boolField);
        }
        #endregion

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
        }

        private void Update()
        {
            CountFallingBlocks(fallingBlocks);
            CheckBlocks(fallingBlocks);
            Redraw();
        }

        private void Redraw()
        {
            foreach (Block b in blocksToRedraw)
            {
                SetPropperPosition(b);
                ProjectIntoBoolField(b);
            }
            blocksToRedraw.Clear();
        }

        private void CountFallingBlocks(List<Block> blocks)
        {
            foreach (Block b in blocks)
            {//version with one fall per tick
                RemoveFromBoolField(b);
                b.CoordinatesY -= 1;
                blocksToRedraw.Add(b);
            }
        }

        /// <summary>
        /// Corrects if Falling blocks already landed.
        /// </summary>
        /// <param name="blocks"></param>
        private void CheckBlocks(List<Block> blocks)
        {
            List<Block> blocksToStop = new List<Block>();
            foreach (Block b in blocks)
            {
                for (int x = 0; x < b.DimensionX; x++)
                {
                    int y = 0;
                    while (b.Shape[x, y] == false) y++;
                    double requiredFreeY = b.CoordinatesY + y;
                    if ((int)requiredFreeY == requiredFreeY) requiredFreeY -= 1;
                    int reqY = (int)requiredFreeY;
                    if (reqY >= 0)
                        if (!boolField[x + (int)b.CoordinatesX, reqY]) continue;
                        else
                        {
                            blocksToStop.Add(b);
                            //StopBlock(b);
                            break;
                        }
                    else
                    {
                        blocksToStop.Add(b);
                        //StopBlock(b);
                        break;
                    }
                }
                ProjectIntoBoolField(b);
            }
            StopBlocks(blocksToStop);
            foreach (Block b in nomore_fallingBlocks) fallingBlocks.Remove(b);
        }

        private void StopBlocks(Block b) => StopBlocks(new List<Block> { b });
        private void StopBlocks(List<Block> blocks)
        {
            foreach (Block b in blocks)
            {
                if ((int)b.CoordinatesY != b.CoordinatesY)
                    b.CoordinatesY = (int)b.CoordinatesY + 1;
                nomore_fallingBlocks.Add(b);
                if (b == activeBlock)
                {
                    ThrowIntoField(preparedBlocks[0]);
                }
            }
        }

        private void CheckBlocks(Block b) => CheckBlocks(new List<Block> { b });

        private void SetPropperPosition(Block b)
        {
            b.Canvas.Margin = new System.Windows.Thickness(
                    Settings.gameFramePadding + Settings.blockResolution * b.CoordinatesX, 0, 0,
                    Settings.gameFramePadding + Settings.blockResolution * b.CoordinatesY);
        }

        private void ThrowIntoField(Block b)
        {
            preparedBlocks.Add(BlockGenerator.NewBlockDefault());

            Block nb = BlockGenerator.RandomlyRotateBlock(b);
            nb.CoordinatesX = DimensionX / 2 - nb.DimensionX / 2 + nb.RotationOffset;
            nb.CoordinatesY = DimensionY - nb.DimensionY;
            Grid.SetColumn(nb.Canvas, 1);
            Grid.SetRow(nb.Canvas, 3);
            SetPropperPosition(nb);
            gameGrid.Children.Add(nb.Canvas);

            //Block nb = BlockGenerator.CutBlock(b);
            allFieldBlocks.Add(nb);
            fallingBlocks.Add(nb);
            ProjectIntoBoolField(nb);
            blocksToRedraw.Add(nb);
            activeBlock = nb;
            preparedBlocks.Remove(b);
        }

        private void ProjectIntoBoolField(Block b)
        {
            for (int x = 0; x < b.DimensionX; x++)
                for (int y = 0; y < b.DimensionY; y++)
                    if (b.Shape[x, y])
                        boolField[(int)b.CoordinatesX + x, (int)b.CoordinatesY + y] = true;
        }

        private void RemoveFromBoolField(Block b)
        {
            for (int x = 0; x < b.DimensionX; x++)
                for (int y = 0; y < b.DimensionY; y++)
                    if (b.Shape[x, y])
                        boolField[(int)b.CoordinatesX + x, (int)b.CoordinatesY + y] = false;
        }

        public void HelloBlock()
        {
            while (preparedBlocks.Count <= Settings.preparedBlocks)
            {
                preparedBlocks.Add(BlockGenerator.NewBlockRandomOrientation());
            }
            ThrowIntoField(preparedBlocks[0]);
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
