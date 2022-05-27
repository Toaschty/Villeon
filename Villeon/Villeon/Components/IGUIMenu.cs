﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.ECS;

namespace Villeon.Components
{
    public interface IGUIMenu
    {
        public IEntity[] GetEntities();

        public bool OnKeyReleased(Keys key);
    }
}
