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
        /*
            How the clean collision works:
            P = Player | E = enemy

            PP
            PP (Pos = 2)

            EE
            EE (Pos = 0)

            Player now moves down onto the enemy: Player pos - 2

            PP
            EE (player and enemy pos are the same = 0)

            The player will now be set on top of the enemy:

            PP
            PP (Pos = 1)
            EE
            EE (Pos = 0)
       */

        [TestMethod]
        public void CleanCollisionUp()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Entity enemy = new Entity(new Transform(new Vector2(0, 0), 1.0f, 0.0f), "Enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, new Vector2(0, 0), 1f, 1f));

            Entity player = new Entity(new Transform(new Vector2(0, 2), 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, new Vector2(0, 2), 1f, 1f));
            player.AddComponent(new DynamicCollider(player.GetComponent<Collider>()));

            // Add every entity to the collisionsystem
            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            // Move the player and then make the collision detection
            player.GetComponent<Transform>().Position -= new Vector2(0, 2);
            collisionSystem.Update(0);

            Assert.AreEqual(new Vector2(0, 1), player.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void CleanCollisionDown()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 enemyStartPos = new Vector2(0, 2);
            Vector2 playerStartPos = new Vector2(0, 0);

            Entity enemy = new Entity(new Transform(enemyStartPos, 1.0f, 0.0f), "enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, enemyStartPos, 1f, 1f));

            Entity player = new Entity(new Transform(playerStartPos, 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, playerStartPos, 1f, 1f));
            player.AddComponent(new DynamicCollider(player.GetComponent<Collider>()));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            player.GetComponent<Transform>().Position += new Vector2(0, 2);
            collisionSystem.Update(0);

            Assert.AreEqual(new Vector2(0, 1), player.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void CleanCollisionRight()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 enemyStartPos = new Vector2(0, 0);
            Vector2 playerStartPos = new Vector2(2, 0);

            Entity enemy = new Entity(new Transform(enemyStartPos, 1.0f, 0.0f), "Enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, enemyStartPos, 1f, 1f));

            Entity player = new Entity(new Transform(playerStartPos, 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, playerStartPos, 1f, 1f));
            player.AddComponent(new DynamicCollider(player.GetComponent<Collider>()));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            player.GetComponent<Transform>().Position -= new Vector2(2, 0);
            collisionSystem.Update(0);

            Assert.AreEqual(new Vector2(1, 0), player.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void CleanCollisionLeft()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 enemyStartPos = new Vector2(2, 0);
            Vector2 playerStartPos = new Vector2(0, 0);

            Entity enemy = new Entity(new Transform(enemyStartPos, 1.0f, 0.0f), "Enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, enemyStartPos, 1f, 1f));

            Entity player = new Entity(new Transform(playerStartPos, 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, playerStartPos, 1f, 1f));
            player.AddComponent(new DynamicCollider(player.GetComponent<Collider>()));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            player.GetComponent<Transform>().Position += new Vector2(2, 0);
            collisionSystem.Update(0);

            Assert.AreEqual(new Vector2(1, 0), player.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void CollisionCornerCorner()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new("collisionSystem");

            Vector2 enemyStartPos = new Vector2(0, 0);
            Vector2 playerStartPos = new Vector2(1, 1);

            Entity enemy = new Entity(new Transform(enemyStartPos, 1.0f, 0.0f), "Enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, enemyStartPos, 1f, 1f));

            Entity player = new Entity(new Transform(playerStartPos, 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, playerStartPos, 1f, 1f));
            player.AddComponent(new DynamicCollider(player.GetComponent<Collider>()));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            player.GetComponent<Transform>().Position -= new Vector2(0.1f, 0.1f);
            collisionSystem.Update(0);

            Assert.AreEqual(new Vector2(0.9f, 1f), player.GetComponent<Transform>().Position);
        }
    }
}