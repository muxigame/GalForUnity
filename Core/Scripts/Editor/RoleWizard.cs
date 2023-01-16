
using GalForUnity.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class RoleWizard : EditorWindow
{
    [MenuItem("Window/UI Toolkit/RoleWizard")]
    public static void ShowExample()
    {
        RoleWizard wnd = GetWindow<RoleWizard>();
        wnd.titleContent = new GUIContent("RoleWizard");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GalForUnity/Core/Scripts/Editor/RoleWizard.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        
        root.Add(labelFromUXML);
        EnumField enumField = root.Q<EnumField>("Gender");
        enumField.Init(Gender.Girl);
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GalForUnity/Core/Scripts/Editor/RoleWizard.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);
    }
}