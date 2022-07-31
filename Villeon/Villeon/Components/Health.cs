using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Health : IComponent
    {
        private int _currentHealth;

        private int _maxHealth;

        private bool _isInvincible;

        private int _protection;

        public Health(int health)
        {
            _maxHealth = health;
            CurrentHealth = health;
            _isInvincible = false;
            _protection = 0;
        }

        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > _maxHealth)
                    value = _maxHealth;
                _currentHealth = value;
            }
        }

        public bool IsInvincible
        {
            get { return _isInvincible; }
            set { _isInvincible = value; }
        }

        public int Protection
        {
            get { return _protection; }
            set { _protection = value; }
        }

        public int MaxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; }
        }

        public void Damage(int damage)
        {
            if (_isInvincible)
                return;

            Random rand = new Random();
            int effectiveProtection = rand.Next(_protection / 2, _protection);
            int effectiveDamage = damage - effectiveProtection;

            if (effectiveDamage < 0)
                return;

            CurrentHealth -= effectiveDamage;
        }

        public void Heal(int heal) => CurrentHealth += heal;
    }
}
