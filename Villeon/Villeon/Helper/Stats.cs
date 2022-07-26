using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private int _attackLevel = 1;
        private int _defenseLevel = 1;

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
            get { return _attackLevel; }
            set { _attackLevel = value; }
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

        public static Stats GetInstance()
        {
            if (_instance == null)
                _instance = new Stats();
            return _instance;
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

        public void IncreaseAttack() => _attackLevel++;

        public void IncreaseDefense() => _defenseLevel++;

        public int GetMaxHealth()
        {
            int health = (int)(StatFunction() * 100);
            return health + (int)(health * 0.1f * _level);
        }

        public int GetAttack()
        {
            int attack = (int)(StatFunction() * 10);
            return attack + (int)(attack * 0.5 * _level);
        }

        public int GetDefense()
        {
            int defense = (int)(StatFunction() * 10);
            return defense + (int)(defense * 1.2f * _level);
        }

        public int GetUnlockProgress(int caveIndex)
        {
            return _unlockProgress[caveIndex];
        }

        public void IncreaseUnlockProgress(int caveIndex)
        {
            _unlockProgress[caveIndex]++;
        }

        // Function for level exp requirements
        private int RequiredExperienceFunction()
        {
            return (int)Math.Floor((0.02f * (float)Math.Pow(_level, 3)) + (3.06f * (float)Math.Pow(_level, 2)) + (105.6f * _level));
        }

        // Function for stat calculation
        private float StatFunction()
        {
            return (float)((0.25f * Math.Sqrt(_healthLevel)) + 0.75f);
        }
    }
}
