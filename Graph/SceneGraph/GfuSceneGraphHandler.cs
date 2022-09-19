//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  EditorWindowHandler.cs
//
//        Created by 半世癫(Roc) at 2022-04-16 01:37:44
//
//======================================================================

using System.Collections.Generic;
using UnityEditor;

namespace GalForUnity.Graph.SceneGraph{
    public static class GfuSceneGraphHandler{
        public static Dictionary<string, SceneGraph> SceneGraph = new Dictionary<string, SceneGraph>();

        public static void Register(SceneGraph sceneGraph){
            if (!sceneGraph||!sceneGraph.graph) return;
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sceneGraph.graph));
            if (!GfuSceneGraphHandler.SceneGraph.ContainsKey(guid))
                GfuSceneGraphHandler.SceneGraph.Add(guid, sceneGraph);
            else
                GfuSceneGraphHandler.SceneGraph[guid] = sceneGraph;
        }

        public static void CancelRegister(SceneGraph gfuGraphAsset){
            if (!gfuGraphAsset||!gfuGraphAsset.graph) return;
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(gfuGraphAsset));
            if (SceneGraph.ContainsKey(guid)) SceneGraph.Remove(guid);
        }

        public static SceneGraph GetGfuGraphAsset(string guid){
            if (SceneGraph.ContainsKey(guid)) return SceneGraph[guid];
            return null;
        }

        public static bool HasRegister(string guid){
            if (string.IsNullOrWhiteSpace(guid)) return false;
            return SceneGraph.ContainsKey(guid);
        }
    }
}