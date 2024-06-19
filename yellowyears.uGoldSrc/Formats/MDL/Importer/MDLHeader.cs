using System.IO;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.Common.Importer;

namespace yellowyears.uGoldSrc.Formats.MDL.Importer
{
    public class MDLHeader
    {
        public HeaderEntry[] Entries { get; private set; } = new HeaderEntry[12]; // Note: In the MDL format, Length is the number of entries, not the total byte size of the lump

        /// <summary>
        /// This should be IDST (not Arctic Monkeys)
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The version of this MDL file
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// The name of this MDL, also contains the containing folders
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The total size of the MDL
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The position of the model's eyes
        /// </summary>
        public Vector3 EyePosition { get; private set; }

        public Vector3 HullMin { get; private set; }
        public Vector3 HullMax { get; private set; }

        /// <summary>
        /// The minimum extent of the clipping box
        /// </summary>
        public Vector3 BoundingBoxMin { get; private set; }

        /// <summary>
        /// The maximum extent of the clipping box
        /// </summary>
        public Vector3 BoundingBoxMax { get; private set; }

        public int Flags { get; private set; }

        public MDLHeader(BinaryReader mdl)
        {
            mdl.BaseStream.Position = 0;

            Id = new string(mdl.ReadChars(4, true));
            Version = mdl.ReadInt32();
            Name = new string(mdl.ReadChars(64, true));
            Length = mdl.ReadInt32();
            EyePosition = mdl.ReadVector3();
            HullMin = mdl.ReadVector3();
            HullMax = mdl.ReadVector3();
            BoundingBoxMin = mdl.ReadVector3();
            BoundingBoxMax = mdl.ReadVector3();
            Flags = mdl.ReadInt32();

            for (int i = 0; i < Entries.Length; i++)
            {
                // Stored in reverse order in MDL to the HeaderEntry ctor
                var length = mdl.ReadInt32();
                var offset = mdl.ReadInt32();
                Entries[i] = new HeaderEntry(offset, length);

                if (i == 5)
                {
                    mdl.BaseStream.Position += 8; // Skip two Int32s (texturedataindex and numskinref)
                }
            }
        }
    }
}