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
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class CreatePoseWizard : EditorWindow
{
    [MenuItem("Window/UI Toolkit/CreatePoseWizard")]
    public static void ShowExample()
    {
        CreatePoseWizard wnd = GetWindow<CreatePoseWizard>();
        wnd.titleContent = new GUIContent("CreatePoseWizard");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GalForUnity/Core/Scripts/Editor/CreatePoseWizard.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GalForUnity/Core/Scripts/Editor/CreatePoseWizard.uss");
    }
}