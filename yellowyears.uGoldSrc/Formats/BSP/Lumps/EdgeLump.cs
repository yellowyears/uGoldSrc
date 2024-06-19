using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class EdgeLump : BSPLump
    {
        public List<Edge> Edges { get; private set; } = new List<Edge>();

        public EdgeLump(HeaderEntry headerEntry) : base (headerEntry)
        {
            Type = LumpType.LUMP_EDGES;
            NumEntries = headerEntry.Length / Edge.TotalSize;
        }
    }
}