//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SavableConfig.cs
//
//        Created by 半世癫(Roc) at 2021-12-06 20:41:21
//
//======================================================================

using System;
using System.Collections.Generic;
using MUX.Type;
using UnityEngine;

namespace GalForUnity.System.Archive.Data{
    [CreateAssetMenu(fileName = "SavableConfig.asset", menuName = "GalForUnity/SavableConfig", order = 3)]
    public class SavableConfig : ScriptableObject{
        
        public int castDictionaryCount = 100;
        public bool saveHierarchy = true;
        
        [SerializeField]
        public List<SerializableType> types=new List<SerializableType>();
        private Dictionary<Type,bool> _typesDictionary;

        public bool Contains(Type type){
            if (types.Count > castDictionaryCount){
                if (_typesDictionary == null){
                    _typesDictionary=new Dictionary<Type, bool>();
                    foreach (var serializableType in types){
                        _typesDictionary.Add(serializableType,true);
                    }
                } else{
                    if (_typesDictionary.ContainsKey(type))
                        return _typesDictionary[type];
                    return false;
                }
            }
            return types.Contains(type);
        }
    }
}
