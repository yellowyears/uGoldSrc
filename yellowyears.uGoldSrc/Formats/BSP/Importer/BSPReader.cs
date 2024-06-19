using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.BSP.Lumps;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Importer;
using yellowyears.uGoldSrc.Formats.Common.Types;
using yellowyears.uGoldSrc.Formats.WAD.Importer;
using yellowyears.uGoldSrc.Formats.WAD.Types;

namespace yellowyears.uGoldSrc.Formats.BSP.Importer
{
    public static class BSPReader
    {
        private static BinaryReader _reader;

        public static BSP30 Read(string rootPath, string modName, string mapName, float unitScale)
        {
            // Create a new BinaryReader with the map file
            var mapPath = Path.Combine(rootPath, modName + "\\maps", mapName + ".bsp");

            using (_reader = new BinaryReader(File.Open(mapPath, FileMode.Open)))
            {
                // Use this BinaryReader to get data from the header
                var header = new BSPHeader(_reader, mapName, unitScale);

                if (header.Version != 30)
                {
                    Debug.LogError("BSP file provided wasn't BSP30! Only BSP30 files are supported!");
                    return null;
                }

                // Try to detect a Blue Shift map (one where the plane and entity lumps are flipped)
                var originalOffset = _reader.BaseStream.Position;
                _reader.BaseStream.Position = header.Entries[1].Offset;

                // If the entity lump's length divides into the size of a plane and the plane lump starts with a { then the lumps are most likely flipped.
                if(header.Entries[0].Length % BSP.Types.Plane.TotalSize == 0 && _reader.ReadChar() == '{')
                {
                    Debug.Log($"Map where entity and plane lumps are flipped detected ({header.Name}), fixing (this is normal for Half-Life: Blue Shift maps).");

                    // Swap the plane and entity lumps
                    var entityHeaderEntry = header.Entries[0];
                    var planeHeaderEntry = header.Entries[1];
                    header.Entries[0] = planeHeaderEntry;
                    header.Entries[1] = entityHeaderEntry;
                }

                // Reset the position of the reader (not exactly necessary since lump readers set it anyways)
                _reader.BaseStream.Position = originalOffset;

                var entityLump = ReadEntities(header.Entries[0]);
                var planeLump = ReadPlanes(header.Entries[1]);
                var mipTextureLump = ReadMipTextures(header.Entries[2], rootPath, modName, entityLump);
                var vertexLump = ReadVertices(header.Entries[3], unitScale);
                var textureInfoLump = ReadTextureInfos(header.Entries[6], unitScale);
                var faceLump = ReadFaces(header.Entries[7]);
                var lightmapLump = ReadLightmaps(header.Entries[8]);
                var leafLump = ReadLeaves(header.Entries[10]);
                var edgeLump = ReadEdges(header.Entries[12]);
                var surfEdgeLump = ReadSurfEdges(header.Entries[13]);
                var modelLump = ReadModels(header.Entries[14]);

                _reader.Close();

                var map = new BSP30 (               
                    header,
                    entityLump,
                    planeLump,
                    mipTextureLump,
                    vertexLump,
                    textureInfoLump,
                    faceLump,
                    leafLump,
                    edgeLump,
                    surfEdgeLump,
                    modelLump);

                return map;
            }
        }

        #region Lump Readers

        private static EntityLump ReadEntities(HeaderEntry headerEntry)
        {
            var entityLump = new EntityLump(headerEntry);

            // Access the LUMP_ENTITIES from the header
            _reader.BaseStream.Position = entityLump.HeaderEntry.Offset;

            // Read all of the entities
            var chars = _reader.ReadChars(entityLump.NumEntries).Where(x => x <= 127).ToArray();
            var rawEntities = new string(chars);

            // Split the raw entities by the start { and end }
            List<string> splitRawEntities = new List<string>();

            bool insideEntity = false;
            string currentString = "";

            foreach (char c in rawEntities)
            {
                if (c == '{')
                {
                    if (insideEntity)
                    {
                        currentString += c;
                    }

                    insideEntity = true;
                }
                else if (c == '}')
                {
                    insideEntity = false;
                    splitRawEntities.Add(currentString);
                    currentString = "";
                }
                else if (insideEntity)
                {
                    currentString += c;
                }
            }

            // Remove all null characters and whitespace entries in the array
            splitRawEntities = Utilities.CleanStringList(splitRawEntities);

            for (int i = 0; i < splitRawEntities.Count; i++)
            {
                var entity = new Entity(splitRawEntities[i]);

                // If the wad index hasn't been found yet
                if (entityLump.wadEntityIndex == -1)
                {
                    // Find the index of the wad attribute
                    var wadIndex = entity.attributes.FindIndex(n => n.key == "wad");
                    if (wadIndex != -1)
                    {
                        // WAD entity has been found
                        entityLump.wadEntityIndex = i;
                        entityLump.wadAttributeIndex = wadIndex;
                    }
                }

                entityLump.Entities.Add(entity);
            }

            return entityLump;
        }

