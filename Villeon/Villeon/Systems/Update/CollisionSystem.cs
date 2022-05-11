using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;

namespace Villeon.Systems
{
    public class CollisionSystem : System, IUpdateSystem
    {
        public CollisionSystem(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Collider));
        }

        private enum Direction
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            NONE,
        }

        // Collider, Transform, Physics
        public void Update(float time)
        {
            if (StateManager.DEBUGPAUSEACTIVE)
            {
                if (StateManager.DEBUGNEXTFRAME)
                {
                    if (StateManager.DEBUGTHISFRAMEPHYSICS)
                    {
                        StateManager.DEBUGTHISFRAMEPHYSICS = false;
                        return;
                    }
                    else
                    {
                        StateManager.DEBUGTHISFRAMEPHYSICS = true;
                    }
                }
                else
                {
                    return;
                }
            }

            List<Collider> entitiesCollider = new List<Collider>();
            List<Collider> dirtyEntitiesCollider = new List<Collider>();

            // Fill entity lists
            foreach (IEntity entity in Entities)
            {
                Collider collider = entity.GetComponent<Collider>();
                Transform transform = entity.GetComponent<Transform>();

                collider.Position = transform.Position + collider.Offset;

                if (transform.Position == collider.LastPosition)
                    collider.HasMoved = false;
                else
                    collider.HasMoved = true;

                collider.HasCollidedLeft = false;
                collider.HasCollidedRight = false;
                collider.HasCollidedTop = false;
                collider.HasCollidedBottom = false;

                if (collider.HasMoved)
                    dirtyEntitiesCollider.Add(collider);
                else
                    entitiesCollider.Add(collider);
            }

            // Test against clean entities
            for (int i = 0; i < dirtyEntitiesCollider.Count(); i++)
            {
                if (i < 0) break;

                bool clean = false;
                Collider collider = dirtyEntitiesCollider[i];

                foreach (Collider e2Collider in entitiesCollider)
                {
                    if (CollidesSAT(collider, e2Collider))
                        HandleCleanCollision(CollidesDirectionAABB(collider, e2Collider), collider, e2Collider);

                    if (collider.Position == collider.LastPosition)
                    {
                        clean = true;
                        break;
                    }
                }

                if (clean)
                {
                    entitiesCollider.Add(collider);
                    dirtyEntitiesCollider.RemoveAt(i);
                    i--;
                    i -= CollidesCleanedEntity(dirtyEntitiesCollider, entitiesCollider, collider, i, 0);
                }
            }

            // Test against dirty entities
            foreach (Collider collider in dirtyEntitiesCollider)
            {
                foreach (Collider e2Collider in dirtyEntitiesCollider)
                {
                    if (collider != e2Collider)
                    {
                        if (CollidesAABB(collider, e2Collider))
                            HandleDirtyCollision(CollidesDirectionAABB(collider, e2Collider), collider);
                    }
                }
            }

            foreach (IEntity entity in Entities)
            {
                Collider collider = entity.GetComponent<Collider>();

                if (collider.HasMoved)
                {
                    Transform transform = entity.GetComponent<Transform>();
                    transform.Position = collider.Position - collider.Offset;
                }

                collider.LastPosition = collider.Position;
            }
        }

        private void HandleCleanCollision(Direction direction, Collider collider, Collider e2Collider)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    collider.HasCollidedTop = true;
                    collider.Position = new Vector2(collider.Position.X, e2Collider.Position.Y - collider.Height);
                    break;
                case Direction.UP:
                    StateManager.IsGrounded = true;

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
        }

        private void HandleDirtyCollision(Direction direction, Collider collider)
        {
            switch (direction)
            {
                case Direction.DOWN:
                    collider.HasCollidedTop = true;
                    collider.Position = new Vector2(collider.Position.X - collider.Offset.X, collider.LastPosition.Y - collider.Offset.Y);
                    break;
                case Direction.UP:
                    StateManager.IsGrounded = true;
                    collider.HasCollidedBottom = true;
                    collider.Position = new Vector2(collider.Position.X - collider.Offset.X, collider.LastPosition.Y - collider.Offset.Y);
                    break;
                case Direction.LEFT:
                    collider.HasCollidedRight = true;
                    collider.Position = new Vector2(collider.LastPosition.X - collider.Offset.X, collider.Position.Y - collider.Offset.Y);
                    break;
                case Direction.RIGHT:
                    collider.HasCollidedLeft = true;
                    collider.Position = new Vector2(collider.LastPosition.X - collider.Offset.X, collider.Position.Y - collider.Offset.Y);
                    break;
            }
        }

        private int CollidesCleanedEntity(List<Collider> dirtyEntitiesCollider, List<Collider> entitiesCollider, Collider cleanedEntityCollider, int lastToTest, int depth)
        {
            int entitiesCleaned = 0;
            for (int i = 0; i <= lastToTest; i++)
            {
                if (lastToTest >= dirtyEntitiesCollider.Count || i < 0)
                    return entitiesCleaned;

                Collider dirtyEntitiyCollider = dirtyEntitiesCollider[i];

                if (CollidesSAT(dirtyEntitiyCollider, cleanedEntityCollider))
                    HandleCleanCollision(CollidesDirectionAABB(dirtyEntitiyCollider, cleanedEntityCollider), dirtyEntitiyCollider, cleanedEntityCollider);

                if (dirtyEntitiyCollider.Position == dirtyEntitiyCollider.LastPosition)
                {
                    entitiesCollider.Add(dirtyEntitiyCollider);
                    dirtyEntitiesCollider.RemoveAt(i);
                    int newEntitiesCleaned = CollidesCleanedEntity(dirtyEntitiesCollider, entitiesCollider, dirtyEntitiyCollider, i--, depth++) + 1;
                    i -= newEntitiesCleaned;
                    entitiesCleaned += newEntitiesCleaned;
                }
            }

            return entitiesCleaned;
        }

        private Direction CollidesDirectionAABB(Collider a, Collider b)
        {
            Vector2 v = new (a.Position.X - a.LastPosition.X, a.Position.Y - a.LastPosition.Y);

            // test for top side
            float mulitplierTop = (b.Position.Y + b.Height + (a.Height / 2) - a.LastCenter.Y) / v.Y;

            // test for bottom side
            float mulitplierBottom = (b.Position.Y - (a.Height / 2) - a.LastCenter.Y) / v.Y;

            // test for right side
            float mulitplierRight = (b.Position.X + b.Width + (a.Width / 2) - a.LastCenter.X) / v.X;

            // test for left side
            float mulitplierLeft = (b.Position.X - (a.Width / 2) - a.LastCenter.X) / v.X;

            int zeros = 0;
            if (mulitplierTop == 0) zeros++;
            if (mulitplierBottom == 0) zeros++;
            if (mulitplierRight == 0) zeros++;
            if (mulitplierLeft == 0) zeros++;
            if (zeros > 1)
            {
                Console.WriteLine(zeros);
                return Direction.UP;
            }

            //Check if collided already last update
            if (CollidesLastLastAABB(a, b))
            {
                if (mulitplierTop > 0 && float.IsFinite(mulitplierTop)) mulitplierTop = float.PositiveInfinity;
                if (mulitplierBottom > 0 && float.IsFinite(mulitplierBottom)) mulitplierBottom = float.PositiveInfinity;
                if (mulitplierRight > 0 && float.IsFinite(mulitplierRight)) mulitplierRight = float.PositiveInfinity;
                if (mulitplierLeft > 0 && float.IsFinite(mulitplierLeft)) mulitplierLeft = float.PositiveInfinity;

                mulitplierTop = MathF.Abs(mulitplierTop);
                mulitplierBottom = MathF.Abs(mulitplierBottom);
                mulitplierRight = MathF.Abs(mulitplierRight);
                mulitplierLeft = MathF.Abs(mulitplierLeft);
            }
            else
            {
                // if vector goes backwards it's automatically not the right direction
                if (mulitplierTop < 0 && float.IsFinite(mulitplierTop)) mulitplierTop = float.PositiveInfinity;
                if (mulitplierBottom < 0 && float.IsFinite(mulitplierBottom)) mulitplierBottom = float.PositiveInfinity;
                if (mulitplierRight < 0 && float.IsFinite(mulitplierRight)) mulitplierRight = float.PositiveInfinity;
                if (mulitplierLeft < 0 && float.IsFinite(mulitplierLeft)) mulitplierLeft = float.PositiveInfinity;
            }

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

        private bool CollidesLastLastAABB(Collider a, Collider b)
        {
            if (a.LastPosition.X >= (b.LastPosition.X + b.Width) || b.LastPosition.X >= (a.LastPosition.X + a.Width))
                return false;
            if (a.LastPosition.Y >= (b.LastPosition.Y + b.Height) || b.LastPosition.Y >= (a.LastPosition.Y + a.Height))
                return false;
            return true;
        }

        private bool CollidesAABB(Collider a, Collider b)
        {
            if (a.Position.X >= (b.Position.X + b.Width) || b.Position.X >= (a.Position.X + a.Width))
                return false;
            if (a.Position.Y >= (b.Position.Y + b.Height) || b.Position.Y >= (a.Position.Y + a.Height))
                return false;
            return true;
        }

        private bool CollidesSAT(Collider a, Collider b)
        {
            Vector2[] polygon1 = a.GetPolygon();
            Vector2[] polygon2 = b.GetPolygon();
            int polygon1Size = a.PolygonSize;
            int polygon2Size = b.PolygonSize;

            for (int shape = 0; shape < 2; shape++)
            {
                if (shape == 1)
                {
                    Vector2[] temp = polygon1;
                    polygon1 = polygon2;
                    polygon2 = temp;
                    polygon1Size = b.PolygonSize;
                    polygon2Size = a.PolygonSize;
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
