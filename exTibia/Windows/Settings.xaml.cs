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
using System.Reflection;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Threading;

using exTibia.Helpers;
using exTibia.Modules;
using exTibia.Objects;

namespace exTibia
{
    public partial class Settings : Window
    {

        private AlertsSafeList windowAlertsSafeList;
        private CbEditItems windowCbEditItems;
        private CbEditWpt windowCbEditWpt;
        private CbSpecialLoc windowCbSpecialLoc;
        private HealerAddEditHealRule windowHealRuleWindow;
        private TargetingSpellOptions windowTargetingSpellOptions;

        public Settings()
        {
            InitializeComponent();
        }

        #region Main Menu

        private void treeViewSettings_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            GC.Collect();

            TreeViewItem treeItem = e.NewValue as TreeViewItem;

            switch (treeItem.Header.ToString())
            {
                #region Main options

                case "Settings":
                    tabMenu.SelectedIndex = 0;
                    break;

                case "Alerts":
                    tabMenu.SelectedIndex = 1;
                    break;

                case "CaveBot":
                    tabMenu.SelectedIndex = 2;
                    #region Loading cavebot fields
                    CavebotSkipNodestxt.Text = CaveBot.Instance.SkipNodes.ToString();
                    CavebotMaxAttemptstxt.Text = CaveBot.Instance.MaxAttempts.ToString();
                    CavebotWalkingMethodCmb.SelectedItem = CaveBot.Instance.WalkingMethod;
                    CavebotWalkFieldsCheckbox.IsChecked = CaveBot.Instance.WalkByFields;
                    CavebotWalkPlayersCheckbox.IsChecked = CaveBot.Instance.WalkByFields;

                    #endregion  
                    break;

                case "Healer":
                    tabMenu.SelectedIndex = 3;
                    break;

                case "Hotkeys":
                    tabMenu.SelectedIndex = 4;
                    break;

                case "Hud":
                    tabMenu.SelectedIndex = 5;
                    break;

                case "Input":
                    tabMenu.SelectedIndex = 6;
                    break;

                case "Looting":
                    tabMenu.SelectedIndex = 7;
                    break;

                case "Targeting":
                    tabMenu.SelectedIndex = 8;
                    #region Loading targeting fields
                    TargetingPriorityHealth.Text = Targeting.Instance.PriorityHealth.ToString();
                    TargetingPriorityDanger.Text = Targeting.Instance.PriorityDanger.ToString();
                    TargetingPriorityOrder.Text = Targeting.Instance.PriorityListOrder.ToString();
                    TargetingPriorityProx.Text = Targeting.Instance.PriorityProximity.ToString();
                    #endregion
                    break;

                case "WarTools":
                    tabMenu.SelectedIndex = 9;
                    break;

                case "Character Info":
                    tabMenu.SelectedIndex = 10;
                    break;

                case "Guild Info":
                    tabMenu.SelectedIndex = 11;
                    break;

                case "Monster Info":
                    tabMenu.SelectedIndex = 12;
                    break;
                case "Item Info":
                    tabMenu.SelectedIndex = 13;
                    break;
                case "Spell Info":
                    tabMenu.SelectedIndex = 14;
                    break;
                case "Who Is Online?":
                    tabMenu.SelectedIndex = 15;
                    break;

                #endregion

                case "":
                    tabMenu.SelectedIndex = 0;
                    break;
            }
        }

        #endregion

        #region Form events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        #endregion

        #region Settings


        #endregion

        #region Healer

        private void ToggleHealer_Unchecked(object sender, RoutedEventArgs e)
        {
            Healer.HealingState = false;
        }

        private void ToggleHealer_Checked(object sender, RoutedEventArgs e)
        {
            Healer.HealingState = true;
        }

        private void HealerNewRule_Click(object sender, RoutedEventArgs e)
        {
            windowHealRuleWindow = new HealerAddEditHealRule();
            windowHealRuleWindow.Owner = this;
            windowHealRuleWindow.ShowDialog();
        }