        private static PlaneLump ReadPlanes(HeaderEntry headerEntry)
        {
            var planeLump = new PlaneLump(headerEntry);

            // Access the LUMP_PLANES from the header
            _reader.BaseStream.Position = planeLump.HeaderEntry.Offset;

            for (int i = 0; i < planeLump.NumEntries; i++)
            {
                var plane = new BSP.Types.Plane(_reader.ReadVector3(), _reader.ReadSingle(), _reader.ReadInt32());
                planeLump.Planes.Add(plane);
            }

            return planeLump;
        }

        private static MipTextureLump ReadMipTextures(HeaderEntry headerEntry, string rootPath, string modName, EntityLump entityLump)
        {
            var mipTextureLump = new MipTextureLump(headerEntry);

            // First, read all the textures from each WAD

            // Get the WAD filenames
            string wads = entityLump.Entities[entityLump.wadEntityIndex].attributes[entityLump.wadAttributeIndex].value; // Get all WADs from entity with "wad" key
            string[] wadFilePaths = wads.Split(';', StringSplitOptions.RemoveEmptyEntries); // Parse the WAD files and loop through their textures
            List<List<WADDirectoryEntry>> wadDirectoryEntries = new List<List<WADDirectoryEntry>>();

            // Get just the filename from the WAD file paths
            for (int i = 0; i < wadFilePaths.Length; i++)
            {
                var splitWadFilePath = wadFilePaths[i].Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                if(splitWadFilePath.Length >= 2)
                {
                    if (splitWadFilePath[splitWadFilePath.Length - 2] == "barney")
                    {
                        // Temporary fix for incorrect folder name in worldspawn
                        splitWadFilePath[splitWadFilePath.Length - 2] = "bshift";
                    }

                    wadFilePaths[i] = splitWadFilePath[splitWadFilePath.Length - 2] + "\\" + splitWadFilePath[splitWadFilePath.Length - 1];
                }
                else
                {
                    wadFilePaths[i] = modName + "\\" + splitWadFilePath[splitWadFilePath.Length - 1];
                }
                string wadFileName = Path.GetFileNameWithoutExtension(wadFilePaths[i]); // Used in texture organisation
                var mipTextures = new List<MipTexture>();

                string wadPath = Path.Combine(rootPath, wadFilePaths[i]);
                if (File.Exists(wadPath))
                {
                    var wad = WADReader.Read(wadPath);

                    // Only take the directory entry so we can later only read the textures we need
                    wadDirectoryEntries.Add(wad.DirectoryEntries);
                }
            }

            // Access the LUMP_TEXTURES from the header
            _reader.BaseStream.Position = mipTextureLump.HeaderEntry.Offset;

            int numTextures = (int)_reader.ReadUInt32();

            var previousReaderPosition = _reader.BaseStream.Position; // Used to switch between mip offset and texture offset
            int[] mipTextureOffsets = new int[numTextures];
            for (int i = 0; i < numTextures; i++)
            {
                // Get offsets of the mip textures
                _reader.BaseStream.Position = previousReaderPosition;
                mipTextureOffsets[i] = mipTextureLump.HeaderEntry.Offset + _reader.ReadInt32();
                previousReaderPosition = _reader.BaseStream.Position; // This is so the logic can be in one loop despite the position moving around

                _reader.BaseStream.Position = mipTextureOffsets[i];
                var textureName = new string(_reader.ReadChars(16, true));

                // Instead of reading the texture and differentiating between bsp/wad, just load it from the wadMipTextures list
                for (int j = 0; j < wadDirectoryEntries.Count; j++)
                {
                    // Find a matching directory entry
                    WADDirectoryEntry matchingDirectoryEntry = wadDirectoryEntries[j].Where(x => x.TextureName.ToLower() == textureName.ToLower()).FirstOrDefault();
                    if (matchingDirectoryEntry != null)
                    {
                        var mipTexture = WADReader.ReadMipTexture(rootPath, matchingDirectoryEntry);

                        // Finally add the mipTexture to the lump
                        mipTextureLump.MipTextures.Add(mipTexture);
                        break;
                    }
                }
            }

            return mipTextureLump;
        }

