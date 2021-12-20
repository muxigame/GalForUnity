//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuInstanceID.cs
//
//        Created by 半世癫(Roc) at 2021-12-15 20:15:17
//
//======================================================================

using System;
using GalForUnity.System;
using UnityEditor;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.InstanceID{
    [Serializable]
    public abstract class GfuInstanceID:ScriptableObject,IInstanceID{
        // Start is called before the first frame update
        private void OnEnable(){
            RegisterInstanceID();
        }
        [SerializeField]
        protected long instanceID;
        public long InstanceID{
            get{
                if (instanceID == 0) instanceID = GfuInstance.CreateInstanceID();
                return instanceID;
            }
            set=>instanceID=value;
        }

        public void RegisterInstanceID(){
            var currentInstanceIDStorage = GameSystem.GetInstance().currentInstanceIDStorage;
            if (!currentInstanceIDStorage){
                Debug.LogError("ID寄存器不存在");
                return;
            }
            if(!currentInstanceIDStorage.HasInstanceID(instanceID))
                currentInstanceIDStorage.Add(instanceID,AssetDatabase.GetAssetPath(this));
        }
    }
}
