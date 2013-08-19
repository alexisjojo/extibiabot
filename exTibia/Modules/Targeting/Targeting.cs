using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading;
using System.Security.Permissions;

using exTibia.Helpers;
using exTibia.Objects;

namespace exTibia.Modules
{
    public class Targeting
    {
        #region Singleton

        static readonly Targeting _instance = new Targeting();

        public static Targeting Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Delegates and events

        public event EventHandler<TargetingEventArgs> TargetingRuleMatch;
        public event EventHandler<TargetingEventArgs> TargetingRuleExecute;
        public event EventHandler<TargetingEventArgs> TargetingRuleExecuted;

        protected virtual void OnTargetingRuleMatch(TargetingEventArgs e)
        {
            if (TargetingRuleMatch != null)
            {
                TargetingRuleMatch(this, e);
            }
        }

        protected virtual void OnTargetingRuleExecute(TargetingEventArgs e)
        {
            if (TargetingRuleExecute != null)
            {
                TargetingRuleExecute(this, e);
            }
        }

        protected virtual void OnTargetingRuleExecuted(TargetingEventArgs e)
        {
            if (TargetingRuleExecuted != null)
            {
                TargetingRuleExecuted(this, e);
            }
        }

        #endregion

        #region Fields

        private ObservableCollection<TargetinRule> _targetingRules = new ObservableCollection<TargetinRule>();
        private ObservableCollection<TargetSetting> _ruleSettings = new ObservableCollection<TargetSetting>();
        private TargetingState _targetState;

        private bool _state = false;
        private Thread _targetingThread;
        private TargetinRule _TargetingRule;
        private bool _hasActiveRule = false;

        private Collection<KeyValuePair<int, TargetPriority>> _priorities = new Collection<KeyValuePair<int, TargetPriority>>();

        private int _PriorityHealth = 80;
        private int _PriorityProximity = 85;
        private int _PriorityDanger = 95;
        private int _PriorityListOrder = 55;

        #endregion

        #region Properties

        public ObservableCollection<TargetinRule> TargetingRules
        {
            get { return _targetingRules; }
        }

        public ObservableCollection<TargetSetting> RuleSettings
        {
            get 
            {
                if (TargetingRules.Where(t => t.Selected).Count() == 1)
                {
                    return TargetingRules.Where(t => t.Selected).First().Settings;
                }

                return _ruleSettings; 
            }
        }

        public Thread TargetingThread
        {
            get { return _targetingThread; }
            set { _targetingThread = value; }
        }

        public Collection<KeyValuePair<int, TargetPriority>> Priorities
        {
            get { return _priorities; }
        }

        public bool State
        {
            get { return _state; }
            set { _state = value; }
        }

        public TargetinRule CurrentTargetingRule
        {
            get { return _TargetingRule; }
            set { _TargetingRule = value; }
        }

        public TargetingState TargetState
        {
            get { return _targetState; }
            set { _targetState = value; }
        }

        public bool HasActiveRule
        {
            get { return _hasActiveRule; }
            set { _hasActiveRule = value; }
        }

        public int PriorityHealth
        {
            get { return _PriorityHealth; }
            set { _PriorityHealth = value; }
        }

        public int PriorityProximity
        {
            get { return _PriorityProximity; }
            set { _PriorityProximity = value; }
        }

        public int PriorityDanger
        {
            get { return _PriorityDanger; }
            set { _PriorityDanger = value; }
        }

        public int PriorityListOrder
        {
            get { return _PriorityListOrder; }
            set { _PriorityListOrder = value; }
        }

        #endregion

        #region Constructor

        public Targeting()
        {
            TargetingRuleExecute += new EventHandler<TargetingEventArgs>(Targeting_TargetingRuleExecute);
            TargetingRuleExecuted += new EventHandler<TargetingEventArgs>(Targeting_TargetingRuleExecuted);
            TargetingRuleMatch += new EventHandler<TargetingEventArgs>(Targeting_TargetingRuleMatch);

            TargetingThread = new Thread(RunTarget);
            TargetingThread.Start();
        }

        #endregion

        #region Methods

        public void Init()
        {
            Priorities.Add(new KeyValuePair<int, TargetPriority>(this.PriorityDanger, TargetPriority.Danger));
            Priorities.Add(new KeyValuePair<int, TargetPriority>(this.PriorityHealth, TargetPriority.Health));
            Priorities.Add(new KeyValuePair<int, TargetPriority>(this.PriorityProximity, TargetPriority.Proximity));
            Priorities.Add(new KeyValuePair<int, TargetPriority>(this.PriorityListOrder, TargetPriority.ListOrder));

            Debug.WriteLine("Instance of Targeting() has been created.", ConsoleColor.Yellow);
        }

