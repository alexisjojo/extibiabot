using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    /// <summary>
    /// Interaction logic for CbEditWpt.xaml
    /// </summary>
    public partial class CbEditWpt : Window
    {
        private Waypoint waypoint;

        public CbEditWpt()
        {
            InitializeComponent();
        }

        public CbEditWpt(Waypoint waypoint)
        {
            InitializeComponent();

            this.waypoint = waypoint;

            OnLoad();
        }

        void OnLoad()
        {
            txtPosX.Text = waypoint.Location.X.ToString();
            txtPosY.Text = waypoint.Location.Y.ToString();
            txtPosZ.Text = waypoint.Location.Z.ToString();
            if (waypoint.WaypointType == WaypointType.Action)
            {
                txtAction.Text = waypoint.ScriptAction;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
