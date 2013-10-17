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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    public partial class MainWindow : Window
    {
        private Settings SettingsWindow = new Settings();
        private GameClientChoose GameClientWindow = new GameClientChoose();
        private HudWindow HudWindow = new HudWindow();


        public MainWindow()
        {
            InitializeComponent();
            InitializeBot();
        }

     
        public void InitializeBot()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            if (TibiaClients.GetProcessesFromClassName("TibiaClient").Count() > 0)
            {
                if (TibiaClients.GetProcessesFromClassName("TibiaClient").Count() == 1)
                {
                    GameClient client = new GameClient(new TibiaClient(TibiaClients.GetProcessesFromClassName("TibiaClient")[0]));
                    client.AssignProcess();
                    Title = String.Format("{0} {2} - {1}", fvi.ProductName, "xd", fvi.ProductVersion.Substring(0, 5));
                }
                else
                {
                    GameClientWindow.ShowDialog();
                    //Title = String.Format("{0} {2} - {1}", fvi.ProductName, "xd", fvi.ProductVersion.Substring(0, 5));
                }
            }
            else
            {
                Title = String.Format("{0} {2} - {1}", fvi.ProductName, "Tibia not found.", fvi.ProductVersion.Substring(0, 5));
            }

#if DEBUG
            HelpMethods.AllocateConsole();
#endif
            Worker.Instance.Init();
            CaveBot.Instance.Init();
            Targeting.Instance.Init();
            Healer.Instance.Init();


            Hud.Instance.Init();
            //PipeServer.Instance.Init();
            //Injector.Instance.Inject();
            //PipeClient.Instance.Connect();
            //PipeClient.Instance.Send(new NetworkMessage(new byte[] { 0x02 }));

            /*
            int loadedItems = Items.LoadItems();           

            if (loadedItems != -1)
                Helpers.Debug.WriteLine(String.Format("Items has been loaded successfully. Loaded items: {0}.", loadedItems), ConsoleColor.White);
            else
                Helpers.Debug.WriteLine(String.Format("Could not load items. Please download again and reinstall the bot."), ConsoleColor.Red);

            int loadedSpells = Spells.Instance.LoadSpells();

            if (loadedSpells != -1)
                Helpers.Debug.WriteLine(String.Format("Spells has been loaded successfully. Loaded spells: {0}.", loadedSpells), ConsoleColor.White);
            else
                Helpers.Debug.WriteLine(String.Format("Could not load items. Please download again and reinstall the bot."), ConsoleColor.Red);

            GuiEquipment.Instance.UpdateGUI();
            var z = GuiEquipment.Instance;
            */
        }



        #region Menu Items

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow.Owner = this;
            SettingsWindow.Show();
        }

        private void MenuItemClients_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            TibiaClients.Instance.Clients.Clear();

            foreach (TibiaClient client in TibiaClients.GameClients())
                TibiaClients.Instance.Clients.Add(client);

            MenuItemClients.ItemsSource = TibiaClients.Instance.Clients;
        }

        private void MenuItemClients_Click(object sender, RoutedEventArgs e)
        {
            GameClientWindow.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void menuItemOfficialWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://extibia.com");
        }

        private void menuItemLatestNews_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://extibia.com/news");
        }

        private void menuItemPurchase_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://extibia.com/purchase");
        }

        private void menuItemSupport_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://extibia.com/support");
        }

        private void menuItemMyAccount_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://accounts.extibia.com");
        }

        private void menuItemPolishFansite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://extibia.pl");
        }

        private void menuItemBrasilFansite_Checked(object sender, RoutedEventArgs e)
        {
            Process.Start("http://extibia.net.br");
        }

        private void menuItemCloseExTibia_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close exTibia?", "exTibia", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
        }

        #endregion
    }
}