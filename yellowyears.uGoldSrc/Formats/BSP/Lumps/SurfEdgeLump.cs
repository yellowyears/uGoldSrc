using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class SurfEdgeLump : BSPLump
    {
        public List<SurfEdge> SurfEdges { get; private set; } = new List<SurfEdge>();

        public SurfEdgeLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_SURFEDGES;
            NumEntries = headerEntry.Length / SurfEdge.TotalSize;
        }
    }
}