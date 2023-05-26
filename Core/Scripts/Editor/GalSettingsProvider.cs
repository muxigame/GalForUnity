using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor
{
    internal class GalSettingsProvider : AssetSettingsProvider, IPreprocessBuildWithReport
    {
        private VisualElement _rootElement;
        private string _searchContext;

        public GalSettingsProvider() : base("Project/" + Styles.GalSettings, () => CurrentSettings)
        {
            if (CurrentSettings == null) CurrentSettings = FindSettings();
            keywords = GetSearchKeywordsFromGUIContentProperties<GalSettings>();
        }

        public static GalSettings CurrentSettings
        {
            get
            {
                EditorBuildSettings.TryGetConfigObject(GalSettings.ConfigName, out GalSettings settings);
                return settings;
            }
            set
            {
                if (value == null)
                    EditorBuildSettings.RemoveConfigObject(GalSettings.ConfigName);
                else
                    EditorBuildSettings.AddConfigObject(GalSettings.ConfigName, value, true);
            }
        }

        public int callbackOrder => 0;


        public void OnPreprocessBuild(BuildReport report)
        {
            var currentSettings = CurrentSettings;
            var settingsType = currentSettings.GetType();
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.RemoveAll(settings => settings.GetType() == settingsType);
            preloadedAssets.Add(currentSettings);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _rootElement = rootElement;
            _searchContext = searchContext;
            base.OnActivate(searchContext, rootElement);
        }

        public override void OnGUI(string searchContext)
        {
            DrawCurrentSettingsGUI();
            EditorGUILayout.Space();

            var invalidSettings = CurrentSettings == null;
            if (invalidSettings) DisplaySettingsCreationGUI();
            else base.OnGUI(searchContext);
        }

        private void DrawCurrentSettingsGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel++;
            var settings = EditorGUILayout.ObjectField("Current Settings", CurrentSettings, typeof(GalSettings), false) as GalSettings;
            EditorGUI.indentLevel--;

            var newSettings = EditorGUI.EndChangeCheck();
            if (newSettings)
            {
                CurrentSettings = settings;
                RefreshEditor();
            }
        }

        private void RefreshEditor()
        {
            base.OnActivate(_searchContext, _rootElement);
        }

        private void DisplaySettingsCreationGUI()
        {
            const string message = "You have no Scene Loader Settings. Would you like to create one?";
            EditorGUILayout.HelpBox(message, MessageType.Info, true);
            if (CurrentSettings == null)
            {
                var clicked = GUILayout.Button("Create");
                if (clicked) CurrentSettings = CreateGalSetting();
            }
        }

        public static GalSettings FindSettings()
        {
            var filter = $"t:{nameof(GalSettings)}";
            var guids = AssetDatabase.FindAssets(filter);
            var hasGuids = guids.Length > 0;
            var path = hasGuids ? AssetDatabase.GUIDToAssetPath(guids[0]) : string.Empty;

            return AssetDatabase.LoadAssetAtPath<GalSettings>(path);
        }

        private static GalSettings CreateGalSetting()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Save Scene Loader Settings", "DefaultSceneLoaderSettings", "asset",
                "Please enter a filename to save the projects Scene Loader settings.");
            var invalidPath = string.IsNullOrEmpty(path);
            if (invalidPath) return null;

            var settings = ScriptableObject.CreateInstance<GalSettings>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();

            Selection.activeObject = settings;
            return settings;
        }

        [SettingsProvider]
        public static SettingsProvider CreateGalSettingsProvider()
        {
            return new GalSettingsProvider
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
        }

        private class Styles
        {
            public static readonly GUIContent GalSettings = new("GalSettings");
            public static readonly GUIContent SomeString = new("Some string");
        }
    }
}