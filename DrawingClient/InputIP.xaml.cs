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
using System.Windows.Shapes;

namespace DrawingClient
{
    /// <summary>
    /// Interaction logic for InputIP.xaml
    /// </summary>
    public partial class InputIP : Window
    {
        public string ip;
        public bool close;
        public InputIP()
        {
            InitializeComponent();
            close = true;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            close = false;
            ip = IPinput.Text;
            this.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
