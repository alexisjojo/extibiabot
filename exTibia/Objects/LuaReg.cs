using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using exTibia.Helpers;
using exTibia.Modules;

using LuaInterface;

namespace exTibia.Objects
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class LuaGlobalAttribute : Attribute
    {
        public string Name { get; set; }
        public string Params { get; set; }
        public string Description { get; set; }

    }

    public static class LuaReg
    {
        public static void RegisterBotFunctions(Lua lua, object o)
        {
            #region Sanity checks
            if (lua == null) throw new ArgumentNullException("lua");
            if (o == null) throw new ArgumentNullException("o");
            #endregion

            foreach (MethodInfo method in o.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                foreach (LuaGlobalAttribute attribute in method.GetCustomAttributes(typeof(LuaGlobalAttribute), true))
                {
                    if (string.IsNullOrEmpty(attribute.Name))
                        lua.RegisterFunction(method.Name, o, method); // CLR name
                    else
                        lua.RegisterFunction(attribute.Name, o, method); // Custom name
                }
            }
        }

        public static void RegisterBotFunctionsStatic(Lua lua, Type type)
        {
            #region Sanity checks
            if (lua == null) throw new ArgumentNullException("lua");
            if (type == null) throw new ArgumentNullException("type");
            if (!type.IsClass) throw new ArgumentException("The type must be a class!", "type");
            #endregion

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                foreach (LuaGlobalAttribute attribute in method.GetCustomAttributes(typeof(LuaGlobalAttribute), false))
                {
                    if (string.IsNullOrEmpty(attribute.Name))
                        lua.RegisterFunction(method.Name, null, method); // CLR name
                    else
                        lua.RegisterFunction(attribute.Name, null, method); // Custom name
                }
            }
        }

        public static void RegisterBotVariables(Lua lua)
        {
            try
            {
                lua["hp"] = Player.Mana;
                lua["wpt"] = CaveBot.Instance.WaypointList;
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }


        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type parameter is used to select an enum type")]
        public static void Enumeration<T>(Lua lua)
        {
            #region Sanity checks
            if (lua == null) throw new ArgumentNullException("lua");
            #endregion

            Type type = typeof(T);
            if (!type.IsEnum) throw new ArgumentException("The type must be an enumeration!");

            string[] names = Enum.GetNames(type);
            var values = (T[])Enum.GetValues(type);

            lua.NewTable(type.Name);
            for (int i = 0; i < names.Length; i++)
            {
                string path = type.Name + "." + names[i];
                lua[path] = values[i];
            }

        }
    }
}
