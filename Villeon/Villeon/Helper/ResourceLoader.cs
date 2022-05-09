namespace Villeon.Helper
{
    using System.IO;
    using Zenseless.OpenTK;
    using Zenseless.Resources;

    // You HAVE to set the "Build Action" property to "embedded resource" to be able to find and load your wonderful file
    public class ResourceLoader
    {
        private static EmbeddedResourceDirectory ResourceDirectory { get; set; } = new EmbeddedResourceDirectory("Villeon.Content");

        // Load in Resource as Stream from given path starting at Content, e.g. "TileMap.TilesetImages.*.png"
        public static Stream LoadContentAsStream(string name)
        {
            return ResourceDirectory.Resource(name).Open();
        }

        // Load in Resource as Texture2D from given path starting at Content, e.g. "TileMap.TilesetImages.*.png"
        public static Texture2D LoadContentAsTexture2D(string name)
        {
            return Texture2DLoader.Load(LoadContentAsStream(name));
        }
    }
}