namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class SurfEdge
    {

        public int SurfEdgeIndex { get; private set; }

        public const int TotalSize = 4;

        public SurfEdge(int surfEdgeIndex)
        {
            SurfEdgeIndex = surfEdgeIndex;
        }
    }
}