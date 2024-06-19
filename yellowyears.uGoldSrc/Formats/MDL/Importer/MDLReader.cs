using System.Collections;
using System.IO;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.Common.Importer;
using yellowyears.uGoldSrc.Formats.MDL.Lumps;

namespace yellowyears.uGoldSrc.Formats.MDL.Importer
{
    public class MDLReader
    {

        private static BinaryReader _reader;

        public static MDL10 Read(string rootPath, string modName, string modelName, float unitScale)
        {
            // Create a new BinaryReader with the map file
            var modelPath = Path.Combine(rootPath, modName + "\\models", modelName + ".mdl");
            using (_reader = new BinaryReader(File.Open(modelPath, FileMode.Open)))
            {
                // Use this BinaryReader to get data from the header
                var header = new MDLHeader(_reader);

                if (header.Version != 10)
                {
                    Debug.LogError("MDL file provided wasn't MDL10! Only MDL10 files are supported!");
                    return null;
                }

                var textureLump = ReadTextures(header.Entries[5]);

                _reader.Close();

                var model = new MDL10(
                    header,
                    textureLump);

                return model;
            }
        }

        #region Lump Reading

        private static TextureLump ReadTextures(HeaderEntry headerEntry)
        {
            var textureLump = new TextureLump(headerEntry);

            // Access the LUMP_TEXTURES from the header
            _reader.BaseStream.Position = textureLump.HeaderEntry.Offset;

            for (int i = 0; i < textureLump.HeaderEntry.Length; i++)
            {
                var texture = new MDL.Types.Texture(new string(_reader.ReadChars(64, true)), _reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32());

                // Store the current offset before moving to the pixels offset
                var currentOffset = (int)_reader.BaseStream.Position;
                _reader.BaseStream.Position = texture.PixelsOffset;

                var pixelColours = new Color[texture.Width * texture.Height];

                for (int j = 0; j < pixelColours.Length; j++)
                {
                    // https://en.wikipedia.org/wiki/8-bit_color
                    // 3 bits red, 3 bits green, 2 bits blue
                    // TODO: find out how GoldSrc actually reads MDL textures

                    var pixelByte = _reader.ReadByte();
                    var pixelBits = new BitArray(pixelByte);

                    pixelColours[j] = new Color();
                }

                // Reset the position back to the current texture
                _reader.BaseStream.Position = currentOffset;

                textureLump.Textures.Add(texture);
            }

            return textureLump;
        }

        #endregion

    }
}