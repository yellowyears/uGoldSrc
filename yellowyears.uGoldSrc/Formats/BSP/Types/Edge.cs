namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Edge
    {

        /// <summary>
        /// Indices into vertex array, start and end vertex
        /// </summary>
        public ushort[] Vertices { get; private set; } = new ushort[2];

        public const int TotalSize = 4;

        public Edge(ushort[] vertices)
        {
            Vertices = vertices;
        }
    }
}