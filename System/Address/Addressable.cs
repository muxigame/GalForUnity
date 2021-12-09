//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  Addressable.cs
//
//        Created by 半世癫(Roc) at 2021-12-05 23:03:08
//
//======================================================================

using System;
using System.Reflection;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.System.Address{
    public abstract class Addressable<T> : GfuInstanceManager<T>,IAddressable where T : class{

        public abstract bool Get(string expression, out object value);
        public abstract void Set(string expression, object value);
        
        public abstract string Parse(object value);

        protected object GetObject(object obj,string fieldName){
            var findObjectItems = FindObject(obj, fieldName);
            var fieldType = findObjectItems.Item1;
            var fieldObject = findObjectItems.Item2;
            var address = findObjectItems.Item3;
            var field = fieldType.GetField(address);
            if (field != null){
                fieldObject = field.GetValue(fieldObject);
            } else if (fieldType.GetProperty(address) is PropertyInfo propertyInfo){
                fieldObject = propertyInfo.GetValue(fieldObject);
            } else{
                Debug.LogError(address + "字段不存在");
                return null;
            }
            return fieldObject;
        }
        
        protected void SetObject(object obj,string fieldName,object value){
            var findObjectItems = FindObject(obj, fieldName);
            var fieldType = findObjectItems.Item1;
            var fieldObject = findObjectItems.Item2;
            var address = findObjectItems.Item3;
            var field = fieldType.GetField(address);
            if (field != null){
                field.SetValue(fieldObject,value);
            } else if (fieldType.GetProperty(address) is PropertyInfo propertyInfo){
                propertyInfo.SetValue(fieldObject,value);
            } else{
                Debug.LogError(address + "字段不存在");
                return;
            }
            return;
        }

        private (Type,object,string) FindObject(object obj,string fieldName){
            var address = fieldName;
            var fieldType = obj.GetType();
            var fieldObject = obj;
            while (address.Contains(".")){
                var substring = address.Substring(0, address.IndexOf(".", StringComparison.Ordinal));
                address=address.Substring(address.IndexOf(".", StringComparison.Ordinal) +1);
                var fieldInfo = fieldType.GetField(substring);
                if (fieldInfo != null){
                    fieldObject = fieldInfo.GetValue(fieldObject);
                    fieldType = fieldObject.GetType();
                }else if (fieldType.GetProperty(substring) is PropertyInfo propertyInfo){
                    fieldObject=propertyInfo.GetValue(fieldObject);
                    fieldType = fieldObject.GetType();
                } else{
                    Debug.LogError(substring +"字段不存在");
                    return (null,null,null);
                }
            }
            return (fieldType,fieldObject,address);
        }
    }
}
