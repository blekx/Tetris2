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
    public partial class Game
    {
        #region field
        /// <summary> IsGameSmooth_NotTicking  </summary>
        public bool Smooth { get; private set; }
        public int DimensionX { get; private set; }
        public int DimensionY { get; private set; }
        /// <summary>rather Velocity of active block while TICKING </summary>
        private double gravity_T;
        /// <summary>More realistic gravity, used for SMOOTH movement </summary>
        private double gravity_S;
        private int Tick_ms;
        private int maxIgnoredLatency;
        /// <summary> [0,0] = Left, Bottom;  False = Free </summary>
        private bool[,] boolField;
        public List<Block> allFieldBlocks = new List<Block>();
        private List<Block> groupToFallTogether = new List<Block>();
        public List<Block> fallingBlocks = new List<Block>();
        private List<Block> nomore_fallingBlocks = new List<Block>();
        public List<Block> blocksToRedraw = new List<Block>();
        private List<Block> preparedBlocks = new List<Block>();
        private ActiveBlock ab;
        //private Block activeBlock;
        private bool abJustLanded_ThrowNew = false;
        //private double activeBlock_vX = 0;
        //private double abvX = 0;
        private double fallingGroup_vY = 0;
        /// <summary> Game (active block) Paused for Removing Lines etc. </summary>
        private bool deactivated = false;


        public Viewbox ParentControlElement { get; private set; }
        //private Viewbox parent;
        public Grid gameGrid;
        private Canvas canvasPlayground;
        private Canvas canvasUp;
        private Canvas canvasRight;

        private DispatcherTimer gameTimer;
        /// <summary> sheduled, unreal value </summary>
        //private DateTime nextGameUpdate = DateTime.Now;
        public DateTime nextGameUpdate = DateTime.Now;

        private LinesManager linesManager;

        #endregion

        #region Constructing
        //public Game(WriteableBitmap bitmap, int dimensionX, int dimensionY, double gravity)
        public Game(object parentControl, int dimensionX, int dimensionY, double gravity)
        {
            //this.bitmap = bitmap;
            //Image = image;
            Smooth = Settings.isGameSmooth_NotTicking;
            ParentControlElement = parentControl as Viewbox;
            DimensionX = dimensionX;
            DimensionY = dimensionY;
            Gravity = gravity;
            boolField = new bool[DimensionX, DimensionY * 2];
            CreateOwnEnvironment();
            ab = new ActiveBlock(this);
            gameTimer = new DispatcherTimer(DispatcherPriority.Send);
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.gameTimerInterval_ms);
            gameTimer.Tick += new EventHandler(GameTimer_Tick);
            gameTimer.Start();
            linesManager = new LinesManager(this);
        }

        public double Gravity
        {
            get => gravity_T;
            private set
            {
                gravity_T = value;
                gravity_S = value * Settings.g_TickingToSmooth_coef;
                Tick_ms = (int)(1000 / gravity_T);
                maxIgnoredLatency = (int)(Tick_ms * Settings.maxIgnoredLatency_ratioFromTick);
            }
        }

        public bool[,] BoolField => boolField;

        //public Game(Image image)
        //            : this((WriteableBitmap)image.Source, 10, 20, 1.5) { }
        public Game(object parentControl)
               : this(parentControl, Settings.gameFieldX, Settings.gameFieldY, Settings.gameDefaultGravity_T) { }


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
            Grid.SetColumn(tb, 3);
            Grid.SetRow(tb, 3);
        }

        public string WriteBoolField()
        {
            return Settings.BoolfieldToString(BoolField);
        }
        #endregion

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (Smooth)
                Update();
            else if (nextGameUpdate.Ticks < DateTime.Now.Ticks)
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
            DateTime t = DateTime.Now;
            ab.CountActiveBlockHorizontally(t);

            TestSuspiciousBlocksOnStartFalling(t);

            if (Smooth) CountFallingBlocksSmoothly(fallingBlocks, t); else CountFallingBlocks_TickingVersion(fallingBlocks);

            CheckLanding(fallingBlocks);
            RedrawOnce();
            List<int> justCompletedTheseLines = LinesManager.CompletedLines(boolField);
            if (justCompletedTheseLines.Count > 0) 
                JustCompletedNewLines(justCompletedTheseLines);

            if (abJustLanded_ThrowNew && !deactivated)
                //ThrowIntoField(preparedBlocks[0]);
                NextActiveBlock();
            //HelperTextOut(activeBlock.fallingDistance_Helper.ToString("0.000") + Environment.NewLine + activeBlock.CoordinatesV.ToString()
            //+ Environment.NewLine + c.ToString());
        }

        private void JustCompletedNewLines(List<int> justCompletedTheseLines)
        {
            deactivated = true;
            List<Block> AddingBlocks = LinesManager.CutAllBlocksByLines(allFieldBlocks, justCompletedTheseLines, this);
            foreach (Block b in AddingBlocks)
                GameField_AddBlock(b);
            // ? boolField: reference / new var??
            foreach (int line in justCompletedTheseLines)
                for (int i = 0; i < DimensionX; i++)
                    boolField[i, line] = false; //free the lines

            // ADD VISUAL EFFECT

            //RedrawOnce(); // 2.?

            groupToFallTogether = allFieldBlocks;
            //TestSuspiciousBlocksOnStartFalling(t);
            if (fallingBlocks.Count == 0) deactivated = false;
        }

        private void TestSuspiciousBlocksOnStartFalling(DateTime t)
        {
            List<Block> theyWillFall = new List<Block>();
            List<Block> theyWont = new List<Block>();
            bool doTest = true;
            while (doTest)
            {
                doTest = false;
                foreach (Block b in groupToFallTogether)
                    if (IsSpace(b, D4.B))
                    {
                        theyWillFall.Add(b);
                        doTest = true;
                    }
                    else theyWont.Add(b);
                foreach (Block b in theyWillFall)
                {
                    groupToFallTogether.Remove(b);
                    fallingBlocks.Add(b);
                }
                //CountFallingBlocksSmoothly(theyWillFall, t);
            }
        }

        private void CountFallingBlocks_TickingVersion(List<Block> blocks)
        {
            foreach (Block b in blocks)
            {//version with one fall per tick
                RemoveFromBoolField(b);
                b.CoordinatesY -= 1;
                blocksToRedraw.Add(b);
            }
        }

        private void CountFallingBlocksSmoothly(List<Block> blocks, DateTime t)
        {
            foreach (Block b in blocks)
            {
                RemoveFromBoolField(b);
                double dt = Count_dt(b, t);
                double fallingDistance = b.CoordinatesV * dt + 0.5 * gravity_S * dt * dt;
                b.fallingDistance_Helper = fallingDistance;
                if (fallingDistance > 1) fallingDistance = 1;
                b.CoordinatesY -= fallingDistance;
                b.CoordinatesV += dt * gravity_S;
                b.CoordinatesT = t;
                blocksToRedraw.Add(b);
                //ProjectIntoBoolField(b);
            }
        }


        /// <summary> Corrects if Falling blocks already landed. </summary>
        private void CheckBlocks_Old(List<Block> blocks)
        {
            List<Block> blocksToStop = new List<Block>();
            foreach (Block b in blocks)
            {
                for (int x = 0; x < b.DimensionX; x++)
                {
                    // v1: for homogenous blocks only:
                    //int y = 0;
                    //while (b.Shape[x, y] == false) y++;
                    // v2: for any blocks, replaced by this FOR-IF loop:
                    for (int y = 0; y < b.DimensionY; y++)
                        if (b.Shape[x, y])
                        {
                            double requiredFreeY = b.CoordinatesY + y;
                            if ((int)requiredFreeY == requiredFreeY) requiredFreeY -= 1;
                            int reqY = (int)requiredFreeY;
                            if (reqY >= 0)
                                if (!BoolField[x + (int)b.CoordinatesX, reqY]) continue;
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
                }
                //ProjectIntoBoolField(b);
            }
            StopBlocks(blocksToStop);
            foreach (Block b in nomore_fallingBlocks) fallingBlocks.Remove(b);
        }

        /// <summary> Corrects if Falling blocks already landed. </summary>
        private void CheckLanding(List<Block> blocks)
        {
            List<Block> blocksToStop = new List<Block>();
            bool checkAgain = true;
            while (checkAgain)
            {
                checkAgain = false;
                List<Block> backupList = new List<Block>(blocks);
                foreach (Block b in backupList)
                {
                    if (!IsSpace(b, D4.O))
                    {
                        checkAgain = true;
                        blocks.Remove(b);
                        blocksToStop.Add(b);
                        nomore_fallingBlocks.Add(b);
                        if (b == ab.block)
                        {
                            abJustLanded_ThrowNew = true;
                            ab.vX = 0;
                            gameGrid.Children.Remove(ab.ghostBlock.Canvas);
                        }
                    }
                    while (!IsSpace(b, D4.O) && IsSpace(b, D4.T))
                    {//bug situation, shoots the block up away
                        b.CoordinatesY++;
                    }
                    ProjectIntoBoolField(b);
                }
            }
            StopBlocks(blocksToStop);
            foreach (Block b in nomore_fallingBlocks) fallingBlocks.Remove(b);
        }

        private bool IsSpace(Block b, D4 direction)
        {   //works also with ACTIVE BLOCK (rudiment)
            int xOff = 0, yOff = 0;
            bool result = true;
            switch (direction)
            {
                case D4.T:
                    yOff = 1;
                    if ((int)(b.CoordinatesY) + b.DimensionY >= DimensionY - 1) result = false;
                    break;
                case D4.B:
                    yOff = -1;
                    if ((int)(b.CoordinatesY) <= 0) result = false;
                    break;
                case D4.L:
                    xOff = -1;
                    if ((int)(b.ghostCoordX) <= 0) result = false;
                    break;
                case D4.R:
                    xOff = 1;
                    if ((int)(b.ghostCoordX) + b.DimensionX >= DimensionX) result = false;
                    break;
                case D4.O:
                    if (b.CoordinatesY < 0) result = false;
                    break;
                case D4.O_FromRotation:
                    if ((int)(b.ghostCoordX) <= 0 || (int)(b.ghostCoordX) + b.DimensionX >= DimensionX) result = false;
                    break;
                default: break;
            }
            if (!result) return result;
            // edge collision was fixed ^
            // block collision:
            for (int x = 0; x < b.DimensionX; x++)
            {
                for (int y = 0; y < b.DimensionY; y++)
                    if (b.Shape[x, y])
                        if (BoolField[(int)(b.ghostCoordX) + x + xOff, (int)(b.CoordinatesY) + y + yOff])
                        {
                            result = false;
                            break;
                        }
                if (!result) break;
            }
            return result;
        }

        private void StopBlocks(Block b) => StopBlocks(new List<Block> { b });
        private void StopBlocks(List<Block> blocks)
        {
            foreach (Block b in blocks)
            {
                if ((int)b.CoordinatesY != b.CoordinatesY)
                    b.CoordinatesY = (int)b.CoordinatesY;

                b.CoordinatesX = b.ghostCoordX;

                b.CoordinatesV = 0;
            }
        }

        private void CheckBlocks(Block b) => CheckLanding(new List<Block> { b });

        private void RedrawOnce()
        {
            foreach (Block b in blocksToRedraw)
            {
                SetCanvasPosition(b);
            }
            blocksToRedraw.Clear();
        }

        public static void SetCanvasPosition(Block b)
        {
            b.Canvas.Margin = new System.Windows.Thickness(
                    Settings.gameFramePadding + Settings.blockResolution * b.CoordinatesX, 0, 0,
                    Settings.gameFramePadding + Settings.blockResolution * b.CoordinatesY);
        }

        private void ThrowIntoField(Block b)
        {
            abJustLanded_ThrowNew = false;
            preparedBlocks.Add(BlockGenerator.NewBlockDefault());

            Block nb = BlockGenerator.RandomlyRotateBlock(b);
            nb.CoordinatesX = DimensionX / 2 - nb.DimensionX / 2 + nb.RotationOffset;
            nb.CoordinatesY = DimensionY - nb.DimensionY;
            Grid.SetColumn(nb.Canvas, 1);
            Grid.SetRow(nb.Canvas, 3);
            SetCanvasPosition(nb);
            gameGrid.Children.Add(nb.Canvas);

            //Block nb = BlockGenerator.CutBlock(b);
            allFieldBlocks.Add(nb);
            fallingBlocks.Add(nb);
            //ProjectIntoBoolField(nb);
            blocksToRedraw.Add(nb);
            ab.block = nb;
            ab.block.ghostCoordX = (int)(nb.CoordinatesX);
            //CreateNewGhostBlock();
            preparedBlocks.Remove(b);
        }

        private void NextActiveBlock()
        {
            abJustLanded_ThrowNew = false;

            ab.GetNextBlock(preparedBlocks[0]);
            preparedBlocks.RemoveAt(0);
            preparedBlocks.Add(BlockGenerator.NewBlockDefault());
        }

        private void ProjectIntoBoolField(Block b) => Rewrite_BoolField_ByBlock(b, true);

        private void RemoveFromBoolField(Block b) => Rewrite_BoolField_ByBlock(b, false);

        private void Rewrite_BoolField_ByBlock(Block b, bool addOrRemove)
        {
            for (int x = 0; x < b.DimensionX; x++)
                for (int y = 0; y < b.DimensionY; y++)
                    if (b.Shape[x, y])
                        //boolField[(int)b.CoordinatesX + x, (int)b.CoordinatesY + y] = false;
                        BoolField[b.ghostCoordX + x, (int)b.CoordinatesY + y] = addOrRemove;

        }

        internal void AddNewlyCutBlocks(List<Block> newlyCutBlocks)
        {
            throw new NotImplementedException();
        }

        public static double Count_dt(Block b, DateTime t)
        {
            double dt = t.Subtract(b.CoordinatesT).TotalSeconds;
            if (dt * 1000 > Settings.gameMaxLag_ms) dt = 0.001 * Settings.gameMaxLag_ms;
            return dt;
        }

        public int HeightOfLanding(Block b)
        {
            int result = 0;
            for (int x = 0; x < b.DimensionX; x++)
            {
                int block_bottom = 0;
                //while (!b.Shape[x, block_bottom] && block_bottom < b.DimensionY) //case of inconsistent blocks
                while (!b.Shape[x, block_bottom])
                    block_bottom++;
                int y = (int)(b.CoordinatesY);
                while (y > 0)
                    if (!BoolField[ab.block.ghostCoordX + x, y + block_bottom - 1])
                        y--;
                    else break;
                if (y > result) result = y;
            }
            return result;
        }

        //private int c = 0;
        public void MoveBlock_1Left()
        {
            RemoveFromBoolField(ab.block);
            if (IsSpace(ab.block, D4.L))
                //ab.block.ghostCoordX -= 1;
                ab.MoveRight(-1);
            ProjectIntoBoolField(ab.block);


            //
            //c += 1;
            //HelperTextOut(c.ToString());
        }
        public void MoveBlock_1Right()
        {
            RemoveFromBoolField(ab.block);
            if (IsSpace(ab.block, D4.R))
                //activeBlock.ab_ghostCoordX += 1;
                ab.MoveRight(1);
            ProjectIntoBoolField(ab.block);
        }

        public void LandBlockInstantly()
        {
            RemoveFromBoolField(ab.block);
            ab.block.CoordinatesY = 0 + HeightOfLanding(ab.block);
            //activeBlock.CoordinatesV += dt * gravity_S;
            //activeBlock.CoordinatesT = t;
            ProjectIntoBoolField(ab.block);
            blocksToRedraw.Add(ab.block);
        }

        public void TryToRotate(int clockwise)
        {
            bool willRotate = false;
            RemoveFromBoolField(ab.block);
            Block rb = BlockGenerator.RotateBlock(ab.block, clockwise);
            rb.ghostCoordX = ab.block.ghostCoordX;
            rb.CoordinatesY = ab.block.CoordinatesY;
            if (IsSpace(rb, D4.O_FromRotation))
                willRotate = true;
            else if (IsSpace(rb, D4.L))
            {
                willRotate = true;
                rb.ghostCoordX -= 1;
            }
            else if (IsSpace(rb, D4.R))
            {
                willRotate = true;
                rb.ghostCoordX += 1;
            }

            if (willRotate)
            {
                allFieldBlocks.Remove(ab.block);
                fallingBlocks.Remove(ab.block);
                blocksToRedraw.Remove(ab.block);

                ab.RotateInto(rb);

                allFieldBlocks.Add(ab.block);
                fallingBlocks.Add(ab.block);
                blocksToRedraw.Add(ab.block);
            }

            ProjectIntoBoolField(ab.block);
        }


        #region tests
        public void HelloBlock()
        {
            while (preparedBlocks.Count <= Settings.preparedBlocks)
            {
                preparedBlocks.Add(BlockGenerator.NewBlockRandomOrientation());
            }
            //ThrowIntoField(preparedBlocks[0]);
            NextActiveBlock();
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
        #endregion
    }
}
