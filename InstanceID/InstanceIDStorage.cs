//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  InstanceIDStorage.cs
//
//        Created by 半世癫(Roc) at 2021-11-20 22:11:30
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalForUnity.InstanceID{
    [CreateAssetMenu(fileName = "InstanceIDStorage.asset", menuName = "GalForUnity/InstanceIDStorage", order = 3)]
    [Serializable]
    public class InstanceIDStorage : ScriptableObject{
        [SerializeField] private List<IDStorageInfo> infos = new List<IDStorageInfo>();

        private Dictionary<long, string> _dictionary = new Dictionary<long, string>();

        public Dictionary<long, string> IDDictionary{
            get{
                if (_dictionary == null || _dictionary.Count == 0 || _dictionary.Count != infos.Count){
                    _dictionary = new Dictionary<long, string>();
                    foreach (var idStorageInfo in infos){
                        _dictionary.Add(idStorageInfo.instanceID, idStorageInfo.path);
                    }
                }

                return _dictionary;
            }
        }

        [Serializable]
        public class IDStorageInfo{
            [SerializeField] public long instanceID;
            [SerializeField] public string path;
        }

        public void Add(long instanceID, string path){
            if(!path.Contains("Resource"))
                Debug.LogError(GfuLanguage.ParseLog("This Resource object is not saved in the Resource directory and may not be loaded in the game:") +path);
            if(!HasInstanceID(instanceID))
                infos.Add(new IDStorageInfo() {
                    instanceID = instanceID, path = path
                });
            else Debug.LogError("InstanceID重复");
        }

        public void Remove(long instanceID){ infos.RemoveAll((x) => x.instanceID == instanceID); }
        public void Remove(string path){ infos.RemoveAll((x) => x.path           == path); }
        public void Move(string sourcePath,string destinationPath){ infos.ForEach((x) => {
            if (x.path == sourcePath) x.path = destinationPath;
        }); }
        public bool HasInstanceID(long instanceID){ return IDDictionary.ContainsKey(instanceID); }

        /// <summary>
        /// 通过索引快速访问instanceID对应的路径，对该值的修改只是暂时的，游戏重启便会重置
        /// </summary>
        /// <param name="instanceID"></param>
        public string this[long instanceID]{
            get => IDDictionary[instanceID];
            set => IDDictionary[instanceID] = value;
        }
    }

    // // ReSharper disable all UnusedMember.Global
    // public class InstanceIDStorageLoadTool{
    //     public static T Load<T>(long instanceID) where T : Object{
    //         var currentInstanceIDStorage = GameSystem.GetInstance().currentInstanceIDStorage;
    //         if (!currentInstanceIDStorage) return null;
    //         if (!currentInstanceIDStorage.HasInstanceID(instanceID)) return null;
    //         var path = currentInstanceIDStorage[instanceID];
    //         if (!path.Contains("Resources")) return null;
    //         var resourcesPath = path.Substring(path.IndexOf("Resources", StringComparison.Ordinal) + 10);
    //         resourcesPath = resourcesPath.Substring(0, resourcesPath.IndexOf(".", StringComparison.Ordinal));
    //         return Resources.Load<T>(resourcesPath);
    //     }
    //
    //     // public static T Load<T>(GfuInstance gfuInstance) where T : Object{
    //     //     return Load<T>(gfuInstance.instanceID);
    //     // }
    //     public static Object Load(long instanceID){
    //         var currentInstanceIDStorage = GameSystem.GetInstance().currentInstanceIDStorage;
    //         if (!currentInstanceIDStorage) return null;
    //         if (!currentInstanceIDStorage.HasInstanceID(instanceID)) return null;
    //         var path = currentInstanceIDStorage[instanceID];
    //         if (!path.Contains("Resources")) return null;
    //         var resourcesPath = path.Substring(path.IndexOf("Resources", StringComparison.Ordinal) + 10);
    //         resourcesPath = resourcesPath.Substring(0, resourcesPath.IndexOf(".", StringComparison.Ordinal));
    //         return Resources.Load(resourcesPath);
    //     }
    //     // public static Object Load(GfuInstance gfuInstance){
    //     //     return Load(gfuInstance.instanceID);
    //     // }
    // }
}
