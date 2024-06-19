using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class FaceLump : BSPLump
    {
        public List<Face> Faces { get; private set; } = new List<Face>();

        public FaceLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_FACES;
            NumEntries = headerEntry.Length / Face.TotalSize;
        }
    }
}