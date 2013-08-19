using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class HudScript 
    {
        #region Fields

        private string _name;
        private int _intervalRate;
        private Collection<HudItem> _hudItems = new Collection<HudItem>();
        private bool _enabled = true;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int IntervalRate
        {
            get { return _intervalRate; }
            set { _intervalRate = value; }
        }

        public Collection<HudItem> HudItems
        {
            get { return _hudItems; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        #endregion

        #region Constructor

        public HudScript(string name, int interval, Collection<HudItem> hudItems, bool enabled)
        {
            this._name = name;
            this._intervalRate = interval;
            this._hudItems = hudItems;
            this._enabled = enabled;
        }

        #endregion
    }
}
