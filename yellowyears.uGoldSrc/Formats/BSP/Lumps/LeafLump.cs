using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class LeafLump : BSPLump
    {
        public List<Leaf> Leaves { get; private set; } = new List<Leaf>();

        public LeafLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_LEAVES;
            NumEntries = headerEntry.Length / Leaf.TotalSize;
        }
    }
}