using System;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using yellowyears.uGoldSrc.Formats.BSP.Importer;

namespace yellowyears.uGoldSrc.Editor
{
    [CustomEditor(typeof(BSPLoader))]
    public class BSPLoaderEditor : UnityEditor.Editor
    {
        public VisualTreeAsset inspectorUXML;

        private BSPLoader _bspLoader;

        private void OnEnable()
        {
            _bspLoader = (BSPLoader)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement inspector = new VisualElement();

            // Load from default reference
            inspectorUXML.CloneTree(inspector);

            // Get references to the buttons
            var loadButton = inspector.Q<Button>("LoadBSP");

            // Assign functionality to the buttons
            loadButton.RegisterCallback<ClickEvent>(OnLoadButtonClick);

            // Return the finished inspector UI
            return inspector;
        }

        private void OnLoadButtonClick(ClickEvent clickEvent)
        {
            _bspLoader.LoadMap();
        }
    }
}