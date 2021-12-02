//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  CompareOperation.cs
//
//        Created by 半世癫(Roc) at 2021-02-08 21:12:37
//
//======================================================================

using System;
using System.Reflection;
using GalForUnity.Graph.GFUNode.Operation;
using UnityEngine;

namespace GalForUnity.Graph.Operation.Logic{
    public class CompareOperation:GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Operation();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Operation();
            base.Update(gfuOperationData);
        }
        public override void OperationOver(){
            Operation();
            base.OperationOver();
        }

        public void Operation(){
            if (ContainerData[0].value is ValueCompareType valueCompareType){
                if(InputData[0].value is float firstFloat&&InputData[1].value is float secondFloat){
                    Equal(valueCompareType,firstFloat,secondFloat);
                    return;
                }
                if(InputData[0].value is Vector2 firstVector2&&InputData[1].value is Vector2 secondVector2){
                    Equal(valueCompareType,firstVector2.x,secondVector2.x);
                    Debug.LogError("Try Equal Vector!");
                    return;
                }
                if(InputData[0].value is Vector3 firstVector3&&InputData[1].value is Vector3 secondVector3){
                    Equal(valueCompareType,firstVector3.x,secondVector3.x);
                    Debug.LogError("Try Equal Vector!");
                    return;
                }
                if(InputData[0].value is Vector4 firstVector4&&InputData[1].value is Vector4 secondVector4){
                    Equal(valueCompareType,firstVector4.x,secondVector4.x);
                    Debug.LogError("Try Equal Vector!");
                    return;
                }
                
                MethodInfo methodInfo = null;
                if (valueCompareType == ValueCompareType.Equal){
                    methodInfo = InputData[0].Type.GetMethod("op_Equality", BindingFlags.Public | BindingFlags.Static);
                }else if (valueCompareType == ValueCompareType.NotEqual){
                    methodInfo = InputData[0].Type.GetMethod("op_Inequality", BindingFlags.Public | BindingFlags.Static);
                }else if (valueCompareType == ValueCompareType.Less){
                    methodInfo = InputData[0].Type.GetMethod("op_LessThan", BindingFlags.Public | BindingFlags.Static);
                }else if (valueCompareType == ValueCompareType.LessOrEqual){
                    methodInfo = InputData[0].Type.GetMethod("op_LessThanOrEqual", BindingFlags.Public | BindingFlags.Static);
                }else if (valueCompareType == ValueCompareType.Greater){
                    methodInfo = InputData[0].Type.GetMethod("op_GreaterThan", BindingFlags.Public | BindingFlags.Static);
                }else if (valueCompareType == ValueCompareType.GreaterOrEqual){
                    methodInfo = InputData[0].Type.GetMethod("op_GreaterThanOrEqual", BindingFlags.Public | BindingFlags.Static);
                }
                
                if (methodInfo != null){
                    var returnBoolean = methodInfo.Invoke(InputData[0].value,new []{ InputData[0].value, InputData[1].value});
                    foreach (var data in OutPutData){
                        data.value = returnBoolean;
                    }
                }
            }else if(ContainerData[0].value is ObjectCompareType objectCompareType){
                if (InputData[0].value.Equals(InputData[1].value)){
                    foreach (var data in OutPutData){
                        if (data.Type != typeof(bool)) Debug.LogError("输出端口的类型不是bool而是:" + data.Type);
                        data.value = objectCompareType == ObjectCompareType.Equal;
                    }
                }else{
                    foreach (var data in OutPutData){
                        if (data.Type != typeof(bool)) Debug.LogError("输出端口的类型不是bool而是:" + data.Type);
                        data.value = objectCompareType == ObjectCompareType.NotEqual;
                    }
                }
            }
            else{
                Debug.LogError("未知异常，位于比较节点的枚举不是一个预期值");
            }
        }

        private void Equal(ValueCompareType valueCompareType,float firstFloat,float secondFloat){
            switch (valueCompareType){
                case ValueCompareType.Equal:
                    foreach (var data in OutPutData){
                        if (data.Type != typeof(bool)) Debug.LogError("输出端口的类型不是bool而是:" + data.Type);
                        data.value = Math.Abs(firstFloat - secondFloat) < 0.000001f;
                    };
                    break;
                case ValueCompareType.NotEqual:
                    foreach (var data in OutPutData){
                        if (data.Type != typeof(bool)) Debug.LogError("输出端口的类型不是bool而是:" + data.Type);
                        data.value = Math.Abs(firstFloat - secondFloat) > 0.000001f;
                    };
                    break;
                case ValueCompareType.Less:
                    foreach (var data in OutPutData){
                        if (data.Type != typeof(bool)) Debug.LogError("输出端口的类型不是bool而是:" + data.Type);
                        data.value = firstFloat < secondFloat;
                    };
                    break;
                case ValueCompareType.LessOrEqual:
                    foreach (var data in OutPutData){
                        if (data.Type != typeof(bool)) Debug.LogError("输出端口的类型不是bool而是:" + data.Type);
                        data.value = firstFloat <= secondFloat;
                    };
                    break;
                case ValueCompareType.Greater:
                    foreach (var data in OutPutData){
                        if (data.Type != typeof(bool)) Debug.LogError("输出端口的类型不是bool而是:" + data.Type);
                        data.value = firstFloat > secondFloat;
                    };
                    break;
                case ValueCompareType.GreaterOrEqual:
                    foreach (var data in OutPutData){
                        if (data.Type != typeof(bool)) Debug.LogError("输出端口的类型不是bool而是:" + data.Type);
                        data.value = firstFloat >= secondFloat;
                    };
                    break;
            }
        }
    }
}