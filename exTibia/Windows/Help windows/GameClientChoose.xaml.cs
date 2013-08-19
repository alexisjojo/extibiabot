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
using System.Windows.Threading;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    /// <summary>
    /// Interaction logic for GameClientChoose.xaml
    /// </summary>
    public partial class GameClientChoose : Window
    {
        public GameClientChoose()
        {
            InitializeComponent();

            foreach (TibiaClient client in TibiaClients.GameClients())
                TibiaClients.Instance.Clients.Add(client);

            ListboxClients.ItemsSource = TibiaClients.Instance.Clients;          
        }

        private void buttonAttach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListboxClients.SelectedItem != null)
                {
                    TibiaClient selectedClient = (TibiaClient)ListboxClients.SelectedItem;
                    GameClient.Tibia = selectedClient.Process;
                    new GameClient(selectedClient).AssignProcess();                    
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            TibiaClients.Instance.Clients.Clear();

            foreach (TibiaClient client in TibiaClients.GameClients())
                TibiaClients.Instance.Clients.Add(client);

            ListboxClients.ItemsSource = TibiaClients.Instance.Clients;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
