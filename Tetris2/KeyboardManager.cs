using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        private int ts_player1_Left;
        private int ts_player1_Right;
        private int ts_player1_LandBlock;
        private int ts_player1_RotateRight;
        private int ts_player1_RotateLeft;
        private int ts_player1_RotateTwice;

        private MainWindow mw;
        private Game g1;

        public KeyboardManager(MainWindow mainWindow)
        {
            this.mw = mainWindow;
            g1 = mw.mainGame;
        }

        internal void AddEvent(Key key, bool down, int timestamp)
        {
            //    switch (key)
            //    {
            //        case k_player1_Left:
            //        case Settings.player1_Left:
            //            g1.MoveLeft();
            //            break;
            //    }                       

            MessageBox.Show(timestamp.ToString() + Environment.NewLine + DateTime.Now.Ticks.ToString());

            if (key == Settings.player1_Left)
            {
                ts_player1_Left = timestamp;
                g1.Lef();
            }
            else if (key == Settings.player1_Right)
            {

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
