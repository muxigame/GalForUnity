using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor.UIElements{
    public class RoleWizard : EditorWindow
    {
        [SerializeField] private RoleAssets roleAssets;

        private PoseList _poseList;

        public RoleAssets RoleAssets
        {
            get => roleAssets;
            set
            {
                roleAssets = value;
                InitBind();
            }
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GalForUnity/Core/Scripts/Editor/RoleWizard.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);
            Debug.Log(roleAssets);
            var name = root.Q<TextField>("Name");
            var enumField = root.Q<EnumField>("Gender");
            var button = root.Q<Button>("CreatePoseButton");
            var save = root.Q<Button>("Save");


            enumField.Init(Gender.Girl);
            button.clickable = new Clickable(() =>
            {
                var spritePose = new SpritePose();
                roleAssets.pose.Add(spritePose);
                _poseList.RefreshItems();
                CreatePoseWizard.Show(spritePose);
            });

            save.clickable = new Clickable(() =>
            {
                if (!AssetDatabase.IsMainAsset(roleAssets))
                {
                    AssetDatabase.CreateAsset(roleAssets, Path.Combine("Assets", name.value) + ".asset");
                    EditorGUIUtility.PingObject(roleAssets);
                }
                else
                {
                    AssetDatabase.SaveAssetIfDirty(roleAssets);
                }
            });
            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GalForUnity/Core/Scripts/Editor/RoleWizard.uss");
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
                if (loadAssetAtPath == null || loadAssetAtPath != typeof(RoleAssets)) return false;
                Show(AssetDatabase.LoadAssetAtPath<RoleAssets>(path));
                return true;
            }

            return false;
        }

        private void InitBind()
        {
            if (!roleAssets) return;
            var serializedObject = new SerializedObject(roleAssets);
            var name = rootVisualElement.Q<TextField>("Name");

            var enumField = rootVisualElement.Q<EnumField>("Gender");
            var age = rootVisualElement.Q<TextField>("Age");
            var height = rootVisualElement.Q<TextField>("Height");
            var weight = rootVisualElement.Q<TextField>("Weight");

            enumField.BindProperty(serializedObject.FindProperty("gender"));
            name.BindProperty(serializedObject.FindProperty("roleName"));
            age.BindProperty(serializedObject.FindProperty("age"));
            height.BindProperty(serializedObject.FindProperty("height"));
            weight.BindProperty(serializedObject.FindProperty("weight"));
            if (rootVisualElement.Q<PoseList>() == null && roleAssets)
            {
                var postContent = rootVisualElement.Q<VisualElement>("PostContent");
                var preview = rootVisualElement.Q<VisualElement>("Preview");
                var poseView = new PoseView(false);
                preview.Add(poseView);
                postContent.Add(_poseList = new PoseList(roleAssets.pose, poseView, item =>
                {
                    var deleteButton = item.Q<Button>("DeleteButton");
                    deleteButton.clickable = new Clickable(() =>
                    {
                        roleAssets.pose.Remove((Pose)item.userData);
                        _poseList.RefreshItems();
                    });
                }));
            }
        }

        public static void Show(RoleAssets roleAssets)
        {
            var wnd = GetWindow<RoleWizard>();
            wnd.titleContent = new GUIContent("RoleWizard");
            wnd.RoleAssets = roleAssets;
        }

        [MenuItem("GalForUnity/角色创建向导")]
        public static void ShowExample()
        {
            var wnd = GetWindow<RoleWizard>();
            wnd.titleContent = new GUIContent("RoleWizard");
            wnd.RoleAssets = CreateInstance<RoleAssets>();
        }
    }
}