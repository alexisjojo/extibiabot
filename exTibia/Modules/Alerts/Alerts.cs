using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using System.Web.Script.Serialization;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{

    #region Help class

    public class StringObject
    {
        public string Value { get; set; }

        public StringObject(string value)
        {
            Value = value;
        }
    }

    #endregion

    public class Alerts
    {
        #region Singleton

        static readonly Alerts _instance = new Alerts();

        public static Alerts Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Constructor

        public Alerts()
        {
            _workerThread = new Thread(Run);
            _workerThread.SetApartmentState(ApartmentState.STA);
            _workerThread.Start();
        }

        #endregion

        #region Fields

        private static bool _alertsState = false;
        private static SoundPlayer soundPlayer;
        private static string path;
        private static bool isPlaying = false;
        private static bool _shouldStop = false;


        private Thread _workerThread;
        private static ObservableCollection<StringObject> safeListPlayerOnScreen = new ObservableCollection<StringObject>();
        private static ObservableCollection<StringObject> safeListPlayerAttacking = new ObservableCollection<StringObject>();
        private static ObservableCollection<StringObject> safeListDefaultMessage = new ObservableCollection<StringObject>();
        private static ObservableCollection<StringObject> safeListPrivateMessage = new ObservableCollection<StringObject>();

        private static ObservableCollection<Alarm> _alertsList = new ObservableCollection<Alarm>()
        {
            new Alarm("Player on Screen",AlertType.PlayerOnScreen,true,false,true,false,safeListPlayerOnScreen),
            new Alarm("Player Attacking",AlertType.PlayerAttacking,false,true,false,true,safeListPlayerAttacking),
            new Alarm("Default Message",AlertType.DefaultMessage,true,true,true,false,safeListDefaultMessage),
            new Alarm("Private Message",AlertType.PrivateMessage,false,true,false,false,safeListPrivateMessage),
            new Alarm("Disconnected",AlertType.Disconnected,true,false,false,true),
            new Alarm("CrashedFroze",AlertType.CrashedFroze,false,true,true,false)
        };



        #endregion

        #region Properties

        public static bool ShouldStop
        {
            get { return Alerts._shouldStop; }
            set { Alerts._shouldStop = value; }
        }

        public static ObservableCollection<Alarm> AlarmList
        {
            get { return _alertsList; }
        }

        public static ObservableCollection<StringObject> SafeListPlayerOnScreen
        {
            get { return safeListPlayerOnScreen; }
        }

        public static ObservableCollection<StringObject> SafeListPlayerAttacking
        {
            get { return safeListPlayerAttacking; }
        }

        public static ObservableCollection<StringObject> SafeListDefaultMessage
        {
            get { return safeListDefaultMessage; }
        }

        public static ObservableCollection<StringObject> SafeListPrivateMessage
        {
            get { return safeListPrivateMessage; }
        }


        #endregion

        #region Methods

        void Run()
        {
            while (true)
            {
                while (_alertsState)
                {
                    foreach (Alarm alarm in AlarmList.Where(a => a.PlaySound || a.PauseBot || a.Disconnect || a.FocusClient))
                    {
                        switch (alarm.AlertType)
                        {
                            case AlertType.PlayerAttacking:
                                if (CheckPlayerAttacking(alarm))
                                    ProcessAlarm(alarm);
                                break;
                            case AlertType.PlayerOnScreen:
                                if (CheckPlayerOnScreen(alarm))
                                    ProcessAlarm(alarm);
                                break;
                            case AlertType.DefaultMessage:
                                if (CheckDefaultMessage(alarm))
                                    ProcessAlarm(alarm);
                                break;
                            case AlertType.PrivateMessage:
                                if (CheckPrivateMessage(alarm))
                                    ProcessAlarm(alarm);
                                break;
                            case AlertType.Disconnected:
                                if (CheckDisconnected(alarm))
                                    ProcessAlarm(alarm);
                                break;
                            case AlertType.CrashedFroze:
                                if (CheckCrashedFroze(alarm))
                                    ProcessAlarm(alarm);
                                break;
                        }
                    }
                    Thread.Sleep(125);
                }
                Thread.Sleep(125);
            }
        }

        static void ProcessAlarm(Alarm alarm)
        {
            if (alarm.FocusClient)
                FocusClient();
            if (alarm.PlaySound)
                PlaySound(alarm.AlertType);
            if (alarm.PauseBot)
                PauseBot();
            if (alarm.Disconnect)
                Disconnect();
        }

        static bool CheckPlayerAttacking(Alarm alarm)
        {
            try
            {
                if (alarm == null)
                    throw new ArgumentNullException("alarm");
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        static bool CheckPlayerOnScreen(Alarm alarm)
        {
            try
            {
                if (alarm == null)
                    throw new ArgumentNullException("alarm");
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        static bool CheckDefaultMessage(Alarm alarm)
        {
            try
            {
                if (alarm == null)
                    throw new ArgumentNullException("alarm");
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        static bool CheckPrivateMessage(Alarm alarm)
        {
            try
            {
                if (alarm == null)
                    throw new ArgumentNullException("alarm");
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        static bool CheckDisconnected(Alarm alarm)
        {
            try
            {
                if (alarm == null)
                    throw new ArgumentNullException("alarm");
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        static bool CheckCrashedFroze(Alarm alarm)
        {
            try
            {
                if (alarm == null)
                    throw new ArgumentNullException("alarm");
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        #endregion

        #region Alert methods

        static void PlaySound(AlertType alertType)
        {
            path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            soundPlayer = new SoundPlayer(path);

            switch (alertType)
            {
                case AlertType.PlayerAttacking:
                    path += "\\sounds\\playerattacking.wav";
                    break;
                case AlertType.PlayerOnScreen:
                    path += "\\sounds\\playeronscreen.wav";
                    break;
                case AlertType.DefaultMessage:
                    path += "\\sounds\\defaultmessage.wav";
                    break;
                case AlertType.PrivateMessage:
                    path += "\\sounds\\privatemessage.wav";
                    break;
                case AlertType.Disconnected:
                    path += "\\sounds\\disconnected.wav";
                    break;
                case AlertType.CrashedFroze:
                    path += "\\sounds\\crashedfroze.wav";
                    break;
            }

            if (!isPlaying)
            {
                isPlaying = true;
                soundPlayer.Play();
                isPlaying = false; ;
            }
        }

        static void FocusClient()
        {
            NativeMethods.SetForegroundWindow(GameClient.Tibia.MainWindowHandle);
        }

        static void PauseBot()
        {
            ShouldStop = true;
        }

        static void Disconnect()
        {
            GameClient.Tibia.Kill();
        }

        #endregion

        #region Load / Save settings

        public static string Serialize()
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            return oSerializer.Serialize(AlarmList);
        }

        public static void DeSerialize(string settings)
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            ObservableCollection<Alarm> converted = oSerializer.Deserialize<ObservableCollection<Alarm>>(settings);

            foreach (Alarm rule in converted)
            {
                AlarmList.Add(rule);
            }
        }

        #endregion
    }
}
