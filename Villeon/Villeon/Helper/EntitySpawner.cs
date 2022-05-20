﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Helper;
using Villeon.Render;

namespace Villeon
{
    public class EntitySpawner
    {
        public EntitySpawner()
        {
        }

        public static void SpawnTrigger(TriggerID id, Vector2 poition, float width, float height)
        {
            IEntity damageEntity = new Entity(new Transform(poition, width, height), "Attack");
            damageEntity.AddComponent(TriggerBuilder.Build(id));
            Manager.GetInstance().AddEntity(damageEntity);
        }

        public static void SpawnTrigger(TriggerID id, Transform transform)
        {
            IEntity damageEntity = new Entity(transform, "Attack");
            damageEntity.AddComponent(TriggerBuilder.Build(id));
            Manager.GetInstance().AddEntity(damageEntity);
        }

        public void Spawn(Vector2 position)
        {
            IEntity entity = new Entity(new Transform(position, 1.0f, 0f), "Peter");
            entity.AddComponent(new Collider(new Vector2(0f, 0f), position, 1.5f, 1f));
            entity.AddComponent(TriggerBuilder.Build(TriggerID.ENEMY));
            entity.AddComponent(new Health(200));
            entity.AddComponent(new Physics());
            entity.AddComponent(new SimpleAI());
            entity.AddComponent(new Sprite(Assets.GetTexture("Player.png"), SpriteLayer.Foreground, true));
            Manager.GetInstance().AddEntity(entity);
        }
    }
}
