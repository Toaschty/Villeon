﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.Generation;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class MouseClickSystem : System, IUpdateSystem
    {
        private EnemyBuilder _entitySpawner = new EnemyBuilder();

        public MouseClickSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Collider));
        }

        public void Update(float time)
        {
        }
    }
}