using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Assets;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class BossSystem : System, IUpdateSystem
    {
        public BossSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Boss), typeof(Health));
        }

        public void Update(float time)
        {
            foreach (IEntity boss in Entities)
            {
                // Is Boss dead?
                if (boss.GetComponent<Health>().CurrentHealth <= 0)
                {
                    SpawnNPC(boss.GetComponent<Boss>());
                    SpawnPortal();
                }
            }
        }

        public void SpawnNPC(Boss boss)
        {
            // SPAWN the Rescued NPC
            NPCLoader.SpawnRescuedNPC("BossScene", boss.CaveIndex);
            Stats.GetInstance().IncreaseUnlockProgress(boss.CaveIndex);
            NPCLoader.UpdateNPCs();

            // Save game
            SaveLoad.Save();

            // Spawn SaveParticle
            IEntity savingIcon = ParticleBuilder.StationaryParticle(new Vector2(-0.95f, -5f), 2, 0.2f, true, "Animations.Saving.png", 0.5f, Components.SpriteLayer.ScreenGuiOnTopOfForeground);
            Manager.GetInstance().AddEntity(savingIcon);
        }

        public void SpawnPortal()
        {
            // Add Boss Portal
            IEntity portalEntity = new Entity(new Transform(new Vector2(5, 4), 1f, 0f), "Portal Trigger");
            portalEntity.AddComponent(new Trigger(TriggerLayerType.PORTAL, 4f, 5f));
            portalEntity.AddComponent(new Portal("VillageScene", Constants.VILLAGE_SPAWN_POINT));
            portalEntity.AddComponent(new Interactable(new Option("Leave Boss Room [E]", Keys.E)));
            portalEntity.AddComponent(Asset.GetSpriteSheet("Sprites.PortalAnimation.png").GetSprite(0, SpriteLayer.Middleground, true));
            Manager.GetInstance().AddEntity(portalEntity);
        }
    }
}
