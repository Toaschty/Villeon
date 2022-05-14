using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.ECS;
using Villeon.Render;
using Villeon.Systems;
using Zenseless.OpenTK;

namespace Villeon.Render
{
    public interface IRenderBatch : IRender
    {
        public bool Full();

        public void AddSprite(IEntity entity);

        public bool HasTexture(Texture2D? texture);

        public bool HasTextureRoom();
    }
}
