using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class ModelLump : BSPLump
    {
        public List<Model> Models { get; private set; } = new List<Model>();

        public ModelLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_MODELS;
            NumEntries = headerEntry.Length / Model.TotalSize;
        }
    }
}