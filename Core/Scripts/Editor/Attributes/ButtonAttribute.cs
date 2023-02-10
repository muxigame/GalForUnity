using System;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace GalForUnity.Core.Editor.Attributes {
    [AttributeUsage(AttributeTargets.All)]
    public class ButtonAttribute : PropertyAttribute{
        public string method;
        public ButtonAttribute(){
            
        }
    }
#if UNITY_EDITOR
    
    [CustomPropertyDrawer(typeof(ButtonAttribute))]

    public class ObjectBuilderPropertyDrawer : PropertyDrawer {
        public override bool CanCacheInspectorGUI(SerializedProperty property){ return base.CanCacheInspectorGUI(property); }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){ 
            base.OnGUI(position, property, label);
            // DrawDefaultInspector();
            Debug.Log(1);
            var methodInfos = property.serializedObject.GetType().GetMethods();
      
        }
    }
#endif
}