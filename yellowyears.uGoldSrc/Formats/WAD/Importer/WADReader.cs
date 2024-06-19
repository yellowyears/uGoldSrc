using System.Collections.Generic;
using System.IO;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.Common.Types;
using yellowyears.uGoldSrc.Formats.WAD.Types;

namespace yellowyears.uGoldSrc.Formats.WAD.Importer
{
    public static class WADReader
    {
        private static string _wadFolderName;
        private static string _wadName;

        public static WAD3 Read(string path)
        {
            // Create a new BinaryReader with the map file
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                // Use this BinaryReader to get data from the header
                var header = new WADHeader(reader);

                _wadFolderName = Path.GetFileName(Path.GetDirectoryName(path));
                _wadName = Path.GetFileNameWithoutExtension(path);

                if (header.Type != "WAD3")
                {
                    Debug.LogError("WAD file provided wasn't WAD3! Only WAD3 files are supported!");
                    return null;
                }

                var directoryEntries = ReadDirectoryEntries(reader, header.NumEntries, header.FirstEntryOffset);

                reader.Close();

                var wad = new WAD3 (
                    header,
                    directoryEntries);

                return wad;
            };
        }
    
        public static List<WADDirectoryEntry> ReadDirectoryEntries(BinaryReader reader, uint numEntries, uint firstEntryOffset)
        {
            var directoryEntries = new List<WADDirectoryEntry>();

            reader.BaseStream.Position = firstEntryOffset;
            for (int i = 0; i < numEntries; i++)
            {
                var directoryEntry = new WADDirectoryEntry(
                    reader.ReadUInt32(),
                    reader.ReadUInt32(),
                    reader.ReadUInt32(),
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadBytes(2),
                    new string(reader.ReadChars(16, true)));

                directoryEntry.FileEntry = ReadFileEntry(directoryEntry);

                if (directoryEntry.FileEntry != null)
                {
                    directoryEntry.FileEntry.WadFolderName = _wadFolderName;
                    directoryEntry.FileEntry.WadName = _wadName;
                }

                directoryEntries.Add(directoryEntry);
            }

            return directoryEntries;
        }

        // Returns MipTexture because that's the only thing supported. Future intention is to make file entry inherited by all types nd return that
        private static MipTexture ReadFileEntry(WADDirectoryEntry directoryEntry)
        {
            switch (directoryEntry.FileType)
            {
                case 0x42:
                    // QPic - not supported
                    return null;
                case 0x43:
                    // MipTexture

                    // Return a barebones MipTexture, the texture is read later
                    return new MipTexture();
                case 0x45:
                    // Font - not supported
                    return null;
                case 0x40:
                    // SprayDecal (same as MipTexture)

                    // Return a barebones MipTexture, the texture is read later
                    return new MipTexture();
                default:
                    return null;
            }
        }

        public static MipTexture ReadMipTexture(string rootPath, WADDirectoryEntry directoryEntry)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(Path.Combine(rootPath, directoryEntry.FileEntry.WadFolderName, directoryEntry.FileEntry.WadName) + ".wad", FileMode.Open)))
            {
                // Workaround for an issue with incorrect positions after reading texture name
                reader.BaseStream.Position = directoryEntry.Offset;
                var startOffset = reader.BaseStream.Position;
                var textureName = reader.ReadChars(16, true);
                reader.BaseStream.Position = startOffset + 16;

                var width = reader.ReadUInt32();
                var height = reader.ReadUInt32();
                var offsets = reader.ReadUInt32Array(4);
                var data = reader.ReadBytes((int)(width * height)); // Mip 0 data
                var mip1 = reader.ReadBytes((int)(width / 2 * (height / 2)));
                var mip2 = reader.ReadBytes((int)(width / 4 * (height / 4)));
                var mip3 = reader.ReadBytes((int)(width / 8 * (height / 8)));
                var padding = reader.ReadBytes(2);

                var pallette = new Color32[256];

                for (int i = 0; i < pallette.Length; i++)
                {
                    var palletteColour = new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), 255);

                    if (palletteColour.Equals(new Color32(0, 0, 255, 255)))
                    {
                        palletteColour = new Color32(0, 0, 0, 0);
                    }

                    pallette[i] = palletteColour;
                }

                var mipTexture = new MipTexture(new string(textureName), directoryEntry.FileEntry.WadFolderName, directoryEntry.FileEntry.WadName, (int)width, (int)height, offsets, data, pallette);
                var texture = Utilities.ReadTexture(mipTexture);
                mipTexture.Texture = texture;

                return mipTexture;
            }
        }
    }
}