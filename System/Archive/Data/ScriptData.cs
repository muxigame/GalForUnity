//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ScriptData.cs
//
//        Created by 半世癫(Roc) at 2021-12-03 10:39:50
//
//======================================================================

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using GalForUnity.InstanceID;
using GalForUnity.System.Address;
using GalForUnity.System.Address.Addresser;
using GalForUnity.System.Address.Exception;
using GalForUnity.System.Archive.Behavior;
using UnityEngine;

namespace GalForUnity.System.Archive.Data{
    /// <summary>
    /// ScriptData保存脚本的json数据和脚本的原始字段对象的表达式，该表达式能够准确定位对象
    /// 也就是说脚本的最终全部保存在该类当中
    /// </summary>
    [Serializable]
    public class ScriptData : ISaveable{
        // public Dictionary<string,string> objectAddressExpression;
        // public Dictionary<string,object> fields;
        public bool activeSelf;
        /// <summary>
        /// 在层级面板中相对于父容器的索引
        /// </summary>
        private int _index;

        public int priority = Int32.MinValue;
        
        public string parentObjectAddressExpression;
        public string ObjectAddressExpression{
            set{
                objectAddressExpression = value;
                if(InstanceIDAddresser.GetInstance().Get(objectAddressExpression,out object obj)){
                    if (obj is MonoBehaviour monoBehaviour){
                        activeSelf = monoBehaviour.enabled;
                    } else if (obj is GameObject gameObject){
                        activeSelf = gameObject.activeSelf;
                        _index = gameObject.transform.GetSiblingIndex();
                        if(gameObject.transform.parent) parentObjectAddressExpression = InstanceIDAddresser.GetInstance().Parse(gameObject.transform.parent.gameObject);
                    } else{
                        Debug.LogError("父层级不是一个Gfu实例");
                    }
                } else{
                    throw new ObjectNotFoundException("找不到指定地址的对象");
                }
            }
            get => objectAddressExpression;
        }
        [SerializeField]
        private string objectAddressExpression;
        public string json;

        public ScriptData(MonoBehaviour monoBehaviour){
            if (monoBehaviour is SavableBehaviour savableBehaviour){
                savableBehaviour.GetObjectData(this);
            } else{
                if (monoBehaviour.TryGetComponent(out GfuInstance gfuInstance)){
                    json = JsonUtility.ToJson(monoBehaviour);
                    ObjectAddressExpression = InstanceIDAddresser.GetInstance().Parse(monoBehaviour);
                    activeSelf = monoBehaviour.enabled;
                }
                var fieldInfos = monoBehaviour.GetType().GetFields<Savable>();
                foreach (var fieldInfo in fieldInfos){
                    var value = (Savable)fieldInfo.GetValue(this);
                    value.Save();
                }
            }
        }
        public ScriptData(GameObject gameObject){
            ObjectAddressExpression = InstanceIDAddresser.GetInstance().Parse(gameObject);
            activeSelf = gameObject.activeSelf;
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ScriptData(SerializationInfo info, StreamingContext context){
            objectAddressExpression = info.GetString(nameof(objectAddressExpression));
            parentObjectAddressExpression = info.GetString(nameof(parentObjectAddressExpression));
            activeSelf = info.GetBoolean(nameof(activeSelf));
            json = info.GetString(nameof(json));
            _index = info.GetInt32(nameof(_index));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context){
            info.AddValue(nameof(json),json);
            info.AddValue(nameof(objectAddressExpression),objectAddressExpression);
            info.AddValue(nameof(parentObjectAddressExpression),parentObjectAddressExpression);
            info.AddValue(nameof(activeSelf),activeSelf);
            info.AddValue(nameof(_index),_index);
        }

        public virtual void Recover(){
            if(string.IsNullOrEmpty(objectAddressExpression)) return;
            if (InstanceIDAddresser.GetInstance().Get(ObjectAddressExpression, out var obj)){
                if (obj is SavableBehaviour savableBehaviour){
                    savableBehaviour.Recover(this);
                } else{
                    if (obj is MonoBehaviour monoBehaviour){
                        JsonUtility.FromJsonOverwrite(json,obj);
                        monoBehaviour.enabled=activeSelf;
                    }else if (obj is GameObject gameObject){
                        if(string.IsNullOrEmpty(parentObjectAddressExpression)) return;
                        if (InstanceIDAddresser.GetInstance().Get(parentObjectAddressExpression, out object parentObj)){
                            gameObject.transform.SetParent(((GameObject)parentObj).transform);
                            gameObject.SetActive(activeSelf);
                            gameObject.transform.SetSiblingIndex(_index);
                        }
                    }
                }
                var fields = obj.GetType().GetFields<Savable>();
                foreach (var fieldInfo in fields){
                    Savable savable=(Savable)fieldInfo.GetValue(obj);
                }
            } else{
                //如果没有查找到这个组件或者对象,代表这个对象存档前有这个组件，但是现在没有这个组件，重新构建表达式创建这个组件
                Debug.Log(objectAddressExpression);
                var expressionParser = new ExpressionParser(ObjectAddressExpression);
                var instanceID = expressionParser.GetInstanceID();
                var expressionCreator = new ExpressionCreator();
                expressionCreator.AddInstanceID(instanceID);
                expressionCreator.AddProtocol(expressionParser.GetProtocol());
                expressionCreator.AddAssemblyName(expressionParser.GetAssemblyName());
                if (InstanceIDAddresser.GetInstance().Get(expressionCreator.Create(), out var gfuInstance)){
                    MonoBehaviour monoBehaviour=null;
                    var instance = gfuInstance as GfuInstance;
                    if(!string.IsNullOrEmpty(expressionParser.GetClassName())) 
                        if(!string.IsNullOrEmpty(expressionParser.GetAssemblyName()))
                            monoBehaviour=instance.gameObject.AddComponent(Assembly.Load(expressionParser.GetAssemblyName()).GetType(expressionParser.GetClassName())) as MonoBehaviour;
                        else
                            monoBehaviour=instance.gameObject.AddComponent(Type.GetType(expressionParser.GetClassName())) as MonoBehaviour;
                    if (monoBehaviour){
                        JsonUtility.FromJsonOverwrite(json,monoBehaviour);
                        monoBehaviour.enabled = activeSelf;
                    }
                } else{
                    new GameObject().AddComponent<GfuInstance>().instanceID = instanceID;
                }
                
            }
            
           
        }
    }
    
}