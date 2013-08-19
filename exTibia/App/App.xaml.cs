using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

[assembly: CLSCompliant(false)]
namespace exTibia
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow mainWindow = new MainWindow();
        LgnWin loginWindow = new LgnWin();
        
        public App()
        {
            mainWindow.Show();
        }
    }

}
