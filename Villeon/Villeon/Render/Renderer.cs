using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Villeon.Components;
using Villeon.ECS;
using Villeon.Systems;
using Zenseless.OpenTK;

namespace Villeon.Render
{
    public class Renderer : Villeon.Systems.System, IRenderSystem
    {
        private const int MAX_BATCH_SIZE = 1024;
        private const int RENDER_LAYERS = 4;
        private Layer[] _renderLayers = new Layer[RENDER_LAYERS];

        // Render order
        // Background
        // Middleground
        // Foreground
        // UI

        // Inherit System
        public Renderer(string name)
            : base(name)
        {
            Signature = Signature.AddToSignature(typeof(Sprite));
            for (int i = 0; i < RENDER_LAYERS; i++)
            {
                _renderLayers[i] = new Layer();
            }
        }

        public void Add(IEntity entity)
        {
            RenderLayer renderLayer = entity.GetComponent<Sprite>().RenderLayer;
            _renderLayers[(int)renderLayer].Add(entity, MAX_BATCH_SIZE);
        }

        public void Remove(IEntity entity)
        {
            for (int i = 0; i < _renderLayers.Length; i++)
            {
                _renderLayers[i].Remove(entity);
            }
        }

        public void Render()
        {
            for (int i = 0; i < _renderLayers.Length; i++)
            {
                _renderLayers[i].Render();
            }
        }
    }
}
