// using GalForUnity.System;
// using GalForUnity.System.Archive;
// using UnityEditor;
// using UnityEngine;
//
// namespace GalForUnity.Editor{
//     [CustomEditor(typeof(ArchiveSystem))]
//     public class ArchiveEditor:UnityEditor.Editor{
//         public override void OnInspectorGUI(){
//             base.OnInspectorGUI();
//             if (GUILayout.Button(GfuLanguage.GfuLanguageInstance.DELETEALLARCHIVE.Value)){
//                 var archiveSystem = ((ArchiveSystem) target);
//                 for (var i = 0; i < archiveSystem.ArchiveSet.Count; i++){
//                     archiveSystem.ArchiveSet.DeleteArchive(i);
//                 }
//                 
//                 archiveSystem.SaveAsync();
//             }
//             if (GUILayout.Button("读取配置文件")){
//                 var archiveSystem = ((ArchiveSystem) target);
//                 archiveSystem.ReadArchiveConfig();
//             }
//             // if (GUILayout.Button("Save")){
//             //     ((ArchiveSystem)target).Save();
//             // }
//             // if (GUILayout.Button("Load")){
//             //     ((ArchiveSystem)target).Load();
//             // }
//             // if (GUILayout.Button("Clear")){
//             //     ((ArchiveSystem)target).Clear();
//             // }
//         }
//     }
// }