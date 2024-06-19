using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.MDL.Types
{
    public class Texture
    {
        /// <summary>
        /// The name of the texture
        /// </summary>
        public string Name { get; private set; }

        public int Flags { get; private set; }

        /// <summary>
        /// The width of the texture
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The height of the texture
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The offset in the MDL file to the pixel data
        /// </summary>
        public int PixelsOffset { get; private set; }

        public Texture2D ReadTexture { get; set; }

        public Texture(string name, int flags, int width, int height, int pixelsOffset)
        {
            Name = name;
            Flags = flags;
            Width = width;
            Height = height;
            PixelsOffset = pixelsOffset;
        }
    }
}