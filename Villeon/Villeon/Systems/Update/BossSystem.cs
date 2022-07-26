using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
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
                    // Spawn NPC
                    Console.WriteLine("Spawning NPC!");
                    EnemySpawner.SpawnEnemy("BossScene", "slime_blue", new Vector2(30, 6));
                    // 30 6

                    // Spawn Portal
                    Console.WriteLine("Spawning Portal!");

                    Stats.GetInstance().IncreaseUnlockProgress(0);
                    NPCLoader.UpdateNPCs();

                    SceneLoader.SetActiveScene("VillageScene");
                }
            }
        }

        public void SpawnNPC()
        {

        }
    }
}
