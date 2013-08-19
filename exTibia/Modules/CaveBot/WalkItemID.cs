using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class WalkItemID
    {
        #region Fields

        private int _id;
        private string _text;

        #endregion

        #region Properties

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        #endregion

        #region Constructor

        public WalkItemID(int id)
        {
            this._id = id;
            this.Text = id.ToString();
        }

        #endregion
    }
}
