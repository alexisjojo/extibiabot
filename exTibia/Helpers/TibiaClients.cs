using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Security.Permissions;
using System.Diagnostics;
using System.ComponentModel;

using exTibia.Modules;
using exTibia.Objects;


namespace exTibia.Helpers
{
    public class TibiaClients : INotifyPropertyChanged
    {

        #region Singleton

        static readonly TibiaClients _instance = new TibiaClients();

        public static TibiaClients Instance
        {
            get { return _instance; }
        }

        #endregion


        #region Fields


        private ObservableCollection<TibiaClient> _clients = new ObservableCollection<TibiaClient>();
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public ObservableCollection<TibiaClient> Clients
        {
            get 
            {
                return _clients; 
            }
        }

        #endregion

        #region Methods

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static Collection<TibiaClient> GameClients()
        {
            Collection<TibiaClient> _clients = new Collection<TibiaClient>();
            foreach (Process p in GetProcessesFromClassName("TibiaClient"))
            {
                TibiaClient client = new TibiaClient(p);
                _clients.Add(client);
            }
            return _clients;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static Process[] GetProcessesFromClassName(string ClassName)
        {
            try
            {
                StringBuilder strBuilder = new StringBuilder(100);
                List<Process> processList = new List<Process>();
                Process[] processlist = Process.GetProcesses();
                foreach (Process proc in processlist)
                {
                    try
                    {
                        if (NativeMethods.GetClassName(proc.MainWindowHandle, strBuilder, 100) != 0)
                        {
                            if (strBuilder.ToString() == ClassName)
                            {
                                strBuilder.Remove(0, strBuilder.Length);
                                processList.Add(proc);
                            }
                        }
                    }
                    catch(InvalidOperationException)
                    {
                        continue;
                    }
                }
                return processList.ToArray();
            }
            catch (InvalidOperationException e)
            {
                Debug.Report(e);
                return null;
            }
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public static string GetCharacterNameFromProcess(Process process)
        {
            try
            {
                GameClient.Tibia = process;
                NativeMethods.OpenProcess(NativeMethods.PROCESS_ALL_ACCESS, 0, (uint)GameClient.Tibia.Id);

                if (!string.IsNullOrEmpty(Player.Name))
                {
                    return Player.Name;
                }

                GameClient.Tibia = null;
                return "Not logged in.";
            }
            catch (InvalidOperationException e)
            {
                Debug.Report(e);
                return null;
            }
        }

        #endregion

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
