//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GraphCreator.cs
//
//        Created by 半世癫(Roc) at 2022-04-16 00:31:35
//
//======================================================================

using System.IO;
using GalForUnity.InstanceID;
using UnityEditor;
using UnityEngine;

namespace GalForUnity.Graph.SceneGraph{
    public class GfuGraphCreator{
        public static string RootPath = Application.dataPath + "/" + "GalForUnityGraph";
        public static string UnityAssetPath = "Assets/GalForUnityGraph";
        public static GfuGraphAsset Create(){
            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);
            var gfuGraphAsset = ScriptableObject.CreateInstance<GfuGraphAsset>();
            AssetDatabase.CreateAsset(gfuGraphAsset, $"{UnityAssetPath}/{GfuInstance.CreateInstanceID()}.asset");
            gfuGraphAsset.hideFlags = HideFlags.None;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return gfuGraphAsset;
        }

        public static string GetCurrentAssetDirectory(){
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets)){
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;

                if (Directory.Exists(path)) return path;
                if (File.Exists(path)) return Path.GetDirectoryName(path);
            }

            return "Assets";
        }
    }
}