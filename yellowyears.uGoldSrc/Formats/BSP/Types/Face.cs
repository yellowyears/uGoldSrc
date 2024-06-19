namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Face
    {
        /// <summary>
        /// Plane the face is parallel to
        /// </summary>
        public int Plane { get; private set; }

        /// <summary>
        /// Set if different normals orientation
        /// </summary>
        public int PlaneSide { get; private set; }

        /// <summary>
        /// Index of the first surfedge
        /// </summary>
        public int FirstEdge { get; private set; }

        /// <summary>
        /// Number of consecutive surfedges
        /// </summary>
        public int NumEdges { get; private set; }

        /// <summary>
        /// Index of the texture info structure
        /// </summary>
        public int TextureInfoIndex { get; private set; }

        /// <summary>
        /// Specify lighting styles
        /// </summary>
        public byte[] Styles { get; private set; } = new byte[4];

        /// <summary>
        /// Offsets into the raw lightmap data; if less than zero, then a lightmap was not baked for the given face.
        /// </summary>
        public int LightmapOffset { get; private set; }

        public const int TotalSize = 20;

        public Face(ushort plane, ushort planeSide, uint firstEdge, ushort numEdges, ushort textureInfo, byte[] styles, uint lightmapOffset)
        {
            Plane = plane;
            PlaneSide = planeSide;
            FirstEdge = (int)firstEdge;
            NumEdges = numEdges;
            TextureInfoIndex = textureInfo;
            Styles = styles;
            LightmapOffset = (int)lightmapOffset;
        }
    }
}