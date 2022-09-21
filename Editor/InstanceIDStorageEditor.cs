//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  InstanceIDStorageEditor.cs
//
//        Created by 半世癫(Roc) at 2021-11-21 23:10:54
//
//======================================================================


using GalForUnity.External;
using GalForUnity.InstanceID;
using UnityEditor;
using UnityEngine;

namespace GalForUnity.Editor{
    [CustomEditor(typeof(InstanceIDStorage))]
    public class InstanceIDStorageEditor : ButtonEditor
    {
        
        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            DrawButton<InstanceIDStorage>("导入可安全读取的ID对象",(x)=>{	
                // if (EditorUtility.DisplayDialog("提示","仅初始化Resources目录","是滴","不，谢谢")){
                //     Resources.FindObjectsOfTypeAll<Tr>()
                // }
                var findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll<GfuInstance>();
                
                foreach (var gfuInstance in findObjectsOfTypeAll){
                    if (EditorUtility.IsPersistent(gfuInstance)){
                        if(!x.HasInstanceID(gfuInstance.instanceID)) x.Add(gfuInstance.instanceID,AssetDatabase.GetAssetPath(gfuInstance));
                    }
                }
                // PlotFlowGraphData[] graphDatas = Resources.FindObjectsOfTypeAll<PlotFlowGraphData>();
                // foreach (var graphData in graphDatas){
                //     if (EditorUtility.IsPersistent(graphData)){
                //         if(!x.HasInstanceID(graphData.InstanceID)) x.Add(graphData.InstanceID,AssetDatabase.GetAssetPath(graphData));
                //     }
                // }
                // PlotItemGraphData[] plotItemGraphDatas = Resources.FindObjectsOfTypeAll<PlotItemGraphData>();
                // foreach (var graphData in plotItemGraphDatas){
                //     if (EditorUtility.IsPersistent(graphData)){
                //         if(!x.HasInstanceID(graphData.InstanceID)) x.Add(graphData.InstanceID,AssetDatabase.GetAssetPath(graphData));
                //     }
                // }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                // x.InitialGameSystem(false);
            });
            DrawButton<InstanceIDStorage>("移除不安全的ID对象",(x)=>{	
                // if (EditorUtility.DisplayDialog("提示","仅初始化Resources目录","是滴","不，谢谢")){
                //     Resources.FindObjectsOfTypeAll<Tr>()
                // }
                foreach (var keyValuePair in x.IDDictionary.ToCap()){
                    if (!keyValuePair.Value.Contains("Resources/")){ x.Remove(keyValuePair.Key);}
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                // x.InitialGameSystem(false);
            });
        }
    }
}
