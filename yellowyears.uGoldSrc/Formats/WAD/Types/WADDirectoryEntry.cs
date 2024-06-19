using yellowyears.uGoldSrc.Formats.Common.Types;

namespace yellowyears.uGoldSrc.Formats.WAD.Types
{
    public class WADDirectoryEntry
    {
        /// <summary>
        /// Offset from the beginning of the WAD3 data
        /// </summary>
        public uint Offset { get; private set; }

        /// <summary>
        /// The entry's size in the archive in bytes
        /// </summary>
        public uint DiskSize { get; private set; }

        /// <summary>
        /// The entry's uncompressed size
        /// </summary>
        public uint EntrySize { get; private set; }

        /// <summary>
        /// File type of the entry
        /// </summary>
        public byte FileType { get; private set; }

        /// <summary>
        /// Whether the file was compressed
        /// </summary>
        public byte Compressed { get; private set; }

        public byte[] Padding { get; private set; } = new byte[2];

        /// <summary>
        /// Null-terminated texture name
        /// </summary>
        public string TextureName { get; private set; }

        public MipTexture FileEntry { get; set; }

        public WADDirectoryEntry(uint offset, uint diskSize, uint entrySize, byte fileType, byte compressed, byte[] padding, string textureName)
        {
            Offset = offset;
            DiskSize = diskSize;
            EntrySize = entrySize;
            FileType = fileType;
            Compressed = compressed;
            Padding = padding;
            TextureName = textureName;
        }
    }
}