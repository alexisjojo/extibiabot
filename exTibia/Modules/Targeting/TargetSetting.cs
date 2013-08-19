using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Objects
{
    public class TargetSetting
    {
        #region Fields

        private string _name;

        private int _hpRangeMin;
        private int _hpRangeMax;
        private int _danger;
        private int _distance;


        private AttackMode _attackMode;
        private MonsterAttack _monsterAttacks;
        private DesiredStance _desiredStance;
        private DesiredAttack _desiredAttack;

        private Collection<SpellSetting> _spells = new Collection<SpellSetting>()
        {
            new SpellSetting(),
            new SpellSetting(),
            new SpellSetting(),
            new SpellSetting()
        };

        #endregion

        #region Properties

        public int HpRangeMin
        {
            get { return _hpRangeMin; }
            set { _hpRangeMin = value; }
        }

        public int HpRangeMax
        {
            get { return _hpRangeMax; }
            set { _hpRangeMax = value; }
        }

        public int Danger
        {
            get { return _danger; }
            set { _danger = value; }
        }

        public AttackMode AttackMode
        {
            get { return _attackMode; }
            set { _attackMode = value; }
        }

        public MonsterAttack MonsterAttacks
        {
            get { return _monsterAttacks; }
            set { _monsterAttacks = value; }
        }

        public DesiredStance DesiredStance
        {
            get { return _desiredStance; }
            set { _desiredStance = value; }
        }

        public DesiredAttack DesiredAttack
        {
            get { return _desiredAttack; }
            set { _desiredAttack = value; }
        }

        public int Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        public Collection<SpellSetting> Spells
        {
            get { return _spells; }
        }

        public string Name
        {
            get { return String.Format("{0} (HP: {1}-{2})", _name, HpRangeMin, HpRangeMax); ; }
            set { _name = value; }
        }

        #endregion

        #region Constructor

        public TargetSetting()
        {

        }

        #endregion
    }

    public class SpellSetting
    {
        #region Fields

        private string _spell = "";
        private bool _synchronize;
        private int _delayMin;
        private int _delayMax;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_spell.ToLower());
            }
        }

        public string Info
        {
            get
            {
                Spell spell = new Spell();
                spell = Spells.Instance.SpellsList.First(s => s.Cast.ToLower() == Name.ToLower());
                return String.Format("{0}", spell.Cast);
            }
        }

        public string InfoExtended
        {
            get
            {
                Spell spell = new Spell();
                spell = Spells.Instance.SpellsList.First(s => s.Cast.ToLower() == Name.ToLower());
                return String.Format("Mana: {0} Level:{1}", spell.Mana, spell.Level);
            }
        }

        public string Spell
        {
            get { return _spell; }
            set { _spell = value; }
        }

        public bool Synchronize
        {
            get { return _synchronize; }
            set { _synchronize = value; }
        }

        public int DelayMin
        {
            get { return _delayMin; }
            set { _delayMin = value; }
        }

        public int DelayMax
        {
            get { return _delayMax; }
            set { _delayMax = value; }
        }

        #endregion

        #region Constructor

        public SpellSetting()
        {

        }

        public SpellSetting(string spell, int min, int max)
        {
            _spell = spell;
            _delayMin = min;
            _delayMax = max;
        }

        #endregion
    }
}
