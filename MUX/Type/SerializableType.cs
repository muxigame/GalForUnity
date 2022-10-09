//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SerializableType.cs
//
//        Created by 半世癫(Roc) at 2021-11-15 17:46:25
//
//======================================================================

using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using UnityEngine;

namespace MUX.Type{
    [Serializable]
    public class SerializableType:ISerializable{
        [NonSerialized]
        private System.Type _type;
        [SerializeField]
        private string stringType;
        [SerializeField]
        private string assembly;
        
        public new System.Type GetType => _type == null ? Assembly.Load(assembly).GetType(stringType) : _type;

        public SerializableType(string stringType){
            _type = System.Type.GetType(stringType);
            if (_type == null){
                assembly = this.stringType = "null";
            } else{
                this.stringType = stringType;
                assembly = _type.Assembly.FullName;
            }
        }
        public SerializableType(System.Type type){
            _type = type;
            if (_type == null){
                assembly=stringType = "null";
            } else{
                stringType = type.FullName;
                assembly = _type.Assembly.FullName;
            }
        }

        public static implicit operator System.Type(SerializableType serializableType){
            if (Assembly.GetExecutingAssembly().FullName != serializableType.assembly) 
                return Assembly.Load(serializableType.assembly)?.GetType(serializableType.stringType);
            return serializableType._type??System.Type.GetType(serializableType.stringType);
        }
        public static implicit operator SerializableType(System.Type type){
            return new SerializableType(type);
        }
        
        public static bool operator ==(SerializableType type1,SerializableType type2){
            return !(type2 is null) && !(type1 is null) && type1.stringType == type2.stringType && type1.assembly == type2.assembly;
        }

        public static bool operator !=(SerializableType type1, SerializableType type2){ return !(type1 == type2); }
        protected bool Equals(SerializableType other){
            return stringType == other.stringType && assembly == other.assembly;
        }
        
        public override bool Equals(object obj){
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SerializableType) obj);
        }

        public override int GetHashCode(){
            unchecked{
                var hashCode = (_type != null ? _type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (stringType != null ? stringType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (assembly   != null ? assembly.GetHashCode() : 0);
                return hashCode;
            }
        }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected SerializableType(SerializationInfo info, StreamingContext context){
            stringType =  info.GetString(nameof(stringType));
            assembly = info.GetString(nameof(assembly));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context){
            info.AddValue(nameof(stringType),stringType);
            info.AddValue(nameof(assembly),assembly);
        }

        public override string ToString(){
            return stringType;
        }
    }
    public class SerializableType<T>:SerializableType{
        public SerializableType():base(typeof(T)){ }
    }
}
