﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Elysium;
using VK.Auth;

namespace VKMusicSync
{
    /// <summary>
    /// Логика взаимодействия для MusicSync.xaml
    /// </summary>
    public partial class MusicSync : Elysium.Controls.Window
    {
        public MusicSync()
        {
            //InitializeComponent();
        }

        public void HelpClick(Object sender,RoutedEventArgs e)
        { 

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private Auth authWindow;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            authWindow = new Auth();
            authWindow.auth.OnInit += OnAuthDone;
            authWindow.ShowDialog();
        }
        private void OnAuthDone()
        {
            if (authWindow != null) authWindow.Close();
        }
    }
}
