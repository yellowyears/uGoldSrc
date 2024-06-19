using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class TextureInfoLump : BSPLump
    {
        public List<TextureInfo> TextureInfos { get; private set; } = new List<TextureInfo>();

        public TextureInfoLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_TEXINFO;
            NumEntries = headerEntry.Length / TextureInfo.TotalSize;
        }
    }
}