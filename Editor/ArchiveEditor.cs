using GalForUnity.System.Archive;
using UnityEditor;
using UnityEngine;

namespace GalForUnity.Editor{
    [CustomEditor(typeof(ArchiveSystem))]
    public class ArchiveEditor:UnityEditor.Editor{
        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            if (GUILayout.Button("添加")){
                ((ArchiveSystem)target).Add();
            }
            if (GUILayout.Button("减少")){
                ((ArchiveSystem)target).Sub();
            }
            if (GUILayout.Button("Save")){
                ((ArchiveSystem)target).Save();
            }
            if (GUILayout.Button("Load")){
                ((ArchiveSystem)target).Load();
            }
            if (GUILayout.Button("Clear")){
                ((ArchiveSystem)target).Clear();
            }
        }
    }
}