using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class EntityLump : BSPLump
    {
        public List<Entity> Entities { get; private set; } = new List<Entity>();

        // These are both used when reading textures to find the wad filenames
        public int wadEntityIndex = -1; // Index for the entity in the entity lump that contains WADs used
        public int wadAttributeIndex = -1; // Index for the actual attribute containing the WADs used

        public EntityLump(HeaderEntry headerEntry) : base(headerEntry)
        {
            Type = LumpType.LUMP_ENTITIES;
            NumEntries = headerEntry.Length;
        }
    }
}