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

using exTibia.Objects;
using exTibia.Modules;

namespace exTibia
{
    /// <summary>
    /// Interaction logic for TargetingSpellOptions.xaml
    /// </summary>
    public partial class TargetingSpellOptions : Window
    {
        private SpellSetting _spellSetting = new SpellSetting();

        public SpellSetting SpellSetting
        {
            get { return _spellSetting; }
            set { _spellSetting = value; }
        }

        public TargetingSpellOptions()
        {
            InitializeComponent();
        }

        public TargetingSpellOptions(SpellSetting setting)
        {
            InitializeComponent();

            TargetingSpellName.Text = setting.Spell;
            TargetingSettingsHPmin.Text = setting.DelayMin.ToString();
            TargetingSettingsHPmax.Text = setting.DelayMax.ToString();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
