﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using Villeon.Components;
using Villeon.Render;
using Villeon.Utils;

namespace Villeon.EntityManagement
{
    public class Scene : IUpdate, IRender
    {
        private HashSet<IEntity> _entities = new ();
        private HashSet<IUpdateSystem> _updateSystems = new ();
        private HashSet<IRenderSystem> _renderSystems = new ();
        private Func<bool>? _startUp;

        public Scene(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void AddSystem(ISystem system)
        {
            foreach (IUpdateSystem updateSystem in _updateSystems)
            {
                // Dont add the system if the system already exists
                if (updateSystem.GetType() == system.GetType())
                    return;
            }

            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                // Dont add the system if the system already exists
                if (renderSystem.GetType() == system.GetType())
                    return;
            }

            if (system is IUpdateSystem)
                _updateSystems.Add((IUpdateSystem)system);

            if (system is IRenderSystem)
                _renderSystems.Add((IRenderSystem)system);

            // Make sure, every system has its assigned Entities
            foreach (IEntity entity in _entities)
            {
                if (system.Signature.Contains(entity.Signature))
                    system.AddEntity(entity);
            }
        }

        public void EntityComponentAdded(IEntity entity)
        {
            AddToSystems(entity);
        }

        public void EntityComponentRemoved<T>(IEntity entity)
            where T : class, IComponent
        {
            foreach (IUpdateSystem updateSystem in _updateSystems)
            {
                if (updateSystem.Signature.Contains(TypeRegistry.GetFlag(typeof(T))))
                    updateSystem.RemoveEntity(entity);
            }

            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                if (renderSystem.Signature.Contains(TypeRegistry.GetFlag(typeof(T))))
                    renderSystem.RemoveEntity(entity);
            }
        }

        public bool RemoveSystem(ISystem system)
        {
            bool removed = false;
            if (system is IUpdateSystem)
                removed = _updateSystems.Remove((IUpdateSystem)system);

            if (system is IRenderSystem)
                removed = _renderSystems.Remove((IRenderSystem)system);

            return removed;
        }

        public void AddEntity(IEntity entity)
        {
            _entities.Add(entity);
            AddToSystems(entity);
        }

        public void AddEntities(params IEntity[] entities)
        {
            foreach (IEntity entity in entities)
            {
                _entities.Add(entity);
                AddToSystems(entity);
            }
        }

        public bool RemoveEntity(IEntity entity)
        {
            if (!_entities.Contains(entity))
                return false;
            bool removed = false;
            removed = _entities.Remove(entity);
            RemoveFromSystems(entity);
            return removed;
        }

        public void RemoveAllEntities()
        {
            foreach (IEntity entity in _entities)
            {
                RemoveEntity(entity);
            }
        }

        public HashSet<IUpdateSystem> GetUpdateSystems() => _updateSystems;

        public HashSet<IRenderSystem> GetRenderSystems() => _renderSystems;

        public HashSet<IEntity> GetEntities() => _entities;

        public void Update(float time)
        {
            foreach (IUpdateSystem system in _updateSystems)
            {
                system.Update(time);
            }
        }

        public void Render()
        {
            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                renderSystem.Render();
            }
        }

        public void AddStartUpFunc(Func<bool> startUp)
        {
            _startUp = startUp;
        }

        public void StartUp()
        {
            if (_startUp != null)
                _startUp();
        }

        private void AddToSystems(IEntity entity)
        {
            foreach (ISystem system in _updateSystems)
            {
                if (!system.Entities.Contains(entity))
                {
                    if (system.Signature.Contains(entity.Signature))
                        system.AddEntity(entity);
                }
            }

            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                if (!renderSystem.Contains(entity))
                {
                    if (renderSystem.Signature.Contains(entity.Signature))
                        renderSystem.AddEntity(entity);
                }
            }
        }

        private void RemoveFromSystems(IEntity entity)
        {
            foreach (IUpdateSystem updateSystem in _updateSystems)
            {
                if (updateSystem.Entities.Contains(entity))
                    updateSystem.RemoveEntity(entity);
            }

            foreach (IRenderSystem renderSystem in _renderSystems)
            {
                if (renderSystem.Contains(entity))
                    renderSystem.RemoveEntity(entity);
            }
        }
    }
}
