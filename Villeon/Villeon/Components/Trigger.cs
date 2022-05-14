using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.ECS;

namespace Villeon.Components
{
    public class Trigger : IComponent
    {
        private float _width;
        private float _height;
        private float _timeActive;
        private Vector2 _offset;
        private Vector2 _position;
        private List<Func<IEntity>> _funcs;
    }
}
