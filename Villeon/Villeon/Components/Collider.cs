using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class Collider : IComponent
    {
        public Collider(Vector2 position, float width, float height)
        {
            this.Position = position;
            this.LastPosition = position;
            this.Width = width;
            this.Height = height;
            this.LastCenter = new Vector2(position.X + (width / 2), position.Y + (height / 2));
            this.Center = new Vector2(position.X + (width / 2), position.Y + (height / 2));
        }

        public bool hasMoved = false;
        public bool hasCollidedTop = false;
        public bool hasCollidedBottom = false;
        public bool hasCollidedLeft = false;
        public bool hasCollidedRight = false;

        public Vector2 LastPosition { get; private set; }

        private Vector2 position;

        public Vector2 Position
        { 
            get 
            {
                return position; 
            }
            set 
            {
                if (!hasMoved)
                {
                    hasMoved = true;
                    LastPosition = position;
                    LastCenter = new Vector2(position.X + Width / 2, position.Y + Height / 2);
                }
                position = value;
                Center = new Vector2(position.X + Width / 2, position.Y + Height / 2);
            }
        }

        public Vector2 LastCenter { get; private set; }

        public Vector2 Center { get; private set; }

        public float Width;

        public float Height;
    }
}
