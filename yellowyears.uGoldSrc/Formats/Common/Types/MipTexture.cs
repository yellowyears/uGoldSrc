using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.Common.Types
{
    public class MipTexture
    {
        /// <summary>
        /// The texture stored within the MipTexture
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Null-terminated texture name
        /// </summary>
        public string TextureName { get; private set; }

        /// <summary>
        /// The name of the folder that the WAD was stored in
        /// </summary>
        public string WadFolderName { get; set; }

        /// <summary>
        /// The name of the WAD that this was read from
        /// </summary>
        public string WadName { get; set; }

        /// <summary>
        /// Width of the texture (must be divisible by 16)
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Width of the texture (must be divisible by 16)
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Offset from start of texture file to each mipmap level's image
        /// </summary>
        public uint[] Offsets { get; private set; } = new uint[4];

        /// <summary>
        /// Raw image data. Each byte points to an index in the palette
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// The color palette for the mipmaps (always 256 * 3 = 768 for GoldSrc)
        /// </summary>
        public Color32[] Pallette { get; private set; }

        public const int TotalSize = 40;

        // Used in the BSP format
        public MipTexture(string textureName, string wadFolderName, string wadName, Texture2D texture)
        {
            TextureName = textureName;
            WadFolderName = wadFolderName;
            WadName = wadName;
            Texture = texture;
            Width = texture.width;
            Height = texture.height;
        }

        // Used in the WAD format when reading directory entries (populated further later)
        public MipTexture()
        {

        }

        // Used in the WAD format when reading textures
        public MipTexture(string textureName, string wadFolderName, string wadName, int width, int height, uint[] offsets, byte[] data, Color32[] pallette)
        {
            TextureName = textureName;
            WadFolderName = wadFolderName;
            WadName = wadName;
            Width = width;
            Height = height;
            Offsets = offsets;
            Data = data;
            Pallette = pallette;
        }
    }
}