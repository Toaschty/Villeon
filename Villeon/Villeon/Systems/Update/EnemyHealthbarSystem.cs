using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;

namespace Villeon.Systems.Update
{
    public class EnemyHealthbarSystem : System, IUpdateSystem
    {
        private Dictionary<IEntity, HealthBar> _healthBars = new Dictionary<IEntity, HealthBar>();

        public EnemyHealthbarSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Health), typeof(EnemyAI), typeof(Sprite));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            Health health = entity.GetComponent<Health>();
            Transform transform = entity.GetComponent<Transform>();
            Sprite sprite = entity.GetComponent<Sprite>();
            _healthBars.Add(entity, new HealthBar(health.CurrentHealth, ref transform, sprite.Width, sprite.Height));
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);

            _healthBars[entity].Remove();
            _healthBars.Remove(entity);
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Health health = entity.GetComponent<Health>();
                _healthBars[entity].Update(health.CurrentHealth);
            }
        }
    }
}
