using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class PlaneLump : BSPLump
    {
        public List<Plane> Planes { get; private set; } = new List<Plane>();

        public PlaneLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_PLANES;
            NumEntries = headerEntry.Length / Plane.TotalSize;
        }
    }
}