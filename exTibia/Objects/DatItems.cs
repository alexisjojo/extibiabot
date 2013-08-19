using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public static class DatItems
    {
        public static int DatAddress(int id)
        {
            try
            {
                if (id < 100)
                    throw new ArgumentException("id is less than 100");

                int baseadd = Memory.ReadInt32(Addresses.Client.DatPointer);
                int datadr = Memory.ReadInt32(baseadd + 8);
                datadr += (int)Addresses.Client.DatStepItems * (id - 100);
                return datadr;
            }
            catch (ArgumentException e) 
            { 
                Debug.Report(e); 
            }

            return -1;
        }

        public static string GetName(int itemID)
        {
            if (itemID > 0) return Memory.ReadString(DatAddress(itemID));
            return "unknown item";
        }

        public static bool GetFlag(int itemID, ItemFlags flag)
        {
            try
            {
                if (itemID > 99 && itemID < 15000)
                {
                    int flags = Memory.ReadInt32(DatAddress(itemID) + Addresses.Client.DatFlagsOffset);
                    if (flags == 0)
                        return false;
                    if ((flags & (int)flag) != 0) return true;
                    return false;
                }
            }
            catch (ArgumentException e)
            {             
                Debug.Report(e);
            }
            return false;
        }

        public static void ShowFlags(int id)
        {
            foreach (ItemFlags itemflag in Enum.GetValues(typeof(ItemFlags)))
            {
                Debug.WriteLine(String.Format("Item flag {0}:{1}", HelpMethods.GetEnumDesc(itemflag), GetFlag(id, itemflag)), ConsoleColor.White);
            }
        }
    }
}
