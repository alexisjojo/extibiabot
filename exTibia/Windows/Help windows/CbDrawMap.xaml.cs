using System;
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

using exTibia.Modules;

namespace exTibia
{
    /// <summary>
    /// Interaction logic for CbDrawMap.xaml
    /// </summary>
    public partial class CbDrawMap : Window
    {
        public CbDrawMap()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void DrawMapCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawMap = true;
        }

        private void DrawItemsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawItems = true;
        }

        private void DrawWayCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawWay = true;
        }

        private void DrawMapCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawMap = false;
        }

        private void DrawItemsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawItems = false;
        }

        private void DrawWayCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawWay = false;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            DrawMapCheckbox.IsChecked = CaveBot.Instance.DrawMap;
            DrawItemsCheckbox.IsChecked = CaveBot.Instance.DrawItems;
            DrawWayCheckbox.IsChecked = CaveBot.Instance.DrawMap;
        }
    }
}
