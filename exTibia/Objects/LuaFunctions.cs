using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using System.Reflection;


namespace exTibia.Objects
{
    public class LuaFunctions
    {

        #region Singleton

        private static LuaFunctions _instance = new LuaFunctions();

        public static LuaFunctions Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Functions to parse

        [LuaGlobalAttribute(
            Name = "closeallbps", 
            Description = "Closing all visible containers.", 
            Params = "")]
        public void closeallbps()
        {
            foreach (Container bp in Inventory.Instance.GetContainers().Reverse())
            {
                bp.Close();
                System.Threading.Thread.Sleep(new Random().Next(300,600));
            }
        }

      
        [LuaGlobalAttribute(
            Name = "say", 
            Description = "Closing all visible containers.", 
            Params = "text - text to say")]
        public void say(string text)
        {
            System.Windows.Forms.MessageBox.Show(text);
        }

        #endregion

    }
}


/*

 * 
battleopen()
connected()
focused()
minimized()
tradeopen()
typedtext()
attacked()
followed()
self()
target
targetingtarget()
createarea(string name, string consider type)
deletearea(string name)
activatearea(string name)
deactivatearea(string name)
addsqm(string area, int x, int y, int z)
playalarm()
moveitemsonto(string backpack from, string backpack destiny, int backpack id, int items)
settargeting(boolean value)
setwalking(boolean value)
setlooting(boolean value)
sethealer(boolean value)
sayon(string message, string channel)
moveonground(int x1, int y1, int z1, int x2, int y2, int z2)
opennewbp(string name, string container)
opennewbp(int id, string container)
islocation()
wpt()
gotoway(int waypoint id)
turn(string direction)
dropitems(int item id, string backpack)
dropitemsat(int item id, string backpack, int x, int y, int z)
presskey(int key)
attackcreature(string name)
attackcreatureat(int x, int y, int z)
useitemfrombpon(int itemid, int x, int y, int z)
findcreature(string name)
itemcount(int itemid)
itemcount(string name)
reachcreature(string name)
islocation(int x, int y, int z)
wait(int x, int y)
waitcontainer(string name)
waitcontainer(int id)
move(string loc)
travel(string location)
flasks()
itemcost(int item)
itemcost(string name)
say(string text)
npcsay(string text)
ischannel(string channel)
waitping()
opentrade()
cast(string spell)
closewindowsall()
closewindows(string name)
maximizewindowsall()
maximizewindow(string name)
minimizewindowsall()
minimizewindows(string name)
buyitems (string name, int amount)
buyitemsupto (string name, int amount)
levitate(string level)
usehotkey(string hotkey)
closetibia()
closebot()
moveitems(int itemid, string from, string where, int amount)
gettile(int x, int y, int z)
gettileown()
gotoway(string way)
reachdp()
reacheddp()
opendepot()
isopened(string name)
isopened(int id)
useonground(string dir)
openmainbp()
setattack(string type)
setchase(string type)
bpopenparent(string name)
getid(string name)
resizebps()
resizebp(string name)
mount()
dismount()

*/