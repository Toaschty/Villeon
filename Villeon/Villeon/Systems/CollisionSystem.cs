using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Villeon.Components;
using OpenTK.Mathematics;

namespace Villeon.Systems
{
    public class CollisionSystem : IUpdateSystem
    {
        public CollisionSystem(string name)
        {
            Name = name;
            Signature.Add<Transform>();
            Signature.Add<Collider>();
        }
        public string Name { get; }

        public List<IEntity> Entities { get; private set; } = new();

        public Signature Signature { get; private set; } = new();

        // Collider, Transform, Physics
        public void Update(double time)
        {
            if (Constants.DEBUGPAUSEACTIVE)
            {
                if (Constants.DEBUGNEXTFRAME)
                {
                    if (Constants.DEBUGTHISFRAMEPHYSICS)
                    {
                        Constants.DEBUGTHISFRAMEPHYSICS = false;
                        return;
                    }
                    else
                    {
                        Constants.DEBUGTHISFRAMEPHYSICS = true;
                    }
                }
                else
                    return;
            }

            List<IEntity> entities = new List<IEntity>();
            List<IEntity> dirtyEntities = new List<IEntity>();

            // Fill entity lists
            foreach (IEntity entity in Entities)
            {
                Collider collider = entity.GetComponent<Collider>();
                collider.hasCollidedLeft = false;
                collider.hasCollidedRight = false;
                collider.hasCollidedTop = false;
                collider.hasCollidedBottom = false;

                if (entity.GetComponent<Collider>().hasMoved)
                    dirtyEntities.Add(entity);
                else
                    entities.Add(entity);
            }

            // Test against clean entities
            for (int i = 0; i < dirtyEntities.Count(); i++)
            {
                bool clean = false;
                IEntity entity = dirtyEntities[i];
                Collider collider = entity.GetComponent<Collider>();

                foreach (IEntity entity2 in entities)
                {
                    Collider e2Collider = entity2.GetComponent<Collider>();

                    if (CollidesSAT(collider, e2Collider))
                        HandleCleanCollision(CollidesDirectionAABB(collider, e2Collider), entity, entity2);

                    if (collider.Position == collider.LastPosition)
                    {
                        clean = true;
                        break;
                    }
                }

                if (clean)
                {
                    entities.Add(entity);
                    dirtyEntities.RemoveAt(i);
                    i--;
                    i -= CollidesCleanedEntity(dirtyEntities, entities, entity, i, 0);
                }
            }

            // Test against dirty entities
            foreach (IEntity entity in dirtyEntities)
            {
                Collider collider = entity.GetComponent<Collider>();

                foreach (IEntity entity2 in dirtyEntities)
                {
                    if (entity != entity2)
                    {
                        Collider e2Collider = entity2.GetComponent<Collider>();

                        if (CollidesAABB(collider, e2Collider))
                            HandleDirtyCollision(CollidesDirectionAABB(collider, e2Collider), entity, entity2);
                    }
                }
            }

            foreach (IEntity entity in Entities)
            {
                Collider collider = entity.GetComponent<Collider>();
                //collider.Position = collider.Position;
                collider.hasMoved = false;
            }
        }

        private void HandleCleanCollision(Direction direction, IEntity entity, IEntity entity2)
        {
            Collider collider = entity.GetComponent<Collider>();
            Collider e2Collider = entity2.GetComponent<Collider>();

            switch (direction)
            {
                case Direction.DOWN:
                    collider.hasCollidedTop = true;
                    collider.ProposePosition = new Vector2(collider.Position.X, e2Collider.Position.Y - collider.Height);
                    break;
                case Direction.UP:
                    collider.hasCollidedBottom = true;
                    collider.ProposePosition = new Vector2(collider.Position.X, e2Collider.Position.Y + e2Collider.Height);
                    break;
                case Direction.LEFT:
                    collider.hasCollidedRight = true;
                    collider.ProposePosition = new Vector2(e2Collider.Position.X - collider.Width, collider.Position.Y);
                    break;
                case Direction.RIGHT:
                    collider.hasCollidedLeft = true;
                    collider.ProposePosition = new Vector2(e2Collider.Position.X + e2Collider.Width, collider.Position.Y);
                    break;
            }
        }

        private void HandleDirtyCollision(Direction direction, IEntity entity, IEntity entity2)
        {
            Collider collider = entity.GetComponent<Collider>();
            Collider e2Collider = entity2.GetComponent<Collider>();

            switch (direction)
            {
                case Direction.DOWN:
                    collider.hasCollidedTop = true;
                    collider.ProposePosition = new Vector2(collider.Position.X, collider.LastPosition.Y);
                    break;
                case Direction.UP:
                    collider.hasCollidedBottom = true;
                    collider.ProposePosition = new Vector2(collider.Position.X, collider.LastPosition.Y);
                    break;
                case Direction.LEFT:
                    collider.hasCollidedRight = true;
                    collider.ProposePosition = new Vector2(collider.LastPosition.X, collider.Position.Y);
                    break;
                case Direction.RIGHT:
                    collider.hasCollidedLeft = true;
                    collider.ProposePosition = new Vector2(collider.LastPosition.X, collider.Position.Y);
                    break;
            }
        }

