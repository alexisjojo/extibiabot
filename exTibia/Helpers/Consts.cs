using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.ComponentModel;
using System.Collections.ObjectModel;

using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public static class Consts
    {
        private static Collection<int> _Fire_IDs = new Collection<int>() { 2118, 2119, 2131, 2132, 2123, 2124 };

        public static Collection<int> Fire_IDs
        {
            get { return Consts._Fire_IDs; }
        }

        private static Collection<int> _Energy_IDs = new Collection<int>() { 2135, 2126 };

        public static Collection<int> Energy_IDs
        {
            get { return Consts._Energy_IDs; }
        }

        private static Collection<int> _Poision_IDs = new Collection<int>() { 2134, 2127 };

        public static Collection<int> Poision_IDs
        {
            get { return Consts._Poision_IDs; }
        }

        private static Collection<HealItem> _HealItems = new Collection<HealItem>()
            {             
                new HealItem("Exura"),
                new HealItem("Exura San"),               
                new HealItem("Exura Vita"),              
                new HealItem("Exura Ico"),             
                new HealItem("Exura Gran"),
                new HealItem("Exura Gran Ico"),
                new HealItem("Exura Gran San"),
                new HealItem("Exura Gran Mas Res"),
                new HealItem("Exana Kor"),
                new HealItem("Exana Flam"),
                new HealItem("Exana Vis"),
                new HealItem("Exana Mort"),
                new HealItem("Exana Pox"),
                new HealItem("Utura"),
                new HealItem("Utura Gran"),            
                new HealItem("Small Health Potion"),
                new HealItem("Health Potion"),
                new HealItem("Strong Health Potion"),
                new HealItem("Great Health Potion"),
                new HealItem("Ultimate Health Potion"),
                new HealItem("Mana Potion"),
                new HealItem("Strong Mana Potion"),
                new HealItem("Great Mana Potion"),
                new HealItem("Great Spirit Potion"),
            };

        public static Collection<HealItem> HealItems
        {
            get { return Consts._HealItems; }
        }

        private static Collection<HealerAdditionalCondition> _AdditionalConditions = new Collection<HealerAdditionalCondition>()
        {
            new HealerAdditionalCondition(HealerAdditional.Burned,false),
            new HealerAdditionalCondition(HealerAdditional.Drunken,false),
            new HealerAdditionalCondition(HealerAdditional.Energized,false),
            new HealerAdditionalCondition(HealerAdditional.Hasted,false),
            new HealerAdditionalCondition(HealerAdditional.InsidePz,false),
            new HealerAdditionalCondition(HealerAdditional.ManaShield,false),
            new HealerAdditionalCondition(HealerAdditional.Paralyzed,false),
            new HealerAdditionalCondition(HealerAdditional.Poisoned,false),
            new HealerAdditionalCondition(HealerAdditional.Pvpsigned,false),
            new HealerAdditionalCondition(HealerAdditional.Strengthened,false)
        };

        public static Collection<HealerAdditionalCondition> AdditionalConditions
        {
            get { return Consts._AdditionalConditions; }
        }

        private static Collection<SpellSetting> _AttackSpells = new Collection<SpellSetting>()
        {
            new SpellSetting("exori vis",2100,2200),
            new SpellSetting("exori tera",2100,2200),
			new SpellSetting("exori flam",2100,2200),
			new SpellSetting("exori frigo",2100,2200),
			new SpellSetting("exori mort",2100,2200),
			new SpellSetting("exori moe ico",2100,2200),
			new SpellSetting("exevo flam hur",4100,4200),
			new SpellSetting("exevo vis lux",4100,4200),
			new SpellSetting("exevo gran vis lux",6100,6200),
			new SpellSetting("exevo vis hur",8100,8200),
			new SpellSetting("exori amp vis",8100,8200),
			new SpellSetting("exori gran flam",8100,8200),
			new SpellSetting("exori gran vis",8100,8200),
			new SpellSetting("exori max flam",30100,30200),
			new SpellSetting("exori max vis",30100,30200),
			new SpellSetting("exevo gran mas vis",40100,40200),
			new SpellSetting("exevo gran mas flam",40100,40200),
			new SpellSetting("exevo frigo hur",4100,4200),
			new SpellSetting("exevo tera hur",4100,4200),
			new SpellSetting("exevo gran frigo hur",8100,8200),
			new SpellSetting("exori gran tera",8100,8200),
			new SpellSetting("exori gran frigo",8100,8200),
			new SpellSetting("exori max frigo",30100,30200),
			new SpellSetting("exevo gran mas tera",40100,40200),
			new SpellSetting("exevo gran mas frigo",40100,40200),
			new SpellSetting("exori con",2100,2200),
			new SpellSetting("exori san",2100,2200),
			new SpellSetting("exevo mas san",4100,4200),
			new SpellSetting("exori gran con",8100,8200),
			new SpellSetting("exori ico",2100,2200),
			new SpellSetting("exori",4100,4200),
			new SpellSetting("exori hur",6100,6200),
			new SpellSetting("exori min",6100,6200),
			new SpellSetting("exori gran",6100,6200),
			new SpellSetting("exori mas",8100,8200),
			new SpellSetting("exori gran ico",30100,30200),
            /*
            new TargetSetting.SettingSpell("adevo mas hur",2100,2200),
			new TargetSetting.SettingSpell("adori vis",2100,2200),
			new TargetSetting.SettingSpell("adori min vis",2100,2200),
			new TargetSetting.SettingSpell("adori tera",2100,2200),
			new TargetSetting.SettingSpell("adori mas frigo",2100,2200),
			new TargetSetting.SettingSpell("adori frigo",2100,2200),
			new TargetSetting.SettingSpell("adori mas tera",2100,2200),
			new TargetSetting.SettingSpell("adori flam",2100,2200),
			new TargetSetting.SettingSpell("adori mas flam",2100,2200),
			new TargetSetting.SettingSpell("adori gran mort",2100,2200),
			new TargetSetting.SettingSpell("adori mas vis",2100,2200),
			new TargetSetting.SettingSpell("adori san",2100,2200),
		*/

        };

        public static Collection<SpellSetting> AttackSpells
        {
            get { return Consts._AttackSpells; }
        }






    }
}
