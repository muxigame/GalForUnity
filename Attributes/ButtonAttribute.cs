using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GalForUnity.Attributes {
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