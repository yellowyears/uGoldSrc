using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.MDL.Lumps
{
    public class MDLLump
    {
        public HeaderEntry HeaderEntry { get; private set; }
        public LumpType Type { get; set; }

        public MDLLump(HeaderEntry headerEntry)
        {
            HeaderEntry = headerEntry;
        }

        public MDLLump(int offset, int length)
        {
            HeaderEntry = new HeaderEntry(offset, length);
        }

        public enum LumpType
        {
            LUMP_BONES, // 0
            LUMP_BONE_CONTROLLERS, // 1
            LUMP_HITBOXES, // 2
            LUMP_SEQUENCES, // 3
            LUMP_SEQUENCE_GROUPS, // 4
            LUMP_TEXTURES, // 5
            LUMP_SKINS, // 6
            LUMP_BODYPARTS, // 7
            LUMP_ATTACHMENTS, // 8
            LUMP_SOUND_TABLES, // 9
            LUMP_SOUND_GROUPS, // 10
            LUMP_TRANSITIONS // 11
        }
    }
}