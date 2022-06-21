using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Exp : IComponent
    {
        private int _currentExp;

        private int _maxExp;

        private int _level;

        public Exp(int maxExp)
        {
            _maxExp = maxExp;
            _currentExp = 0;
            _level = 1;
        }

        public int CurrentExp { get => _currentExp; }

        public int MaxExp { get => _maxExp; }

        public int Level { get => _level; }

        public bool GainExp(int expGain)
        {
            bool hasLeveledUp = false;

            _currentExp += expGain;

            // Went over the maxExp 
            if (_currentExp > _maxExp)
            {
                // set exp to the overshoot
                _currentExp = _currentExp - _maxExp;

                // Calc. new max Exp
                _maxExp = (int)(_maxExp * 1.1f);

                // Level up
                _level++;
                hasLeveledUp = true;
            }

            return hasLeveledUp;
        }
    }
}
