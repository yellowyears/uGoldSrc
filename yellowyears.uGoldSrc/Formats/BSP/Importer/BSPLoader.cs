using System;
using System.IO;
using UnityEngine;
using yellowyears.uGoldSrc.Formats.BSP.Types;
using yellowyears.uGoldSrc.Components;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace yellowyears.uGoldSrc.Formats.BSP.Importer
{
    [AddComponentMenu("uGoldSrc/BSP Loader")]
    public class BSPLoader : MonoBehaviour
    {

        #region Game Settings

        [SerializeField] private string modName = "valve";
        [SerializeField] private string rootPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Half-Life";

        #endregion

        #region BSP Import Settings

        [SerializeField] private float unitScale = 0.021875f;
        [SerializeField] private FilterMode textureFilterMode = FilterMode.Point;
        [SerializeField] private FilterMode skyboxFilterMode = FilterMode.Bilinear;
        [SerializeField] private LayerMask triggerLayer;
        [SerializeField] private LayerMask staticLayer;
        [SerializeField] private string mapName;

        #endregion

        private BSP30 map;
        private MapInfo mapInfo;

        private Transform mapGroup;
        private Transform entityGroup;
        private Transform faceGroup;
        private Transform lightGroup;

        private string skyboxName = "desert";

#if UNITY_EDITOR
        [MenuItem("GameObject/uGoldSrc/BSP Loader", priority = 1)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            // Create a custom game object
            GameObject go = new GameObject("uGoldSrc Loader");
            go.AddComponent<BSPLoader>();

            // Ensure it gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif

        private void Initialise()
        {
            map = null;
            mapInfo = null;

            mapGroup = null;
            entityGroup = null;
            faceGroup = null;
            lightGroup = null;
        }

        public void LoadMap()
        {
            Debug.Log($"uGoldSrc: Starting to read {mapName}");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Initialise();

            map = BSPReader.Read(rootPath, modName, mapName, unitScale);

            SetupGroups();

            mapInfo = mapGroup.gameObject.AddComponent<MapInfo>();
            mapInfo.mapName = map.Header.Name;
            mapInfo.mapVersion = map.Header.Version;
            mapInfo.mapScale = map.Header.Scale;

            LoadEntitites();
            LoadMapGeometry();

#if UNITY_EDITOR
            Selection.activeGameObject = mapGroup.gameObject;
            EditorUtility.SetDirty(mapInfo);
#endif

            watch.Stop();
            Debug.Log($"uGoldSrc: Finished reading {mapName} in {watch.ElapsedMilliseconds}ms");
        }

        private void SetupGroups()
        {
            mapGroup = new GameObject(mapName).transform;

            entityGroup = new GameObject("{ Entities }").transform;
            entityGroup.parent = mapGroup;

            faceGroup = new GameObject("{ Faces }").transform;
            faceGroup.parent = mapGroup;

            lightGroup = new GameObject("{ Lights }").transform;
            lightGroup.parent = mapGroup;
        }

        private void LoadEntitites()
        {
            foreach (var entity in map.EntityLump.Entities)
            {
                // Create and setup entity objects
                var entityObject = new GameObject(entity.className);
                entityObject.transform.parent = entityGroup;

                var entityInfo = entityObject.AddComponent<EntityInfo>();
                mapInfo.entityInfos.Add(entityInfo);
                entityInfo.entity = entity;

                // Setup transform position
                string rawOrigin = Utilities.GetAttributeValue(entity, "origin");

                if (rawOrigin != null)
                {
                    // Split the origin by its spaces
                    string[] splitOriginValues = rawOrigin.Split(" ");

                    // Parse the split values into a Vector3 and multiply by map scale
                    Vector3 origin = Utilities.FixVector3(new Vector3(Convert.ToSingle(splitOriginValues[0]), Convert.ToSingle(splitOriginValues[1]), Convert.ToSingle(splitOriginValues[2]))) * unitScale;
                    entityObject.transform.position = origin;
                }

                // Setup transform angle
                string rawAngle = Utilities.GetAttributeValue(entity, "angle");

                if(rawAngle != null)
                {
                    // Parse the raw angle into a euler angle
                    Vector3 angle = new Vector3(0, -Convert.ToSingle(rawAngle), 0);
                    entityObject.transform.eulerAngles = angle;
                }

                // Special cases for certain classnames

                // Light entities
                if (entity.className.StartsWith("light"))
                {
                    entityObject.transform.parent = lightGroup;

                    Light light = entityObject.AddComponent<Light>();
#if UNITY_EDITOR
                    light.lightmapBakeType = LightmapBakeType.Baked;
#endif
                    light.type = LightType.Point;

                    string rawLight = Utilities.GetAttributeValue(entity, "_light");
                    string[] splitLightValues = rawLight.Split(" ");
                    Color lightColour = Utilities.GetLightColour(splitLightValues);

                    // The light is stored as R, G, B, Intensity
                    light.color = new Color(lightColour.r, lightColour.g, lightColour.b, 1);
                    light.intensity = lightColour.a * Settings.Instance.lightIntensityScale;

                    // Spotlights have special
                    if (entity.className == "light_spot")
                    {
                        light.type = LightType.Spot;

                        string rawPitch = Utilities.GetAttributeValue(entity, "pitch");

                        // Overrides entity angle
                        float pitch = Convert.ToSingle(rawPitch);
                        
                        // In GoldSrc, -90 is straight down but in unity 90 is straight down
                        entityObject.transform.eulerAngles = new Vector3(-pitch, 0, 0);

                        // Set inner/outer spot
                        light.innerSpotAngle = Convert.ToSingle(Utilities.GetAttributeValue(entity, "_cone"));
                    }
                }

                // World Spawn
                if(entity.className == "worldspawn")
                {
                    // Try get skybox name
                    var skyName = Utilities.GetAttributeValue(entity, "skyname");

                    // If there is a skyname attribute then we want to change from the default skybox
                    if(skyName != null)
                    {
                        skyboxName = skyName;
                    }
                }
            }
        }

        private void LoadMapGeometry()
        {
            // We can put all of the faces in the 0th model in a separate object
            var mapGeometryObject = new GameObject("*0");
            mapGeometryObject.transform.parent = faceGroup;

            // Load all of the map geometry, which is the 0th model
            LoadModel(0, mapGeometryObject.transform);

            // For the rest of the models, their indexes are found on the entities
            for(int i = 0; i < mapInfo.entityInfos.Count; i++)
            {
                var entityInfo = mapInfo.entityInfos[i];

                // Get and check whether the entity has a model, and if it contains an index (starts with *)
                var modelAttribute = Utilities.GetAttributeValue(entityInfo.entity, "model");
                if (modelAttribute != null && modelAttribute.StartsWith('*'))
                {
                    // Get the number after the * which is the index into the model
                    var modelIndex = Convert.ToInt32(modelAttribute.Split("*")[1]);

                    // As this entity contains a model, we should move it into the face group
                    entityInfo.transform.parent = faceGroup;

                    // If the entity has an origin attribute we need to reset the face positions to 0,0,0
                    bool hasOrigin = Utilities.GetAttributeValue(entityInfo.entity, "origin") != null;

                    // If the entity has an angles we also need to reset the rotation to 0,0,0
                    bool hasAngles = Utilities.GetAttributeValue(entityInfo.entity, "angles") != null;

                    // Load the current model
                    LoadModel(modelIndex, entityInfo.transform, hasOrigin, hasAngles);
                }
            }
        }

        private void LoadModel(int modelIndex, Transform parent, bool resetFacePositions = false, bool resetFaceRotations = false)
        {
            // Get the model from the model lump
            var model = map.ModelLump.Models[modelIndex];

            // Loop through the model's faces 
            for (int i = 0; i < model.NumFaces; i++)
            {
                // Get all of the necessary objects from the lumps
                var face = map.FaceLump.Faces[model.FirstFace + i];
                var textureInfo = map.TextureInfoLump.TextureInfos[face.TextureInfoIndex];
                var mipTexture = map.MipTextureLump.MipTextures[textureInfo.MipTextureIndex];

                // Create the mesh of the face
                var faceMesh = CreateMesh(face);

                // Create the mesh object and add a mesh filter
                var meshObject = new GameObject($"{mipTexture.WadFolderName}/{mipTexture.WadName}/{mipTexture.TextureName}");
                var meshFilter = meshObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = faceMesh;

                // Add a mesh renderer and set its shared material to avoid a leak
                var meshRenderer = meshObject.AddComponent<MeshRenderer>();

                // The folder that the materials/textures are saved in
                var assetsPath = Path.Combine("Assets/_uGoldSrc", "assets", mipTexture.WadFolderName, mipTexture.WadName);

                var texture = Utilities.GetTexture2D(mipTexture, textureFilterMode, assetsPath);

                // Get the material for this face. If the texture name is "sky" then we want to use the skybox material
                Material material;
                if (texture.name == "sky")
                {
                    var skyboxCubemap = Utilities.GetSkyboxCubemap(skyboxName, skyboxFilterMode, rootPath, modName);

                    if(File.Exists(Path.Combine("Assets/_uGoldSrc", "assets", modName, "skybox", skyboxCubemap.name) + ".png"))
                    {
                        material = Utilities.GetSkyboxMaterial(skyboxCubemap, modName);
                    }
                    else
                    {
                        material = Utilities.GetSkyboxMaterial(skyboxCubemap, "valve");
                    }
                }
                else
                {
                    material = Utilities.GetMaterial(texture, assetsPath);
                }

                meshRenderer.sharedMaterial = material;

                // Set the object's parent 
                meshObject.transform.parent = parent.transform;

                // This is only used for entities with an origin attribute, it fixes an issue with models being placed at 0,0,0
                if (resetFacePositions)
                {
                    meshObject.transform.localPosition = Vector3.zero;
                }

                // Similarly to the positions this fixes the rotations for entities with the angles attribute
                if(resetFaceRotations)
                {
                    meshObject.transform.localEulerAngles = Vector3.zero;
                }
            }

            mapInfo.models.Add(parent.gameObject);
        }

        private Mesh CreateMesh(Face face)
        {
            var vertices = new Vector3[face.NumEdges];
            // Create the array of vertices from the edges
            for (int i = 0; i < face.NumEdges; i++)
            {
                var edgeIndex = map.SurfEdgeLump.SurfEdges[face.FirstEdge + i].SurfEdgeIndex;
                var edge = map.EdgeLump.Edges[Mathf.Abs(edgeIndex)];
                vertices[i] = map.VertexLump.Vertices[edgeIndex > 0 ? edge.Start : edge.End].VertexPosition;
            }

            // Triangulate the mesh
            var triangles = new int[(face.NumEdges - 2) * 3];
            for (int i = 1, j = 0; i < vertices.Length - 1; i++, j += 3)
            {
                triangles[j] = 0;
                triangles[j + 1] = i;
                triangles[j + 2] = i + 1;
            }

            // Get UVs from the TextureInfo data
            var textureInfo = map.TextureInfoLump.TextureInfos[face.TextureInfoIndex];
            var mipTexture = map.MipTextureLump.MipTextures[textureInfo.MipTextureIndex];
            var width = mipTexture.Width;
            var height = mipTexture.Height;

            var uvs = new Vector2[face.NumEdges];
            for (int i = 0; i < uvs.Length; i++)
            {
                // uvs.z is negative due to earlier texture flipping for correct saving
                uvs[i] = new Vector2((Vector3.Dot(vertices[i], textureInfo.XScale) + textureInfo.XShift * unitScale) / (width * unitScale), -(Vector3.Dot(vertices[i], textureInfo.YScale) + textureInfo.YShift * unitScale) / (height * unitScale));
            }

            // Create the mesh
            var faceMesh = new Mesh
            {
                name = mipTexture.TextureName,
                vertices = vertices,
                triangles = triangles,
                uv = uvs,
            };

            faceMesh.RecalculateNormals();

            return faceMesh;
        }
    }
}
