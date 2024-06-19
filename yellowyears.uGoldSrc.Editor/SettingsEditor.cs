using UnityEditor;
using UnityEngine.UIElements;

namespace yellowyears.uGoldSrc.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : UnityEditor.Editor
    {
        public VisualTreeAsset inspectorUXML;

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement inspector = new VisualElement();

            // Load from default reference
            inspectorUXML.CloneTree(inspector);

            // Return the finished inspector UI
            return inspector;
        }
    }
}