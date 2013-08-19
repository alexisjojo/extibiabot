using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class HealerAdditionalCondition
    {
        #region Fields

        private HealerAdditional _Additional = new HealerAdditional();
        private bool _active = false;

        #endregion

        #region Properties
        
        public string TypeName
        {
            get { return _Additional.ToString(); }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public HealerAdditional Condition
        {
            get { return _Additional; }
            set { _Additional = value; }
        }

        #endregion

        #region Constructor

        public HealerAdditionalCondition(HealerAdditional additional, bool active)
        {
            _Additional = additional;
            _active = active;
        }

        #endregion
    }
}
