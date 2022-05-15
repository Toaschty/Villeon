using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Mathematics;
using Villeon;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Systems;

namespace VilleonTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class CollisionTests
    {

        [TestMethod]
        public void CollisionUP()
        {
            TypeRegistry.Init();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(10, 10);
            Vector2 start_pos_player = new Vector2(10, 11);

            // Collision Up
            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "enemy");
            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");

            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 0.5f, 0.5f));
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 0.5f, 0.5f));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            // Act
            collisionSystem.Update(0);
            player.GetComponent<Transform>().Position -= new Vector2(0, 1);
            collisionSystem.Update(0);

            // Assert
            Assert.AreEqual(new Vector2(10, 10.5f), player.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void CollisionDown()
        {
            // Test when both player and enemy are going to the same point one of them should be set back to his start position
            TypeRegistry.Init();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(10, 10);
            Vector2 start_pos_player = new Vector2(10, 9);

            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "enemy");
            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");

            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 0.5f, 0.5f));
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 0.5f, 0.5f));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            collisionSystem.Update(0);
            player.GetComponent<Transform>().Position += new Vector2(0, 1);
            collisionSystem.Update(0);

            Assert.AreEqual(start_pos_enemy, enemy.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void CollisionRight()
        {
            // Test when both player and enemy are going to the same point one of them should be set back to his start position
            TypeRegistry.Init();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(11, 10);
            Vector2 start_pos_player = new Vector2(9, 10);

            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "enemy");
            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");

            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 0.5f, 0.5f));
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 0.5f, 0.5f));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            collisionSystem.Update(0);
            enemy.GetComponent<Transform>().Position += new Vector2(-1, 0);
            player.GetComponent<Transform>().Position += new Vector2(1, 0);
            collisionSystem.Update(0);

            Assert.AreEqual(start_pos_enemy, enemy.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void CollisionLeft()
        {
            // Test when both player and enemy are going to the same point one of them should be set back to his start position
            TypeRegistry.Init();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(11, 10);
            Vector2 start_pos_player = new Vector2(9, 10);

            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "enemy");
            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");

            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 0.5f, 0.5f));
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 0.5f, 0.5f));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            collisionSystem.Update(0);
            enemy.GetComponent<Transform>().Position += new Vector2(-1, 0);
            player.GetComponent<Transform>().Position += new Vector2(1, 0);
            collisionSystem.Update(0);

            Assert.AreEqual(start_pos_enemy, enemy.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void CleanCollision()
        {
            // Test when both player and enemy are going to the same point one of them should be set back to his start position
            TypeRegistry.Init();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(11, 11);
            Vector2 start_pos_secondenemy = new Vector2(12, 11);
            Vector2 start_pos_player = new Vector2(10f, 10f);
            Vector2 start_pos_secondplayer = new Vector2(8f, 10f);

            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "enemy");
            Entity secondEnemy = new Entity(new Transform(start_pos_secondenemy, 1.0f, 0.0f), "secondEnemey");
            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");
            Entity secondPlayer = new Entity(new Transform(start_pos_secondplayer, 1.0f, 0.0f), "secondPlayer");

            // Add collider components for every entity
            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 0.5f, 0.5f));
            secondEnemy.AddComponent(new Collider(Vector2.Zero, start_pos_secondenemy, 0.5f, 0.5f));
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 0.5f, 0.5f));
            secondPlayer.AddComponent(new Collider(Vector2.Zero, start_pos_secondplayer, 0.5f, 0.5f));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(secondEnemy);
            collisionSystem.Entities.Add(player);
            collisionSystem.Entities.Add(secondPlayer);

            collisionSystem.Update(0);
            player.GetComponent<Transform>().Position += new Vector2(0.5f, 0.5f);
            enemy.GetComponent<Transform>().Position += new Vector2(0.5f, 0.5f);
            collisionSystem.Update(0);

            Assert.AreEqual(start_pos_enemy, enemy.GetComponent<Transform>().Position);
        }
    }
}