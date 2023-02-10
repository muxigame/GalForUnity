using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GalForUnity.Core.Editor.IMGUI{
    public class BaseEditor : UnityEditor.Editor{ // ReSharper disable all MemberCanBePrivate.Global
        private Object monoScript;

        protected virtual void OnEnable(){ monoScript = MonoScript.FromMonoBehaviour(this.target as MonoBehaviour); }

        protected bool CheckRemove(ReorderableList reorderableList){
            if (reorderableList.count > 0){
                return reorderableList.displayRemove = true;
            }

            return reorderableList.displayRemove = false;
        }

        public T GetFirstAttribute<T>(SerializedProperty serializedProperty) where T : class{
            if (serializedProperty.HasAttribute<T>()){
                return (T) serializedProperty.GetAttributes<T>()[0];
            }

            return null;
        }

        protected void DrawMonoScript(){
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", this.monoScript, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }
    }

    public static class Extern{
        public static T GetFirstAttribute<T>(this SerializedProperty serializedProperty) where T : class{
            if (serializedProperty.HasAttribute<T>()){
                return (T) serializedProperty.GetAttributes<T>()[0];
            }

            return null;
        }
    }
}