﻿using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var tmpMainWindow = new MainWindow();            
            tmpMainWindow.Show();
        }
    }
}
