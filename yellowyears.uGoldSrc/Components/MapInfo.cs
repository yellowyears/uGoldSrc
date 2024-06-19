using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace yellowyears.uGoldSrc.Components
{
    public class MapInfo : MonoBehaviour
    {

        #region Map Info

        public string mapName;
        public int mapVersion;
        public float mapScale;

        #endregion

        public List<GameObject> models = new List<GameObject>();

        public List<EntityInfo> entityInfos = new List<EntityInfo>();

        public List<MeshFilter> combinedMeshFilters = new List<MeshFilter>();

        public void SetLayersAndFlags()
        {
            foreach (var model in models)
            {
                bool isTrigger = false;
                bool isStatic = false;

                if (Settings.Instance.triggerEntityClassNames.Any(model.name.Contains))
                {
                    isTrigger = true;
                    isStatic = true;
                    model.layer = Settings.Instance.triggerLayer;
                }
                else if (!Settings.Instance.nonStaticEntityClassNames.Any(model.name.Contains))
                {
                    isStatic = true;
                    model.layer = Settings.Instance.staticLayer;
                }

                model.isStatic = isStatic;

                // Apply the same to the child objects
                foreach (var child in model.GetComponentsInChildren<Transform>())
                {
                    child.gameObject.isStatic = isStatic;

                    if (isTrigger)
                    {
                        child.gameObject.layer = Settings.Instance.triggerLayer;
                        
                        if (child.TryGetComponent<Renderer>(out var renderer)) renderer.enabled = false;
                    }
                    else if (isStatic)
                    {
                        model.layer = Settings.Instance.staticLayer;
                    }

#if UNITY_EDITOR
                    EditorUtility.SetDirty(child.gameObject);
#endif
                }

#if UNITY_EDITOR
                EditorUtility.SetDirty(model);
#endif
            }

            Debug.Log($"Set layers and static flags on {mapName}");
        }

        public void CombineByMaterial()
        {
            // This wouldn't get serialized so it can't really be cached. Not the best solution but works for now
            List<Dictionary<Material, List<MeshFilter>>> modelsSortedMeshFilters = new List<Dictionary<Material, List<MeshFilter>>>();

            foreach (var model in models)
            {
                // The dictionary contains a material and a list of all mesh filters that use that material
                Dictionary<Material, List<MeshFilter>> sortedMeshFilters = new Dictionary<Material, List<MeshFilter>>();

                // Get every mesh filter of the model and sort them into the dictionary
                foreach (var meshFilter in model.GetComponentsInChildren<MeshFilter>())
                {
                    if (!meshFilter.TryGetComponent<Renderer>(out var meshRenderer)) continue;

                    var material = meshRenderer.sharedMaterial;

                    // If the dictionary already contains the material then add it, otherwise create a new list and add it
                    if (sortedMeshFilters.ContainsKey(material))
                    {
                        List<MeshFilter> meshFilters = sortedMeshFilters[material];
                        meshFilters.Add(meshFilter);
                    }
                    else
                    {
                        List<MeshFilter> meshFilters = new List<MeshFilter>
                        {
                            meshFilter
                        };

                        sortedMeshFilters.Add(material, meshFilters);
                    }
                }

                // Add the current model's dictionary to the list of all models
                modelsSortedMeshFilters.Add(sortedMeshFilters);
            }

            int originalMeshCount = 0;
            int combinedMeshCount = 0;
            // Now the meshes are sorted, combine each model's meshes by the material they use
            for (int i = 0; i < modelsSortedMeshFilters.Count; i++)
            {
                foreach (KeyValuePair<Material, List<MeshFilter>> entry in modelsSortedMeshFilters[i])
                {
                    CombineInstance[] combine = new CombineInstance[entry.Value.Count];
                    Transform parent = null;
                    string meshName = "Combined Mesh";

                    var isTrigger = false;
                    var isStatic = false;

                    for(int j = 0; j < combine.Length; j++)
                    {
                        originalMeshCount++;

                        combine[j].mesh = entry.Value[j].sharedMesh;
                        combine[j].transform = entry.Value[j].transform.localToWorldMatrix;

                        parent = entry.Value[j].transform.parent;
                        meshName = entry.Value[j].name;

                        isTrigger = entry.Value[j].gameObject.layer == Settings.Instance.triggerLayer;
                        isStatic = entry.Value[j].gameObject.isStatic;

                        DestroyImmediate(entry.Value[j].gameObject);
                    }

                    Mesh combinedMesh = new Mesh();
                    combinedMesh.name = meshName;
                    combinedMesh.CombineMeshes(combine);

                    var combinedMeshObject = new GameObject(meshName);
                    var combinedMeshFilter = combinedMeshObject.AddComponent<MeshFilter>();
                    combinedMeshFilter.sharedMesh = combinedMesh;

                    combinedMeshCount++;

                    var combinedMeshRenderer = combinedMeshObject.AddComponent<MeshRenderer>();
                    combinedMeshRenderer.sharedMaterial = entry.Key;

                    combinedMeshObject.isStatic = isStatic;

                    if(isTrigger)
                    {
                        combinedMeshObject.layer = Settings.Instance.triggerLayer;
                        combinedMeshRenderer.enabled = false;
                    }
                    else if (isStatic)
                    {
                        combinedMeshObject.layer = Settings.Instance.staticLayer;
                    }

                    combinedMeshObject.transform.parent = parent;

#if UNTIY_EDITOR
                    EditorUtility.SetDirty(combinedMeshObject);
#endif

                    combinedMeshFilters.Add(combinedMeshFilter);
                }
            }

            if (originalMeshCount == combinedMeshCount)
            {
                Debug.LogWarning("This map has already been combined or something went wrong.");
            }
            else
            {
                Debug.Log($"Combined {originalMeshCount} mesh filters into {combinedMeshCount} in {mapName}");
            }
        }

        public void SeparateByLooseParts()
        {
            for (int i = 0; i < combinedMeshFilters.Count; i++)
            {
                // Find nearest vertex
                var mesh = combinedMeshFilters[i].sharedMesh;

                var originalTriangles = mesh.triangles.ToList();
            }
        }

        public void ExportMap()
        {
            var savePath = Path.Combine("Assets/_uGoldSrc", "maps");
            var prefabSavePath = Path.Combine(savePath, "prefabs");
            var modelSavePath = Path.Combine(savePath, "models");
        }
    }
}