        private static VertexLump ReadVertices(HeaderEntry headerEntry, float mapScale)
        {
            var vertexLump = new VertexLump(headerEntry);

            // Access the LUMP_VERTICES from the header
            _reader.BaseStream.Position = vertexLump.HeaderEntry.Offset;

            for (int i = 0; i < vertexLump.NumEntries; i++)
            {
                var vertex = new Vertex(_reader.ReadVector3(), mapScale);
                vertexLump.Vertices.Add(vertex);
            }

            return vertexLump;
        }

        private static TextureInfoLump ReadTextureInfos(HeaderEntry headerEntry, float mapScale)
        {
            var textureInfoLump = new TextureInfoLump(headerEntry);

            // Access the LUMP_TEXINFO from the header
            _reader.BaseStream.Position = textureInfoLump.HeaderEntry.Offset;

            for (int i = 0; i <= textureInfoLump.NumEntries; i++)
            {
                var textureInfo = new TextureInfo(_reader.ReadVector3(), _reader.ReadSingle(), _reader.ReadVector3(), _reader.ReadSingle(), _reader.ReadUInt32(), _reader.ReadUInt32());

                textureInfoLump.TextureInfos.Add(textureInfo);
            }

            return textureInfoLump;
        }

        private static FaceLump ReadFaces(HeaderEntry headerEntry)
        {
            var faceLump = new FaceLump(headerEntry);

            // Access the LUMP_FACES from the header
            _reader.BaseStream.Position = faceLump.HeaderEntry.Offset;

            // Fill the faces array
            for (int i = 0; i < faceLump.NumEntries; i++)
            {
                var face = new Face(_reader.ReadUInt16(), _reader.ReadUInt16(), _reader.ReadUInt32(), _reader.ReadUInt16(), _reader.ReadUInt16(), _reader.ReadBytes(4), _reader.ReadUInt32());
                faceLump.Faces.Add(face);
            }

            return faceLump;
        }

        private static LightmapLump ReadLightmaps(HeaderEntry headerEntry)
        {
            var lightmapLump = new LightmapLump(headerEntry);

            // Access the LUMP_LIGHTING from the header
            _reader.BaseStream.Position = lightmapLump.HeaderEntry.Offset;

            // Fill the lightmaps array

            return lightmapLump;
        }

        private static LeafLump ReadLeaves(HeaderEntry headerEntry)
        {
            var leafLump = new LeafLump(headerEntry);

            // Access the LUMP_LEAVES from the header
            _reader.BaseStream.Position = leafLump.HeaderEntry.Offset;

            // Fill the leaves array
            for (int i = 0; i < leafLump.NumEntries; i++)
            {
                var leaf = new Leaf(_reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadVector3(readShorts: true), _reader.ReadVector3(readShorts: true), _reader.ReadUInt16(), _reader.ReadUInt16(), _reader.ReadBytes(4));
                leafLump.Leaves.Add(leaf);
            }

            return leafLump;
        }

        private static EdgeLump ReadEdges(HeaderEntry headerEntry)
        {
            var edgeLump = new EdgeLump(headerEntry);

            // Access the LUMP_EDGES from the header
            _reader.BaseStream.Position = edgeLump.HeaderEntry.Offset;

            for (int i = 0; i < edgeLump.NumEntries; i++)
            {
                var edge = new Edge(new ushort[] { _reader.ReadUInt16(), _reader.ReadUInt16() });
                edgeLump.Edges.Add(edge);
            }

            return edgeLump;
        }        
        
        private static SurfEdgeLump ReadSurfEdges(HeaderEntry headerEntry)
        {
            var surfEdgeLump = new SurfEdgeLump(headerEntry);

            // Access the LUMP_SURFEDGES from the header
            _reader.BaseStream.Position = surfEdgeLump.HeaderEntry.Offset;

            for(int i = 0; i < surfEdgeLump.NumEntries; i++)
            {
                var surfEdge = new SurfEdge(_reader.ReadInt32());
                surfEdgeLump.SurfEdges.Add(surfEdge);
            }

            return surfEdgeLump;
        }

        private static ModelLump ReadModels(HeaderEntry headerEntry)
        {
            var modelLump = new ModelLump(headerEntry);

            // Access the LUMP_MODELS from the header
            _reader.BaseStream.Position = modelLump.HeaderEntry.Offset;

            for(int i = 0; i < modelLump.NumEntries; i++)
            {
                var model = new Model(_reader.ReadVector3(), _reader.ReadVector3(), _reader.ReadVector3(), new int[] { _reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32() }, _reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32());
                modelLump.Models.Add(model);
            }

            return modelLump;
        }
    }

    #endregion
}