using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using GalForUnity.Model;
using GalForUnity.System.Address.Addresser;
using MUX.Type;
using UnityEngine;

namespace GalForUnity.System.Archive.Data{
    [Serializable]
    
    [KnownType(typeof(List<RoleDataItem>))]
    public class ComponentValue:KeyValue,ISerializable{
        
        [SerializeField]
        public string addressExpression;
        public SerializableType serializableType;
        public int priority;
        // [SerializeField]
        // public new Object value;
        
        public ComponentValue(string name,Component component,object value){
            addressExpression=InstanceIDAddresser.GetInstance().Parse(component);
            this.value = value;
            this.name = name;
            serializableType = value?.GetType();
        }
        
        public Component Value(){
            if(InstanceIDAddresser.GetInstance().Get(addressExpression,out object obj))
                return (Component) obj;
            return null;
        }
        
        public Component Set(){
            if (InstanceIDAddresser.GetInstance().Get(addressExpression, out object obj)){
                if(!string.IsNullOrEmpty(name)) obj.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Static)?.SetValue(obj,value);
                return obj as Component;
            }
            return null;
        }
        public FieldInfo Field(){
            if (InstanceIDAddresser.GetInstance().Get(addressExpression, out object obj)){
                if (!string.IsNullOrEmpty(name))
                    return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                else
                    return null;
            }
            return null;
        }

        public override string ToString(){
            return  name + ":" + addressExpression + ":" + value+"-----"+serializableType;
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ComponentValue(SerializationInfo info, StreamingContext context){
            addressExpression = (string) info.GetValue(nameof(addressExpression),typeof(string));
            name = (string) info.GetValue(nameof(name),typeof(string));
            serializableType = (SerializableType) info.GetValue(nameof(serializableType),typeof(SerializableType));
            value = info.GetValue(nameof(value),serializableType.GetType);
            priority = info.GetInt32(nameof(priority));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context){
            info.AddValue(nameof(addressExpression),addressExpression);
            info.AddValue(nameof(name),name);
            info.AddValue(nameof(value),value);
            info.AddValue(nameof(priority),priority);
            info.AddValue(nameof(serializableType),serializableType);
        }
    }
}