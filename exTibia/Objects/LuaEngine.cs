using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using exTibia.Helpers;
using exTibia.Modules;

using LuaInterface;

namespace exTibia.Objects
{
    public class LuaEngine
    {

        #region Singleton

        private static LuaEngine _instance = new LuaEngine();

        public static LuaEngine Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Fields

        public static Lua pLuaVM = null;

        #endregion

        #region Constructor

        public LuaEngine()
        {
            pLuaVM = new Lua();
            pLuaVM.DebugHook += new EventHandler<DebugHookEventArgs>(pLuaVM_DebugHook);
            
            LuaReg.RegisterBotFunctions(pLuaVM, LuaFunctions.Instance);
            LuaReg.RegisterBotVariables(pLuaVM);
            pLuaVM.DoString("say(getlocation().X)");

        }

        #endregion

        #region DebugHook

        void pLuaVM_DebugHook(object sender, DebugHookEventArgs e)
        {
            //do zrobienia

        }

        #endregion
    }
}
