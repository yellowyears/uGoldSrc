using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class MarkSurfaceLump : BSPLump
    {
        public List<MarkSurface> MarkSurfaces { get; private set; } = new List<MarkSurface>();

        public MarkSurfaceLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_MARKSURFACES;
            NumEntries = headerEntry.Length / MarkSurface.TotalSize;
        }
    }
}