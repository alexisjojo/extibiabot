using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia.Helpers
{
    public static class Debug
    {
        public static void Report(Exception e)
        {
            string exception = e.ToString() + "\n\n";
            string question = "";

            string finaltext = exception + question;

            if (MessageBox.Show(finaltext, "", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {

            }	 
        }

        public static void Dump(bool player)
        {
            #region Player

            if (player)
            {
                Trace.WriteLine(String.Format("Player.Name:  {0}", Player.Name));
                Trace.WriteLine(String.Format("Player.Experience:  {0}", Player.Experience));
                Trace.WriteLine(String.Format("Player.ID:  {0}", Player.ID));
                Trace.WriteLine(String.Format("Player.Health:  {0}", Player.Health));
                Trace.WriteLine(String.Format("Player.HealthMax:  {0}", Player.HealthMax));
                Trace.WriteLine(String.Format("Player.Mana:  {0}", Player.Mana));
                Trace.WriteLine(String.Format("Player.ManaMax:  {0}", Player.ManaMax));
                Trace.WriteLine(String.Format("Player.Soul:  {0}", Player.Soul));
                Trace.WriteLine(String.Format("Player.Cap:  {0}", Player.Cap));
                Trace.WriteLine(String.Format("Player.X:  {0}", Player.X));
                Trace.WriteLine(String.Format("Player.Y:  {0}", Player.Y));
                Trace.WriteLine(String.Format("Player.Z:  {0}", Player.Z));
            }

            #endregion
        }

        public static void WriteLine(string debugText, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(String.Format(exTibia.Resources.Resources.DebugMessage, DateTime.Now.ToShortTimeString(), debugText));
        }
    }
}
