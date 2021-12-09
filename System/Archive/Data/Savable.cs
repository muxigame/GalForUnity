//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  Saveable.cs
//
//        Created by 半世癫(Roc) at 2021-12-03 18:19:11
//
//======================================================================

using System;
using System.Collections.Generic;
using UnityEngine;


namespace GalForUnity.System.Archive.Data{
    /// <summary>
    /// Savable是一个用来保存不可序列化对象的数据，Unity的内置对象大都不可序列化，如Transform,SprineRender
    /// 其余对象不需要手动添加，一般都可以序列化
    /// </summary>
    [Serializable]
    public class Savable {
        
        [SerializeField]
        protected List<ComponentValue> componentValues=new List<ComponentValue>();
        
        protected Savable(){ }
        
        public void AddValue(string name,Component component, object value){
            componentValues.Add(new ComponentValue(name,component,value));
        }
        
        public virtual void Recover(){
            var type = GetType();
            foreach (var keyValue in componentValues){
                if (keyValue is ComponentValue componentValue){
                    var component = componentValue.Value();
                    var fieldInfo = component.GetType().GetField(keyValue.name);
                    if (fieldInfo==null){
                        component.GetType().GetProperty(keyValue.name)?.SetValue(component,keyValue.value);
                    } else{
                        fieldInfo.SetValue(component,keyValue.value);
                    }
                } else{
                    type.GetField(keyValue.name).SetValue(this,keyValue.value);
                }
            }
        }
        
    }
}
// [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
// protected Saveable(SerializationInfo info, StreamingContext context){
//     Type type=GetType();
//     foreach (var serializationEntry in info){
//         type.GetField(serializationEntry.Name).SetValue(this,serializationEntry.Value);
//     }
// }
// public virtual void GetObjectData(SerializationInfo info, StreamingContext context){
//     var type = GetType();
//     var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static );
//     foreach (var fieldInfo in fieldInfos){
//         info.AddValue(fieldInfo.Name,fieldInfo.GetValue(this));
//     }
// }
