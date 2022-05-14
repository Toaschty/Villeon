using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Mathematics;
using Villeon;
using Villeon.Components;
using Villeon.Systems;

namespace VilleonTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CollisionTest()
        {
            // Arrange
            TypeRegistry.Init();
            CollisionSystem collisionSystem = new ("collisionSystem");
            IEntity entity = new Entity(new Transform(new Vector2(5.0f, 5.0f), 1.0f, 0.0f), "Marin");
            IEntity entity2 = new Entity(new Transform(new Vector2(5.5f, 5.0f), 1.0f, 0.0f), "Gojo");

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
    }
}