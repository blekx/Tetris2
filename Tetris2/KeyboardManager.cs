using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Tetris2
{
    public class KeyboardManager
    {
        /*
        public static Key player1_Left = Key.Left;
        public static Key player1_Right = Key.Right;
        public static Key player1_LandBlock = Key.Space;
        public static Key player1_RotateRight = Key.D;
        public static Key player1_RotateLeft = Key.A;
        public static Key player1_RotateTwice = Key.S;
        */
        //private const Key k_player1_Left = Settings.player1_Left;
        //private const Key k_player1_Left = Key.Left;

        /// <summary> time stamps </summary>
        private DateTime ts_player1_Left;
        private DateTime ts_player1_Right;
        private DateTime ts_player1_LandBlock;
        private DateTime ts_player1_RotateRight;
        private DateTime ts_player1_RotateLeft;
        private DateTime ts_player1_RotateTwice;
        /// <summary> next orders sent to game </summary>
        private int nos_player1_Left;
        private int nos_player1_Right;
        private bool ts_player1_Left_IsInactive = true;
        private bool ts_player1_Right_IsInactive = true;


        private DateTime lastClosingAttempt;
        private TimeSpan closingDoubleclickMaxInterval = new TimeSpan(250 * 10000);
        //private int lastClosingAttempt;
        //private int closingDoubleclickMaxInterval = 300;
        /// <summary> Repeating Key Interval - def: about each 90 ms </summary>
        private int rki = Settings.gameRepeatingKeyInterval_ms;
        private DispatcherTimer keyTimer;

        private MainWindow mw;
        private Game g1;

        private DateTime helper_ts_check;

        public KeyboardManager(MainWindow mainWindow)
        {
            this.mw = mainWindow;
            g1 = mw.mainGame;
            keyTimer = new DispatcherTimer(DispatcherPriority.Send);
            keyTimer.Interval = new TimeSpan(10000 * Settings.keyTimerInterval_ms);
            keyTimer.Tick += new EventHandler(KeyTimer_Tick);
            //keyTimer.Start();
        }

        private void KeyTimer_Tick(object sender, EventArgs e)
        {
            DateTime t = DateTime.Now;
            long timestamp = System.Diagnostics.Stopwatch.GetTimestamp();
            Check_P1_Left(t);
            Check_P1_Right(t);
            g1.HelperTextOut(t.ToString("ss.ffff") + Environment.NewLine + helper_ts_check.ToString("ss.ffff"));
            //            throw new NotImplementedException();
        }

        private void Check_P1_Left(DateTime t)
        {
            if (!ts_player1_Left_IsInactive)
            {
                int nos_P1L_ShouldBe = (int)(t.Subtract(ts_player1_Left).TotalMilliseconds / rki) + 1;
                while (nos_P1L_ShouldBe > nos_player1_Left)
                {
                    g1.Lef();
                    nos_player1_Left++;
                }
            }
            //throw new NotImplementedException();
        }

        private void Check_P1_Right(DateTime t)
        {
            if (!ts_player1_Right_IsInactive)
            {
                int nos_P1R_ShouldBe = (int)(t.Subtract(ts_player1_Right).TotalMilliseconds / rki) + 1;
                while (nos_P1R_ShouldBe > nos_player1_Right)
                {
                    g1.Right();
                    nos_player1_Right++;
                }
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="down"></param>
        /// <param name="timestamp">ms</param>
        public void AddKeyDown(Key key, DateTime t)
        {
            //    switch (key)
            //    {
            //        case k_player1_Left:
            //        case Settings.player1_Left:
            //            g1.MoveLeft();
            //            break;
            //    }                       

            //helper_ts_check = t;

            if (key == Settings.player1_Left)
            {
                if (ts_player1_Left_IsInactive)
                {
                    ts_player1_Left_IsInactive = false;
                    ts_player1_Left = t;

                    g1.Lef();
                    nos_player1_Left = 1;
                }
                keyTimer.Start();
            }
            else if (key == Settings.player1_Right)
            {
                if (ts_player1_Right_IsInactive)
                {
                    ts_player1_Right_IsInactive = false;
                    ts_player1_Right = t;

                    g1.Right();
                    nos_player1_Right = 1;
                }
                keyTimer.Start();
            }
            else if (key == Settings.player1_LandBlock)
            {

            }
            else if (key == Settings.player1_RotateLeft)
            {

            }
            else if (key == Settings.player1_RotateRight)
            {

            }
            else if (key == Settings.player1_RotateTwice)
            {

            }
            else switch (key)
                {
                    case Key.Escape:
                        if (t.Subtract(lastClosingAttempt).Ticks < closingDoubleclickMaxInterval.Ticks)
                            mw.Close();
                        lastClosingAttempt = t;
                        break;
                    default:
                        MessageBox.Show(t.ToString() + Environment.NewLine + DateTime.Now.Ticks.ToString()
                            + Environment.NewLine + key.ToString());
                        break;
                }
        }

        internal void AddKeyUp(Key key, DateTime t)
        {
            if (key == Settings.player1_Left)
            {
                Check_P1_Left(t);
                nos_player1_Left = 0;
                ts_player1_Left_IsInactive = true;
                if (ts_player1_Right_IsInactive) keyTimer.Stop();
            }
            else if (key == Settings.player1_Right)
            {
                Check_P1_Right(t);
                nos_player1_Right = 0;
                ts_player1_Right_IsInactive = true;
                if (ts_player1_Left_IsInactive) keyTimer.Stop();
            }
            else if (key == Settings.player1_LandBlock)
            {

            }
            else if (key == Settings.player1_RotateLeft)
            {

            }
            else if (key == Settings.player1_RotateRight)
            {

            }
            else if (key == Settings.player1_RotateTwice)
            {

            }
        }
    }
}
