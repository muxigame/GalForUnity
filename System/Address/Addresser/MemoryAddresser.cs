//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  MemoryAddresser.cs
//
//        Created by 半世癫(Roc) at 2021-12-04 12:39:33
//
//======================================================================

using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalForUnity.System.Address.Addresser{
    public class MemoryAddresser : Addressable<MemoryAddresser>{
        public override bool Get(string expression, out object value){
            value = null;
            var expressionProcessor = new ExpressionParser(expression);
            if (expressionProcessor.GetProtocol() == ExpressionProtocol.Memory){
                var className = expressionProcessor.GetClassName();
                var fieldName = expressionProcessor.GetFieldName();
                if(string.IsNullOrEmpty(expressionProcessor.GetObjectName())&&string.IsNullOrEmpty(className)) value = null;
                Assembly assembly=null;
                GameObject wantFindGameObject=null;
                Component component=null;
                if (!string.IsNullOrEmpty(expressionProcessor.GetAssemblyName())) assembly = Assembly.Load(expressionProcessor.GetAssemblyName());
                if (!string.IsNullOrEmpty(expressionProcessor.GetObjectName())) wantFindGameObject = GameObject.Find(expressionProcessor.GetObjectName());
                if (!string.IsNullOrEmpty(className)){
                    if (wantFindGameObject!=null)
                        component = wantFindGameObject.GetComponent(assembly == null ? Type.GetType(className) : assembly.GetType(className));
                    else
                        component = (Component) Object.FindObjectOfType(assembly == null ? Type.GetType(className) : assembly.GetType(className));
                }
                if (!string.IsNullOrEmpty(fieldName)){
                    if (component) value = GetObject(component, fieldName);
                    if (wantFindGameObject) value = GetObject(wantFindGameObject, fieldName);
                } else{
                    if (component) value = component;
                    if (wantFindGameObject) value = wantFindGameObject;
                }
                return true;
            }
            return false;
        }
        public override void Set(string expression, object value){
            var expressionProcessor = new ExpressionParser(expression);
            if (expressionProcessor.GetProtocol() == ExpressionProtocol.Memory){
                var className = expressionProcessor.GetClassName();
                var fieldName = expressionProcessor.GetFieldName();
                if(string.IsNullOrEmpty(expressionProcessor.GetObjectName()) &&string.IsNullOrEmpty(className)) return;
                Assembly assembly=null;
                GameObject wantFindGameObject=null;
                Component component=null;
                if (!string.IsNullOrEmpty(expressionProcessor.GetAssemblyName())) assembly = Assembly.Load(expressionProcessor.GetAssemblyName());
                if (!string.IsNullOrEmpty(expressionProcessor.GetObjectName())) wantFindGameObject = GameObject.Find(expressionProcessor.GetObjectName());
                if (!string.IsNullOrEmpty(className)){
                    if (wantFindGameObject !=null)
                        component = wantFindGameObject.GetComponent(assembly == null ? Type.GetType(className) : assembly.GetType(className));
                    else
                        component = (Component) Object.FindObjectOfType(assembly == null ? Type.GetType(className) : assembly.GetType(className));
                }
                if (!string.IsNullOrEmpty(fieldName)){
                    if (component) SetObject(component, fieldName,value);
                    if (wantFindGameObject)  SetObject(wantFindGameObject, fieldName,value);
                } else{
                    if (component) throw new NotImplementedException("尝试对组件赋值");
                    if (wantFindGameObject)  throw new NotImplementedException("尝试对游戏对象赋值");
                }
            }
        }

        public override string Parse(object value){
            return new ExpressionProcessor(value).GetMemoryExpression();
        }
    }
}

// if (AddressableExpression.ClassName.IsMatch(address)){
//     var spiltExpression = SpiltExpression(address);
//     value = FindMemory(spiltExpression.Item1,spiltExpression.Item2);
//     return;
// }
            // string address = otherAddress.Trim();
            // string className = otherClassName.Trim();
            // if (string.IsNullOrEmpty(address)&&string.IsNullOrEmpty(className)) return null;
            // if (address.IndexOf("+", StringComparison.Ordinal) == 0) address = address.Substring(1);
            // if (className.LastIndexOf("+", StringComparison.Ordinal) == className.Length-1) className = className.Substring(0,className.Length -1);
            // Type fieldType = null;
            // object fieldObject = null;
            // Assembly assembly=null;
            //
            // //address:Data.PlotFlowController
            // //className:GalForUnity.System.GameSystem[GameObject]
            // if (AddressableExpression.ObjectName.IsMatch(className)){
            //     var objectName = AddressableExpression.ObjectName.Match(className).Value;
            //     className = className.Replace(objectName, "");
            //     objectName = objectName.Replace("[", "").Replace("]", "");
            //     var assemblyName = AddressableExpression.AssemblyName.Match(className).Value;
            //     if (!string.IsNullOrEmpty(assemblyName)){
            //         className=className.Replace(assemblyName,"");
            //         assemblyName = assemblyName.Replace("(", "").Replace(")", "");
            //         assembly = Assembly.Load(assemblyName);
            //     }
            //     var find = GameObject.Find(objectName);
            //     if (!find){
            //         Debug.LogError(objectName +"对象不存在");
            //         return null;
            //     }
            //     
            //     
            //     if (assembly!=null){
            //         fieldType = assembly.GetType(className);
            //     } else{
            //         fieldType = Type.GetType(className);
            //     }
            //     fieldObject = find.GetComponent(fieldType);
            //     if (string.IsNullOrEmpty(address)) return fieldObject;
            // } else{
            //     var assemblyName = AddressableExpression.AssemblyName.Match(className).Value;
            //     if (!string.IsNullOrEmpty(assemblyName)){
            //         className=className.Replace(assemblyName,"");
            //         assemblyName = assemblyName.Replace("(", "").Replace(")", "");
            //         assembly=Assembly.Load(assemblyName);
            //     }
            //     Type type = null;
            //     if (assembly !=null){
            //         type = assembly.GetType(className);
            //     } else{
            //         type = Type.GetType(className);
            //     }
            //     if(type == null) return null;
            //     var classObject = FindObjectOfType(type);
            //     if(classObject == null) return null;
            //     fieldObject = classObject;
            // }
            // while (address.Contains(".")){
            //     var substring = address.Substring(0, address.IndexOf(".", StringComparison.Ordinal));
            //     address=address.Substring(address.IndexOf(".", StringComparison.Ordinal) +1);
            //     var fieldInfo = fieldType.GetField(substring);
            //     if (fieldInfo != null){
            //         fieldObject = fieldInfo.GetValue(fieldObject);
            //         fieldType = fieldObject.GetType();
            //     }else if (fieldType.GetProperty(substring) is PropertyInfo propertyInfo){
            //         fieldObject=propertyInfo.GetValue(fieldObject);
            //         fieldType = fieldObject.GetType();
            //     } else{
            //         Debug.LogError(substring +"字段不存在");
            //         return null;
            //     }
            // }
            // var field = fieldType.GetField(address);
            // if (field != null){
            //     fieldObject = field.GetValue(fieldObject);
            // }else if (fieldType.GetProperty(address) is PropertyInfo propertyInfo){
            //     fieldObject=propertyInfo.GetValue(fieldObject);
            // } else{
            //     Debug.LogError(address +"字段不存在");
            //     return null;
            // }
            // return fieldObject;