        private void HealingEditBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var dataContext = button.DataContext;
            HealRule healrule = (HealRule)dataContext;
            windowHealRuleWindow = new HealerAddEditHealRule(healrule);
            windowHealRuleWindow.Owner = this;
            windowHealRuleWindow.ShowDialog();
        }

        private void HealingDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Are you sure you want to delete this heal rule?","ExTibia", System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Question) 
                == System.Windows.Forms.DialogResult.Yes)
            {
                Button button = sender as Button;
                var dataContext = button.DataContext;
                HealRule healrule = (HealRule)dataContext;
                Healer.Instance.HealRules.Remove(healrule);
            }
        }

        private void HealerClean_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Are you sure you want to delete all heal rules?", "ExTibia", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question)
                == System.Windows.Forms.DialogResult.Yes)
            {
                Healer.Instance.HealRules.Clear();
            }
        }

        #endregion

        #region Alerts

        private void AlertsSafeList_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var dataContext = button.DataContext;
            Alarm alarmItem = (Alarm)dataContext;
            windowAlertsSafeList = new AlertsSafeList(alarmItem);
            windowAlertsSafeList.Owner = this;
            windowAlertsSafeList.ShowDialog();
        }

        #endregion

        #region Cavebot

        private void ToggleCavebot_Checked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.State = true;
        }

        private void ToggleCavebot_Unchecked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.State = false;
            if (Walker.Instance.IsWalking)
                Walker.Instance.PauseWalking();
        }

        private void WayListDBClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            ListBoxItem lbItem = sender as ListBoxItem;
            var dataContext = lbItem.DataContext;
            Waypoint data = (Waypoint)dataContext;
            CbEditWpt window = new CbEditWpt(data);
            window.Owner = this;
            window.ShowDialog();
        }

        private void CaveBotAddWpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Location location = Player.Location;

                switch ((Directions)CavebotWptDirection.SelectedItem)
                {
                    case Directions.North:
                        location.Y--;
                        break;
                    case Directions.West:
                        location.X--;
                        break;
                    case Directions.East:
                        location.X++;
                        break;
                    case Directions.South:
                        location.Y++;
                        break;
                    case Directions.NorthEast:
                        location.Y--;
                        location.X++;
                        break;
                    case Directions.NorthWest:
                        location.Y--;
                        location.X--;
                        break;
                    case Directions.SouthEast:
                        location.Y++;
                        location.X++;
                        break;
                    case Directions.SouthWest:
                        location.Y++;
                        location.X--;
                        break;
                }

                Waypoint waypoint = new Waypoint((WaypointType)CavebotWptType.SelectedItem, location, "");

                if (CavebotWptList.SelectedIndex != -1)
                    CaveBot.Instance.WaypointList.Insert(CavebotWptList.SelectedIndex + 1, waypoint);
                else
                    CaveBot.Instance.WaypointList.Add(waypoint);
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void CavebotSetActive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem lbItem = sender as MenuItem;
                var dataContext = lbItem.DataContext;
                Waypoint data = (Waypoint)dataContext;
                CaveBot.Instance.WaypointList.Where(i => i == data).First().Active = true;
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void CavebotEditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem lbItem = sender as MenuItem;
                var dataContext = lbItem.DataContext;
                Waypoint data = (Waypoint)dataContext;
                windowCbEditWpt = new CbEditWpt(data);
                windowCbEditWpt.Owner = this;
                windowCbEditWpt.ShowDialog();
            }
            catch(Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void CavebotDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem lbItem = sender as MenuItem;
                var dataContext = lbItem.DataContext;
                Waypoint data = (Waypoint)dataContext;
                if (MessageBox.Show("Are you sure you want to remove selected waypoint?", "exTibia", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    CaveBot.Instance.WaypointList.Remove(data);
                }
            }
            catch(Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void CavebotDeleteAllBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to remove all waypoints?", "exTibia", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    CaveBot.Instance.WaypointList.Clear();
                }
            }
            catch(Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void CavebotDrawMapCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawMap = true;
        }

        private void CavebotDrawMapCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawMap = false;
        }

        private void CavebotDrawWayCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawWay = true;
        }

        private void CavebotDrawWayCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawWay = false;
        }

        private void SpecialAreaAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SpecialArea newArea = new SpecialArea("NewArea", 0, 0, 0, 0, WalkSender.Walking);
                CaveBot.Instance.SpecialAreasList.Add(newArea);
                windowCbSpecialLoc = new CbSpecialLoc(newArea);
                windowCbSpecialLoc.Owner = this;
                windowCbSpecialLoc.ShowDialog();
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void SpecialAreaEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem lbItem = sender as MenuItem;
                var dataContext = lbItem.DataContext;
                SpecialArea data = (SpecialArea)dataContext;
                CbSpecialLoc window = new CbSpecialLoc(data);
                window.Owner = this;
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void SpecialAreaRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem lbItem = sender as MenuItem;
                var dataContext = lbItem.DataContext;
                SpecialArea data = (SpecialArea)dataContext;
                if (MessageBox.Show("Are you sure you want to remove selected special area?", "exTibia", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    CaveBot.Instance.SpecialAreasList.Remove(data);
                }
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void CavebotWalkPlayersCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.WalkByPlayers = true;
        }

        private void CavebotWalkPlayersCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.WalkByPlayers = false;
        }

        private void CavebotWalkFieldsCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.WalkByFields = true;
        }

        private void CavebotWalkFieldsCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.WalkByFields = false;
        }

        private void CavebotWalkableIdsbtn_Click(object sender, RoutedEventArgs e)
        {
            windowCbEditItems = new CbEditItems(true);
            windowCbEditItems.Owner = this;
            windowCbEditItems.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            windowCbEditItems.ShowDialog();  
        }

        private void CavebotNonWalkableIdsbtn_Click(object sender, RoutedEventArgs e)
        {
            windowCbEditItems = new CbEditItems(false);
            windowCbEditItems.Owner = this;
            windowCbEditItems.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            windowCbEditItems.ShowDialog();
        }

        private void CavebotSkipNodestxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int amount = 0;
                if (int.TryParse(CavebotSkipNodestxt.Text, out amount))
                    if (amount > 0 && amount < 10)
                    {
                        CaveBot.Instance.SkipNodes = amount;
                    }
                    else
                        CavebotSkipNodestxt.Text = CaveBot.Instance.SkipNodes.ToString();
                else
                    CavebotSkipNodestxt.Text = CaveBot.Instance.SkipNodes.ToString();
            }
            catch(InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            } 
        }

        private void CavebotMaxAttemptstxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int amount = 0;
                if (int.TryParse(CavebotSkipNodestxt.Text, out amount))
                    if (amount > 0 && amount < 10)
                    {
                        CaveBot.Instance.MaxAttempts = amount;
                    }
                    else
                        CavebotMaxAttemptstxt.Text = CaveBot.Instance.MaxAttempts.ToString();
                else
                    CavebotMaxAttemptstxt.Text = CaveBot.Instance.MaxAttempts.ToString();


            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }   

        private void CavebotWalkingMethodCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CaveBot.Instance.WalkingMethod = (WalkingMethod)CavebotWalkingMethodCmb.SelectedItem;
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void CavebotDrawingOptions_Click(object sender, RoutedEventArgs e)
        {
            CaveBot.Instance.DrawMapWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            CaveBot.Instance.DrawMapWindow.Owner = this;
            CaveBot.Instance.DrawMapWindow.ShowDialog();
        }

        #endregion

        #region Targeting

        private void ToggleTargeting_Checked(object sender, RoutedEventArgs e)
        {
            Targeting.Instance.State = true;
        }

        private void ToggleTargeting_Unchecked(object sender, RoutedEventArgs e)
        {
            Targeting.Instance.State = false;
        }

        private void TargetingAddMonsterbtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(TargetingAddMonsterTxt.Text))
                {
                    TargetinRule targetingRule = new TargetinRule();
                    targetingRule.MonsterName = TargetingAddMonsterTxt.Text;

                    TargetSetting settingOne = new TargetSetting();

                    settingOne.Name = String.Format("Settings {0}", targetingRule.Settings.Count + 1);

                    targetingRule.Settings.Add(settingOne);

                    Targeting.Instance.TargetingRules.Add(targetingRule);

                    TargetingAddMonsterTxt.Text = "";
                }
            }
            catch
            {

            }
        }

        private void TargetingMonstersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lbItem = sender as ListBox;
            if (lbItem.SelectedItems.Count == 0) return;

            var dataContext = lbItem.SelectedItems[0];
            TargetinRule rule = (TargetinRule)dataContext;
            TargetingSettingsListbox.ItemsSource = rule.Settings;
            TargetingGroupBoxSettings.Header = String.Format("Settings - {0}", rule.MonsterName);
        }

        private void TargetingSettingsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListBox lbItem = sender as ListBox;

                if (e.RemovedItems.Count == 1)
                {
                    var dataContext = e.RemovedItems[0];
                    TargetSetting setting = (TargetSetting)dataContext;

                    try
                    {
                        setting.HpRangeMin = int.Parse(TargetingSettingsHPmin.Text);
                        setting.HpRangeMax = int.Parse(TargetingSettingsHPmax.Text);
                        setting.Danger = int.Parse(TargetingSettingsDangerLevel.Text);
                        setting.Distance = int.Parse(TargetingSettingsDistance.Text);

                        setting.MonsterAttacks = (MonsterAttack)TargetingSettingsMonstersAttack.SelectedItem;
                        setting.AttackMode = (AttackMode)TargetingSettingsAttackMode.SelectedItem;
                        setting.DesiredStance = (DesiredStance)TargetingSettingsDesiredStance.SelectedItem;
                        setting.DesiredAttack = (DesiredAttack)TargetingSettingsDesiredAttack.SelectedItem;

                        setting.Spells[0] = (SpellSetting)TargetingSettingsFirstSpell.SelectedItem;
                        setting.Spells[1] = (SpellSetting)TargetingSettingsSecondSpell.SelectedItem;
                        setting.Spells[2] = (SpellSetting)TargetingSettingsThirdSpell.SelectedItem;
                        setting.Spells[3] = (SpellSetting)TargetingSettingsForthSpell.SelectedItem;
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("An error has been occured. Please check fields in settings.", "ExTibia", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }
                }

                if (e.AddedItems.Count == 1)
                {
                    var dataContext = e.AddedItems[0];
                    TargetSetting setting = (TargetSetting)dataContext;
                    TargetingGroupBoxSettings.Header = String.Format("{0} - {1}", TargetingGroupBoxSettings.Header, setting.Name);
                    TargetingSettingsHPmin.Text = string.Format("{0}", setting.HpRangeMin);
                    TargetingSettingsHPmax.Text = string.Format("{0}", setting.HpRangeMax);
                    TargetingSettingsDangerLevel.Text = string.Format("{0}", setting.Danger);
                    TargetingSettingsMonstersAttack.SelectedItem = setting.MonsterAttacks;
                    TargetingSettingsAttackMode.SelectedItem = setting.AttackMode;
                    TargetingSettingsDesiredStance.SelectedItem = setting.DesiredStance;
                    TargetingSettingsDesiredAttack.SelectedItem = setting.DesiredAttack;
                    TargetingSettingsFirstSpell.SelectedItem = setting.Spells[0];
                    TargetingSettingsSecondSpell.SelectedItem = setting.Spells[1];
                    TargetingSettingsThirdSpell.SelectedItem = setting.Spells[2];
                    TargetingSettingsForthSpell.SelectedItem = setting.Spells[3];
                }


            }
            catch(Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void TargetingAddSettingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(TargetingAddSettingTxt.Text))
                {

                    if (TargetingMonstersListBox.SelectedItems.Count == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Firstly select a monster to which you want to add a setting","ExTibia",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Information);
                        return;
                    }

                    var dataContext = TargetingMonstersListBox.SelectedItems[0];
                    TargetinRule targetingRule = (TargetinRule)dataContext;

                    TargetSetting settingOne = new TargetSetting();
                    settingOne.Name = String.Format("Settings {0}", targetingRule.Settings.Count + 1);
                    targetingRule.Settings.Add(settingOne);
                    TargetingAddSettingTxt.Text = "";
                }
            }
            catch(Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void TargetingRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem lbItem = sender as MenuItem;
                var dataContext = lbItem.DataContext;

                TargetinRule data = (TargetinRule)dataContext;

                if (MessageBox.Show("Are you sure you want to remove selected monster?", "exTibia", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.Yes)
                {
                    Targeting.Instance.TargetingRules.Remove(data);
                    TargetingSettingsListbox.ItemsSource = null;
                }
            }
            catch(Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void TargetingSettingsFirstSpellbtn_Click(object sender, RoutedEventArgs e)
        {
            if ((SpellSetting)TargetingSettingsFirstSpell.SelectedItem != null)
            {
                windowTargetingSpellOptions = new TargetingSpellOptions((SpellSetting)TargetingSettingsFirstSpell.SelectedItem);
                windowTargetingSpellOptions.Owner = this;
                windowTargetingSpellOptions.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                windowTargetingSpellOptions.ShowDialog();
                TargetingSettingsFirstSpell.SelectedItem = windowTargetingSpellOptions.SpellSetting;
            }


        }

        private void TargetingSettingsSecondSpellbtn_Click(object sender, RoutedEventArgs e)
        {
            if ((SpellSetting)TargetingSettingsFirstSpell.SelectedItem != null)
            {
                windowTargetingSpellOptions = new TargetingSpellOptions((SpellSetting)TargetingSettingsSecondSpell.SelectedItem);
                windowTargetingSpellOptions.Owner = this;
                windowTargetingSpellOptions.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                windowTargetingSpellOptions.ShowDialog();
                TargetingSettingsSecondSpell.SelectedItem = windowTargetingSpellOptions.SpellSetting;
            }
        }

        private void TargetingSettingsThirdSpellbtn_Click(object sender, RoutedEventArgs e)
        {
            if ((SpellSetting)TargetingSettingsFirstSpell.SelectedItem != null)
            {
                windowTargetingSpellOptions = new TargetingSpellOptions((SpellSetting)TargetingSettingsThirdSpell.SelectedItem);
                windowTargetingSpellOptions.Owner = this;
                windowTargetingSpellOptions.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                windowTargetingSpellOptions.ShowDialog();
                TargetingSettingsThirdSpell.SelectedItem = windowTargetingSpellOptions.SpellSetting;
            }
        }

        private void TargetingSettingsForthSpellbtn_Click(object sender, RoutedEventArgs e)
        {

            if ((SpellSetting)TargetingSettingsFirstSpell.SelectedItem != null)
            {
                windowTargetingSpellOptions = new TargetingSpellOptions((SpellSetting)TargetingSettingsForthSpell.SelectedItem);
                windowTargetingSpellOptions.Owner = this;
                windowTargetingSpellOptions.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                windowTargetingSpellOptions.ShowDialog();
                TargetingSettingsForthSpell.SelectedItem = windowTargetingSpellOptions.SpellSetting;
            }
        }

        private void TargetingRemoveClick(object sender, RoutedEventArgs e)
        {

        }

        private void TargetingPriorityHealth_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int value = 0;
                if (int.TryParse(TargetingPriorityHealth.Text, out value))
                    if (value > 0 && value < 100)
                    {
                        Targeting.Instance.PriorityHealth = value;
                    }
                    else
                        TargetingPriorityHealth.Text = Targeting.Instance.PriorityHealth.ToString();
                else
                    TargetingPriorityHealth.Text = Targeting.Instance.PriorityHealth.ToString();
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void TargetingPriorityDanger_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int value = 0;
                if (int.TryParse(TargetingPriorityDanger.Text, out value))
                    if (value > 0 && value < 100)
                    {
                        Targeting.Instance.PriorityDanger = value;
                    }
                    else
                        TargetingPriorityDanger.Text = Targeting.Instance.PriorityDanger.ToString();
                else
                    TargetingPriorityDanger.Text = Targeting.Instance.PriorityDanger.ToString();
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void TargetingPriorityOrder_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int value = 0;
                if (int.TryParse(TargetingPriorityOrder.Text, out value))
                    if (value > 0 && value < 100)
                    {
                        Targeting.Instance.PriorityListOrder = value;
                    }
                    else
                        TargetingPriorityOrder.Text = Targeting.Instance.PriorityListOrder.ToString();
                else
                    TargetingPriorityOrder.Text = Targeting.Instance.PriorityListOrder.ToString();
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        private void TargetingPriorityProx_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int value = 0;
                if (int.TryParse(TargetingPriorityProx.Text, out value))
                    if (value > 0 && value < 100)
                    {
                        Targeting.Instance.PriorityProximity = value;
                    }
                    else
                        TargetingPriorityProx.Text = Targeting.Instance.PriorityProximity.ToString();
                else
                    TargetingPriorityProx.Text = Targeting.Instance.PriorityProximity.ToString();
            }
            catch (Exception ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        #endregion

        #region Character Info

        private void CharInfoFetch_Click(object sender, RoutedEventArgs e)
        {
            Web.Player.LookupPlayer(CharInfoCharacterName.Text, new Web.Player.LookupReceived(CharInfoProcess));
            Web.Player.LookupTime(CharInfoCharacterName.Text, new Web.Player.LookupTimeReceived(CharInfoTimes));
        }

        private void CharInfoTimes(Web.Player.LastOnlineTimes times)
        {
            if (times != null)
            {
                int index = times.LastTimes.IndexOf("\n\t\tLAST   WEEK\t\t\t\t");

                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    CharTimeAgoSummary.Text = times.LastTimes[index + 2]; // week
                    CharTimeToday.Text = times.LastTimes[index + 10];
                    CharTimeAgo1.Text = times.LastTimes[index + 9];
                    CharTimeAgo2.Text = times.LastTimes[index + 8];
                    CharTimeAgo3.Text = times.LastTimes[index + 7];
                    CharTimeAgo4.Text = times.LastTimes[index + 6];
                    CharTimeAgo5.Text = times.LastTimes[index + 5];
                    CharTimeAgo6.Text = times.LastTimes[index + 4];
                    CharTimeAgo7.Text = times.LastTimes[index + 3];

                }));
            }
        }

        private void CharInfoProcess(Web.Player.CharInfo player)
        {
            if (player != null)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        CharInfoSex.Text = player.Sex;
                        CharInfoVocation.Text = player.Profession;
                        CharInfoLevel.Text = player.Level.ToString();
                        CharInfoAchievements.Text = player.Achievements;
                        CharInfoWorld.Text = player.World;
                        CharInfoResidence.Text = player.Residence;
                        CharInfoLastLogin.Text = player.LastLogin;
                        CharInfoAccountStatus.Text = player.AccountStatus;
                    }));
            }
        }

        #endregion

    }
}
