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
            Signature.IncludeAND(typeof(Health), typeof(EnemyAI));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            Health health = entity.GetComponent<Health>();
            Transform transform = entity.GetComponent<Transform>();
            _healthBars.Add(entity, new HealthBar(health.CurrentHealth, new OpenTK.Mathematics.Vector2(-0.5f, 3f), ref transform));
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
