using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;

namespace Villeon.Helper
{
    [Serializable]
    public class Stats
    {
        private static Stats? _instance;

        // Level
        private int _level = 1;
        private long _experience = 0;
        private long _requiredExperience = 50;

        // Player Stats
        private int _healthLevel = 1;
        private int _damageLevel = 1;
        private int _defenseLevel = 1;
        private int _itemAttack = 0;
        private int _itemDefense = 0;

        private string _itemAttackName;
        private string _itemDefenseName;

        // Progress
        private int _progress = 0;

        // NPC Unlocks
        private List<int> _unlockProgress = new List<int>() { 0, 0, 0, 0 };

        private Stats()
        {
        }

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public long Experience
        {
            get { return _experience; }
            set { _experience = value; }
        }

        public long RequiredExperience
        {
            get { return _requiredExperience; }
            set { _requiredExperience = value; }
        }

        public int HealthLevel
        {
            get { return _healthLevel; }
            set { _healthLevel = value; }
        }

        public int AttackLevel
        {
            get { return _damageLevel; }
            set { _damageLevel = value; }
        }

        public int DefenseLevel
        {
            get { return _defenseLevel; }
            set { _defenseLevel = value; }
        }

        public int Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public List<int> UnlockProgress
        {
            get { return _unlockProgress; }
            set { _unlockProgress = value; }
        }

        public int ItemDamage
        {
            get => _itemAttack;
        }

        public int ItemDefense
        {
            get => _itemDefense;
        }

        public string ItemDamageName
        {
            get => _itemAttackName;
        }

        public string ItemDefenseName
        {
            get => _itemDefenseName;
        }

        public static Stats GetInstance()
        {
            if (_instance == null)
                _instance = new Stats();
            return _instance;
        }

        public void SetAttackItem(Item item)
        {
            _itemAttackName = item.Name;
            _itemAttack = item.Damage;
        }

        public void SetDefenseItem(Item item)
        {
            _itemDefenseName = item.Name;
            _itemDefense = item.Defense;
        }

        // Gain experience
        public void GainExperience(int exp)
        {
            _experience += exp;

            while (_experience > _requiredExperience)
            {
                _progress = 1;
                _level++;
                _experience -= _requiredExperience;
                _requiredExperience = RequiredExperienceFunction();
            }
        }

        public void IncreaseHealth() => _healthLevel++;

        public void IncreaseDamage() => _damageLevel++;

        public void IncreaseDefense() => _defenseLevel++;

        public int GetMaxHealth()
        {
            return (int)(StatFunction(_level) * 150) + (int)(StatFunction(_healthLevel) * 150);
        }

        public int GetAttack()
        {
            return (int)(StatFunction(_level) * 30) + (int)(StatFunction(_damageLevel) * 30) + _itemAttack;
        }

        public int GetDefense()
        {
            float defense = StatDefense(_level + _defenseLevel) + _itemDefense;
            return defense > 80f ? 80 : (int)defense;
        }

        public int GetUnlockProgress(int caveIndex)
        {
            return _unlockProgress[caveIndex];
        }

        public int IncreaseUnlockProgress(int caveIndex)
        {
            if (_unlockProgress[caveIndex] < Constants.MAXNPCUNLOCKS)
            {
                _unlockProgress[caveIndex]++;
            }

            return _unlockProgress[caveIndex];
        }

        // Function for level exp requirements
        private int RequiredExperienceFunction()
        {
            return (int)Math.Floor((0.02f * (float)Math.Pow(_level, 3)) + (3.06f * (float)Math.Pow(_level, 2)) + (105.6f * _level));
        }

        // Function for stat calculation
        private float StatFunction(int level)
        {
            return (float)((0.25f * Math.Sqrt(level)) + 0.75f);
        }

        private float StatDefense(int level)
        {
            return (float)(0.827f * Math.Log(level) / Math.Log(1.1));
        }
    }
}
