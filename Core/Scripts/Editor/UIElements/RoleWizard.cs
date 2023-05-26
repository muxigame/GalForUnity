using System.IO;
using GalForUnity.Core.External;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor{
    public class RoleWizard : EditorWindow
    {
        [FormerlySerializedAs("roleAssets")] [SerializeField] private GalObject galObject;

        private PoseList _poseList;

        public GalObject GalObject
        {
            get => galObject;
            set
            {
                galObject = value;
                InitBind();
            }
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GalForUnity/Core/Scripts/Editor/UIElements/RoleWizard.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);
            Debug.Log(galObject);
            var name = root.Q<TextField>("Name");
            var enumField = root.Q<EnumField>("Gender");
            var button = root.Q<Button>("CreatePoseButton");
            var save = root.Q<Button>("Save");


            enumField.Init(Gender.Girl);
            button.clickable = new Clickable(() =>
            {
                var spritePose = new SpritePose();
                galObject.pose.Add(spritePose);
                _poseList.RefreshItems();
                CreatePoseWizard.Show(spritePose);
            });

            save.clickable = new Clickable(() =>
            {
                if (!AssetDatabase.IsMainAsset(galObject))
                {
                    AssetDatabase.CreateAsset(galObject, Path.Combine("Assets", name.value) + ".asset");
                    EditorGUIUtility.PingObject(galObject);
                }
                else
                {
                    AssetDatabase.SaveAssetIfDirty(galObject);
                }
                
                var defaultSprite = galObject.GetDefaultSprite();
                if (defaultSprite)
                {
                    EditorGUIUtility.SetIconForObject(galObject,defaultSprite.texture);
                    AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(galObject)).SaveAndReimport();
                }
         

              
            });
            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GalForUnity/Core/Scripts/Editor/UIElements/RoleWizard.uss");
            root.styleSheets.Add(styleSheet);

            InitBind();
        }

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceID);
            if (path.EndsWith(".asset"))
            {
                var loadAssetAtPath = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (loadAssetAtPath == null || loadAssetAtPath != typeof(GalObject)) return false;
                Show(AssetDatabase.LoadAssetAtPath<GalObject>(path));
                return true;
            }

            return false;
        }

        private void InitBind()
        {
            if (!galObject) return;
            var serializedObject = new SerializedObject(galObject);
            var name = rootVisualElement.Q<TextField>("Name");

            var enumField = rootVisualElement.Q<EnumField>("Gender");
            var age = rootVisualElement.Q<TextField>("Age");
            var height = rootVisualElement.Q<TextField>("Height");
            var weight = rootVisualElement.Q<TextField>("Weight");

            enumField.BindProperty(serializedObject.FindProperty("gender"));
            name.BindProperty(serializedObject.FindProperty("objectName"));
            age.BindProperty(serializedObject.FindProperty("age"));
            height.BindProperty(serializedObject.FindProperty("height"));
            weight.BindProperty(serializedObject.FindProperty("weight"));
            if (rootVisualElement.Q<PoseList>() == null && galObject)
            {
                var postContent = rootVisualElement.Q<VisualElement>("PostContent");
                var preview = rootVisualElement.Q<VisualElement>("Preview");
                var poseView = new PoseView(false);
                preview.Add(poseView);
                postContent.Add(_poseList = new PoseList(galObject.pose, poseView, item =>
                {
                    var deleteButton = item.Q<Button>("DeleteButton");
                    deleteButton.clickable = new Clickable(() =>
                    {
                        galObject.pose.Remove((Pose)item.userData);
                        _poseList.RefreshItems();
                    });
                }));
            }
        }

        public static void Show(GalObject galObject)
        {
            var wnd = GetWindow<RoleWizard>();
            wnd.titleContent = new GUIContent("RoleWizard");
            wnd.GalObject = galObject;
        }

        [MenuItem("GalForUnity/角色创建向导")]
        public static void ShowExample()
        {
            var wnd = GetWindow<RoleWizard>();
            wnd.titleContent = new GUIContent("RoleWizard");
            wnd.GalObject = CreateInstance<GalObject>();
        }
    }
}