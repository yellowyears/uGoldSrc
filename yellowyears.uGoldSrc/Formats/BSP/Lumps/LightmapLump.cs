using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class LightmapLump : BSPLump
    {
        public List<Lightmap> Leaves { get; private set; } = new List<Lightmap>();

        public LightmapLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_LIGHTING;
        }
    }
}