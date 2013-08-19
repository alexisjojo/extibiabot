using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public static class HotkeysGame
    {
        public static int FindItem(int item)
        {
            try
            {
                if (item < 0)
                    throw new ArgumentNullException("item");

                for (int i = 0; i < 36; i++)
                {
                    int item_id = Memory.ReadInt32(Addresses.Hotkey.ObjectStart + Addresses.Hotkey.ObjectStep * i);

                    if (item == item_id)
                    {
                        return i + 1;
                    }
                }
                return 0;
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return 0;
        }

        public static int FindHotkey(string name)
        {
            try
            {

                if (String.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");

                name = name.ToLower();
                name = name.Replace("#s ", "");
                name = name.Replace("#S ", "");
                if (FindSpell(name) != 0)
                {
                    return FindSpell(name);
                }

                if (NameToId(name) != 0)
                {
                    int itemid = NameToId(name);
                    return FindItem(itemid);
                }

                return -1;
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return -1;
        }

        public static int NameToId(string name)
        {
            try
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                int result = Items.FindByName(name).ItemID;
                return result;
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return 0;
        }

        public static int FindSpell(string spell)
        {
            try
            {
                if (spell == null)
                    throw new ArgumentNullException("spell");

                Spell temp = new Spell();
                temp = Spells.Instance.FindSpell(spell);

                if (temp != null)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        string hotkey = Memory.ReadString(Addresses.Hotkey.TextStart + Addresses.Hotkey.TextStep * i);
                        hotkey = hotkey.ToLower();
                        hotkey = hotkey.Replace("#s ", "");

                        if (hotkey == temp.Cast)
                        {
                            return i + 1;
                        }
                    }
                }
                else
                {
                    int itemId = NameToId(spell);

                    for (int i = 0; i < 36; i++)
                    {
                        int hotkey = Memory.ReadInt32(Addresses.Hotkey.ObjectStart + Addresses.Hotkey.ObjectStep * i);

                        if (hotkey == itemId)
                            return i + 1;
                    }
                }


                return 0;
            }
            catch (ArgumentNullException ex)
            {
                Helpers.Debug.Report(ex);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return 0;
        }
    }
}
