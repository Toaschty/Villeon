// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Villeon
{
    public class Start
    {
        public static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }

    // Way to use Graphics component, or maybe use a different approach
        // -> GetComponent("Graphics") for example?
    // Way to make objects visible / invisible
    // Debugging Renderer (Collission boxes, etc) -> Easy with current solution! 

    // Object Spawner class -> Putting objects into the list
    // Rendering / Transforming with camera in a good way?

    // Needed
    // Viewport, Collision / Collision Boxes, Terrain/Blocks, Movement, Physics
    // Camera / Transformation, 

}