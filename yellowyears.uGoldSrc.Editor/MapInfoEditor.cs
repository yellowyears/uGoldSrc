using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using yellowyears.uGoldSrc.Components;

namespace yellowyears.uGoldSrc.Editor
{
    [CustomEditor(typeof(MapInfo))]
    public class MapInfoEditor : UnityEditor.Editor
    {
        public VisualTreeAsset inspectorUXML;

        private MapInfo _mapInfo;

        private void OnEnable()
        {
            _mapInfo = (MapInfo)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement inspector = new VisualElement();

            // Load from default reference
            inspectorUXML.CloneTree(inspector);

            // Get references to the buttons
            var setLayersAndFlagsButton = inspector.Q<Button>("SetLayersAndFlags");
            var combineButton = inspector.Q<Button>("CombineByMaterial");
            var separateButton = inspector.Q<Button>("SeparateByLooseParts");
            var exportMapButton = inspector.Q<Button>("ExportMap");

            // Assign functionality to the buttons
            setLayersAndFlagsButton.RegisterCallback<ClickEvent>(OnSetLayersAndFlagsButtonPressed);
            combineButton.RegisterCallback<ClickEvent>(OnCombineButtonClicked);
            separateButton.RegisterCallback<ClickEvent>(OnSeparateButtonClicked);
            exportMapButton.RegisterCallback<ClickEvent>(OnExportMapButtonClicked);

            // Get references to fields
            var mapScaleField = inspector.Q<FloatField>("MapScale");
            var mapVersionField = inspector.Q<IntegerField>("MapVersion");
            var mapNameField = inspector.Q<TextField>("MapName");

            // Disable the readonly fields
            mapScaleField.SetEnabled(false);
            mapVersionField.SetEnabled(false);
            mapNameField.SetEnabled(false);

            if (_mapInfo.models.Count == 0)
            {
                setLayersAndFlagsButton.SetEnabled(false);
            }

            if(_mapInfo.combinedMeshFilters.Count == 0)
            {
                separateButton.SetEnabled(false);
            }

            // Return the finished inspector UI
            return inspector;
        }

        private void OnSetLayersAndFlagsButtonPressed(ClickEvent clickEvent)
        {
            _mapInfo.SetLayersAndFlags();
        }

        private void OnCombineButtonClicked(ClickEvent clickEvent)
        {
            _mapInfo.CombineByMaterial();
        }

        private void OnSeparateButtonClicked(ClickEvent clickEvent)
        {
            _mapInfo.SeparateByLooseParts();
        }

        private void OnExportMapButtonClicked(ClickEvent clickEvent)
        {
            _mapInfo.ExportMap();
        }
    }
}
