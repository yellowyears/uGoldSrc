using System.IO;
using UnityEditor;
using yellowyears.uGoldSrc.Formats.RAD.Importer;

namespace yellowyears.uGoldSrc.Editor
{
    public class RADReaderEditor
    {
        [MenuItem("uGoldSrc/Read RAD From Disk")]
        public static void ReadRADFromDisk()
        {
            // Get the path to the RAD file
            var radPath = EditorUtility.OpenFilePanel("Select the lights.rad file", "Assets", "rad");

            // If they backed out of the file panel window
            if (radPath == string.Empty) return;

            // Read the RAD file selected
            var rad = RADReader.Read(radPath);

            Utilities.CreateExportRootFolder();

            // Save the scriptable object to the export folder
            var savePath = $"Assets/_uGoldSrc/{Path.GetFileNameWithoutExtension(radPath)}.asset";
            AssetDatabase.CreateAsset(rad, savePath);
            AssetDatabase.Refresh();

            // Set the RAD in the Settings file
            var savedRad = AssetDatabase.LoadAssetAtPath<RAD>(savePath);
            Settings.Instance.lightsRad = savedRad;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}