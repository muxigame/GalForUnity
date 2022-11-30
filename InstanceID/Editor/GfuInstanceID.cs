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

        [SerializeField]
        protected long instanceID;
        public long InstanceID{
            get{
                // if (instanceID == 0) instanceID = GfuInstance.CreateInstanceID();
                return instanceID;
            }
            set=>instanceID=value;
        }
        
#if UNITY_EDITOR
        /// 编译完成后ScriptObject的OnEnable调用早于场景中的对象调用<see cref="RegisterInstanceID"/>
        /// 因此使用GfuRunOnMono延迟执行
        private void OnEnable(){
            GfuRunOnMono.Update(RegisterInstanceID);
        }
#endif 
        public void RegisterInstanceID(){
// #if UNITY_EDITOR
//             var currentInstanceIDStorage = GameSystem.GetInstance()?.currentInstanceIDStorage;//这会造成这行代码获取不到值
//             if (!currentInstanceIDStorage){
//                 Debug.LogError("ID寄存器不存在");
//                 return;
//             }
//             if(!currentInstanceIDStorage.HasInstanceID(instanceID))
//                 currentInstanceIDStorage.Add(instanceID,AssetDatabase.GetAssetPath(this));
// #endif
        }
    }
}
