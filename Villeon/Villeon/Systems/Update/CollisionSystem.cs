using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;

namespace Villeon.Systems
{
    public class CollisionSystem : System, IUpdateSystem
    {
        public static int CollisionChecks = 0;
        private List<IEntity> _colliderEntities = new List<IEntity>();
        private List<IEntity> _dynamicColliderEntities = new List<IEntity>();

        public CollisionSystem(string name)
            : base(name)
        {
            Signature.IncludeOR(typeof(Collider), typeof(DynamicCollider));

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

            if (entity.GetComponent<Collider>() is not null)
            {
                _colliderEntities.Add(entity);
            }

            if (entity.GetComponent<DynamicCollider>() is not null)
            {
                _dynamicColliderEntities.Add(entity);
                Console.WriteLine("Added DynamicCollider: " + entity.Name);
            }
        }

        public override void RemoveEntity(IEntity entity)
        {
            base.RemoveEntity(entity);
        }

        // Collider, Transform, Physics
        public void Update(float time)
        {
            List<Collider> entitiesCollider = new List<Collider>();
            List<Collider> dynamicEntitiesCollider = new List<Collider>();
            List<DynamicCollider> dynamicColliders = new List<DynamicCollider>();

            FillEntityLists(entitiesCollider, dynamicEntitiesCollider, dynamicColliders);

            CheckAndResolveCollisions(entitiesCollider, dynamicEntitiesCollider, dynamicColliders);

            UpdatePositions();
        }

        private void CheckAndResolveCollisions(List<Collider> entitiesCollider, List<Collider> dynamicEntitiesCollider, List<DynamicCollider> dynamicColliders)
        {
            for (int i = 0; i < dynamicEntitiesCollider.Count(); i++)
            {
                Collider collider = dynamicEntitiesCollider[i];

                foreach (Collider e2Collider in entitiesCollider)
                {
                    if (CollidesSAT(collider, dynamicColliders[i], e2Collider))
                        HandleCleanCollision(CollidesDirectionAABB(collider, dynamicColliders[i], e2Collider), collider, dynamicColliders[i], e2Collider);

                    if (collider.Position == dynamicColliders[i].LastPosition)
                        break;
                }
            }
        }

        private void FillEntityLists(List<Collider> entitiesCollider, List<Collider> dynamicEntitiesCollider, List<DynamicCollider> dynamicColliders)
        {
            foreach (IEntity entity in Entities)
            {
                Collider collider = entity.GetComponent<Collider>();

                collider.Position = entity.GetComponent<Transform>().Position + collider.Offset;

                collider.ResetHasCollided();

                DynamicCollider dynamicCollider = entity.GetComponent<DynamicCollider>();
                if (dynamicCollider != null)
                {
                    dynamicCollider.Position = collider.Position;
                    dynamicEntitiesCollider.Add(collider);
                    dynamicColliders.Add(dynamicCollider);
                }
                else
                {
                    entitiesCollider.Add(collider);
                }
            }
        }

        private void UpdatePositions()
        {
            foreach (IEntity entity in Entities)
            {
                DynamicCollider dynamicCollider = entity.GetComponent<DynamicCollider>();
                if (dynamicCollider != null)
                {
                    Collider collider = entity.GetComponent<Collider>();
                    entity.GetComponent<Transform>().Position = collider.Position - collider.Offset;
                    dynamicCollider.LastPosition = collider.Position;
                }
            }
        }

        private void HandleCleanCollision(Direction direction, Collider collider, DynamicCollider dynamicCollider, Collider e2Collider)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    collider.HasCollidedTop = true;
                    collider.Position = new Vector2(collider.Position.X, e2Collider.Position.Y - collider.Height);
                    break;
                case Direction.UP:
                    collider.HasCollidedBottom = true;
                    collider.Position = new Vector2(collider.Position.X, e2Collider.Position.Y + e2Collider.Height);
                    break;
                case Direction.LEFT:
                    collider.HasCollidedRight = true;
                    collider.Position = new Vector2(e2Collider.Position.X - collider.Width, collider.Position.Y);
                    break;
                case Direction.RIGHT:
                    collider.HasCollidedLeft = true;
                    collider.Position = new Vector2(e2Collider.Position.X + e2Collider.Width, collider.Position.Y);
                    break;
            }

            dynamicCollider.Position = collider.Position;
        }

        private Direction CollidesDirectionAABB(Collider a, DynamicCollider da, Collider b)
        {
            Vector2 v = new (a.Position.X - da.LastPosition.X, a.Position.Y - da.LastPosition.Y);

            // test for top side
            float mulitplierTop = (b.Position.Y + b.Height + (a.Height / 2) - da.LastCenter.Y) / v.Y;

            // test for bottom side
            float mulitplierBottom = (b.Position.Y - (a.Height / 2) - da.LastCenter.Y) / v.Y;

            // test for right side
            float mulitplierRight = (b.Position.X + b.Width + (a.Width / 2) - da.LastCenter.X) / v.X;

            // test for left side
            float mulitplierLeft = (b.Position.X - (a.Width / 2) - da.LastCenter.X) / v.X;

            int zeros = Convert.ToInt32(mulitplierTop == 0) +
                        Convert.ToInt32(mulitplierBottom == 0) +
                        Convert.ToInt32(mulitplierRight == 0) +
                        Convert.ToInt32(mulitplierLeft == 0);
            if (zeros > 1)
            {
                Console.WriteLine(zeros);
                return Direction.UP;
            }

            CheckEdgeCases(v, ref mulitplierTop, ref mulitplierBottom, ref mulitplierRight, ref mulitplierLeft);

            if (mulitplierLeft < mulitplierBottom && mulitplierLeft < mulitplierRight && mulitplierLeft < mulitplierTop)
                return Direction.LEFT;
            if (mulitplierRight < mulitplierBottom && mulitplierRight < mulitplierTop && mulitplierRight < mulitplierLeft)
                return Direction.RIGHT;

            if (mulitplierTop < mulitplierBottom && mulitplierTop < mulitplierRight && mulitplierTop < mulitplierLeft)
                return Direction.UP;
            if (mulitplierBottom < mulitplierTop && mulitplierBottom < mulitplierRight && mulitplierBottom < mulitplierLeft)
                return Direction.DOWN;

            return Direction.NONE;
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

        private bool CollidesSAT(Collider a, DynamicCollider da, Collider b)
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
    }
}
