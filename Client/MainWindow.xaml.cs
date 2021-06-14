﻿using MMWSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Client
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
        MQSocket client_sock;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            client_sock = new MQSocket(6661,true, "192.168.1.3");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lbl1.Content = "Start ...";

            while (true)
            {
                var filename = client_sock.recv_file("E:/");
                Thread.Sleep(1000);
                MessageBox.Show("Received " + filename);
                lbl1.Content = "Success";
            }
            
            
        }
    }
}
