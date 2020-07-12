using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Tetris2
{
    class Monitoring
    {
        private MainWindow mw;
        private DispatcherTimer timer;
        private double frequency = 10; //Hz

        public Monitoring(MainWindow mainWindow)
        {
            this.mw = mainWindow;


            timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Interval = new TimeSpan(0, 0, 0, 0, (int)(1000 / frequency));
            timer.Tick += new EventHandler(Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            mw.tbTest.Text = mw.borderBlinker.Report()
                + Environment.NewLine + string.Format("{0},{1:0.000}",
                mw.mainGame.nextGameUpdate.Second,
                mw.mainGame.nextGameUpdate.Millisecond);
            mw.tbGamefield.Text = mw.mainGame.WriteBoolField();
        }

        public void UpdateBoolfieldVisualisation()
        {
            mw.tbGamefield.Text = mw.mainGame.WriteBoolField();
        }

        internal void Start()
        {
            timer.Start();
        }
    }
}
