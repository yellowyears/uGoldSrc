using System.IO;

namespace yellowyears.uGoldSrc.Formats.WAD.Importer
{
    public class WADHeader
    {
        /// <summary>
        /// WAD file format/version
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Number of Directory entries
        /// </summary>
        public uint NumEntries { get ; private set; }

        /// <summary>
        /// Offset in the WAD file for the first entry
        /// </summary>
        public uint FirstEntryOffset { get; private set; }

        public WADHeader(BinaryReader reader)
        {
            reader.BaseStream.Position = 0;

            Type = new string(reader.ReadChars(4, true));
            NumEntries = reader.ReadUInt32();
            FirstEntryOffset = reader.ReadUInt32();
        }
    }
}