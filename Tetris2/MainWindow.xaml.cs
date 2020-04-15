using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BorderBlinker borderBlinker;
        private Monitoring monitoring;
        //private Game mainGame;
        public Game mainGame;

        public MainWindow()
        {
            InitializeComponent();
            borderBlinker = new BorderBlinker(this);
            mainGame = new Game(ViewBoxGame1);
            //Game g2 = new Game(ViewBoxGame2);

            #region tests:
            monitoring = new Monitoring(this);
            monitoring.Start();
            tbTest2.Text = mainGame.ShowHelloBlock();
            #endregion
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        int i, j;
        Random r = new Random();

        /// <summary>
        /// test2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tbTest2.Text = mainGame.ShowHelloBlock();
            Canvas c = Painter.PaintBlock(mainGame.GiveHelloBlock());
            Grid.SetColumn(c, r.Next(5));// i);
            Grid.SetRow(c, r.Next(5));// j);
            i++;
            j += 2;
            if (i >= 5) i -= 5;
            if (j >= 5) j -= 5;

            (ViewBoxGame1.Child as Grid).Children.Add(c);
            //GameGrid.Children.Add
        }
    }
}