        private int CollidesCleanedEntity(List<IEntity> dirtyEntities, List<IEntity> entities, IEntity cleanedEntity , int lastToTest, int depth)
        {
            Collider e2Collider = cleanedEntity.GetComponent<Collider>();
            int entitiesCleaned = 0;

            for (int i = 0; i <= lastToTest; i++)
            {
                if (lastToTest >= dirtyEntities.Count || i < 0)
                    return entitiesCleaned;

                IEntity entity = dirtyEntities[i];
                Collider collider = entity.GetComponent<Collider>();

                if (CollidesSAT(collider, e2Collider))
                    HandleCleanCollision(CollidesDirectionAABB(collider, e2Collider), entity, cleanedEntity);

                if (collider.Position == collider.LastPosition)
                {
                    entities.Add(entity);
                    dirtyEntities.RemoveAt(i);
                    int newEntitiesCleaned = CollidesCleanedEntity(dirtyEntities, entities, entity, i--, depth++) + 1;
                    i -= newEntitiesCleaned;
                    entitiesCleaned += newEntitiesCleaned;
                }
            }

            return entitiesCleaned;
        }

        private Direction CollidesDirectionAABB(Collider a, Collider b)
        {
            Vector2 v = new(a.Position.X - a.LastPosition.X, a.Position.Y - a.LastPosition.Y);

            // test for top side
            float mTop = (b.Position.Y + b.Height + (a.Height / 2) - a.LastCenter.Y) / v.Y;
            // test for bottom side
            float mBottom = (b.Position.Y - (a.Height / 2) - a.LastCenter.Y) / v.Y;
            // test for right side
            float mRight = (b.Position.X + b.Width + (a.Width / 2) - a.LastCenter.X) / v.X;
            // test for left side
            float mLeft = (b.Position.X - (a.Width / 2) - a.LastCenter.X) / v.X;

            int zeros = 0;
            if (mTop == 0) zeros++;
            if (mBottom == 0) zeros++;
            if (mRight == 0) zeros++;
            if (mLeft == 0) zeros++;
            if (zeros > 1)
            {
                Console.WriteLine(zeros);
                return Direction.UP;
            }

            //Check if collided already last update
            /*if(CollidesLastLastAABB(a, b))
            {
                if (mTop > 0 && float.IsFinite(mTop)) mTop = float.PositiveInfinity;
                if (mBottom > 0 && float.IsFinite(mBottom)) mBottom = float.PositiveInfinity;
                if (mRight > 0 && float.IsFinite(mRight)) mRight = float.PositiveInfinity;
                if (mLeft > 0 && float.IsFinite(mLeft)) mLeft = float.PositiveInfinity;

                mTop = MathF.Abs(mTop);
                mBottom = MathF.Abs(mBottom);
                mRight = MathF.Abs(mRight);
                mLeft = MathF.Abs(mLeft);
            }
            else
            {*/
                // if vector goes backwards it's automatically not the right direction
                if (mTop < 0 && float.IsFinite(mTop)) mTop = float.PositiveInfinity;
                if (mBottom < 0 && float.IsFinite(mBottom)) mBottom = float.PositiveInfinity;
                if (mRight < 0 && float.IsFinite(mRight)) mRight = float.PositiveInfinity;
                if (mLeft < 0 && float.IsFinite(mLeft)) mLeft = float.PositiveInfinity;
            //}      

            if (v.X == 0)
            {
                mRight = float.PositiveInfinity;
                mLeft = float.PositiveInfinity;
            }
            if (v.Y == 0)
            {
                mTop = float.PositiveInfinity;
                mBottom = float.PositiveInfinity;
            }

            if (mLeft < mBottom && mLeft < mRight && mLeft < mTop)
                return Direction.LEFT;
            if (mRight < mBottom && mRight < mTop && mRight < mLeft)
                return Direction.RIGHT;

            if (mTop < mBottom && mTop < mRight && mTop < mLeft)
                return Direction.UP;
            if (mBottom < mTop && mBottom < mRight && mBottom < mLeft)
                return Direction.DOWN;

            return Direction.NONE;
        }

        private bool CollidesAABB(Collider a, Collider b)
        {
            if (a.Position.X >= (b.Position.X + b.Width) || b.Position.X >= (a.Position.X + a.Width))
                return false;
            if (a.Position.Y >= (b.Position.Y + b.Height) || b.Position.Y >= (a.Position.Y + a.Height))
                return false;
            return true;
        }

