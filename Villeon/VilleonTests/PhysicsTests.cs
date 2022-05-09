﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK.Mathematics;
using Villeon;
using Villeon.Components;
using Villeon.Systems;

namespace VilleonTests
{
    [TestClass]
    public class PhysicsTests
    {
        private Scene _testScene = new Scene("Testing");

        [TestInitialize]
        public void SetUp()
        {
            TypeRegistry.Init();

            _testScene.AddSystem(new PhysicsSystem("Physics"));
            _testScene.AddSystem(new CollisionSystem("Collision"));
        }

        [TestMethod]
        public void StopEntityIfCollidesWithFloor()
        {
            /*
                    PPP
                    PPP

                     |
                    \|/

            ###################
            ###################
            */

            // Instanciate physics entity
            Entity physicEntity = new Entity("Physic");
            physicEntity.AddComponent(new Physics());
            physicEntity.AddComponent(new Collider(new Vector2(0, 0), new Vector2(0, 5), 1, 1));

            // Instanciate "Floor" to collide with
            Entity floor = new Entity("Floor");
            floor.AddComponent(new Collider(new Vector2(0, 0), new Vector2(-5, 0), 10, 1));

            // Setup Test-Scene
            _testScene.AddEntity(physicEntity);
            _testScene.AddEntity(floor);

            // Update the PhysicsSystem 10 times with 0.1f time between updates
            for (int i = 0; i < 10; i++)
            {
                _testScene.Update(0.1f);
            }

            // -12 equals the default Y velocity a entity has when standing still with an delta time of 0.1f
            Assert.AreEqual(-12, physicEntity.GetComponent<Physics>().Velocity.Y);

            // Clean up
            _testScene.RemoveEntity(physicEntity);
            _testScene.RemoveEntity(floor);
        }

        [TestMethod]
        public void StopEntityIfCollidesWithCeiling()
        {
            /*
            ###################
            ###################

                    /|\
                     |

                    PPP
                    PPP
            */

            // Instanciate physics entity
            Entity physicEntity = new Entity("Physic");
            physicEntity.AddComponent(new Physics());
            physicEntity.AddComponent(new Collider(new Vector2(0, 0), new Vector2(0, 0), 1, 1));

            // Instanciate "Ceiling" to collide with
            Entity ceiling = new Entity("Ceiling");
            ceiling.AddComponent(new Collider(new Vector2(0, 0), new Vector2(-5, 10), 10, 1));

            // Setup Test-Scene
            _testScene.AddEntity(physicEntity);
            _testScene.AddEntity(ceiling);

            // Update the PhysicsSystem 10 times with 0.1f time between updates
            for (int i = 0; i < 10; i++)
            {
                _testScene.Update(0.1f);

                // Set acceleration to +120 to "invert" Gravity after each update
                physicEntity.GetComponent<Physics>().Acceleration = new Vector2(0, 120f);
            }

            // 12 equals the default Y velocity a entity has when standing still with an delta time of 0.1f
            Assert.AreEqual(12, physicEntity.GetComponent<Physics>().Velocity.Y);

            // Clean up
            _testScene.RemoveEntity(physicEntity);
            _testScene.RemoveEntity(ceiling);
        }

        [TestMethod]
        public void StopEntityIfCollidesWithLeftWall()
        {
            /*
            ##
            ##  /__    PPP
            ##  \      PPP
            ###################
            ###################
            */

            // Instanciate physics entity
            Entity physicEntity = new Entity("Physic");
            physicEntity.AddComponent(new Physics());
            physicEntity.AddComponent(new Collider(new Vector2(0, 0), new Vector2(0, 1), 1, 1));

            // Instanciate "Floor" to collide with
            Entity floor = new Entity("Floor");
            floor.AddComponent(new Collider(new Vector2(0, 0), new Vector2(-25, 0), 50, 1));

            // Instanciate "Wall" to collide with
            Entity wall = new Entity("Wall");
            wall.AddComponent(new Collider(new Vector2(0, 0), new Vector2(-25, 1), 1, 10));

            // Setup Test-Scene
            _testScene.AddEntity(physicEntity);
            _testScene.AddEntity(floor);
            _testScene.AddEntity(wall);

            // Update the PhysicsSystem 50 times with 0.1f time between updates
            for (int i = 0; i < 50; i++)
            {
                _testScene.Update(0.1f);

                // Add acceleration of -80f in x direction each update
                physicEntity.GetComponent<Physics>().Acceleration += new Vector2(-80f, 60f);
            }

            // -8 equals the default X velocity a entity has when standing still with an delta time of 0.1f
            Assert.AreEqual(-8, physicEntity.GetComponent<Physics>().Velocity.X);

            // Clean up
            _testScene.RemoveEntity(physicEntity);
            _testScene.RemoveEntity(floor);
            _testScene.RemoveEntity(wall);
        }

        [TestMethod]
        public void StopEntityIfCollidesWithRightWall()
        {
            /*
                             ##
                 PPP    __\  ##
                 PPP      /  ##
            ###################
            ###################
            */

            // Instanciate physics entity
            Entity physicEntity = new Entity("Physic");
            physicEntity.AddComponent(new Physics());
            physicEntity.AddComponent(new Collider(new Vector2(0, 0), new Vector2(0, 1), 1, 1));

            // Instanciate "Floor" to collide with
            Entity floor = new Entity("Floor");
            floor.AddComponent(new Collider(new Vector2(0, 0), new Vector2(-25, 0), 50, 1));

            // Instanciate "Wall" to collide with
            Entity wall = new Entity("Wall");
            wall.AddComponent(new Collider(new Vector2(0, 0), new Vector2(25, 1), 1, 10));

            // Setup Test-Scene
            _testScene.AddEntity(physicEntity);
            _testScene.AddEntity(floor);
            _testScene.AddEntity(wall);

            // Update the PhysicsSystem 50 times with 0.1f time between updates
            for (int i = 0; i < 50; i++)
            {
                _testScene.Update(0.1f);

                // Add acceleration of -80f in x direction each update
                physicEntity.GetComponent<Physics>().Acceleration += new Vector2(80f, 60f);
            }

            // -8 equals the default X velocity a entity has when standing still with an delta time of 0.1f
            Assert.AreEqual(8, physicEntity.GetComponent<Physics>().Velocity.X);

            // Clean up
            _testScene.RemoveEntity(physicEntity);
            _testScene.RemoveEntity(floor);
            _testScene.RemoveEntity(wall);
        }
    }
}
