using System;
using UnityEngine;

namespace GalForUnity.System.Archive.Data{
    // ReSharper disable all MemberCanBePrivate.Global
    [Serializable]
    public class KeyValue{
        public KeyValue(){ }
        public KeyValue(string name,object value){
            this.name = name;
            this.value = value;
        }
        [SerializeField]
        public string name;
        [SerializeField]
        public object value;
    }
}