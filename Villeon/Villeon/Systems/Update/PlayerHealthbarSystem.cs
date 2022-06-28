using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class PlayerHealthbarSystem : System, IUpdateSystem
    {
        private static PlayerHealthBar? _healthBar;
        private static Health? _playerHealth;
        private static int _maxPlayerHealth = 0;
        private float _currentHealth;
        private float _currentMaxHealth;

        public PlayerHealthbarSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health), typeof(Player));

            _maxPlayerHealth = Stats.GetInstance().GetHealth();
            _playerHealth = new Health(_maxPlayerHealth);
            _healthBar = new PlayerHealthBar(_maxPlayerHealth);
        }

        public static void Init()
        {
            _healthBar = new PlayerHealthBar(_maxPlayerHealth);
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                _playerHealth = entity.GetComponent<Health>();
            }

            if (_playerHealth!.CurrentHealth != _currentHealth)
            {
                _healthBar!.UpdateHealthbar(_playerHealth.CurrentHealth);
                _currentHealth = _playerHealth.CurrentHealth;
            }

            if (Stats.GetInstance().GetHealth() != _currentMaxHealth)
            {
                _playerHealth.MaxHealth = Stats.GetInstance().GetHealth();
                _healthBar!.UpdateMaxHealth(_playerHealth.MaxHealth);
                _currentMaxHealth = _playerHealth.MaxHealth;
            }
        }
    }
}
