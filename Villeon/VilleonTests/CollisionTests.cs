using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Mathematics;
using Villeon;
using Villeon.Components;
using Villeon.Systems;

namespace VilleonTests
{
    [TestClass]
    public class CollisionTests
    {
        [TestMethod]
        public void CollisionTest()
        {
            // Arrange
            TypeRegistry.Init();
            CollisionSystem collisionSystem = new ("collisionSystem");
            IEntity entity = new Entity("Marin");
            IEntity entity2 = new Entity("Gojo");

            entity.AddComponent(new Transform(new Vector2(5.0f, 5.0f), 1.0f, 0.0f));
            entity2.AddComponent(new Transform(new Vector2(5.5f, 5.0f), 1.0f, 0.0f));
            entity.AddComponent(new Collider(Vector2.Zero, new Vector2(5.0f, 5.0f), 0.5f, 0.5f));
            entity2.AddComponent(new Collider(Vector2.Zero, new Vector2(5.5f, 5.0f), 0.5f, 0.5f));
            collisionSystem.Entities.Add(entity);
            collisionSystem.Entities.Add(entity2);

            // Act
            collisionSystem.Update(0);
            entity.GetComponent<Transform>().Position += new Vector2(0.2f, 0f);
            collisionSystem.Update(0);

            // Assert
            Assert.AreEqual(5f, entity.GetComponent<Transform>().Position.X);
        }

        [TestMethod]
        public void AnotherCollisionTest()
        {
            // Arrange
            TypeRegistry.Init();
            CollisionSystem collisionSystem = new ("collisionSystem");
            IEntity wall = new Entity("Wall");
            IEntity player = new Entity("Player");

            wall.AddComponent(new Transform(new Vector2(10.0f, 10.0f), 1.0f, 0.0f));
            wall.AddComponent(new Collider(Vector2.Zero, new Vector2(5.0f, 5.0f), 0.5f, 0.5f));

            player.AddComponent(new Transform(new Vector2(8.0f, 8.0f), 1.0f, 0.0f));
            player.AddComponent(new Collider(Vector2.Zero, new Vector2(8.0f, 8.0f), 0.5f, 0.5f));
            player.AddComponent(new Physics());

            collisionSystem.Entities.Add(wall);
            collisionSystem.Entities.Add(player);

            // Act
            collisionSystem.Update(0);
            player.GetComponent<Transform>().Position += new Vector2(2.0f, 2.0f);
            collisionSystem.Update(0);
            player.GetComponent<Transform>().Position += new Vector2(1.0f, 1.0f);
            collisionSystem.Update(0);

            // Assert
            Assert.AreEqual(11.0f, player.GetComponent<Transform>().Position.X);
        }
    }
}