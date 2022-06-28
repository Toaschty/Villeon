using System;
using System.Collections.Generic;
using Villeon.Components;
using Villeon.EntityManagement;

namespace Villeon.Systems.TriggerSystems
{
    public class TriggerLayer
    {
        private HashSet<IEntity> _actors = new HashSet<IEntity>();
        private HashSet<IEntity> _receivers = new HashSet<IEntity>();

        /// <summary>
        /// Gets the Collision pair of an actor and a receiver.
        /// </summary>
        /// <value>
        /// Item1: actor
        /// Item2: receiver.
        /// </value>
        public List<Tuple<IEntity, IEntity>> Collisions
        {
            get
            {
                List<Tuple<IEntity, IEntity>> collisionPair = new List<Tuple<IEntity, IEntity>>();
                foreach (IEntity actor in _actors)
                {
                    foreach (IEntity receiver in _receivers)
                    {
                        if (Collides(actor, receiver))
                            collisionPair.Add(Tuple.Create(actor, receiver));
                    }
                }

                return collisionPair;
            }
        }

        public void AddActingEntiy(IEntity entity)
        {
            _actors.Add(entity);
        }

        public void AddReceiverEntiy(IEntity entity)
        {
            _receivers.Add(entity);
        }

        public void RemoveEntity(IEntity entity)
        {
            if (_actors.Contains(entity))
            {
                entity.GetComponent<Trigger>().ToBeRemoved = true;
                _actors.Remove(entity);
            }

            if (_receivers.Contains(entity))
            {
                entity.GetComponent<Trigger>().ToBeRemoved = true;
                _receivers.Remove(entity);
            }
        }

        private bool Collides(IEntity entityA, IEntity entityB)
        {
            Trigger triggerA = entityA.GetComponent<Trigger>();
            Trigger triggerB = entityB.GetComponent<Trigger>();
            return CollidesAABB(triggerA, triggerB);
        }

        private bool CollidesAABB(Trigger a, Trigger b)
        {
            if (a.Position.X >= b.Position.X + b.Width || b.Position.X >= a.Position.X + a.Width)
                return false;
            if (a.Position.Y >= b.Position.Y + b.Height || b.Position.Y >= a.Position.Y + a.Height)
                return false;
            return true;
        }
    }
}