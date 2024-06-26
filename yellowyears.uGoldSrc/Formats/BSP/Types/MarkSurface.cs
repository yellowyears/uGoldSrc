namespace yellowyears.uGoldSrc.Formats.BSP.Types
{
    public class MarkSurface
    {

        public int MarkSurfaceIndex { get; private set; }

        public const int TotalSize = 2;

        public MarkSurface(int markSurfaceIndex)
        {
            MarkSurfaceIndex = markSurfaceIndex;
        }
    }
}