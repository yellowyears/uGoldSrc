using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Lumps
{
    public class BSPLump 
    {
        public HeaderEntry HeaderEntry { get; private set; }
        public LumpType Type { get; set; }
        public int NumEntries { get; set; }

        public BSPLump(HeaderEntry headerEntry)
        {
            HeaderEntry = headerEntry;
        }        

        public BSPLump(int offset, int length)
        {
            HeaderEntry = new HeaderEntry(offset, length);
        }

        public enum LumpType
        {
            LUMP_ENTITIES, // 0
            LUMP_PLANES, // 1
            LUMP_TEXTURES, // 2
            LUMP_VERTICES, // 3
            LUMP_VISIBILITY, // 4
            LUMP_NODES, // 5
            LUMP_TEXINFO, // 6
            LUMP_FACES, // 7
            LUMP_LIGHTING, // 8
            LUMP_CLIPNODES, // 9
            LUMP_LEAVES, // 10
            LUMP_MARKSURFACES, // 11
            LUMP_EDGES, // 12
            LUMP_SURFEDGES, // 13
            LUMP_MODELS, // 14
            HEADER_LUMPS // 15
        }
    }
}