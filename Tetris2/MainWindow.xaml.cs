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
        private Game mainGame;

        public MainWindow()
        {
            InitializeComponent();
            borderBlinker = new BorderBlinker(this);
            mainGame = new Game(ImageGameField);
                        
            monitoring = new Monitoring(this);
            monitoring.Start();
            tbTest2.Text = mainGame.ShowHelloBlock();
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

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            //throw new Exception("Border" + sender.ToString());// +BorderSystem.Windows.Shapes.Rectangle);
        }
    }
}
