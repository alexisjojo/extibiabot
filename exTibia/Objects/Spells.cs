using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class Spells
    {

        #region Singleton

        private static Spells _instance = new Spells();

        public static Spells Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Class Spells

        private Collection<Spell> _spellsList = new Collection<Spell>();

        public Collection<Spell> SpellsList
        {
            get { return _spellsList; }
        }

        public int LoadSpells()
        {
            try
            {
                string spellspath = System.IO.File.ReadAllText("spells.dat");
                return DeSerialize(spellspath);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return -1;
        }

        public Spell FindSpell(string name)
        {
            try
            {
                Spell spell = new Spell();

                if (name != null)
                {
                    for (int index = 0; index < SpellsList.Count; ++index)
                    {
                        if (SpellsList[index].Cast.Trim().ToUpper() == name.Trim().ToUpper())
                        {
                            spell = SpellsList[index];
                            return spell;
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        #endregion

        #region Load / Save settings

        public static string Serialize()
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            return oSerializer.Serialize(Instance.SpellsList);
        }

        public static int DeSerialize(string settings)
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            List<Spell> converted = oSerializer.Deserialize<List<Spell>>(settings);
            foreach (Spell spell in converted)
                Instance.SpellsList.Add(spell);

            return Instance.SpellsList.Count;
        }

        #endregion
    }

        #region Class Spell

    public class Spell
    {
        public string Name { get; set; }

        public string Cast { get; set; }

        public int Mana { get; set; }

        public int Level { get; set; }

        public int Magic { get; set; }

        public bool Premium { get; set; }

        public int Soul { get; set; }

        public double StartCoolDown { get; set; }

        public double Cooldown { get; set; }

        public double CooldownCategory { get; set; }

        public int CoolDownID { get; set; }

        public int CooldownTimeStart { get; set; }

        public int CooldownTimeEnd { get; set; }

        public SpellCategory SpellCategory { get; set; }

        public SpellType SpellType { get; set; }

        public int Range { get; set; }


    }

    #endregion
}
