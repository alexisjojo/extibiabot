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
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    /// <summary>
    /// Interaction logic for AlertsSafeList.xaml
    /// </summary>
    public partial class AlertsSafeList : Window
    {
        private Alarm _alarm;

        internal AlertsSafeList(Alarm alarm)
        {
            InitializeComponent();
            SafeListListBox.ItemsSource = alarm.SafeList;
            _alarm = alarm;
        }

        private void SafelistDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Are you sure you want to delete this player from safe list?", "ExTibia", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Button button = sender as Button;
                var dataContext = button.DataContext;
                StringObject player = (StringObject)dataContext;
                _alarm.SafeList.Remove(player);
            }
        }

        private void AlertsAddtoList_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text != "")
            {
                _alarm.SafeList.Add(new StringObject(txtName.Text));
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
