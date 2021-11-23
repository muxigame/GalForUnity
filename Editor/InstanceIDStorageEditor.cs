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


using GalForUnity.InstanceID;
using UnityEditor;
using UnityEngine;

namespace GalForUnity.Editor{
    [CustomEditor(typeof(InstanceIDStorage))]
    public class InstanceIDStorageEditor : ButtonEditor
    {
        
        public override void OnInspectorGUI(){
            DrawButton<InstanceIDStorage>("初始化ID寄存器",(x)=>{	
                // if (EditorUtility.DisplayDialog("提示","仅初始化Resources目录","是滴","不，谢谢")){
                //     Resources.FindObjectsOfTypeAll<Tr>()
                // }
                var findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll<GfuInstance>();
                foreach (var gfuInstance in findObjectsOfTypeAll){
                    if (EditorUtility.IsPersistent(gfuInstance)){
                        if(!x.HasInstanceID(gfuInstance.instanceID)) x.Add(gfuInstance.instanceID,AssetDatabase.GetAssetPath(gfuInstance));
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                // x.InitialGameSystem(false);
            });
        }
    }
}
