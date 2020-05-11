using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Tetris2
{
    public class ActiveBlock
    {
        public double vX = 0;
        //public int ghostCoordX;
        public Block block;
        public Block ghostBlock;
        private Game game;

        public ActiveBlock(Game game)
        {
            this.game = game;
        }


        //public void 


        public void CountActiveBlockHorizontally(DateTime t)
        {
            int limitationWay = 2;
            double s = this.block.ghostCoordX - this.block.CoordinatesX;

            /// maxBrakingDistanceInLastTimerTick
            double s2m = 0.5 * Settings.gameAB_HorizDecceleration * Math.Pow(Settings.gameTimerInterval_ms / 1000.0, 2);
            /// maxVelocityToBrakeInLastTimerTick
            double v2m = Settings.gameTimerInterval_ms / 1000.0 * Settings.gameAB_HorizDecceleration;
            ///<summary> direction right</summary>
            double dirR = (this.block.CoordinatesX < this.block.ghostCoordX) ? 1 : -1;
            double dt = Game.Count_dt(this.block, t);

            if (Math.Abs(s) < s2m && this.vX < v2m)
            {//finish, stop it!
                this.vX = 0;
                this.block.CoordinatesX = this.block.ghostCoordX;
            }
            else if (dirR * this.vX < 0)
            {//moving into opposite side
                double speedIncrement = dirR * dt * Settings.gameAB_HorizDecceleration * Settings.gameAB_HorAgressivenessOfStopping;
                if (Math.Abs(speedIncrement) > Math.Abs(this.vX))
                {// only slowly = will be stopped
                    this.block.CoordinatesX += 0.5 * this.vX * dt;
                    this.vX = 0;
                }
                else
                {// faster movement into a wrong side
                    this.block.CoordinatesX += 0.5 * (this.vX + speedIncrement) * dt;
                    this.vX += speedIncrement;
                }
            }
            else
            {//moving correctly
                /// part "2" = Theoretical deccelerating part of the horizontal movement 
                /// minimal required
                /// Time
                double t2 = Math.Abs(this.vX) / Settings.gameAB_HorizDecceleration;
                /// and Distance
                /// to stop naturally
                double s2 = this.vX * 1 / 2 * t2;

                if (limitationWay == 1)
                {
                    if (Math.Abs(s) <= Math.Abs(s2) * Settings.gameAB_HorAgressivenessOfStopping)
                    {//too close to stop normally
                        if (Math.Abs(this.vX) < 0.1)
                        {//case "Hard Hit"
                            this.vX = 0;
                            this.block.CoordinatesX = this.block.ghostCoordX;
                        }
                        else
                        {//case "just very excessive speed"
                            this.vX /= Settings.gameAB_HorAgressivenessOfStopping;
                        }
                        return;
                    }
                }

                if (Math.Abs(s) <= Math.Abs(s2))
                {// movement too fast, slowing down

                    double natural_speedDecrement = dt * Settings.gameAB_HorizDecceleration;
                    // ^ only a wrong theory yet ^ , we actually need to count it:
                    double speedDecrement = dt * 1 / 2 / Math.Abs(s) * this.vX * this.vX;

                    if (limitationWay == 2)
                    {// Will not slow down extremely, rather will stop too late and return back
                        if (speedDecrement > natural_speedDecrement * Settings.gameAB_HorAgressivenessOfStopping)
                            speedDecrement = natural_speedDecrement * Settings.gameAB_HorAgressivenessOfStopping;
                    }

                    this.block.CoordinatesX += this.vX * dt - dirR * dt * speedDecrement / 2;
                    this.vX -= dirR * speedDecrement;
                }
                else
                {// movement too slow, accelerating
                    this.block.CoordinatesX += this.vX * dt +
                        dirR * dt * dt * Settings.gameAB_HorizAcceleration / 2;
                    this.vX += dirR * dt * Settings.gameAB_HorizAcceleration;
                }
                // X - (OK), vX - (OK), t - (?)
                // ...Timestamp "block last seen/drawn at"
                // will be rewriten only once, only in falling, not when moving to the side:
                //
                // activeBlock.CoordinatesT = t;
            }
        }

        internal void MoveRight(int step)
        {
            this.block.ghostCoordX += step;
            ghostBlock.CoordinatesX += step;
            ghostBlock.CoordinatesY = game.HeightOfLanding(block);
            Game.SetCanvasPosition(ghostBlock);
        }

        private void VisualiseCanvas(Block b)
        {
            Grid.SetColumn(b.Canvas, 1);
            Grid.SetRow(b.Canvas, 3);
            Game.SetCanvasPosition(b);
            game.gameGrid.Children.Add(b.Canvas);
        }

        private void CreateNewGhostBlock()
        {
            ghostBlock = BlockGenerator.Ghost(block, game.HeightOfLanding(block));
            //ghostBlock.ghostCoordX = block.ghostCoordX;
            VisualiseCanvas(ghostBlock);
            //game.blocksToRedraw.Add(ghostBlock);
        }

        internal void GetNextBlock(Block b)
        {
            block = BlockGenerator.RandomlyRotateBlock(b);
            block.CoordinatesX = game.DimensionX / 2 - block.DimensionX / 2 + block.RotationOffset;
            block.CoordinatesY = game.DimensionY - block.DimensionY;
            VisualiseCanvas(block);

            game.allFieldBlocks.Add(block);
            game.fallingBlocks.Add(block);
            game.blocksToRedraw.Add(block);
            this.block.ghostCoordX = (int)(block.CoordinatesX);

            CreateNewGhostBlock();

        }

        internal void RotateInto(Block rb)
        {
            game.gameGrid.Children.Remove(block.Canvas);
            game.gameGrid.Children.Remove(ghostBlock.Canvas);
            block = rb;
            VisualiseCanvas(block);
            CreateNewGhostBlock();
        }
    }
}
