//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  InstanceIDAddresser.cs
//
//        Created by 半世癫(Roc) at 2021-12-05 21:51:01
//
//======================================================================

using System;
using System.Reflection;
using GalForUnity.InstanceID;
using GalForUnity.System.Address.Exception;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;
using Object = UnityEngine.Object;

namespace GalForUnity.System.Address.Addresser{
    public class InstanceIDAddresser : Addressable<InstanceIDAddresser>{
        
        public override bool Get(string expression, out object value){
            value = null;
            var expressionProcessor = new ExpressionParser(expression);
            var expressionProtocol = expressionProcessor.GetProtocol();
            if (expressionProtocol == ExpressionProtocol.InstanceID){
                var instanceID = expressionProcessor.GetInstanceID();
                var obj = GfuInstance.FindAllWithGfuInstanceID(instanceID);
                if(obj==null) throw new ObjectNotFoundException("没有找到对象");
                var className = expressionProcessor.GetClassName();
                var assemblyName = expressionProcessor.GetAssemblyName();
                var fieldName = expressionProcessor.GetFieldName();
                Assembly assembly=null;
                if (!string.IsNullOrEmpty(assemblyName)) assembly=Assembly.Load(assemblyName);
                if (!string.IsNullOrEmpty(className)){
                    if (obj is Component component) obj = component.GetComponent(assembly == null ? Type.GetType(className) : assembly.GetType(className));
                    else throw new NotImplementedException("这不是一个组件对象,但你尝试通过它获取组件");
                }
                if (!string.IsNullOrEmpty(fieldName)) obj = GetObject(obj, fieldName) as Object;
                value = obj;
                return value!=null;
            }
            return false;
        }

        public override void Set(string expression, object value){
            var expressionProcessor = new ExpressionParser(expression);
            var expressionProtocol = expressionProcessor.GetProtocol();
            if (expressionProtocol == ExpressionProtocol.InstanceID){
                var instanceID = expressionProcessor.GetInstanceID();
                var obj = GfuInstance.FindAllWithGfuInstanceID(instanceID);
                if(obj == null) throw new ObjectNotFoundException("没有找到对象");
                var className = expressionProcessor.GetClassName();
                var assemblyName = expressionProcessor.GetAssemblyName();
                var fieldName = expressionProcessor.GetFieldName();
                Assembly assembly=null;
                if (!string.IsNullOrEmpty(assemblyName)) assembly=Assembly.Load(assemblyName);
                if (!string.IsNullOrEmpty(className)){
                    if (obj is Component component) obj = component.GetComponent(assembly == null ? Type.GetType(className) : assembly.GetType(className));
                    else throw new NotImplementedException("这不是一个组件对象,但你尝试通过它获取组件");
                }
                if (!string.IsNullOrEmpty(fieldName)){
                    SetObject(obj, fieldName,value);
                    return;
                }
            }
            throw new NotImplementedException("尝试对组件赋值");
        }

        public override string Parse(object value){
            return new ExpressionProcessor(value).GetInstanceIDExpression();
        }
    }
}
