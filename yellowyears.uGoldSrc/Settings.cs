using UnityEngine;
using UnityEngine.Rendering;
using yellowyears.uGoldSrc.Formats.RAD.Importer;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace yellowyears.uGoldSrc
{
    public class Settings : ScriptableObject
    {

        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                LoadInstance();

                return _instance;
            }
        }

        private static void LoadInstance()
        {
            if (_instance != null) return;

#if UNITY_EDITOR
            _instance = AssetDatabase.LoadAssetAtPath<Settings>("Assets/_uGoldSrc/uGoldSrc Settings.asset");

            if (_instance != null) return;
#endif

            _instance = CreateInstance<Settings>();

#if UNITY_EDITOR
            Utilities.CreateExportRootFolder();
            AssetDatabase.CreateAsset(_instance, "Assets/_uGoldSrc/uGoldSrc Settings.asset");
#endif

            SetDefaultValues();

#if UNITY_EDITOR
            EditorUtility.SetDirty(_instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        private static void SetDefaultValues()
        {
            if (GraphicsSettings.defaultRenderPipeline == null)
            {
                _instance.litShader = Shader.Find("Standard");
            }
            else
            {
                _instance.litShader = GraphicsSettings.defaultRenderPipeline.defaultShader;
            }
            
            _instance.skyboxShader = Shader.Find("Skybox/Cubemap");
            _instance.skyboxTextureKeyword = "_Tex";

            _instance.wadFolderAliases = new NameAlias[] { new NameAlias("barney", "bshift") };

            _instance.triggerEntityClassNames = new string[] { "trigger_auto", "trigger_autosave", "trigger_camera", "trigger_cdaudio", "trigger_changelevel", "trigger_changetarget", "trigger_counter", "trigger_endsection", "trigger_gravity", "trigger_hurt", "trigger_monsterjump", "trigger_multiple", "trigger_once", "trigger_push", "trigger_relay", "trigger_teleport", "trigger_transition", "func_ladder", "func_friction" };
            _instance.nonStaticEntityClassNames = new string[] { "func_rotating", "func_door", "func_door_rotating", "func_platrot", "func_pushable" };

            // Try to find existing layers, but they can be default with no problem
            var triggerLayer = LayerMask.NameToLayer("Trigger");
            _instance.triggerLayer = triggerLayer == -1 ? 0 : triggerLayer;

            var staticLayer = LayerMask.NameToLayer("Static");
            _instance.staticLayer = staticLayer == -1 ? 0 : staticLayer;

            _instance.lightIntensityScale = 0.05f;
        }

        #region Shaders

        public Shader litShader;
        public Shader skyboxShader;

        public string skyboxTextureKeyword;

        #endregion

        #region Import Settings

        public NameAlias[] wadFolderAliases;

        #endregion

        #region Map Layers and Static Flags

        public int triggerLayer;
        public int staticLayer;

        public string[] triggerEntityClassNames;
        public string[] nonStaticEntityClassNames;

        #endregion

        #region Lighting

        public RAD lightsRad;
        public float lightIntensityScale;

        #endregion

        [Serializable]
        public class NameAlias
        {
            public string originalFolderName;
            public string actualFolderName;

            public NameAlias(string originalFolderName, string actualFolderName)
            {
                this.originalFolderName = originalFolderName;
                this.actualFolderName = actualFolderName;
            }
        }
    }
}