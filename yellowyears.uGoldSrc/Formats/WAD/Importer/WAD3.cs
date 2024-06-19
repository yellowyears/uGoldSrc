using System.Collections.Generic;
using yellowyears.uGoldSrc.Formats.WAD.Types;

namespace yellowyears.uGoldSrc.Formats.WAD.Importer
{
    public class WAD3
    {
        public WADHeader Header { get; private set; }

        public List<WADDirectoryEntry> DirectoryEntries { get; private set; } = new List<WADDirectoryEntry>();

        public WAD3(WADHeader wadHeader, List<WADDirectoryEntry> directoryEntries)
        {
            Header = wadHeader;
            DirectoryEntries = directoryEntries;
        }
    }
}