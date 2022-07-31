using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;

namespace Villeon.Systems.Update
{
    public class NPCNameSignSystem : System, IUpdateSystem
    {
        private Dictionary<IEntity, NpcNameSign> _entityToSign = new Dictionary<IEntity, NpcNameSign>();

        public NPCNameSignSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(NPC), typeof(Sprite));
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);

            NPC npc = entity.GetComponent<NPC>();
            Transform transform = entity.GetComponent<Transform>();
            Sprite sprite = entity.GetComponent<Sprite>();

            Vector2 position = transform.Position + new Vector2((sprite.Width / 8f) + 0.25f, ((sprite.Height / 8f) / 2f) - 0.15f);
            NpcNameSign nameSign = new NpcNameSign(position, npc.Name, SpriteLayer.GUIBackground);
            nameSign.Spawn(npc.Scene);
            _entityToSign.Add(entity, nameSign);
        }

        public void Update(float time)
        {
        }
    }
}
