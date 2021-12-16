//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SavableBehaviour.cs
//
//        Created by 半世癫(Roc) at 2021-12-03 21:10:56
//
//======================================================================

using System;
using GalForUnity.External;
using GalForUnity.System.Address.Addresser;
using GalForUnity.System.Archive.Data;
using UnityEngine;

namespace GalForUnity.System.Archive.Behavior{
    /// <summary>
    /// SavableBehaviour继承自MonoBehaviour，提供可运行时保存数据的能力
    /// </summary>
    [Serializable]
    public abstract class SavableBehaviour : MonoBehaviour{
        [SerializeField]
        [HideInInspector]
        // protected Savable savableData;
        public virtual void GetObjectData(ScriptData scriptData){
            GetObjectData();
            if (scriptData != null){
                var fieldInfos = GetType().GetFields<Savable>();
                foreach (var fieldInfo in fieldInfos){
                    var value = (Savable)fieldInfo.GetValue(this);
                    value.Save();
                }
                scriptData.json = JsonUtility.ToJson(this);
                scriptData.ObjectAddressExpression = InstanceIDAddresser.GetInstance().Parse(this);
                scriptData.activeSelf = enabled;
            }
        }

        public virtual void GetObjectData(){}
        public virtual void Recover(ScriptData scriptData){
            if (scriptData != null){
                if (InstanceIDAddresser.GetInstance().Get(scriptData.ObjectAddressExpression, out var obj)){
                    JsonUtility.FromJsonOverwrite(scriptData.json,obj);
                    if (obj is MonoBehaviour monoBehaviour){
                        monoBehaviour.enabled=scriptData.activeSelf;
                    }
                }
            }
            Recover();
        }

        public virtual void Recover(){}
    }
}

// var type = GetType();
// var fieldInfos = type.GetFields();
// foreach (var fieldInfo in fieldInfos){
//     if (fieldInfo.FieldType.IsSubclassOf(typeof(Component)) || fieldInfo.FieldType == typeof(Component)){
//         var value = (Component)fieldInfo.GetValue(this);
//         info.objectAddressExpression.Add(fieldInfo.Name,ObjectAddresser.ParseMemory(value,value != null ? value.name : null));
//     } else if (fieldInfo.FieldType.IsSubclassOf(typeof(GraphData)) || fieldInfo.FieldType == typeof(GraphData)){
//         info.objectAddressExpression.Add(fieldInfo.Name,ObjectAddresser.ParseInstanceID((GraphData)fieldInfo.GetValue(this)));
//     }else if (fieldInfo.FieldType.GetInterface(nameof(IEnumerable)) !=null){
//         info.objectAddressExpression.Add(fieldInfo.Name,ObjectAddresser.ParseInstanceID((GraphData)fieldInfo.GetValue(this)));
//     } else{
//         info.fields.Add(fieldInfo.Name,fieldInfo.GetValue(this));
//     }
// }
// var type = GetType();
// foreach (var saveableItem in saveable.objectAddressExpression){
//     type.GetField(saveableItem.Key).SetValue(this,ObjectAddresser.Find(saveableItem.Value));
// }
// foreach (var keyValuePair in saveable.fields){
//     type.GetField(keyValuePair.Key).SetValue(this,keyValuePair.Value);
// }