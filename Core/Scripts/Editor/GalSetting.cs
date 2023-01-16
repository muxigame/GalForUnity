using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Scripts.Editor{
    [CreateAssetMenu(fileName = "GalSetting", menuName = "ScriptableObjects/GalSetting", order = 1)]
    public class GalSetting : ScriptableObject{
        public string name;
    }

    internal class GalSettingsProvider : SettingsProvider{
        private GalSetting _galSetting;
        private SerializedObject _customSettings;

        public GalSettingsProvider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope){ }

        public static bool IsSettingsAvailable(){ return true; }

        public override void OnActivate(string searchContext, VisualElement rootElement){
            if (!_galSetting){
                if (!string.IsNullOrEmpty(GalPrefs.instance.globalID) && GlobalObjectId.TryParse(GalPrefs.instance.globalID, out var id)){
                    _galSetting = (GalSetting) GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
                } else if (GalPrefs.instance.globalID == null){
                    var instance = ScriptableObject.CreateInstance<GalSetting>();
                    AssetDatabase.CreateAsset(instance, "Assets/GalSetting.asset");
                    _galSetting = instance;
                    GalPrefs.instance.globalID = GlobalObjectId.GetGlobalObjectIdSlow(instance).ToString();
                    GalPrefs.instance.Save();
                }
            }

            if (_galSetting) _customSettings = new SerializedObject(_galSetting);
        }

        public override void OnGUI(string searchContext){
            if (_customSettings == null && _galSetting != null) _customSettings = new SerializedObject(_galSetting);
            var targetObject = _galSetting;
            _galSetting = (GalSetting) EditorGUILayout.ObjectField(Styles.GalSetting, targetObject, typeof(GalSetting), false);
            if (targetObject != _galSetting){
                GalPrefs.instance.globalID = GlobalObjectId.GetGlobalObjectIdSlow(_galSetting).ToString();
                GalPrefs.instance.Save();
            }

            if (_galSetting == null && GalPrefs.instance.globalID != null){
                GalPrefs.instance.globalID = "";
                GalPrefs.instance.Save();
            }

            if (_customSettings == null) return;

            EditorGUILayout.PropertyField(_customSettings.FindProperty("name"), Styles.GalSetting);
            _customSettings.ApplyModifiedPropertiesWithoutUndo();
        }


        [SettingsProvider]
        public static SettingsProvider CreateGalSettingsProvider(){
            return  new GalSettingsProvider("Project/GalSetting", SettingsScope.Project){
                keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
            };
        }

        private class Styles{
            public static readonly GUIContent GalSetting = new GUIContent("GalSetting");
            public static readonly GUIContent SomeString = new GUIContent("Some string");
        }
    }
}