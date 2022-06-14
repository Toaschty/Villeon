using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;

namespace Villeon.Systems.Update
{
    public class PlayerHealthbarSystem : System, IUpdateSystem
    {
        private PlayerHealthBar _healthBar;
        private Health _playerHealth;
        private float _currentHealth;

        public PlayerHealthbarSystem(string name, int maxPlayerHealth)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health), typeof(Player));

            _playerHealth = new Health(maxPlayerHealth);
            _healthBar = new PlayerHealthBar(maxPlayerHealth);
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                _playerHealth = entity.GetComponent<Health>();
            }

            if (_playerHealth.CurrentHealth != _currentHealth)
            {
                _healthBar.UpdateHealthbar(_playerHealth.CurrentHealth);
                _currentHealth = _playerHealth.CurrentHealth;
            }
        }
    }
}
