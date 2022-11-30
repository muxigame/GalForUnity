// //======================================================================
// //
// //       CopyRight 2019-2021 © MUXI Game Studio 
// //       . All Rights Reserved 
// //
// //        FileName :  AutoInstanceID.cs
// //
// //        Created by 半世癫(Roc) at 2021-11-20 23:50:27
// //
// //======================================================================
//
// #if UNITY_EDITOR
// using GalForUnity.System;
// using UnityEditor;
// using UnityEngine;
// #endif
//
//
// #if UNITY_EDITOR
// namespace GalForUnity.InstanceID{
//     /// <summary>
//     /// 
//     /// </summary>
//     public class AutoInstanceID : UnityEditor.AssetModificationProcessor
//     {
//         //导入资源创建资源时候调用
//         public static void OnWillCreateAsset(string path){
//             if (path.EndsWith(".prefab")){
//                 EditorApplication.delayCall += () => {
//                     var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<GameObject>(path);
//                     var gfuInstance = loadAssetAtPath.GetComponent<GfuInstance>();
//                     if(gfuInstance&&!path.Contains("Resource"))
//                         Debug.LogError(GfuLanguage.ParseLog("This Resource object is not saved in the Resource directory and may not be loaded in the game:") +path);
//                     gfuInstance.Init();
//                     gfuInstance.SafeInstanceID();
//                     var currentInstanceIDStorage = GameSystem.GetInstance()?.currentInstanceIDStorage;
//                     if (gfuInstance && currentInstanceIDStorage != null){
//                         currentInstanceIDStorage.Add(gfuInstance.instanceID, path);
//                         AssetDatabase.SaveAssets();
//                         AssetDatabase.Refresh();
//                     }
//
//                 };
//             }else if (path.Equals(".asset")){
//                 var gfuInstance = AssetDatabase.LoadAssetAtPath<GfuInstanceID>(path);
//                 if (!gfuInstance) return;
//                 if(gfuInstance &&!path.Contains("Resource"))
//                     Debug.LogError(GfuLanguage.ParseLog("This Resource object is not saved in the Resource directory and may not be loaded in the game:") +path);
//             }
//         }
//
//         private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options){
//             // var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
//             var currentInstanceIDStorage = GameSystem.GetInstance()?.currentInstanceIDStorage;
//             if (currentInstanceIDStorage != null){
//                 currentInstanceIDStorage.Remove(assetPath);
//                 AssetDatabase.SaveAssets();
//                 AssetDatabase.Refresh();
//             }
//             return 0;
//         }
//
//         private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath){
//             if (destinationPath.Equals(".prefab")){
//                 var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
//                 if (!loadAssetAtPath) loadAssetAtPath=AssetDatabase.LoadAssetAtPath<GameObject>(destinationPath);
//                 if (!loadAssetAtPath) return 0;
//                 var gfuInstance = loadAssetAtPath.GetComponent<GfuInstance>();
//                 if(gfuInstance &&!destinationPath.Contains("Resource"))
//                     Debug.LogError(GfuLanguage.ParseLog("This Resource object is not saved in the Resource directory and may not be loaded in the game:") +destinationPath);
//                 var currentInstanceIDStorage = GameSystem.GetInstance()?.currentInstanceIDStorage;
//                 if (gfuInstance && currentInstanceIDStorage != null){
//                     currentInstanceIDStorage.Move(sourcePath,destinationPath);
//                     AssetDatabase.SaveAssets();
//                     AssetDatabase.Refresh();
//                 }
//             }else if (destinationPath.Equals(".asset")){
//                 var gfuInstance = AssetDatabase.LoadAssetAtPath<GfuInstanceID>(sourcePath);
//                 if (!gfuInstance) gfuInstance=AssetDatabase.LoadAssetAtPath<GfuInstanceID>(destinationPath);
//                 if (!gfuInstance) return 0;
//                 if(gfuInstance &&!destinationPath.Contains("Resource"))
//                     Debug.LogError(GfuLanguage.ParseLog("This Resource object is not saved in the Resource directory and may not be loaded in the game:") +destinationPath);
//                 var currentInstanceIDStorage = GameSystem.GetInstance()?.currentInstanceIDStorage;
//                 if (gfuInstance && currentInstanceIDStorage != null){
//                     currentInstanceIDStorage.Move(sourcePath,destinationPath);
//                     AssetDatabase.SaveAssets();
//                     AssetDatabase.Refresh();
//                 }
//             }
//            
//             return 0;
//         }
//     }
// }
// #endif