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
        private float _currentHealth; // Health of the previous frame
        private float _currentMaxHealth;

        public PlayerHealthbarSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health), typeof(Player));

            _maxPlayerHealth = Stats.GetInstance().GetMaxHealth();
            _playerHealth = new Health(_maxPlayerHealth);
            _healthBar = new PlayerHealthBar(_maxPlayerHealth);
        }

        public static void Init()
        {
            _healthBar = new PlayerHealthBar(_maxPlayerHealth);
        }

        public void Update(float time)
        {
            // Get the healthbar Component
            foreach (IEntity entity in Entities)
            {
                _playerHealth = entity.GetComponent<Health>();
            }

            // Update health if it changed since the last frame
            if (_playerHealth!.CurrentHealth != _currentHealth)
            {
                _healthBar!.UpdateHealthbar(_playerHealth.CurrentHealth);
                _currentHealth = _playerHealth.CurrentHealth;
                //_maxPlayerHealth = (int)_currentHealth;
            }

            // Update maxHealth 
            if (Stats.GetInstance().GetMaxHealth() != _currentMaxHealth)
            {
                _playerHealth.MaxHealth = Stats.GetInstance().GetMaxHealth();
                _healthBar!.UpdateMaxHealth(_playerHealth.MaxHealth);
                _playerHealth.CurrentHealth = _playerHealth.MaxHealth;
                _currentMaxHealth = _playerHealth.MaxHealth;
                _maxPlayerHealth = _playerHealth.MaxHealth;
            }

            global::System.Console.WriteLine("currentHealth:" + _currentHealth);
            global::System.Console.WriteLine("currentMaxHealth:" + _currentMaxHealth);
            global::System.Console.WriteLine("_maxPlayerHealth:" + _maxPlayerHealth);
            global::System.Console.WriteLine("_playerHealth.MaxHealth:" + _playerHealth.MaxHealth);
            global::System.Console.WriteLine("_playerHealth.CurrentHealth:" + _playerHealth.CurrentHealth);
        }
    }
}
