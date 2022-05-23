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

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            player.GetComponent<Transform>().Position += new Vector2(2, 0);
            collisionSystem.Update(0);

            Assert.AreEqual(new Vector2(1, 0), player.GetComponent<Transform>().Position);
        }

        /*
            How the dirty collision work:
            P = Player | E = enemy

            EE
            EE (Pos = 2)

               (Here is 0)

            PP
            PP (Pos = -2)

            Both the player and the enemy are moving to 0 and collide there.
            Here now is important that it depends on which order
            the ENEMY and the PLAYER are registered to the collision system.
            In this example first the enemy is registered and then the player.
            The first registered element is the first one to be checked for collision.

            EE
            PP (player and enemy pos are the same = 0)

            Enemey and the player are having the same position
            -> Collision detected: enemy is going to be reset to his last position

            EE
            EE (Pos = 2)

            PP
            PP (Pos = 0)
        */

        [TestMethod]
        public void DirtyCollisionUp()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(0, 2);
            Vector2 start_pos_player = new Vector2(0, -2);

            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "Enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 1f, 1f));

            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 1f, 1f));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            enemy.GetComponent<Transform>().Position -= new Vector2(0, 2);
            player.GetComponent<Transform>().Position += new Vector2(0, 2);

            collisionSystem.Update(0);

            Assert.AreEqual(start_pos_enemy, enemy.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void DirtyCollisionDown()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(0, 2);
            Vector2 start_pos_player = new Vector2(0, -2);

            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "Enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 1f, 1f));

            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 1f, 1f));

            collisionSystem.Entities.Add(player);
            collisionSystem.Entities.Add(enemy);

            enemy.GetComponent<Transform>().Position -= new Vector2(0, 2);
            player.GetComponent<Transform>().Position += new Vector2(0, 2);

            collisionSystem.Update(0);

            Assert.AreEqual(start_pos_player, player.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void DirtyCollisionRight()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(2, 0);
            Vector2 start_pos_player = new Vector2(-2, 0);

            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "Enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 1f, 1f));

            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 1f, 1f));

            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            enemy.GetComponent<Transform>().Position -= new Vector2(2, 0);
            player.GetComponent<Transform>().Position += new Vector2(2, 0);

            collisionSystem.Update(0);

            Assert.AreEqual(start_pos_enemy, enemy.GetComponent<Transform>().Position);
        }

        [TestMethod]
        public void DirtyCollisionLeft()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            Vector2 start_pos_enemy = new Vector2(2, 0);
            Vector2 start_pos_player = new Vector2(-2, 0);

            Entity enemy = new Entity(new Transform(start_pos_enemy, 1.0f, 0.0f), "Enemy");
            enemy.AddComponent(new Collider(Vector2.Zero, start_pos_enemy, 1f, 1f));

            Entity player = new Entity(new Transform(start_pos_player, 1.0f, 0.0f), "Player");
            player.AddComponent(new Collider(Vector2.Zero, start_pos_player, 1f, 1f));

            collisionSystem.Entities.Add(player);
            collisionSystem.Entities.Add(enemy);

            enemy.GetComponent<Transform>().Position -= new Vector2(2, 0);
            player.GetComponent<Transform>().Position += new Vector2(2, 0);

            collisionSystem.Update(0);

            Assert.AreEqual(start_pos_player, player.GetComponent<Transform>().Position);
        }

        /*
            Explanation for how the CollidesCleanedEntity function operates and how to test it.

            W = Wall | P = Player | E = Enemy

            WW PP EE
            WW PP EE

            player and enemy are moving:

            WP EE
            WP EE

            player is inside the wall
                -> collision detection set the player to his last position:

            WW PE
            WW PE

            The last known position for the player is where the enemy is now
                -> another Collision: set the enemy to his last known position
        */

        [TestMethod]
        public void CleanedCollision()
        {
            TypeRegistry.SetupTypes();
            CollisionSystem collisionSystem = new ("collisionSystem");

            // Create all entities
            Entity wall = new Entity(new Transform(new Vector2(0, 0), 1f, 0f), "Wall");
            wall.AddComponent(new Collider(new Vector2(0, 0), 1f, 2));

            Entity player = new Entity(new Transform(new Vector2(1f, 0), 1f, 0f), "Player");
            player.AddComponent(new Collider(new Vector2(1f, 0), 1f, 1f));

            Entity enemy = new Entity(new Transform(new Vector2(2f, 0), 1f, 0f), "Enemy");
            enemy.AddComponent(new Collider(new Vector2(2f, 0), 1f, 1f));

            // Add entities to the collisionsystem
            collisionSystem.Entities.Add(wall);
            collisionSystem.Entities.Add(enemy);
            collisionSystem.Entities.Add(player);

            Transform playerTransform = player.GetComponent<Transform>();
            playerTransform.Position += new Vector2(-1f, 0f);

            Transform enemyTransform = enemy.GetComponent<Transform>();
            enemyTransform.Position += new Vector2(-1f, 0f);

            collisionSystem.Update(0);

            Assert.AreEqual(2f, enemy.GetComponent<Transform>().Position.X);
        }
    }
}