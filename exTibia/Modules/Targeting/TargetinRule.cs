using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class TargetinRule
    {
        #region Fields

        private string _monsterName;
        private string _cathegory;
        private int _count;
        private ObservableCollection<TargetSetting> _targetSettings = new ObservableCollection<TargetSetting>();
        private bool _selected = false;
        private int _settingID;

        #endregion

        #region Properties

        public string MonsterName
        {
            get { return _monsterName; }
            set { _monsterName = value; }
        }

        public string Cathegory
        {
            get { return _cathegory; }
            set { _cathegory = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public ObservableCollection<TargetSetting> Settings
        {
            get { return _targetSettings; }
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public int SettingID
        {
            get { return _settingID; }
            set { _settingID = value; }
        }

        #endregion

        #region Constructor

        public TargetinRule()
        {

        }

        #endregion
    }
}
