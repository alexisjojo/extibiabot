using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class HealItem
    {
        #region Fields

        private string _name;

        #endregion

        #region Properties

        public string Name
        {
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(_name.ToLower()); ; }
            set { _name = value; }
        }

        #endregion

        #region Constructor

        public HealItem()
        {

        }

        public HealItem(string name)
        {
            _name = name;
        }

        #endregion
    }
}