        private bool CollidesSAT(Collider x, Collider y)
        {
            List<Vector2> polygon1 = CreatePolygon(x);
            List<Vector2> polygon2 = CreatePolygon(y);

            for (int shape = 0; shape < 2; shape++)
            {
                if (shape == 1)
                {
                    polygon1 = CreatePolygon(y);
                    polygon2 = CreatePolygon(x);
                }

                for (int a = 0; a < polygon1.Count(); a++)
                {
                    int b = (a + 1) % polygon1.Count;
                    Vector2 normalAxis = new(-(polygon1[b].Y - polygon1[a].Y), polygon1[b].X - polygon1[a].X);
                    float d = (float)Math.Sqrt((normalAxis.X * normalAxis.X) + (normalAxis.Y * normalAxis.Y));
                    normalAxis = new Vector2(normalAxis.X / d, normalAxis.Y / d);

                    float minR1 = float.PositiveInfinity;
                    float maxR1 = float.NegativeInfinity;
                    for (int i = 0; i < polygon1.Count; i++)
                    {
                        float q = (polygon1[i].X * normalAxis.X) + (polygon1[i].Y * normalAxis.Y);
                        minR1 = Math.Min(minR1, q);
                        maxR1 = Math.Max(maxR1, q);
                    }

                    float minR2 = float.PositiveInfinity;
                    float maxR2 = float.NegativeInfinity;
                    for (int i = 0; i < polygon2.Count; i++)
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

        public static List<Vector2> CreatePolygon(Collider collider)
        {
            float x = collider.Position.X;
            float y = collider.Position.Y;
            float w = collider.Width;
            float h = collider.Height;
            float lx = collider.LastPosition.X; // + (w * 0.005f);
            float ly = collider.LastPosition.Y; // + (h * 0.005f);
            float lw = w; // - (w * 0.01f);
            float lh = h; // - (h * 0.01f);

            Vector2 direction = new(x - lx, y - ly);

            List<Vector2> polygon = new();

            //didn't move
            if (direction == Vector2.Zero)
            {
                polygon.Add(new Vector2(x, y + h));
                polygon.Add(new Vector2(x + w, y + h));
                polygon.Add(new Vector2(x + w, y));
                polygon.Add(new Vector2(x, y));
            }
            //moved straight right
            else if (direction.X > 0 && direction.Y == 0)
            {
                polygon.Add(new Vector2(lx, ly));
                polygon.Add(new Vector2(x + w, y));
                polygon.Add(new Vector2(x + w, y + h));
                polygon.Add(new Vector2(lx, ly + lh));
            }
            //moved straight left
            else if (direction.X < 0 && direction.Y == 0)
            {
                polygon.Add(new Vector2(x, y));
                polygon.Add(new Vector2(lx + lw, ly));
                polygon.Add(new Vector2(lx + lw, ly + lh));
                polygon.Add(new Vector2(x, y + h));
            }
            //moved straight up
            else if (direction.X == 0 && direction.Y > 0)
            {
                polygon.Add(new Vector2(lx, ly));
                polygon.Add(new Vector2(lx + lw, ly));
                polygon.Add(new Vector2(x + w, y + h));
                polygon.Add(new Vector2(x, y + h));
            }
            //moved straight down
            else if (direction.X == 0 && direction.Y < 0)
            {
                polygon.Add(new Vector2(lx, ly + lh));
                polygon.Add(new Vector2(lx + lw, ly + lh));
                polygon.Add(new Vector2(x + w, y));
                polygon.Add(new Vector2(x, y));
            }
            //moved up right
            else if (direction.X > 0 && direction.Y > 0)
            {
                polygon.Add(new Vector2(lx, ly));
                polygon.Add(new Vector2(lx + lw, ly));
                polygon.Add(new Vector2(x + w, y));
                polygon.Add(new Vector2(x + w, y + h));
                polygon.Add(new Vector2(x, y + h));
                polygon.Add(new Vector2(lx, ly + lh));
            }
            //moved down right
            else if (direction.X > 0 && direction.Y < 0)
            {
                polygon.Add(new Vector2(lx, ly));
                polygon.Add(new Vector2(x, y));
                polygon.Add(new Vector2(x + w, y));
                polygon.Add(new Vector2(x + w, y + h));
                polygon.Add(new Vector2(lx + lw, ly + lh));
                polygon.Add(new Vector2(lx, ly + lh));
            }
            //moved up left
            else if (direction.X < 0 && direction.Y > 0)
            {
                polygon.Add(new Vector2(x, y));
                polygon.Add(new Vector2(lx, ly));
                polygon.Add(new Vector2(lx + lw, ly));
                polygon.Add(new Vector2(lx + lw, ly + lh));
                polygon.Add(new Vector2(x + w, y + h));
                polygon.Add(new Vector2(x, y + h));
            }
            //moved down left
            else if (direction.X < 0 && direction.Y < 0)
            {
                polygon.Add(new Vector2(x, y));
                polygon.Add(new Vector2(x + w, y));
                polygon.Add(new Vector2(lx + lw, ly));
                polygon.Add(new Vector2(lx + lw, ly + lh));
                polygon.Add(new Vector2(lx, ly + lh));
                polygon.Add(new Vector2(x, y + h));
            }

            return polygon;
        }

        private void printList(List<IEntity> list, string name)
        {
            Console.Write(name + ": ");
            foreach (IEntity entity in list)
            {
                Console.Write(entity.Name + " ");
            }
            Console.WriteLine();
        }

        enum Direction
        {
            UP, DOWN, LEFT, RIGHT, NONE
        }
    }
}
