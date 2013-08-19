using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class Alarm
    {
        #region Fields

        private string _name;
        private AlertType _alertType;
        private bool _playSound;
        private bool _focusClient;
        private bool _pauseBot;
        private bool _disconnect;
        private ObservableCollection<StringObject> _safeList;
        private Visibility _buttonVisibility;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public AlertType AlertType
        {
            get { return _alertType; }
            set { _alertType = value; }
        }

        public Visibility ButtonVisibility
        {
            get { return _buttonVisibility; }
            set { _buttonVisibility = value; }
        }

        public bool PlaySound
        {
            get { return _playSound; }
            set { _playSound = value; }
        }

        public bool FocusClient
        {
            get { return _focusClient; }
            set { _focusClient = value; }
        }

        public bool PauseBot
        {
            get { return _pauseBot; }
            set { _pauseBot = value; }
        }

        public bool Disconnect
        {
            get { return _disconnect; }
            set { _disconnect = value; }
        }

        public ObservableCollection<StringObject> SafeList
        {
            get { return _safeList; }
        }

        #endregion

        #region Constructors

        public Alarm(string name, AlertType alertType, bool playSound, bool focusClient, bool pauseBot, bool disconnect)
        {
            this._name = name;
            this._alertType = alertType;
            this._playSound = playSound;
            this._focusClient = focusClient;
            this._pauseBot = pauseBot;
            this._disconnect = disconnect;
            this._buttonVisibility = Visibility.Hidden;
        }

        public Alarm(string name, AlertType alertType, bool playSound, bool focusClient, bool pauseBot, bool disconnect, ObservableCollection<StringObject> safeList)
        {
            this._name = name;
            this._alertType = alertType;
            this._playSound = playSound;
            this._focusClient = focusClient;
            this._pauseBot = pauseBot;
            this._disconnect = disconnect;
            this._safeList = safeList;
            this._buttonVisibility = Visibility.Visible;
        }

        #endregion
    }
}
