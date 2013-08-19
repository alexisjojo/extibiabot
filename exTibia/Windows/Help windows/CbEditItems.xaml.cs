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

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    /// <summary>
    /// Interaction logic for CbEditItems.xaml
    /// </summary>
    public partial class CbEditItems : Window
    {
        private bool _walkables;

        public bool Walkables
        {
            get { return _walkables; }
            set { _walkables = value; }
        }
       
        public CbEditItems(bool walkables)
        {
            InitializeComponent();

            Walkables = walkables;

            if (Walkables)
            {
                ItemsListbox.ItemsSource = CaveBot.Instance.WalkableIds;
                textTitle.Text = "List of walkable items:";
            }
            else
            {
                textTitle.Text = "List of nonwalkable items:";
                ItemsListbox.ItemsSource = CaveBot.Instance.NonWalkableIds;
            }
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int itemID = 0;

                if (int.TryParse(txtItemID.Text, out itemID))
                {
                    if (itemID > 0 && itemID < 20000)
                        if (Walkables)
                            CaveBot.Instance.WalkableIds.Add(new WalkItemID(itemID));
                        else
                            CaveBot.Instance.NonWalkableIds.Add(new WalkItemID(itemID));
                }
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem lbItem = sender as MenuItem;
                var dataContext = lbItem.DataContext;
                WalkItemID data = (WalkItemID)dataContext;
                if (MessageBox.Show("Are you sure you want to remove selected item?", "exTibia", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    if (Walkables)
                        CaveBot.Instance.WalkableIds.Remove(data);
                    else
                        CaveBot.Instance.NonWalkableIds.Remove(data);
                }
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
