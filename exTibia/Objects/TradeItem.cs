using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class TradeItem
    {
        #region Fields

        private int _id;
        private string _name;
        private int _weight;
        private int _buy;
        private int _sell;
        private int _qtd;
        private int _index;
        
        #endregion

        #region Properties

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        public int Buy
        {
            get { return _buy; }
            set { _buy = value; }
        }
        public int Sell
        {
            get { return _sell; }
            set { _sell = value; }
        }
        public int Qtd
        {
            get { return _qtd; }
            set { _qtd = value; }
        }
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        #endregion
    }
}
