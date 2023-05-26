using GalForUnity.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace GalForUnity.Core
{
    [CreateAssetMenu(fileName = "GalSettings", menuName = "ScriptableObjects/GalSettings", order = 1)]
    public partial class GalSettings : ScriptableObject
    {
        public const string ConfigName = "com.muxigame.galforunity.settings";
        private static GalSettings _instance;
        [SerializeField]
        private Vector2 globalPoint;

        private static GalSettings Instance
        {
            get
            {
                if (_instance) return _instance;
#if UNITY_EDITOR
                EditorBuildSettings.TryGetConfigObject(ConfigName, out _instance);
                if (!_instance) _instance = GalSettingsProvider.FindSettings();
#else
                _instance = FindObjectOfType<SceneLoaderSettings>();
#endif
                return _instance;
            }
        }

        private void OnEnable()
        {
            if (_instance == null) _instance = this;
        }
    }

    public partial class GalSettings
    {
        public static Vector2 GlobalPoint
        {
            get => Instance.globalPoint;
            set => Instance.globalPoint = value;
        }
    }
}