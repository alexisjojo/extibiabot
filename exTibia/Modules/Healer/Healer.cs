using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Security.Permissions;
using System.Diagnostics;
using System.Security;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class Healer
    {
        #region Singleton

        private static Healer _instance = new Healer();

        public static Healer Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Constructor

        public Healer()
        {
            HealingThread = new Thread(Heal);
            HealingThread.Start();
        }

        #endregion

        #region Fields

        private ObservableCollection<HealRule> _HealRules = new ObservableCollection<HealRule>();
        private Thread HealingThread;
        private static bool _HealingState = false;
        private int _HealingPauseTime = -1;
        private HealerState _healerState;

        #endregion

        #region Properties

        public HealerState HealState
        {
            get { return _healerState; }
            set { _healerState = value; }
        }

        public static bool HealingState
        {
            get { return Healer._HealingState; }
            set { Healer._HealingState = value; }
        }

        public ObservableCollection<HealRule> HealRules
        {
            get { return _HealRules; }
        }      

        #endregion

        #region Methods

        public void Heal()
        {
            while (true)
            {
                while (_HealingState)
                {
                    if (_HealingPauseTime != -1 && _HealingPauseTime != 0)
                    {
                        Thread.Sleep(_HealingPauseTime);
                        _HealingPauseTime = -1;
                    }

                    if (HealRules.Count > 0)
                    {
                        foreach (HealRule rule in HealRules.OrderByDescending(i => i.priority))
                        {
                            if (Match(rule))
                            {
                                HealRule healrule = rule;

                                Queues.Add(new Action(() =>
                                    {
                                        Execute(healrule);                                       
                                    }), healrule.priority, healrule.lifetime, true, false);                                                              
                            }
                            Thread.Sleep(25);
                        }
                    }
                    Thread.Sleep(25);
                }
                Thread.Sleep(125);
            }
        }

        public bool Match(HealRule rule)
        {
            HealState = HealerState.CheckingRule;

            if (rule.Additionals != null)
            {
                List<HealerAdditionalCondition> conditions = new List<HealerAdditionalCondition>();
                conditions = rule.Additionals.Where(c => c.Active).ToList();
                
                if (conditions.Count > 0)
                {
                    foreach (HealerAdditionalCondition condition in conditions)
                    {
                        if (!MatchCondition(condition)) return false;
                    }
                }
            }

            bool matchhp = false;
            bool matchmp = false;

            switch (rule.HPRange)
            {
                case HealerRangeType.Exact:
                    matchhp = (Player.Health > rule.HP_MIN && Player.Health < rule.HP_MAX) ? true : false;
                    break;

                case HealerRangeType.Percent:
                    matchhp = (Player.HealthPercent > rule.HP_MIN && Player.HealthPercent < rule.HP_MAX) ? true : false;
                    break;
            }

            switch (rule.MPRange)
            {
                case HealerRangeType.Exact:
                    matchmp = (Player.Mana > rule.MP_MIN && Player.Mana < rule.MP_MAX) ? true : false;
                    break;

                case HealerRangeType.Percent:
                    matchmp = (Player.ManaPercent > rule.MP_MIN && Player.ManaPercent < rule.MP_MAX) ? true : false;
                    break;
            }

            return (matchhp && matchmp);
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Execute(HealRule rule)
        {
            HealState = HealerState.ExecuteRule;

            bool IsSpell = (Spells.Instance.FindSpell(rule.Item.Name) != null) ? true : false;

            if (IsSpell)
            {
                if (!new Cooldown().CanCast(rule.Item.Name)) return;
                if (HotkeysGame.FindSpell(rule.Item.Name) != 0) 
                {
                    InputControl.UseHot(HotkeysGame.FindSpell(rule.Item.Name)); 
                }
                Thread.Sleep(new Random().Next(rule.DL_MIN,rule.DL_MAX));
            }
            else
            {
                int itemID = (Items.FindByName(rule.Item.Name).ItemID);
                if (HotkeysGame.FindItem(itemID) != 0) 
                { 
                    InputControl.UseHot(HotkeysGame.FindItem(itemID)); 
                }
                Thread.Sleep(new Random().Next(rule.DL_MIN, rule.DL_MAX));
            }           
        }

        public static bool MatchCondition(HealerAdditionalCondition condition)
        {
            if (condition.Condition == HealerAdditional.Burned)
                return condition.Active && Player.IsBurned;

            if (condition.Condition == HealerAdditional.Drunken)
                return condition.Active && Player.IsDrunken;

            if (condition.Condition == HealerAdditional.Energized)
                return condition.Active && Player.IsEnergized;

            if (condition.Condition == HealerAdditional.Hasted)
                return condition.Active && Player.IsHasted;

            if (condition.Condition == HealerAdditional.InsidePz)
                return condition.Active && Player.IsPz;

            if (condition.Condition == HealerAdditional.ManaShield)
                return condition.Active && Player.IsManaShield;

            if (condition.Condition == HealerAdditional.Paralyzed)
                return condition.Active && Player.IsParalyzed;

            if (condition.Condition == HealerAdditional.Poisoned)
                return condition.Active && Player.IsPoisoned;

            if (condition.Condition == HealerAdditional.Pvpsigned)
                return condition.Active && Player.IsPvpSigned;

            if (condition.Condition == HealerAdditional.Strengthened)
                return condition.Active && Player.IsStrengthened;
            return true;
        }

        public void Init()
        {
            Helpers.Debug.WriteLine("Instance of Healer() has been created.", ConsoleColor.Yellow);
        }

        #endregion

        #region Load / Save settings
    
        public static string Serialize()
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            return oSerializer.Serialize(Instance.HealRules);
        }

        public static void DeSerialize(string settings)
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            ObservableCollection<HealRule> converted = oSerializer.Deserialize<ObservableCollection<HealRule>>(settings);
          
            foreach (HealRule rule in converted)
            {
                Instance.HealRules.Add(rule);
            }
        }

        #endregion
    }
}
