using System.IO;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.BSP.Importer
{
    public class BSPHeader
    {
        /// <summary>
        /// Entries contain the offset and length for each lump
        /// </summary>
        public HeaderEntry[] Entries { get; private set; } = new HeaderEntry[16];

        /// <summary>
        /// The version of the BSP map that was read
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// The name of the BSP map that was read
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The scale that the BSP map was imported at
        /// </summary>
        public float Scale { get; private set; }

        /// <summary>
        /// The header contains the positions for lumps, the map version, scale and the name.
        /// </summary>
        /// <param name="bspReader">The BinaryReader that the map is being loaded from</param>
        /// <param name="mapName">The name of the map that is being imported</param>
        /// <param name="mapScale">The scale of the map that is being imported</param>
        public BSPHeader(BinaryReader bspReader, string mapName, float mapScale)
        {
            // Start at the base of the BSP file
            bspReader.BaseStream.Position = 0;
            
            // Read the version and set the map name and scale
            Version = bspReader.ReadInt32();
            Name = mapName;
            Scale = mapScale;

            // Fill out the entries array with the offset and length
            for (var i = 0; i < 16; i++)
            {
                Entries[i] = new HeaderEntry(bspReader.ReadInt32(), bspReader.ReadInt32());
            }
        }

    }
}