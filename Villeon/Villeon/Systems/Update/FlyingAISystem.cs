using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation.DungeonGeneration;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class FlyingAISystem : System, IUpdateSystem
    {
        private IEntity? _playerEntity;

        public FlyingAISystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Physics), typeof(DynamicCollider), typeof(FlyingAI))
                .IncludeOR(typeof(Player));
        }

        public override void AddEntity(IEntity entity)
        {
            if (entity.GetComponent<Player>() is not null)
            {
                _playerEntity = entity;
                return;
            }

            base.AddEntity(entity);
        }

        public override void RemoveEntity(IEntity entity)
        {
            if (entity.GetComponent<Player>() is not null)
            {
                _playerEntity = null;
                return;
            }

            base.RemoveEntity(entity);
        }

        public void Update(float time)
        {
            if (_playerEntity is null)
                return;

            Transform playerTransform = _playerEntity.GetComponent<Transform>();
            DynamicCollider playerDynamicCollider = _playerEntity.GetComponent<DynamicCollider>();
            foreach (IEntity enemyEntity in Entities)
            {
                Transform enemyTransform = enemyEntity.GetComponent<Transform>();
                Physics physics = enemyEntity.GetComponent<Physics>();
                physics.Weight = 0;

                Vector2 playerDirection = enemyTransform.Position - playerTransform.Position;
                Vector2 targetPosition = enemyTransform.Position;

                if (playerDirection.Length < 10)
                {
                    targetPosition = GetTargetPosition(
                    new Vector2((float)Math.Floor(enemyTransform.Position.X), (float)Math.Floor(enemyTransform.Position.Y)),
                    new Vector2((float)Math.Floor(playerTransform.Position.X), (float)Math.Floor(playerTransform.Position.Y) + 1));

                    Vector2 direction = targetPosition - new Vector2((float)Math.Floor(enemyTransform.Position.X), (float)Math.Floor(enemyTransform.Position.Y)) + new Vector2(0.125f);
                    if (direction != Vector2.Zero)
                        direction.Normalize();

                    if (targetPosition.X != 0)
                        physics.Acceleration += direction * 50f;

                    Effect effect = enemyEntity.GetComponent<Effect>();
                    if (playerDirection.Length < 2 && !effect.Effects.ContainsKey("AttackCooldown"))
                    {
                        IEntity attackEntity = new Entity(new Transform(enemyTransform.Position, 1f, 0), "Attack");
                        attackEntity.AddComponent(new Trigger(TriggerLayerType.FRIEND, Vector2.Zero, 1.0f, 1.0f, 0.1f));
                        FlyingAI enemyAI = enemyEntity.GetComponent<FlyingAI>();
                        attackEntity.AddComponent(new Damage(enemyAI.Damage));
                        Manager.GetInstance().AddEntity(attackEntity);
                        effect.Effects.Add("AttackCooldown", 1);
                    }
                }

                Random random = new Random();
                Vector2 randomAcceleration = new Vector2(((float)random.NextDouble() - 0.5f) * 5f, (float)random.NextDouble() - 0.5f) * 50f;
                physics.Acceleration += randomAcceleration;
            }
        }

        private Vector2 GetTargetPosition(Vector2 enemyPosition, Vector2 playerPosition)
        {
            int[,]? dungeonGrid = SpawnDungeon.CurrentDungeon?.Clone() as int[,];
            if (dungeonGrid is null)
                return playerPosition;

            if (playerPosition == enemyPosition)
                return playerPosition;

            if (enemyPosition.X < 0 ||
                enemyPosition.Y < 0 ||
                enemyPosition.X >= dungeonGrid.GetLength(1) ||
                enemyPosition.Y >= dungeonGrid.GetLength(0))
            return playerPosition;

            dungeonGrid[dungeonGrid.GetLength(0) - 1 - (int)enemyPosition.Y, (int)enemyPosition.X] = 167;

            List<Path> leaves = new List<Path>();
            leaves.Add(new Path(null, enemyPosition));

            Path? finishedPath = null;
            bool newPathFound = true;
            while (finishedPath is null && newPathFound)
            {
                List<Path> newLeaves = new List<Path>();
                newPathFound = false;
                foreach (Path leaf in leaves)
                {
                    Vector2 newPosition = leaf.Position + new Vector2(1f, 0f);
                    newPathFound = AddLeaf(dungeonGrid, newLeaves, leaf, newPosition, playerPosition, ref finishedPath) | newPathFound;
                    newPosition = leaf.Position - new Vector2(1f, 0f);
                    newPathFound = AddLeaf(dungeonGrid, newLeaves, leaf, newPosition, playerPosition, ref finishedPath) | newPathFound;
                    newPosition = leaf.Position + new Vector2(0f, 1f);
                    newPathFound = AddLeaf(dungeonGrid, newLeaves, leaf, newPosition, playerPosition, ref finishedPath) | newPathFound;
                    newPosition = leaf.Position - new Vector2(0f, 1f);
                    newPathFound = AddLeaf(dungeonGrid, newLeaves, leaf, newPosition, playerPosition, ref finishedPath) | newPathFound;

                    if ((newPosition - enemyPosition).Length > 20)
                    {
                        newPathFound = false;
                        break;
                    }

                    if (finishedPath is not null)
                        break;
                }

                leaves = new List<Path>(newLeaves);
            }

            if (finishedPath is not null)
            {
                while (finishedPath.LastPath!.LastPath is not null)
                    finishedPath = finishedPath.LastPath;

                return finishedPath.Position;
            }
            else
            {
                return playerPosition;
            }
        }

        private bool AddLeaf(int[,] dungeonGrid, List<Path> newLeaves, Path leaf, Vector2 newPosition, Vector2 playerPosition, ref Path? finishedPath)
        {
            if (newPosition.X >= 0 &&
                newPosition.Y > 0 &&
                newPosition.X < dungeonGrid.GetLength(1) &&
                newPosition.Y < dungeonGrid.GetLength(0) &&
                (dungeonGrid[dungeonGrid.GetLength(0) - 1 - (int)newPosition.Y, (int)newPosition.X] < 80 ||
                dungeonGrid[dungeonGrid.GetLength(0) - 1 - (int)newPosition.Y, (int)newPosition.X] > 167))
            {
                Path newLeaf = new Path(leaf, newPosition);
                if (newPosition == playerPosition)
                    finishedPath = newLeaf;

                dungeonGrid[dungeonGrid.GetLength(0) - 1 - (int)newPosition.Y, (int)newPosition.X] = 167;
                newLeaves.Add(newLeaf);
                return true;
            }

            return false;
        }

        private void PrintDungeon(int[,] dungeonGrid)
        {
            Console.WriteLine();
            Console.WriteLine();
            for (int y = 0; y < dungeonGrid.GetLength(0); y++)
            {
                for (int x = 0; x < dungeonGrid.GetLength(1); x++)
                {
                    if (dungeonGrid[y, x] < 80 || dungeonGrid[y, x] > 167)
                        Console.Write("  ");
                    else if (dungeonGrid[y, x] == 200)
                        Console.Write(" X");
                }

                Console.WriteLine();
            }
        }

        private class Path
        {
            private Path? _lastPath;
            private Vector2 _position;

            public Path(Path? lastPath, Vector2 position)
            {
                _lastPath = lastPath;
                _position = position;
            }

            public Path? LastPath => _lastPath;

            public Vector2 Position => _position;
        }
    }
}
