namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class Edge
    {

        /// <summary>
        /// Index of the start edge into the vertex array
        /// </summary>
        public ushort Start { get; private set; }

        /// <summary>
        /// Index of the end edge into the vertex array
        /// </summary>
        public ushort End { get; private set; }

        public const int TotalSize = 4;

        public Edge(ushort start, ushort end)
        {
            Start = start;
            End = end;
        }
    }
}