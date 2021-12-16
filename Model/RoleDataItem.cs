using System;
using UnityEngine;

namespace GalForUnity.Model{
    /// <summary>
    /// 某一项角色数据
    /// </summary>
    [Serializable]
    public class RoleDataItem : ICloneable{
        public RoleDataItem(string name, int otherValue, int index){
            this.name = name;
            value = otherValue;
            this.index = index;
        }

        [SerializeField] public string name;
        [SerializeField] public int value;
        public int index;

        public object Clone(){ return new RoleDataItem(name, value, index); }
    }
}