using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class HealRule
    {
        #region Fields

        private int _HP_MIN;
        private int _HP_MAX;
        private int _MP_MIN;
        private int _MP_MAX;
        private int _DL_MIN;
        private int _DL_MAX;
        private int _priority;
        private int _lifetime;

        private HealerRangeType _HPRange;
        private HealerRangeType _MPRange;
        private HealItem _Item;

        private Collection<HealerAdditionalCondition> _Additionals = new Collection<HealerAdditionalCondition>();

        #endregion

        #region Properties

        public int HP_MIN
        {
            get { return _HP_MIN; }
            set { _HP_MIN = value; }
        }
        
        public int HP_MAX
        {
            get { return _HP_MAX; }
            set { _HP_MAX = value; }
        }
        
        public int MP_MIN
        {
            get { return _MP_MIN; }
            set { _MP_MIN = value; }
        }
        
        public int MP_MAX
        {
            get { return _MP_MAX; }
            set { _MP_MAX = value; }
        }       

        public int DL_MIN
        {
            get { return _DL_MIN; }
            set { _DL_MIN = value; }
        }       

        public int DL_MAX
        {
            get { return _DL_MAX; }
            set { _DL_MAX = value; }
        }
        
        public int priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
        
        public int lifetime
        {
            get { return _lifetime; }
            set { _lifetime = value; }
        }

        public HealerRangeType HPRange
        {
            get { return _HPRange; }
            set { _HPRange = value; }
        }
        
        public HealerRangeType MPRange
        {
            get { return _MPRange; }
            set { _MPRange = value; }
        }
        
        public HealItem Item
        {
            get { return _Item; }
            set { _Item = value; }
        }
        
        public Collection<HealerAdditionalCondition> Additionals
        {
            get { return _Additionals; }
        }

        [ScriptIgnore]
        public string TextName
        {
            get { return Item.Name; }
        }

        [ScriptIgnore]
        public string TextHPrange
        {
            get { return String.Format("{0} to {1}  ({2})", this.HP_MIN, this.HP_MAX, TextHPrangeType); }
        }

        [ScriptIgnore]
        public string TextMPrange
        {
            get { return String.Format("{0} to {1} ({2})", this.MP_MIN, this.MP_MAX, TextMPrangeType); }
        }

        [ScriptIgnore]
        public string TextPriority
        {
            get { return String.Format("{0}", this.priority); }
        }

        [ScriptIgnore]
        public string TextHPrangeType
        {
            get { return (HPRange == HealerRangeType.Exact) ? "Exact" : "Percent"; }
        }

        [ScriptIgnore]
        public string TextMPrangeType
        {
            get { return (MPRange == HealerRangeType.Exact) ? "Exact" : "Percent"; }
        }

        #endregion

        #region Constructors

        public HealRule()
        {

        }

        public HealRule(HealItem _item, int _HP_MIN, int _HP_MAX, int _MP_MIN, int _MP_MAX, int _DL_MIN, int _DL_MAX, int _priority, int _lifetime, HealerRangeType _HPran, HealerRangeType _MPran, Collection<HealerAdditionalCondition> _additionals)
        {
            Item = _item;

            HPRange = _HPran;
            MPRange = _MPran;

            foreach (HealerAdditionalCondition additional in _additionals)
                Additionals.Add(additional);

            HP_MIN = _HP_MIN;
            HP_MAX = _HP_MAX;
            MP_MIN = _MP_MIN;
            MP_MAX = _MP_MAX;

            DL_MIN = _DL_MIN;
            DL_MAX = _DL_MAX;

            priority = _priority;
            lifetime = _lifetime;
        }

        #endregion
    }
}
