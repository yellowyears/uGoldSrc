using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.Common.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class MipTextureLump : BSPLump
    {
        public List<MipTexture> MipTextures { get; private set; } = new List<MipTexture>();

        public MipTextureLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_TEXTURES;
            NumEntries = headerEntry.Length / MipTexture.TotalSize;
        }
    }
}