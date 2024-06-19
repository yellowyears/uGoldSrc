using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class TextureInfo
    {
        public Vector3 XScale { get; private set; }

        /// <summary>
        /// Texture shift in s direction
        /// </summary>
        public float XShift { get; private set; }

        public Vector3 YScale { get; private set; }

        /// <summary>
        /// Texture shift in t direction
        /// </summary>
        public float YShift { get; private set; }

        /// <summary>
        /// Index into textures array
        /// </summary>
        public int MipTextureIndex { get; private set; }

        /// <summary>
        /// Texture flags, seem to always be 0
        /// </summary>
        public int Flags { get; private set; }

        public const int TotalSize = 40;

        public TextureInfo(Vector3 xScale, float xShift, Vector3 yScale, float yShift, uint mipTextureIndex, uint flags)
        {
            XScale = xScale;
            XShift = xShift;

            YScale = yScale;
            YShift = yShift;

            MipTextureIndex = (int)mipTextureIndex;
            Flags = (int)flags;
        }
    }
}