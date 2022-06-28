using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.GUI
{
    public class NpcNameSign
    {
        private List<IEntity> _name = new List<IEntity>();

        public NpcNameSign(Vector2 position, string npcName, SpriteLayer frameLayer)
        {
            // Figure out startingposition of the boxes
            Text npcNameText = new Text(npcName, position, "Alagard", frameLayer, 0f, 0f, 0.2f);
            List<IEntity> textEntities = npcNameText.Letters;

            // Add locally
            _name.AddRange(textEntities);
        }

        public void Spawn(string scene)
        {
            foreach (Entity text in _name)
            {
                Manager.GetInstance().AddEntityToScene(text, scene);
            }
        }

        public void Delete()
        {
            foreach (Entity text in _name)
            {
                Manager.GetInstance().RemoveEntity(text);
            }

            _name.Clear();
        }
    }
}
