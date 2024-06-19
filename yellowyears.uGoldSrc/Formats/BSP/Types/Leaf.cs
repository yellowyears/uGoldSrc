using UnityEngine;

namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Leaf
    {

        public int ContentType { get; private set; }
        public int VisOffset { get; private set; }
        public Vector3 BoxMin { get; private set; }
        public Vector3 BoxMax { get; private set; }
        public ushort FirstMarkSurface { get; private set; }
        public ushort NumMarkSurfaces { get; private set; }
        public byte[] AmbientLevels { get; private set; } = new byte[4]; // Unused in GoldSrc

        public const int TotalSize = 28;

        public Leaf(int contentType, int visOffset, Vector3 boxMin, Vector3 boxMax, ushort firstMarkSurface, ushort numMarkSurfaces, byte[] ambientLevels)
        {
            ContentType = contentType;
            VisOffset = visOffset;
            BoxMin = boxMin;
            BoxMax = boxMax;
            FirstMarkSurface = firstMarkSurface;
            NumMarkSurfaces = numMarkSurfaces;
            AmbientLevels = ambientLevels;
        }
    }
}