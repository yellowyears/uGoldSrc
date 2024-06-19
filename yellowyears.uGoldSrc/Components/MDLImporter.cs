#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using yellowyears.uGoldSrc.Formats.MDL.Importer;

namespace yellowyears.uGoldSrc.Components
{
    public class MDLImporter : MonoBehaviour
    {
        public string rootPath;
        public string modName; // This is the name of the folder which contains the maps e.g valve or 
        public string modelName;
        public float unitScale = 0.021875f;

        public void Import()
        {
            var mdl = MDLReader.Read(rootPath, modName, modelName, unitScale);

            Debug.Log(mdl.Header.Name);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MDLImporter))]
    public class MDLImporterEditor : Editor
    {
        private MDLImporter _importer;

        private void OnEnable()
        {
            _importer = (MDLImporter)target;
        }

        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Test"))
            {
                _importer.Import();
            }
        }
    }
#endif
}