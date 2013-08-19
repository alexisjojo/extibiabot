using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Cooldown
    {
        #region Singleton

        static readonly Cooldown _instance = new Cooldown();

        public static Cooldown Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        public int CategoryAttackTimeStart { get; private set; }

        public int CategoryAttackTimeEnd { get; private set; }

        public int CategoryHealingTimeStart { get; private set; }

        public int CategoryHealingTimeEnd { get; private set; }

        public int CategorySupportTimeStart { get; private set; }

        public int CategorySupportTimeEnd { get; private set; }

        public int CategorySpecialTimeStart { get; private set; }

        public int CategorySpecialTimeEnd { get; private set; }

        #endregion

        #region Methods

        public static int TibiaTime
        {
            get
            {
                return Memory.ReadInt32(Addresses.Client.TibiaTime);
            }
        }

        public void Refresh()
        {
            try
            {
                int pointer_main = Memory.ReadInt32(Addresses.Client.CoolDownCategoryStart);
                int pointer_atk = Memory.ReadInt32(pointer_main);
                int pointer_hea = Memory.ReadInt32(pointer_atk);
                int pointer_sup = Memory.ReadInt32(pointer_hea);

                this.CategoryAttackTimeStart = Memory.ReadInt32(pointer_atk + 12);
                this.CategoryAttackTimeEnd = Memory.ReadInt32(pointer_atk + 16);
                this.CategoryHealingTimeStart = Memory.ReadInt32(pointer_hea + 12);
                this.CategoryHealingTimeEnd = Memory.ReadInt32(pointer_hea + 16);
                this.CategorySupportTimeStart = Memory.ReadInt32(pointer_sup + 12);
                this.CategorySupportTimeEnd = Memory.ReadInt32(pointer_sup + 16);
                this.CategorySpecialTimeStart = Memory.ReadInt32(pointer_main + 12);
                this.CategorySpecialTimeEnd = Memory.ReadInt32(pointer_main + 16);

                int int5 = Memory.ReadInt32(Addresses.Client.CoolDownItems);
                int int6 = Memory.ReadInt32(Addresses.Client.CoolDownItemsStart);
                int Ptr = Memory.ReadInt32(int6);
                for (int index = 0; index < int5; ++index)
                {
                    int int7 = Memory.ReadInt32(Ptr);
                    int CoolDownID = Memory.ReadInt32(Ptr + 8);
                    int CoolDownStart = Memory.ReadInt32(Ptr + 12);
                    int CoolDownEnd = Memory.ReadInt32(Ptr + 16);
                    int6 = int7;

                    foreach (Spell spell in Spells.Instance.SpellsList)
                    {
                        if (spell.CoolDownID == CoolDownID)
                        {
                            spell.CooldownTimeStart = CoolDownStart;
                            spell.CooldownTimeEnd = CoolDownEnd;
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        public int CooldownTime(string spell)
        {
            try
            {
                if (spell == null)
                    throw new ArgumentNullException("spell");

                int num1 = -1;
                Refresh();
                bool flag = false;
                spell = spell.ToLower().Trim();
                Spell spell1 = new Spell();
                foreach (Spell spell2 in Spells.Instance.SpellsList)
                {
                    if ((spell2.Name.ToLower().Trim() == spell ? 0 : (!(spell2.Cast.ToLower().Trim() == spell) ? 1 : 0)) == 0)
                    {
                        flag = true;
                        spell1 = spell2;
                        break;
                    }
                }
                if (flag)
                {
                    int num2 = 0;
                    if (spell1.SpellCategory == SpellCategory.Attack)
                        num2 = this.CategoryAttackTimeEnd;
                    if (spell1.SpellCategory == SpellCategory.Healing)
                        num2 = this.CategoryHealingTimeEnd;
                    if (spell1.SpellCategory == SpellCategory.Support)
                        num2 = this.CategorySupportTimeEnd;
                    int num3 = Memory.ReadInt32(Addresses.Client.TibiaTime);

                    num1 = num2 <= spell1.CooldownTimeEnd ? Utils.Positive(spell1.CooldownTimeEnd - num3) : Utils.Positive(num2 - num3);
                }
                return num1;
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return 0;
        }

        public bool CanCast(string spell)
        {
            try
            {
                if (spell == null)
                    throw new ArgumentNullException("spell");

                Refresh();
                bool flag1 = false;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;

                spell = spell.ToLower().Trim();
                Spell spell1 = new Spell();
                foreach (Spell spell2 in Spells.Instance.SpellsList)
                {
                    if ((spell2.Name.ToLower().Trim() == spell ? 0 : (!(spell2.Cast.ToLower().Trim() == spell) ? 1 : 0)) == 0)
                    {
                        flag2 = true;
                        spell1 = spell2;
                        break;
                    }
                }
                if (flag2)
                {

                    int num1 = Memory.ReadInt32(Addresses.Client.TibiaTime);
                    int num2 = 0;
                    if (spell1.SpellCategory == SpellCategory.Attack)
                    {
                        num2 = this.CategoryAttackTimeEnd;
                        flag4 = true;
                    }
                    if (spell1.SpellCategory == SpellCategory.Healing)
                    {
                        num2 = this.CategoryHealingTimeEnd;
                        flag4 = true;
                    }
                    if (spell1.SpellCategory == SpellCategory.Support)
                    {
                        num2 = this.CategorySupportTimeEnd;
                        flag4 = true;
                    }
                    if (flag4)
                    {
                        if (num1 > num2)
                            flag3 = true;
                        if (flag3 && num1 > spell1.CooldownTimeEnd)
                            flag1 = true;
                    }
                    else
                        flag1 = true;
                }
                return flag1;
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return false;
        }

        #endregion
    }
}
