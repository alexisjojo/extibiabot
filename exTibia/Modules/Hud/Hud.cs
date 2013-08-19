using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.Globalization;
using System.ComponentModel;
using System.Threading;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class Hud
    {

        #region Singleton

        private static Hud _instance = new Hud();

        public static Hud Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private ObservableCollection<HudScript> _hudScripts = new ObservableCollection<HudScript>();
        private bool _state = true;
        private Thread _hudThread;

        #endregion

        #region Properties

        public bool State
        {
            get { return _state; }
            set { _state = value; }
        }

        public Thread HudThread
        {
            get { return _hudThread; }
            set { _hudThread = value; }
        }

        public ObservableCollection<HudScript> HudScripts
        {
            get { return _hudScripts; }
        }

        #endregion

        #region Constructor

        public Hud()
        {
            HudThread = new Thread(RunHuD);
            HudThread.Start();
        }
        
        #endregion

        #region Methods

        public void Init()
        {
            Helpers.Debug.WriteLine("Instance of Hud() has been created.", ConsoleColor.Yellow);
        }

        public void RunHuD()
        {
            while (true)
            {
                while (State)
                {
                    Thread.Sleep(50);

                    if (HudScripts.Count == 0)
                        continue;

                    if (HudScripts.Count == 1)
                    {
                        PipeClient.Instance.Send(HudScripts[0].HudItems[0].ToNetworkMessage());
                        Thread.Sleep(HudScripts[0].IntervalRate);
                    }
                    else
                        for (int i = 0; i <= HudScripts.Count - 1; i++)
                        {
                            foreach (HudItem item in HudScripts[i].HudItems)
                            {
                                PipeClient.Instance.Send(item.ToNetworkMessage());
                                if (i == HudScripts.Count - 1)
                                    Thread.Sleep(HudScripts[i].IntervalRate);
                                else
                                    Thread.Sleep(Math.Abs(HudScripts[i].IntervalRate - HudScripts[i + 1].IntervalRate));
                            }
                        }
                }
                Thread.Sleep(50);
            }
        }

        #endregion

    }
}
