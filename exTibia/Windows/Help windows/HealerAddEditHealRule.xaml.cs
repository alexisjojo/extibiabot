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
using System.Collections.ObjectModel;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    /// <summary>
    /// Interaction logic for AddEditHealRule.xaml
    /// </summary>
    public partial class HealerAddEditHealRule : Window
    {

        private HealRule _healRule;

        public HealerAddEditHealRule()
        {
            InitializeComponent();
            cbAddCondition.ItemsSource = Consts.AdditionalConditions;
            btnAdd.Visibility = System.Windows.Visibility.Visible;
            btnSave.Visibility = System.Windows.Visibility.Hidden;
        }

        public HealerAddEditHealRule(HealRule healRule)
        {
            InitializeComponent();

            if (healRule != null)
            {
                this._healRule = healRule;
                btnAdd.Visibility = System.Windows.Visibility.Hidden;
                btnSave.Visibility = System.Windows.Visibility.Visible;
                FillFields();
            }
        }

        public void FillFields()
        {
            cbHealItem.SelectedIndex = cbHealItem.Items.IndexOf(Consts.HealItems.First(i => i.Name.ToLower() == _healRule.Item.Name.ToLower()));

            txHPrangeMin.Text = String.Format("{0}", _healRule.HP_MIN);
            txHPrangeMax.Text = String.Format("{0}", _healRule.HP_MAX);

            txMPrangeMin.Text = String.Format("{0}", _healRule.MP_MIN);
            txMPrangeMax.Text = String.Format("{0}", _healRule.MP_MAX);

            txDelayMin.Text = String.Format("{0}", _healRule.DL_MIN);
            txDelayMax.Text = String.Format("{0}", _healRule.DL_MAX);

            txPriority.Text = String.Format("{0}", _healRule.priority);
            txLifeTime.Text = String.Format("{0}", _healRule.lifetime);

            rbHPexa.IsChecked = (_healRule.HPRange == HealerRangeType.Exact) ? true : false;
            rbHPper.IsChecked = (_healRule.HPRange == HealerRangeType.Percent) ? true : false;
            rbMPexa.IsChecked = (_healRule.MPRange == HealerRangeType.Exact) ? true : false;
            rbMPper.IsChecked = (_healRule.MPRange == HealerRangeType.Percent) ? true : false;

            foreach (HealerAdditionalCondition condition in Consts.AdditionalConditions)
            {
                HealerAdditionalCondition item = condition;
                if (_healRule.Additionals.Where(c => c.TypeName == item.TypeName).Count() > 0)
                    item.Active = _healRule.Additionals.FirstOrDefault(c => c.TypeName == condition.TypeName).Active;
                else
                    item.Active = false;
                cbAddCondition.Items.Add(item);
            }
        }

        public bool ValidiateFields()
        {
            if (cbHealItem.SelectedIndex == -1)
            {
                System.Windows.Forms.MessageBox.Show("Please select item or spell to use.");
                return false;
            }

            if (String.IsNullOrEmpty(txHPrangeMin.Text) || !txHPrangeMin.Text.All(char.IsDigit) ||
                String.IsNullOrEmpty(txHPrangeMax.Text) || !txHPrangeMax.Text.All(char.IsDigit))
            {
                System.Windows.Forms.MessageBox.Show("Incorrect value in health range fields.");
                return false;
            }

            if (String.IsNullOrEmpty(txMPrangeMin.Text) || !txMPrangeMin.Text.All(char.IsDigit) ||
                String.IsNullOrEmpty(txMPrangeMax.Text) || !txMPrangeMax.Text.All(char.IsDigit))
            {
                System.Windows.Forms.MessageBox.Show("Incorrect value in mana range fields.");
                return false;
            }

            if (String.IsNullOrEmpty(txPriority.Text) || !txPriority.Text.All(char.IsDigit))
            {
                System.Windows.Forms.MessageBox.Show("Incorrect value in priority field.");
                return false;
            }

            if (String.IsNullOrEmpty(txLifeTime.Text) || !txLifeTime.Text.All(char.IsDigit))
            {
                System.Windows.Forms.MessageBox.Show("Incorrect value in lifetime field.");
                return false;
            }

            if (rbHPexa.IsChecked == false && rbHPper.IsChecked == false)
            {
                System.Windows.Forms.MessageBox.Show("Please specify health range type.");
                return false;
            }

            if (rbMPexa.IsChecked == false && rbMPper.IsChecked == false)
            {
                System.Windows.Forms.MessageBox.Show("Please specify mana range type.");
                return false;
            }

            return true;

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ValidiateFields())
            {
                HealRule newrule = new HealRule();

                newrule.Item = (HealItem)cbHealItem.SelectedItem;
                newrule.HP_MIN = Convert.ToInt32(txHPrangeMin.Text);
                newrule.HP_MAX = Convert.ToInt32(txHPrangeMax.Text);
                newrule.MP_MIN = Convert.ToInt32(txMPrangeMin.Text);
                newrule.MP_MAX = Convert.ToInt32(txMPrangeMax.Text);
                newrule.DL_MIN = Convert.ToInt32(txDelayMin.Text);
                newrule.DL_MAX = Convert.ToInt32(txDelayMax.Text);
                newrule.lifetime = Convert.ToInt32(txLifeTime.Text);
                newrule.priority = Convert.ToInt32(txPriority.Text);

                if (rbHPexa.IsChecked == true)
                    newrule.HPRange = HealerRangeType.Exact;
                else
                    newrule.HPRange = HealerRangeType.Percent;

                if (rbMPexa.IsChecked == true)
                    newrule.MPRange = HealerRangeType.Exact;
                else
                    newrule.MPRange = HealerRangeType.Percent;

                foreach (HealerAdditionalCondition condition in cbAddCondition.Items)
                    newrule.Additionals.Add(condition);

                Healer.Instance.HealRules.Add(newrule);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidiateFields())
            {
                Healer.Instance.HealRules.Remove(_healRule);
                HealRule newrule = new HealRule();

                newrule.Item = (HealItem)cbHealItem.SelectedItem;
                newrule.HP_MIN = Convert.ToInt32(txHPrangeMin.Text);
                newrule.HP_MAX = Convert.ToInt32(txHPrangeMax.Text);
                newrule.MP_MIN = Convert.ToInt32(txMPrangeMin.Text);
                newrule.MP_MAX = Convert.ToInt32(txMPrangeMax.Text);
                newrule.DL_MIN = Convert.ToInt32(txDelayMin.Text);
                newrule.DL_MAX = Convert.ToInt32(txDelayMax.Text);
                newrule.lifetime = Convert.ToInt32(txLifeTime.Text);
                newrule.priority = Convert.ToInt32(txPriority.Text);

                if (rbHPexa.IsChecked == true)
                    newrule.HPRange = HealerRangeType.Exact;
                else
                    newrule.HPRange = HealerRangeType.Percent;

                if (rbMPexa.IsChecked == true)
                    newrule.MPRange = HealerRangeType.Exact;
                else
                    newrule.MPRange = HealerRangeType.Percent;

                foreach (HealerAdditionalCondition condition in cbAddCondition.Items)
                    newrule.Additionals.Add(condition);

                Healer.Instance.HealRules.Add(newrule);
                this.Hide();
            }
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
