using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class TextureInfo
    {
        public Vector3 VScale { get; private set; }

        /// <summary>
        /// Texture shift in s direction
        /// </summary>
        public float SShift { get; private set; }

        public Vector3 TScale { get; private set; }

        /// <summary>
        /// Texture shift in t direction
        /// </summary>
        public float TShift { get; private set; }

        /// <summary>
        /// Index into textures array
        /// </summary>
        public int MipTextureIndex { get; private set; }

        /// <summary>
        /// Texture flags, seem to always be 0
        /// </summary>
        public int Flags { get; private set; }

        public const int TotalSize = 40;

        public TextureInfo(Vector3 sScale, float sShift, Vector3 tScale, float tShift, uint mipTextureIndex, uint flags)
        {
            VScale = sScale;
            SShift = sShift;

            TScale = tScale;
            TShift = tShift;

            MipTextureIndex = (int)mipTextureIndex;
            Flags = (int)flags;
        }
    }
}