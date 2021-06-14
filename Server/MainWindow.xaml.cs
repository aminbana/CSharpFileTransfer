using MMWSoftware;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        MQSocket sock; 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sock = new MQSocket(6661, false, "192.168.1.10");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] filePaths = Directory.GetFiles("C:/Users/Amin/Desktop/Wallpapers/");
            
            foreach (String file in filePaths)
            {
                MessageBox.Show(file);
                sock.send_file(file);
            } 
            
        }
    }
}
