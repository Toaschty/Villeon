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

        public Health(int health)
        {
            _maxHealth = health;
            CurrentHealth = health;
            _isInvincible = false;
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

        public void Damage(int damage) => CurrentHealth -= damage * Convert.ToInt32(!_isInvincible);

        public void Heal(int heal) => CurrentHealth += heal;
    }
}
