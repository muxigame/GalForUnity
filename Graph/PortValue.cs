using System;
using System.Numerics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalForUnity.Graph{
    [Serializable]
    public class PortValue : ISerializationCallbackReceiver{
        public enum JsonValueType : byte{
            Null = 0,
            UnityObject = 1,
            String = 2,
            Boolean = 3,
            IntNumber = 4,
            UIntNumber = 5,
            FloatNumber = 6,
            Bytes = 7,
            Guid = 8,
            Uri = 9,
            Date = 10
        }

        [SerializeField] private string serializeValue;
        [SerializeField] private JsonValueType jsonValueType;
        [SerializeField] private Object unityObject;

        public PortValue(object value){ Value = value; }
      

        public object Value{ get; set; }

        public void OnBeforeSerialize(){
            if (Value is Object obj)
                unityObject = obj;
            else if(Value is not null)
                serializeValue = Value.ToString();
            jsonValueType = GetValueType(Value);
        }

        public void OnAfterDeserialize(){ Value = GetValue(serializeValue); }

        public JsonValueType GetValueType(object value){
            if (value == null) return JsonValueType.Null;
            switch (value){
                case string:
                    return JsonValueType.String;
                case ulong:
                case uint:
                case ushort:
                    return JsonValueType.UIntNumber;
                case long:
                case int:
                case short:
                case sbyte:
                case byte:
                case BigInteger:
                    return JsonValueType.IntNumber;
                case double:
                case float:
                case decimal:
                    return JsonValueType.FloatNumber;
                case DateTime:
                case DateTimeOffset:
                    return JsonValueType.Date;
                case byte[]:
                    return JsonValueType.Bytes;
                case bool:
                    return JsonValueType.Boolean;
                case Guid:
                    return JsonValueType.Guid;
                case Uri:
                    return JsonValueType.Uri;
                case Object:
                    return JsonValueType.UnityObject;
                default:
                    throw new ArgumentException($"Could not determine JSON object type for type {value.GetType()}");
            }
        }

        public object GetValue(string value){
            switch (jsonValueType){
                case JsonValueType.UnityObject:
                    return unityObject;
                case JsonValueType.Uri:
                    return new Uri(value);
                case JsonValueType.String:
                    return value;
                case JsonValueType.UIntNumber:
                    return Convert.ToUInt64(value);
                case JsonValueType.IntNumber:
                    return Convert.ToInt64(value);
                case JsonValueType.FloatNumber:
                    return Convert.ToDouble(value);
                case JsonValueType.Date:
                    return Convert.ToDateTime(value);
                case JsonValueType.Boolean:
                    return Convert.ToBoolean(value);
                case JsonValueType.Bytes:
                    return Convert.FromBase64String(value);
                case JsonValueType.Guid:
                    return Guid.Parse(value);
                case JsonValueType.Null:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}