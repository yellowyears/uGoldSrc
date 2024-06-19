using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.Common.Importer;
using yellowyears.uGoldSrc.Formats.MDL.Types;

namespace yellowyears.uGoldSrc.Formats.MDL.Lumps
{
    public class TextureLump : MDLLump
    {
        public List<Texture> Textures { get; private set; } = new List<Texture>();

        public TextureLump(HeaderEntry headerEntry) : base (headerEntry)
        {
            Type = LumpType.LUMP_TEXTURES;
        }
    }
}