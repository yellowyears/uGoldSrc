using yellowyears.uGoldSrc.Formats.MDL.Lumps;

namespace yellowyears.uGoldSrc.Formats.MDL.Importer
{
    // https://github.com/malortie/assimp/wiki/MDL:-Half-Life-1-file-format
    public class MDL10
    {
        public MDLHeader Header { get; private set; }

        public TextureLump TextureLump { get; private set; }

        public MDL10(MDLHeader header, TextureLump textureLump)
        {
            Header = header;

            TextureLump = textureLump;
        }
    }
}