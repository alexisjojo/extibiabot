using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public class Client
    {
        #region Singleton

        static readonly Client _instance = new Client();

        public static Client Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        private string _statusBarText;
        private int _statusBarTime;
        private int _attackMode;
        private int _chaseType;
        private bool _isConnected;

        #endregion

        #region Properties

        public bool IsConnected
        {
            get { return Convert.ToBoolean(Memory.ReadInt32(Addresses.Client.IsConnected)); }
            set { _isConnected = value; }
        }

        public int ChaseType
        {
            get { return Memory.ReadInt32(Addresses.Client.ChaseType); }
            set { _chaseType = value; }
        }

        public int AttackMode
        {
            get { return Memory.ReadInt32(Addresses.Client.AttackMode); }
            set { _attackMode = value; }
        }

        public string StatusBarText
        {
            get { return Memory.ReadString(Addresses.Client.StatusBarText); }
            set { _statusBarText = value; }
        }
        
        public int StatusBarTime
        {
            get { return Memory.ReadInt32(Addresses.Client.StatusbarTime); }
            set { _statusBarTime = value; }
        }

        #endregion
    }

    
}
