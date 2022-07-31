using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.Update
{
    public class CollisionSystem : System, IUpdateSystem
    {
        private List<Collider> _colliders = new List<Collider>();
        private HashSet<Tuple<DynamicCollider, Transform>> _dynamicTuple = new HashSet<Tuple<DynamicCollider, Transform>>();
        private IEntity? _player = null;

        public CollisionSystem(string name)
            : base(name)
        {
            Signature.IncludeOR(typeof(Collider), typeof(DynamicCollider))
                .IncludeAND(typeof(Collider), typeof(DynamicCollider), typeof(Player));
        }

        private enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            NONE,
        }

        public override void AddEntity(IEntity entity)
        {
            base.AddEntity(entity);

            Collider collider = entity.GetComponent<Collider>();
            DynamicCollider dynamicCollider = entity.GetComponent<DynamicCollider>();

            if (dynamicCollider is not null)
            {
                Transform dynamicTransform = entity.GetComponent<Transform>();
                dynamicCollider.Position = dynamicTransform.Position + dynamicCollider.Offset;
                dynamicCollider.ResetHasCollided();
                _dynamicTuple.Add(Tuple.Create(dynamicCollider, dynamicTransform));
            }

            if (collider is not null)
            {
                _colliders.Add(collider);
            }

            if (entity.HasComponent<Player>())
            {
                _player = entity;
            }
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);

            Collider collider = entity.GetComponent<Collider>();
            DynamicCollider dynamicCollider = entity.GetComponent<DynamicCollider>();
            if (dynamicCollider is not null)
            {
                Transform dynamicTransform = entity.GetComponent<Transform>();

                // Remove tuple with the dynamicCollider
                var tupleToRemove = Tuple.Create(dynamicCollider, dynamicTransform);
                _dynamicTuple.Remove(tupleToRemove);
            }

            if (collider is not null)
            {
                _colliders.Remove(collider);
            }
        }

        // Collider, Transform, Physics
        public void Update(float time)
        {
            UpdateColliderPosition();
            CheckAndResolveCollisions();
            UpdateTransformPositions();
        }

        private void CheckAndResolveCollisions()
        {
            foreach (var tuple in _dynamicTuple)
            {
                DynamicCollider dynamicCollider = tuple.Item1;

                foreach (Collider e2collider in _colliders)
                {
                    if (CollidesSAT(dynamicCollider, e2collider))
                    {
                        Direction direction = CollidesDirectionAABB(dynamicCollider, e2collider);
                        HandleCleanCollision(direction, dynamicCollider, e2collider);
                    }

                    if (dynamicCollider.Position == dynamicCollider.LastPosition)
                        break;
                }
            }
        }

        private void UpdateColliderPosition()
        {
            foreach (var tuple in _dynamicTuple)
            {
                // Skip this entity if player isn't in range
                if (!IsInRangeOfPlayer(tuple.Item2.Position))
                    continue;

                // Set Collider position
                tuple.Item1.Position = tuple.Item2.Position + tuple.Item1.Offset;

                // Reset Collider bool
                tuple.Item1.ResetHasCollided();
            }
        }

        private void UpdateTransformPositions()
        {
            foreach (var tuple in _dynamicTuple)
            {
                // Set Dynamic Transform
                tuple.Item2.Position = tuple.Item1.Position - tuple.Item1.Offset;

                // Set Dynamic Collider Last position from Collider
                tuple.Item1.LastPosition = tuple.Item1.Position;
            }
        }

        private void HandleCleanCollision(Direction direction, DynamicCollider dynamicCollider, Collider e2Collider)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    dynamicCollider.HasCollidedTop = true;
                    dynamicCollider.Position = new Vector2(dynamicCollider.Position.X, e2Collider.Position.Y - dynamicCollider.Height);
                    break;
                case Direction.UP:
                    dynamicCollider.HasCollidedBottom = true;
                    dynamicCollider.Position = new Vector2(dynamicCollider.Position.X, e2Collider.Position.Y + e2Collider.Height);
                    break;
                case Direction.LEFT:
                    dynamicCollider.HasCollidedRight = true;
                    dynamicCollider.Position = new Vector2(e2Collider.Position.X - dynamicCollider.Width, dynamicCollider.Position.Y);
                    break;
                case Direction.RIGHT:
                    dynamicCollider.HasCollidedLeft = true;
                    dynamicCollider.Position = new Vector2(e2Collider.Position.X + e2Collider.Width, dynamicCollider.Position.Y);
                    break;
            }
        }

        private Direction CollidesDirectionAABB(DynamicCollider da, Collider b)
        {
            Vector2 v = new (da.Position.X - da.LastPosition.X, da.Position.Y - da.LastPosition.Y);

            // test for top side
            float mulitplierTop = (b.Position.Y + b.Height + (da.Height / 2) - da.LastCenter.Y) / v.Y;

            // test for bottom side
            float mulitplierBottom = (b.Position.Y - (da.Height / 2) - da.LastCenter.Y) / v.Y;

            // test for right side
            float mulitplierRight = (b.Position.X + b.Width + (da.Width / 2) - da.LastCenter.X) / v.X;

            // test for left side
            float mulitplierLeft = (b.Position.X - (da.Width / 2) - da.LastCenter.X) / v.X;

            int zeros = (Convert.ToInt32(mulitplierTop == 0) * 4) +
                                    (Convert.ToInt32(mulitplierLeft == 0) * 4) +
                                    (Convert.ToInt32(mulitplierRight == 0) * 3) +
                                    (Convert.ToInt32(mulitplierBottom == 0) * 2);
            if (zeros > 6)
                return Direction.UP;
            else if (zeros > 5)
                return Direction.LEFT;
            else if (zeros > 4)
                return Direction.RIGHT;

            CheckEdgeCases(v, ref mulitplierTop, ref mulitplierBottom, ref mulitplierRight, ref mulitplierLeft);

            if (mulitplierLeft < mulitplierBottom && mulitplierLeft < mulitplierRight && mulitplierLeft < mulitplierTop)
                return Direction.LEFT;
            if (mulitplierRight < mulitplierBottom && mulitplierRight < mulitplierTop && mulitplierRight < mulitplierLeft)
                return Direction.RIGHT;

            if (mulitplierTop < mulitplierBottom && mulitplierTop < mulitplierRight && mulitplierTop < mulitplierLeft)
                return Direction.UP;

            return Direction.DOWN;
        }

        private void CheckEdgeCases(Vector2 v, ref float mulitplierTop, ref float mulitplierBottom, ref float mulitplierRight, ref float mulitplierLeft)
        {
            // if vector goes backwards it's automatically not the right direction
            if (mulitplierTop < 0 && float.IsFinite(mulitplierTop)) mulitplierTop = float.PositiveInfinity;
            if (mulitplierBottom < 0 && float.IsFinite(mulitplierBottom)) mulitplierBottom = float.PositiveInfinity;
            if (mulitplierRight < 0 && float.IsFinite(mulitplierRight)) mulitplierRight = float.PositiveInfinity;
            if (mulitplierLeft < 0 && float.IsFinite(mulitplierLeft)) mulitplierLeft = float.PositiveInfinity;

            if (v.X == 0)
            {
                mulitplierRight = float.PositiveInfinity;
                mulitplierLeft = float.PositiveInfinity;
            }

            if (v.Y == 0)
            {
                mulitplierTop = float.PositiveInfinity;
                mulitplierBottom = float.PositiveInfinity;
            }
        }

        private bool CollidesSAT(DynamicCollider da, Collider b)
        {
            Vector2[] polygon1 = da.GetPolygon();
            Vector2[] polygon2 = b.GetPolygon();
            int polygon1Size = da.PolygonSize;
            int polygon2Size = 4;

            for (int shape = 0; shape < 2; shape++)
            {
                if (shape == 1)
                {
                    Vector2[] temp = polygon1;
                    polygon1 = polygon2;
                    polygon2 = temp;
                    polygon1Size = 4;
                    polygon2Size = da.PolygonSize;
                }

                for (int v1 = 0; v1 < polygon1Size; v1++)
                {
                    int v2 = (v1 + 1) % polygon1Size;
                    Vector2 normalAxis = new (-(polygon1[v2].Y - polygon1[v1].Y), polygon1[v2].X - polygon1[v1].X);
                    float d = (float)Math.Sqrt((normalAxis.X * normalAxis.X) + (normalAxis.Y * normalAxis.Y));
                    normalAxis /= d;

                    float minR1 = float.PositiveInfinity;
                    float maxR1 = float.NegativeInfinity;
                    for (int i = 0; i < polygon1Size; i++)
                    {
                        float q = (polygon1[i].X * normalAxis.X) + (polygon1[i].Y * normalAxis.Y);
                        minR1 = Math.Min(minR1, q);
                        maxR1 = Math.Max(maxR1, q);
                    }

                    float minR2 = float.PositiveInfinity;
                    float maxR2 = float.NegativeInfinity;
                    for (int i = 0; i < polygon2Size; i++)
                    {
                        float q = (polygon2[i].X * normalAxis.X) + (polygon2[i].Y * normalAxis.Y);
                        minR2 = Math.Min(minR2, q);
                        maxR2 = Math.Max(maxR2, q);
                    }

                    if (!(maxR2 > minR1 && maxR1 > minR2)) return false;
                }
            }

            return true;
        }

        private bool IsInRangeOfPlayer(Vector2 colliderEntityPosition)
        {
            if (_player is null)
                return false;

            Transform playerTransform = _player.GetComponent<Transform>();
            Vector2 distance = playerTransform.Position - colliderEntityPosition;

            // Length between player and enemy
            float length = distance.LengthFast;
            if (length < 20)
                return true;

            return false;
        }
    }
}
