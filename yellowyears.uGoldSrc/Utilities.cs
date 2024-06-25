using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Formats.Common.Types;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace yellowyears.uGoldSrc
{
    public static class Utilities
    {
        public static List<string> CleanStringList(List<string> list)
        {
            var cleanList = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Contains('\0') || string.IsNullOrWhiteSpace(list[i])) continue;

                // Remove and whitespace or newlines at start and end
                list[i] = list[i].Trim();
                cleanList.Add(list[i]);
            }

            return cleanList;
        }

        public static string GetAttributeValue(Entity entity, string key)
        {
            int attrIndex = entity.attributes.FindIndex(n => n.key == key);

            if(attrIndex != -1)
            {
                return entity.attributes[attrIndex].value;
            }

            return null;
        }

        public static Color GetLightColour(string[] splitLightValues)
        {
            // TODO: Some light entities have only one colour 

            if (splitLightValues.Length < 3) return new Color(1, 1, 1, 1);

            float r = Convert.ToSingle(splitLightValues[0]) / 255;
            float g = Convert.ToSingle(splitLightValues[1]) / 255;
            float b = Convert.ToSingle(splitLightValues[2]) / 255;

            Color lightColour;
            if(splitLightValues.Length == 4)
            {
                float a = Convert.ToSingle(splitLightValues[3]);
                lightColour = new Color(r, g, b, a);
            }
            else
            {
                // Use the GoldSrc default brightness 200
                lightColour = new Color(r, g, b, 200);
            }

            // R, G, B, Intensity
            return lightColour;
        }

        public static Texture2D ReadTexture(MipTexture mipTexture)
        {
            var texture = new Texture2D(mipTexture.Width, mipTexture.Height);

            // Each entry of this array is a pixel
            Color32[] textureColours = new Color32[mipTexture.Data.Length];
            for (int i = 0; i < textureColours.Length; i++)
            {
                textureColours[i] = new Color32();
                textureColours[i] = mipTexture.Pallette[mipTexture.Data[i]];
            }

            texture.SetPixels32(textureColours);

            texture = FlipTextureVertically(texture);
            texture.Apply();

            return texture;
        }

        public static Texture2D FlipTextureVertically(Texture2D texture)
        {
            var flippedTexture = new Texture2D(texture.width, texture.height);

            var originalPixels = texture.GetPixels();
            var newPixels = new Color32[originalPixels.Length];

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    newPixels[x + y * texture.width] = originalPixels[x + (texture.height - y - 1) * texture.width];
                }
            }

            flippedTexture.SetPixels32(newPixels);
            flippedTexture.Apply();
            return flippedTexture;
        }

        public static Vector3 FixVector3(Vector3 vector3)
        {
            return new Vector3(-vector3.x, vector3.z, -vector3.y);
        }

        public static void CreateExportRootFolder()
        {
#if UNITY_EDITOR
            var exportPath = "Assets/_uGoldSrc";
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }
#endif
        }

        public static Material GetMaterial(Texture2D texture, string saveAndLoadPath)
        {
#if UNITY_EDITOR
            // saveAndLoadPath should be <export folder>/assets/<folder containing wad>/<wad name>
            var materialPath = Path.Combine(saveAndLoadPath, texture.name) + ".mat";

            // Try to load the material at that path
            var existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (existingMaterial != null)
            {
                // If there was a material found at that path return it
                return existingMaterial;
            }
            else
            {
                // Create the material from scratch as it could used in and outside of the save and load
                var material = new Material(Settings.Instance.litShader);
                material.mainTexture = texture; // This texture could come from the mipTexture, but it is better to use a texture returned from GetTexture()
                material.name = texture.name;

                // { = Masked/Transparent Textures
                if (material.name.StartsWith("{"))
                {
                    // Enable alpha clipping on the material
                    material.SetFloat("_AlphaClip", 1);
                    material.SetFloat("_Cutoff", 1);
                }

                material.SetFloat("_Smoothness", 0);
                material.SetFloat("_Glossiness", 0);

                // Else, we need to save the new material at the same path
                if (!Directory.Exists(saveAndLoadPath))
                {
                    Directory.CreateDirectory(saveAndLoadPath);
                }

                // Create the material asset at the path
                AssetDatabase.CreateAsset(material, materialPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Load the material to ensure it is the same reference
                material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                return material;
            }
#else
            return null;
#endif
        }

        public static Material GetSkyboxMaterial(Cubemap skyboxCubemap, string modName)
        {
#if UNITY_EDITOR
            // saveAndLoadPath should be <export folder>/assets/<mod (game) name>/skybox
            var saveAndLoadPath = Path.Combine("Assets/_uGoldSrc", "assets", modName, "skybox");
            var materialPath = Path.Combine(saveAndLoadPath, skyboxCubemap.name) + ".mat";

            // Try to load the material at that path
            var existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (existingMaterial != null)
            {
                // If there was a material found at that path return it
                return existingMaterial;
            }
            else
            {
                // Create the material from scratch
                var material = new Material(Settings.Instance.skyboxShader);
                material.mainTexture = skyboxCubemap;
                material.name = skyboxCubemap.name;

                // Else, we need to save the new material at the same path
                if (!Directory.Exists(saveAndLoadPath))
                {
                    Directory.CreateDirectory(saveAndLoadPath);
                }

                // Create the material asset at the path
                AssetDatabase.CreateAsset(material, materialPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                // Load the material to ensure it is the same reference
                material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                return material;
            }
#else
            return null;
#endif
        }

        public static Texture2D GetTexture2D(MipTexture mipTexture, FilterMode filterMode, string saveAndLoadPath)
        {
#if UNITY_EDITOR
            // saveAndLoadPath should be <export folder>/assets/<folder containing wad>/<wad name>
            var texturePath = Path.Combine(saveAndLoadPath, mipTexture.TextureName) + ".png";

            // Try to load the texture at that path
            var existingTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if(existingTexture != null)
            {
                return existingTexture;
            }
            else
            {
                // This will be used if a texture can't be found / saveAndLoad is false
                var texture = mipTexture.Texture;
                texture.name = mipTexture.TextureName;

                // Else, we need to save the texture at the same path
                if (!Directory.Exists(saveAndLoadPath))
                {
                    Directory.CreateDirectory(saveAndLoadPath);
                }

                // Save the texture to a PNG 
                byte[] textureData = texture.EncodeToPNG();
                using (var file = File.Open(texturePath, FileMode.Create))
                {
                    file.Write(textureData, 0, textureData.Length);
                }

                AssetDatabase.Refresh();

                // Get the texture's import settings to set the texture filter mode
                TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                importer.filterMode = filterMode;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.SaveAndReimport();

                // Load the texture to ensure it is the same reference
                texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                return texture;
            }
#else
            return null;
#endif
        }

        public static Cubemap GetSkyboxCubemap(string skyboxName, FilterMode filterMode, string rootPath, string modName)
        {
#if UNITY_EDITOR
            // saveAndLoadPath should be <export folder>/assets/<mod (game) name>/skybox
            var saveAndLoadPath = Path.Combine("Assets/_uGoldSrc", "assets", modName, "skybox");
            var cubemapPath = Path.Combine(saveAndLoadPath, skyboxName) + ".png";

            // Try to load the texture at that path
            var existingCubemap = AssetDatabase.LoadAssetAtPath<Cubemap>(cubemapPath);
            if (existingCubemap != null)
            {
                return existingCubemap;
            }
            else
            {
                // Attempt to find the skybox in the valve folder (if the mod name isn't valve)
                if (modName != "valve")
                {
                    existingCubemap = AssetDatabase.LoadAssetAtPath<Cubemap>(Path.Combine("Assets/_uGoldSrc/assets/valve/skybox", skyboxName) + ".png");
                    if (existingCubemap != null)
                    {
                        return existingCubemap;
                    }
                }

                // Else, we need to create the texture at the same path

                // Skyboxes are stored in <game install>/<mod name>/gfx/env
                var skyTexturesPath = Path.Combine(rootPath, modName, "gfx", "env");
                var skySuffixes = new string[] { "up", "ft", "lf", "bk", "rt", "dn" };

                // Each sky texture is placed separately on the grid
                Vector2Int[] cubemapPlacements = new Vector2Int[]
                {
                    new Vector2Int(1, 2),
                    new Vector2Int(2, 1),
                    new Vector2Int(3, 1),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 0),
                };

                // Get each sky texture
                bool useAlternatePath = false;
                var textures = new Texture2D[6];
                for (int i = 0; i < textures.Length; i++)
                {
                    // Generate the sky texture path
                    var skyTexturePath = Path.Combine(skyTexturesPath, skyboxName) + skySuffixes[i] + ".tga";

                    // If the file doesn't exist in the current mod then default to the valve folder
                    if(!File.Exists(skyTexturePath))
                    {
                        useAlternatePath = true;

                        skyTexturesPath = Path.Combine(rootPath, "valve", "gfx", "env");
                        skyTexturePath = Path.Combine(skyTexturesPath, skyboxName) + skySuffixes[i] + ".tga";

                        saveAndLoadPath = Path.Combine("Assets/_uGoldSrc", "assets", "valve", "skybox");
                        cubemapPath = Path.Combine(saveAndLoadPath, skyboxName) + ".png";

                        if (!Directory.Exists(saveAndLoadPath))
                        {
                            Directory.CreateDirectory(saveAndLoadPath);
                        }
                    }

                    // The filepath where the sky texture will be saved to
                    var savedSkyTexturePath = Path.Combine(saveAndLoadPath, skyboxName) + skySuffixes[i] + ".tga";

                    if (!Directory.Exists(saveAndLoadPath))
                    {
                        Directory.CreateDirectory(saveAndLoadPath);
                    }

                    // Copy the file over from the install path to the assets path
                    File.Copy(skyTexturePath, savedSkyTexturePath, true);
                    AssetDatabase.Refresh();

                    // Get the importer and mark the textures as readable
                    var textureImporter = AssetImporter.GetAtPath(savedSkyTexturePath) as TextureImporter;
                    textureImporter.isReadable = true;
                    textureImporter.SaveAndReimport();
                    AssetDatabase.Refresh();

                    // Load the sky texture to ensure it's the correct reference
                    var skyTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(savedSkyTexturePath);
                    if (skyTexture != null)
                    {
                        textures[i] = skyTexture;
                    }
                }

                // Ensure that the textures are all the same size
                var size = textures[0].width;
                if (textures.Any(x => (x.width != size) || (x.height != size)))
                {
                    Debug.LogError("All textures in a cubemap must be the same width/height!");
                    return null;
                }

                // Now, generate the cubemap
                var skyboxCubemap = new Texture2D(size * 4, size * 3, TextureFormat.RGB24, false);
                for (int i = 0; i < 6; i++)
                {
                    // Set the pixels of each area in the cubemap
                    skyboxCubemap.SetPixels(cubemapPlacements[i].x * size, cubemapPlacements[i].y * size, size, size, textures[i].GetPixels());

                    // The filepath where the sky texture was saved to
                    var savedModName = useAlternatePath == true ? "valve" : modName;
                    var savedSkyTexturePath = Path.Combine("Assets/_uGoldSrc", "assets", savedModName, "skybox", skyboxName) + skySuffixes[i] + ".tga";

                    // Finally, delete the copied sky texture
                    AssetDatabase.DeleteAsset(savedSkyTexturePath);
                }

                AssetDatabase.Refresh();

                // Apply the changes to the cubemap texture
                skyboxCubemap.Apply(false);

                // Save the cubemap as a png file
                var cubemapBytes = skyboxCubemap.EncodeToPNG();
                File.WriteAllBytes(cubemapPath, cubemapBytes);
                AssetDatabase.Refresh();

                // Get the cubemap's import settings to set the texture filter mode
                TextureImporter importer = AssetImporter.GetAtPath(cubemapPath) as TextureImporter;
                importer.textureShape = TextureImporterShape.TextureCube;
                importer.filterMode = filterMode;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.SaveAndReimport();
                AssetDatabase.Refresh();

                // Load the cubemap to ensure it is the same reference
                var cubemap = AssetDatabase.LoadAssetAtPath<Cubemap>(cubemapPath);
                return cubemap;
            }
#else
            return null;
#endif
        }
    }
}