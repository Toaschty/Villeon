using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class BossCameraSystem : System, IUpdateSystem
    {
        private IEntity? _player;
        private IEntity? _boss;

        public BossCameraSystem(string name)
            : base(name)
        {
            Signature
                .IncludeAND(typeof(Player), typeof(Effect), typeof(Fokus))
                .IncludeAND(typeof(Boss));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);

            Player player = entity.GetComponent<Player>();
            Boss boss = entity.GetComponent<Boss>();

            if (player is not null)
                _player = entity;
            if (boss is not null)
                _boss = entity;
        }

        public void Update(float time)
        {
            Transform playerTransform = _player!.GetComponent<Transform>();
            Effect effect = _player!.GetComponent<Effect>();
            Fokus fokus = _player!.GetComponent<Fokus>();

            if (effect.Effects.ContainsKey("FokusBoss"))
            {
                Transform bossTransform = _boss!.GetComponent<Transform>();
                DynamicCollider bossCollider = _boss!.GetComponent<DynamicCollider>();
                fokus.Offset = bossTransform.Position - playerTransform.Position + new Vector2(bossCollider.Width / 2, bossCollider.Height / 2);
                fokus.Intensity = 10;
            }
            else
            {
                fokus.Offset = Vector2.Zero;
                fokus.Intensity = 100;
            }
        }
    }
}