        private void RunTarget()
        {
        execute:
            try
            {
                while (true)
                {
                    Collection<Creature> creatures = new Collection<Creature>();
                    Collection<Creature> creaturesMatching = new Collection<Creature>();
                    List<TargetinRule> rulesMatching = new List<TargetinRule>();

                    while (State)
                    {

                        Thread.Sleep(500);

                        TargetState = TargetingState.Running;

                        if (TargetingRules.Count == 0)
                            continue;

                        if (!Queues.HasTask(Task.TargetingExecuteRule))
                            SetActiveRule(false);

                        if (HasActiveRule)
                            continue;

                        creatures = Creature.CreaturesInBw();

                        if (creatures.Count == 0)
                            continue;

                        rulesMatching = new List<TargetinRule>();

                        int settingID = -1;

                        foreach (TargetinRule rule in TargetingRules)
                            if (RuleMatch(ref creatures, rule, out settingID))
                            {
                                rule.SettingID = settingID;
                                rulesMatching.Add(rule);
                                settingID = -1;
                            }

                        if (rulesMatching.Count == 0)
                            continue;

                        if (!State)
                            break;

                        creaturesMatching.Clear();

                        foreach (Creature c in creatures)
                            if (c.Name.ToLower() == rulesMatching[0].MonsterName.ToLower())
                                creaturesMatching.Add(c);

                        OnTargetingRuleMatch(new TargetingEventArgs(rulesMatching[0], creaturesMatching));

                    }
                    Thread.Sleep(100);
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
                goto execute;
            }
        }

        public bool RuleMatch(ref Collection<Creature> creatures, TargetinRule targetingRule, out int settingID)
        {
            settingID = -1;

            try
            {

                TargetState = TargetingState.CheckingRule;

                if (creatures.Where(c => c.Name.ToLower() == targetingRule.MonsterName.ToLower()).Count() == 0)
                    return false;

                if (creatures.Where(c => c.Name.ToLower() == targetingRule.MonsterName.ToLower()).Count() < targetingRule.Count)
                    return false;

                List<Creature> creaturesToCheck = creatures.Where(c => c.Name.ToLower() == targetingRule.MonsterName.ToLower()).ToList();

                foreach (TargetSetting t in targetingRule.Settings)
                {
                    if (creaturesToCheck.Any(c => c.HPBar >= t.HpRangeMin && c.HPBar <= t.HpRangeMax))
                    {
                        settingID = targetingRule.Settings.IndexOf(t);
                        break;
                    }
                }
                if (settingID == -1)
                    return false;

                return true;
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }

            return false;
        }

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void ExecuteRule(Creature creature, TargetinRule targetingRule, int settingID)
        {
            OnTargetingRuleExecute(new TargetingEventArgs(null, creature));

            Targeter.Instance.Target = creature;
            Targeter.Instance.Rule = targetingRule.Settings[settingID];
            Targeter.Instance.Attack();  
        }

        public void RuleExecute()
        {
            OnTargetingRuleExecute(new TargetingEventArgs());
        }

        public void RuleExecuted()
        {
            OnTargetingRuleExecuted(new TargetingEventArgs());
        }

        private void SetActiveRule(bool value)
        {
            HasActiveRule = value;
        }

        public bool InTargeting(string monsterName)
        {
            return TargetingRules.Any(r => r.MonsterName.ToLower() == monsterName.ToLower());
        }

        public IComparer<Creature> GetComparer(int i)
        {
            try
            {
                switch (Priorities.OrderByDescending(k => k.Key).ElementAt(i).Value)
                {
                    case TargetPriority.Health:
                        return new CreatureSortByHealth();
                    case TargetPriority.Danger:
                        return new CreatureSortByDanger();
                    case TargetPriority.ListOrder:
                        return new CreatureSortByListOrder();
                    case TargetPriority.Proximity:
                        return new CreatureSortProximity();
                }
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }

            return null;
        }

        #endregion

        #region Events

        void Targeting_TargetingRuleMatch(object sender, TargetingEventArgs e)
        {

            if (Targeter.Instance.IsAttacking)
                return;
            
            List<Creature> ordered = new List<Creature>();

            ordered = e.Creatures.OrderBy(c => c, GetComparer(0)).ThenBy(c => c, GetComparer(1))
                .ThenBy(c => c, GetComparer(2)).ThenBy(c => c, GetComparer(3)).ToList();

            CaveBot.Stop();

            Queues.Add(Task.TargetingExecuteRule, new Action(() =>
            {
                ExecuteRule(ordered[0], e.TargetingRule, e.TargetingRule.SettingID);
            }), 95, 100, false, false);
        }

        void Targeting_TargetingRuleExecuted(object sender, TargetingEventArgs e)
        {
            try
            {
                SetActiveRule(false);
            }
            catch (InvalidOperationException ex)
            {
                Helpers.Debug.Report(ex);
            }
        }

        void Targeting_TargetingRuleExecute(object sender, TargetingEventArgs e)
        {
            SetActiveRule(true);
        }

        #endregion
    }
}


/*
10:#FF00000
9: #FF3300
8: #FF6600
7: #FF9900
6: #FFCC00
5: #FFFF00
4: #CCFF00
3: #99FF00
2: #33FF00
1: #00CC00
*/
