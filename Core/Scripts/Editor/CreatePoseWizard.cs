//======================================================================
//
//       CopyRight 2019-2023 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  CreatePoseWizard.cs
//
//        Created by 半世癫(Roc) at 2023-01-18 00:53:17
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GalForUnity.Core.Scripts.Editor
{
    public class CreatePoseWizard : EditorWindow
    {
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;
            // Import UXML
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/GalForUnity/Core/Scripts/Editor/CreatePoseWizard.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);
            var button = labelFromUXML.Q<Button>("CreatePoseButton");
            var poseEditor = labelFromUXML.Q<VisualElement>("PoseEditor");
            var poseView = labelFromUXML.Q<PoseView>("PoseView");
            button.clickable = new Clickable(() =>
            {
                var poseBindingAnchor = new PoseBindingAnchor();
                poseView.Add(poseBindingAnchor);
                poseEditor.Add(new PoseBindingPoint(poseBindingAnchor,poseView));
            });
            poseView.RegisterCallback<MouseUpEvent>((x) =>
            {
                var type = Type.GetType("UnityEditor.ObjectSelector,UnityEditor");
                var objectSelector = type.GetProperty("get", BindingFlags.Public | BindingFlags.Static)
                    .GetValue(null, null);
                Action<Object> Action = (selectedObject) =>
                {
                    if (selectedObject is Sprite sprite)
                    {
                        poseView.ShowPose(sprite);
                    }

                    if (selectedObject == null)
                    {
                        poseView.RemovePose();
                    }
                };
                type.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null,
                        new Type[7]
                        {
                            typeof(Object),
                            typeof(Type[]),
                            typeof(Object),
                            typeof(bool),
                            typeof(List<int>),
                            typeof(Action<Object>),
                            typeof(Action<Object>)
                        }, null)
                    ?.Invoke(objectSelector,
                        new object[7]
                        {
                            null, new[] { typeof(Sprite) }, null, false, null, Action, null
                        })
                    ;

                // ObjectSelectorWindow.Show(new ObjectSelectorSearchContext()
                // {
                //     // requiredTypes = new []{typeof(Sprite),typeof(Texture),typeof(Texture2D),},
                //     visibleObjects = VisibleObjects.Assets
                // }, (x) =>
                // {
                //     
                // }, (unityObject,y) =>
                // {
                //     poseContent.style.backgroundImage = new StyleBackground((Sprite)unityObject);
                // });
            });
            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GalForUnity/Core/Scripts/Editor/CreatePoseWizard.uss");
        }

        [MenuItem("Window/UI Toolkit/CreatePoseWizard")]
        public static void ShowExample()
        {
            var wnd = GetWindow<CreatePoseWizard>();
            wnd.titleContent = new GUIContent("CreatePoseWizard");
        }
    }
}