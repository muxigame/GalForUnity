

using System;
using GalForUnity.Graph.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor.UIElements
{
    public class CreatePoseWizard : EditorWindow
    {
        [NonSerialized] public SpritePose SpritePose;
        private bool binded;
        private ListView bindingPoints;

        private void Update()
        {
            if (SpritePose == null) Close();
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/GalForUnity/Core/Scripts/Editor/CreatePoseWizard.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/GalForUnity/Core/Scripts/Editor/CreatePoseWizard.uss");
        }

        private void InitBind()
        {
            if (SpritePose == null || binded) return;
            binded = true;
            var serializedObject = new SerializedObject(this);
            var button = rootVisualElement.Q<Button>("CreatePoseButton");
            var poseName = rootVisualElement.Q<TextField>("PoseName");
            var poseView = rootVisualElement.Q<PoseView>("PoseView");
            var type = typeof(SpritePose);
            var poseEditor = rootVisualElement.Q<VisualElement>("PoseEditor");
            poseName.TrackSerializedObjectValue(serializedObject);
            poseName.CreateBinder(type.GetField("name"), SpritePose);
            poseView.TrackSerializedObjectValue(serializedObject);
            poseView.CreateBinder(type.GetField("sprite"), SpritePose);
            poseView.SetValueWithoutNotify(SpritePose.sprite);
            foreach (var spritePoseBindingPoint in SpritePose.bindingPoints)
            {
                var poseBindingAnchor = new PoseBindingAnchor(spritePoseBindingPoint.point);
                poseView.Add(poseBindingAnchor);
                poseEditor.Add(new PoseBindingPoint(poseBindingAnchor, poseView, spritePoseBindingPoint));
            }
            button.clickable = new Clickable(() =>
            {
                var poseBindingAnchor = new PoseBindingAnchor();
                var bindingPoint = new BindingPoint();
                SpritePose.bindingPoints.Add(bindingPoint);
                poseView.Add(poseBindingAnchor);
                poseEditor.Add(new PoseBindingPoint(poseBindingAnchor, poseView, bindingPoint));
            });
        }
        [MenuItem("GalForUnity/调试/姿势创建向导")]
        public static void ShowExample()
        {
            var wnd = GetWindow<CreatePoseWizard>();
            wnd.titleContent = new GUIContent("CreatePoseWizard");
        }

        public static void Show(SpritePose spritePose)
        {
            var wnd = CreatePoseWizard.CreateWindow<CreatePoseWizard>();
            wnd.SpritePose = spritePose;
            wnd.titleContent = new GUIContent("CreatePoseWizard");
            wnd.Show();
            wnd.InitBind();
        }
    }
}