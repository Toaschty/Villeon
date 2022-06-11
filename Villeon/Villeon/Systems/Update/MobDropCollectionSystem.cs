using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.Components;
using Villeon.ECS;

namespace Villeon.Systems.Update
{
    internal class MobDropCollectionSystem : TriggerActionSystem, IUpdateSystem
    {
        public MobDropCollectionSystem(string name)
            : base(name)
        {
            Signature.
                IncludeAND(typeof(Trigger), typeof(Player)).
                IncludeAND(typeof(Trigger), typeof(Drops));
        }

        // Add Entity to TriggerLayer & Base
        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);
            TriggerLayerType triggerLayerType = entity.GetComponent<Trigger>().TriggerLayers;
            Player receiver = entity.GetComponent<Player>();
            Drops actor = entity.GetComponent<Drops>();
            AddEntityToLayer(receiver, actor, entity);
        }

        public void Update(float time)
        {
            // Iterate through each Trigger Layer
            foreach (TriggerLayerType layerKey in TriggerLayers.Keys)
            {
                // Continue if there is no collision on this layer
                if (TriggerLayers[layerKey].Collisions is null)
                    continue;

                // Do action when collision happend
                foreach (var collisionPair in TriggerLayers[layerKey].Collisions)
                {
                    IEntity actor = collisionPair.Item1;
                    IEntity receiver = collisionPair.Item2;

                    Sprite actorSpriteCopy = new Sprite(actor.GetComponent<Sprite>());
                    actorSpriteCopy.IsDynamic = false;

                    GUIHandler.GetInstance().InventoryMenu.AddItem(new GUI.Item("EnemyPotion", actorSpriteCopy, 10, GUI.Item.ITEM_TYPE.POTION));

                    // Move all items from DROP
                    // into the inventory of PLAYER

                    RemoveEntity(actor);
                }
            }
        }
    }
}
