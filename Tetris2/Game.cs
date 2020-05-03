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
        private List<Block> allFieldBlocks = new List<Block>();
        private List<Block> groupToFallTogether = new List<Block>();
        private List<Block> fallingBlocks = new List<Block>();
        private List<Block> nomore_fallingBlocks = new List<Block>();
        private List<Block> blocksToRedraw = new List<Block>();
        private List<Block> preparedBlocks = new List<Block>();
        private Block activeBlock;
        private bool willThrowANewBlock = false;
        private double activeBlock_vX = 0;
        private double fallingGroup_vY = 0;
        private bool deactivated = false;


        public Viewbox ParentControlElement { get; private set; }
        //private Viewbox parent;
        private Grid gameGrid;
        private Canvas canvasPlayground;
        private Canvas canvasUp;
        private Canvas canvasRight;

        private DispatcherTimer gameTimer;
        /// <summary> sheduled, unreal value </summary>
        //private DateTime nextGameUpdate = DateTime.Now;
        public DateTime nextGameUpdate = DateTime.Now;

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
            gameTimer = new DispatcherTimer(DispatcherPriority.Send);
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, Settings.gameTimerInterval_ms);
            gameTimer.Tick += new EventHandler(GameTimer_Tick);
            gameTimer.Start();
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
            return Settings.BoolfieldToString(boolField);
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
            CountActiveBlockHorizontally(t);

            TestSuspiciousBlocksOnStartFalling(t);

            if (Smooth) CountFallingBlocksSmoothly(fallingBlocks, t); else CountFallingBlocks_TickingVersion(fallingBlocks);

            CheckLanding(fallingBlocks);
            RedrawOnce();

            if (willThrowANewBlock && !deactivated) ThrowIntoField(preparedBlocks[0]);
            //HelperTextOut(activeBlock.fallingDistance_Helper.ToString("0.000") + Environment.NewLine + activeBlock.CoordinatesV.ToString()
            //+ Environment.NewLine + c.ToString());
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


        private void CountActiveBlockHorizontally(DateTime t)
        {
            int limitationWay = 2;
            double s = activeBlock.ab_ghostCoordX - activeBlock.CoordinatesX;

            /// maxBrakingDistanceInLastTimerTick
            double s2m = 0.5 * Settings.gameAB_HorizDecceleration * Math.Pow(Settings.gameTimerInterval_ms / 1000.0, 2);
            /// maxVelocityToBrakeInLastTimerTick
            double v2m = Settings.gameTimerInterval_ms / 1000.0 * Settings.gameAB_HorizDecceleration;
            ///<summary> direction right</summary>
            double dirR = (activeBlock.CoordinatesX < activeBlock.ab_ghostCoordX) ? 1 : -1;
            double dt = Count_dt(activeBlock, t);

            if (Math.Abs(s) < s2m && activeBlock_vX < v2m)
            {//finish, stop it!
                activeBlock_vX = 0;
                activeBlock.CoordinatesX = activeBlock.ab_ghostCoordX;
            }
            else if (dirR * activeBlock_vX < 0)
            {//moving into opposite side
                double speedIncrement = dirR * dt * Settings.gameAB_HorizDecceleration * Settings.gameAB_HorAgressivenessOfStopping;
                if (Math.Abs(speedIncrement) > Math.Abs(activeBlock_vX))
                {// only slowly = will be stopped
                    activeBlock.CoordinatesX += 0.5 * activeBlock_vX * dt;
                    activeBlock_vX = 0;
                }
                else
                {// faster movement into a wrong side
                    activeBlock.CoordinatesX += 0.5 * (activeBlock_vX + speedIncrement) * dt;
                    activeBlock_vX += speedIncrement;
                }
            }
            else
            {//moving correctly
                /// part "2" = Theoretical deccelerating part of the horizontal movement 
                /// minimal required
                /// Time
                double t2 = Math.Abs(activeBlock_vX) / Settings.gameAB_HorizDecceleration;
                /// and Distance
                /// to stop naturally
                double s2 = activeBlock_vX * 1 / 2 * t2;

                if (limitationWay == 1)
                {
                    if (Math.Abs(s) <= Math.Abs(s2) * Settings.gameAB_HorAgressivenessOfStopping)
                    {//too close to stop normally
                        if (Math.Abs(activeBlock_vX) < 0.1)
                        {//case "Hard Hit"
                            activeBlock_vX = 0;
                            activeBlock.CoordinatesX = activeBlock.ab_ghostCoordX;
                        }
                        else
                        {//case "just very excessive speed"
                            activeBlock_vX /= Settings.gameAB_HorAgressivenessOfStopping;
                        }
                        return;
                    }
                }

                if (Math.Abs(s) <= Math.Abs(s2))
                {// movement too fast, slowing down

                    double natural_speedDecrement = dt * Settings.gameAB_HorizDecceleration;
                    // ^ only a wrong theory yet ^ , we actually need to count it:
                    double speedDecrement = dt * 1 / 2 / Math.Abs(s) * activeBlock_vX * activeBlock_vX;

                    if (limitationWay == 2)
                    {// Will not slow down extremely, rather will stop too late and return back
                        if (speedDecrement > natural_speedDecrement * Settings.gameAB_HorAgressivenessOfStopping)
                            speedDecrement = natural_speedDecrement * Settings.gameAB_HorAgressivenessOfStopping;
                    }

                    activeBlock.CoordinatesX += activeBlock_vX * dt - dirR * dt * speedDecrement / 2;
                    activeBlock_vX -= dirR * speedDecrement;
                }
                else
                {// movement too slow, accelerating
                    activeBlock.CoordinatesX += activeBlock_vX * dt +
                        dirR * dt * dt * Settings.gameAB_HorizAcceleration / 2;
                    activeBlock_vX += dirR * dt * Settings.gameAB_HorizAcceleration;
                }
                // X - (OK), vX - (OK), t - (?)
                // ...Timestamp "block last seen/drawn at"
                // will be rewriten only once, only in falling, not when moving to the side:
                //
                // activeBlock.CoordinatesT = t;
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
                List<Block> reducedList = new List<Block>(blocks);
                foreach (Block b in reducedList)
                {
                    if (!IsSpace(b, D4.O))
                    {
                        checkAgain = true;
                        blocks.Remove(b);
                        blocksToStop.Add(b);
                        nomore_fallingBlocks.Add(b);
                        if (b == activeBlock)
                        {
                            willThrowANewBlock = true;
                            activeBlock_vX = 0;
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
        {
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
                    if ((int)(b.ab_ghostCoordX) <= 0) result = false;
                    break;
                case D4.R:
                    xOff = 1;
                    if ((int)(b.ab_ghostCoordX) + b.DimensionX >= DimensionX) result = false;
                    break;
                case D4.O:
                    if (b.CoordinatesY < 0) result = false;
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
                        if (boolField[(int)(b.ab_ghostCoordX) + x + xOff, (int)(b.CoordinatesY) + y + yOff])
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

                b.CoordinatesX = b.ab_ghostCoordX;

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

        private void SetCanvasPosition(Block b)
        {
            b.Canvas.Margin = new System.Windows.Thickness(
                    Settings.gameFramePadding + Settings.blockResolution * b.CoordinatesX, 0, 0,
                    Settings.gameFramePadding + Settings.blockResolution * b.CoordinatesY);
        }

        private void ThrowIntoField(Block b)
        {
            willThrowANewBlock = false;
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
            nb.ab_ghostCoordX = (int)(nb.CoordinatesX);
            activeBlock = nb;
            preparedBlocks.Remove(b);
        }

        private void ProjectIntoBoolField(Block b) => Rewrite_BoolField_ByBlock(b, true);

        private void RemoveFromBoolField(Block b) => Rewrite_BoolField_ByBlock(b, false);

        private void Rewrite_BoolField_ByBlock(Block b, bool addOrRemove)
        {
            for (int x = 0; x < b.DimensionX; x++)
                for (int y = 0; y < b.DimensionY; y++)
                    if (b.Shape[x, y])
                        //boolField[(int)b.CoordinatesX + x, (int)b.CoordinatesY + y] = false;
                        boolField[b.ab_ghostCoordX + x, (int)b.CoordinatesY + y] = addOrRemove;

        }

        private double Count_dt(Block b, DateTime t)
        {
            double dt = t.Subtract(b.CoordinatesT).TotalSeconds;
            if (dt * 1000 > Settings.gameMaxLag_ms) dt = 0.001 * Settings.gameMaxLag_ms;
            return dt;
        }

        //private int c = 0;
        public void Lef()
        {
            RemoveFromBoolField(activeBlock);
            if (IsSpace(activeBlock, D4.L)) activeBlock.ab_ghostCoordX -= 1;
            ProjectIntoBoolField(activeBlock);
            //
            //c += 1;
            //HelperTextOut(c.ToString());
        }
        public void Right()
        {
            RemoveFromBoolField(activeBlock);
            if (IsSpace(activeBlock, D4.R)) activeBlock.ab_ghostCoordX += 1;
            ProjectIntoBoolField(activeBlock);
        }

        #region tests
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
        #endregion
    }
}
