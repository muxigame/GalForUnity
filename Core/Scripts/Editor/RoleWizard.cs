
using System;
using System.Collections.Generic;
using GalForUnity.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;


public class RoleWizard : EditorWindow
{
    public List<NamedSprite> NamedSprites=new List<NamedSprite>();
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
        var serializedObject = new SerializedObject(this);
        var serializedProperty = serializedObject.FindProperty("NamedSprites");
        Debug.Log(serializedProperty);
        // var field = new ListView(serializedProperty);
        // labelFromUXML.Add(field);
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
 
    [Serializable]
    public class MyClass:Object{
        
    }
    [Serializable]
    public class NamedSprite{
        public string name;
        public Sprite Sprite;

        public static implicit operator NamedSprite(Sprite sprite){
            return new NamedSprite(){
                Sprite = sprite, name = sprite.name
            };
        }
    }
}