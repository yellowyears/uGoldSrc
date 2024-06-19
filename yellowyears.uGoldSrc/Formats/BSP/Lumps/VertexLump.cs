using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class VertexLump : BSPLump
    {
        public List<Vertex> Vertices { get; private set; } = new List<Vertex>();

        public VertexLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_VERTICES;
            NumEntries = headerEntry.Length / Vertex.TotalSize;
        }
    }
}