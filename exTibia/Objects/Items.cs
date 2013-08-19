using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    #region Item Class

    public class Item
    {
        public string Name { get; set; }

        public int ItemID { get; set; }

        public int NPCvalue { get; set; }

        public int NPCprice { get; set; }
    }

    #endregion

    public class Items
    {
        #region Singleton

        static readonly Items _instance = new Items();

        public static Items Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Methods

        private static Collection<Item> _itemsList = new Collection<Item>();

        public static Collection<Item> ItemsList
        {
            get { return Items._itemsList; }
        }

        public static int LoadItems()
        {
            try
            {
                string itempath = System.IO.File.ReadAllText("items.dat");
                return DeSerialize(itempath);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return -1;
        }

        public static Item FindById(int id)
        {
            try
            {
                foreach (Item item in ItemsList)
                {
                    if (item.ItemID == id)
                    {
                        return item;
                    }
                }
                return null;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
            return null;
        }

        public static Item FindByName(string name)
        {
            try
            {
                foreach (Item item in ItemsList)
                {
                    if (item.Name.ToLower() == name.ToLower())
                    {
                        return item;
                    }
                }
                return null;
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
            return oSerializer.Serialize(ItemsList);
        }

        public static int DeSerialize(string settings)
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            List<Item> converted = oSerializer.Deserialize<List<Item>>(settings);

            foreach (Item item in converted)
                ItemsList.Add(item);

            return converted.Count;
        }

        #endregion
    }
}
