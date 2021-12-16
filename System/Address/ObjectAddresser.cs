//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ObjectAddresser.cs
//
//        Created by 半世癫(Roc) at 2021-12-02 17:57:02
//
//======================================================================



using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GalForUnity.Graph;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.InstanceID;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalForUnity.System.Address{
    public class ObjectAddresser : MonoBehaviour{
        /// <summary>
        /// Memory 语法示例："[Memory:[GameObject]GalForUnity.System.GameSystem:Data.PlotFlowController]"
        /// InstanceID 语法示例："[InstanceID:[15619684219856165878]GalForUnity.System.GameSystem:Data.PlotFlowController]"
        /// UnityInstanceID 语法示例："[UnityInstanceID:[15619684219856165878]:Data.PlotFlowController]"(仅限UnityEditor)
        /// Resource 语法示例："[Resource:Dir.FileName]"
        /// Path 语法示例："[Path:Dir.FileName.suffix]"
        /// </summary>
        /// <param name="otherAddress"></param>
        /// <param name="type"></param>
        // public static object Find<T>(string otherAddress){
        //     if (string.IsNullOrEmpty(otherAddress)) return null;
        //     if (cache&&ExpressionCache.ContainsKey(otherAddress)) return ExpressionCache[otherAddress];
        //     if (IsNull.IsMatch(otherAddress)) return null;
        //     if (!IsAddress.IsMatch(otherAddress)) return null;
        //     var expression = getExpression.Match(otherAddress).Value;
        //     string findType = expression.Substring(0, expression.IndexOf(":", StringComparison.Ordinal)+1);
        //     var address = expression.Replace(findType, "");
        //     if (address.Contains("|")){
        //         var strings = address.Split('|');
        //         List<object> objects=new List<object>();
        //         foreach (var s in strings){
        //             objects.Add(Get<T>(findType,s));
        //         }
        //         if(cache) ExpressionCache[otherAddress] = objects;
        //         return objects;
        //     }
        //
        //     var obj = Get<T>(findType, address);
        //     ExpressionCache[otherAddress] = obj;
        //     return obj;
        // }
        public static object Find(string otherAddress,object obj=null){
            // if (string.IsNullOrEmpty(otherAddress)) return null;
            // if (cache &&ExpressionCache.ContainsKey(otherAddress)) return ExpressionCache[otherAddress];
            // if (IsNull.IsMatch(otherAddress)) return null;
            // if (!IsAddress.IsMatch(otherAddress)) return null;
            // var expression = getExpression.Match(otherAddress).Value;
            // string findType = expression.Substring(0, expression.IndexOf(":", StringComparison.Ordinal) +1);
            // var address = expression.Replace(findType, "");
            // if (address.Contains("|")){
            //     var strings = address.Split('|');
            //     List<object> objects=new List<object>();
            //     foreach (var s in strings){
            //         objects.Add(Get(findType,s,obj));
            //     }
            //
            //     return objects;
            // }
            // return Get(findType,address,obj);
            return null;
        }
        // public static object Get<T>(string findType,string address){
        //     switch (findType){
        //         case "Memory:":
        //             return FindMemory(address);
        //         case "InstanceID:": 
        //             return FindInstanceID(address);
        //         case "UnityInstanceID:": 
        //             return FindUnityInstanceID(address);
        //         case "Resource:": 
        //             return FindResource(address);
        //         case "Json:": 
        //             return FindJson(address,typeof(T));
        //         case "Path:": 
        //             return FindPath(address);
        //     }
        //     return null;
        // }
        public static object Get(string findType,string address,object obj=null){
            // switch (findType){
            //     case "Memory:":
            //         return FindMemory(address);
            //     case "InstanceID:": 
            //         return FindInstanceID(address);
            //     case "UnityInstanceID:": 
            //         return FindUnityInstanceID(address);
            //     case "Resource:": 
            //         return FindResource(address);
            //     case "Json:": 
            //         return FindJson(address,obj);
            //     case "Path:": 
            //         return FindPath(address);
            // }
            return null;
        }
        public static void Set(string otherAddress, object value){
            // if (IsNull.IsMatch(otherAddress)) return;
            // if (!IsAddress.IsMatch(otherAddress)) return;
            // var expression = getExpression.Match(otherAddress).Value;
            // string findType = expression.Substring(0, expression.IndexOf(":", StringComparison.Ordinal) +1);
            // switch (findType){
            //     case "Memory:":
            //         SetMemory(expression.Replace(findType,""),value);
            //         break;
            // }
        }
        // public static object FindMemory(string otherAddress){
        //     if (ClassName.IsMatch(otherAddress)){
        //         var value = ClassName.Match(otherAddress).Value;
        //         return FindMemory(value, getAddress.Match(otherAddress).Value);
        //     }
        //     return null;
        // }
        //
        //
        // public static void SetMemory(string otherClassName,string otherAddress,object value){
        //     string address = otherAddress.Trim();
        //     string className = otherClassName.Trim();
        //     if (string.IsNullOrEmpty(address)) throw new NullReferenceException();
        //     if (address.IndexOf("+", StringComparison.Ordinal) == 0) address = address.Substring(1);
        //     if (className.LastIndexOf("+", StringComparison.Ordinal) == className.Length-1) className = className.Substring(0,className.Length -1);
        //     Type fieldType = null;
        //     object fieldObject = null;
        //     //address:Data.PlotFlowController
        //     //className:GalForUnity.System.GameSystem[GameObject]
        //     if (ObjectName.IsMatch(className)){
        //         var objectName = ObjectName.Match(className).Value;
        //         className = className.Replace(objectName, "");
        //         objectName = objectName.Replace("[", "").Replace("]", "");
        //         var find = GameObject.Find(objectName);
        //         if (!find){
        //             Debug.LogError(objectName +"对象不存在");
        //             throw new NullReferenceException();
        //         }
        //         fieldType = Type.GetType(className);
        //         fieldObject = find.GetComponent(fieldType);
        //         if (string.IsNullOrEmpty(address)) fieldObject=value;
        //     } else{
        //         var type = Type.GetType(className);
        //         if(type == null) throw new NullReferenceException();
        //         var classObject = FindObjectOfType(type);
        //         if(classObject == null) throw new NullReferenceException();
        //         fieldObject = classObject;
        //     }
        //     while (address.Contains(".")){
        //         var substring = address.Substring(0, address.IndexOf(".", StringComparison.Ordinal));
        //         address=address.Substring(address.IndexOf(".", StringComparison.Ordinal) +1);
        //         var fieldInfo = fieldType.GetField(substring);
        //         if (fieldInfo != null){
        //             fieldObject = fieldInfo.GetValue(fieldObject);
        //             fieldType = fieldObject.GetType();
        //         }else if (fieldType.GetProperty(substring) is PropertyInfo propertyInfo){
        //             fieldObject=propertyInfo.GetValue(fieldObject);
        //             fieldType = fieldObject.GetType();
        //         } else{
        //             Debug.LogError(substring +"字段不存在");
        //             throw new NullReferenceException();
        //         }
        //     }
        //     var field = fieldType.GetField(address);
        //     if (field != null){
        //         field.SetValue(fieldObject,value);
        //     }else if (fieldType.GetProperty(address) is PropertyInfo propertyInfo){
        //         propertyInfo.SetValue(fieldObject,value);
        //     } else{
        //         Debug.LogError(address +"字段不存在");
        //         throw new NullReferenceException(null);;
        //     }
        // }
        // public static object FindInstanceID(string instanceID){
        //     if (instanceID.Contains(".")){
        //         //目前只有GraphData里的NodeData存在需要两次instanceID才能定位
        //         var strings = instanceID.Split('.');
        //         GraphData graphData = (GraphData) GfuInstance.FindAllWithGfuInstanceID(long.Parse(strings[0]));
        //         var longInstanceID = long.Parse(strings[1]);
        //         for (var i = 0; i < graphData.Nodes.Count; i++){
        //             if (graphData.Nodes[i].instanceID == longInstanceID){
        //                 return graphData.Nodes[i];
        //             }
        //         }
        //     }
        //     return GfuInstance.FindAllWithGfuInstanceID(long.Parse(instanceID));
        // }
        public static object FindUnityInstanceID(string instanceID){
#if UNITY_EDITOR
            return EditorUtility.InstanceIDToObject(int.Parse(instanceID));
#else
            return null;
#endif
        }
        public static object FindInstanceID<T>(string instanceID) where T : Object{
            return GfuInstance.FindAllWithGfuInstanceID<T>(long.Parse(instanceID));
        }
        public static object FindResource(string path){
            return Resources.Load(path);
        }
        public static object FindJson(string json,Type type){
            return JsonUtility.FromJson(json,type);
        }
        
        public static object FindJson(string json,object obj){
            object _obj=obj;
            JsonUtility.FromJsonOverwrite(json,_obj);
            return _obj;
        }
        
        public static object FindPath(string path){
            var fileStream = File.Open(path, FileMode.Open);
            List<byte> bytes=new List<byte>();
            int data;
            while ((data=fileStream.ReadByte())!=-1){
                bytes.Add((byte)data);
            }
            return bytes.ToArray();
        }
        public static string ParseJson(object otherObject){
            if (otherObject.GetType().IsPrimitive || otherObject is string){
                return "[Json:" + otherObject + "]";
            }
            return "[Json:"+JsonUtility.ToJson(otherObject)+"]";
        }
        public static string ParseInstanceID(object otherObject){
            if (otherObject is GfuInstance gfuInstance) return gfuInstance.instanceID.ToString();
            if (otherObject is MonoBehaviour monoBehaviour) return (monoBehaviour.GetComponent<GfuInstance>()?.instanceID ??0).ToString();
            if (otherObject is Component component) return (component.GetComponent<GfuInstance>()?.instanceID ??0).ToString();
            if (otherObject is GameObject gameObject) return (gameObject.GetComponent<GfuInstance>()?.instanceID ??0).ToString();
            if (otherObject is GraphData graphData) return graphData.InstanceID.ToString();
            if (otherObject is GfuGraph gfuGraph) return gfuGraph.graphData.InstanceID.ToString();
            if (otherObject is NodeData nodeData) return nodeData.instanceID.ToString();
            if (otherObject is GfuNode gfuNode) return gfuNode.nodeData.instanceID.ToString();
            return null;
        }

        public static string Parse(object otherObject){
#if UNITY_EDITOR
            return ParseMemory(otherObject);
#else
            return ParseJson(otherObject);
#endif
        }
        
        /// <summary>
        /// 解析组件中字段的地址
        /// </summary>
        /// <param name="otherObject"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string ParseMemory(object otherObject,object field){
            if (otherObject == null) return "[]";
            if (otherObject is Component component){
                var expression = "Memory:" +component.GetType().FullName +"[" +component.gameObject.name +"]";
                var fieldInfos = otherObject.GetType().GetFields();
                foreach (var fieldInfo in fieldInfos){
                    if (fieldInfo.GetValue(otherObject) == field){
                        expression += "+"+fieldInfo.Name;
                        break;
                    }
                }
                return "[" +expression +"]";
            }
            return "[]";
        }
        /// <summary>
        /// 解析组件的地址
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public static string ParseMemory(object otherObject){
            if (otherObject == null) return "[]";
            // if (otherObject is Component component){
            //     var expression = "Memory:" +component.GetType().FullName +"[" +component.gameObject.name +"]";
            //     return "[" +expression +"]";
            // }
            string expression = "";
            var type = otherObject.GetType();
            if (type.GetInterface(nameof(IEnumerable)) != null&&!type.IsSubclassOf(typeof(MonoBehaviour))&&type!=typeof(MonoBehaviour)){
                var enumerable = (IEnumerable) otherObject;
                foreach (var enumerableElement in enumerable){
                    if (enumerableElement is Component component){
                        expression+="|" +ParseMemoryPrivate(component.GetType().FullName, component.gameObject.name);
                    } else{
                        //TODO 这里未来要加入递归解析的代码
                        expression+="|" + ParseJson(enumerableElement);
                    }
                }
                if(!string.IsNullOrEmpty(expression)) return expression.Substring(1);
            }
            return ParseMemoryPrivate(otherObject,null);
        }
        /// <summary>
        /// 解析组件中字段的地址
        /// </summary>
        /// <param name="otherObject"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static string ParseMemory(object otherObject,string fieldName){
            string expression = "";
            var fieldObject = otherObject.GetType().GetField(fieldName).GetValue(otherObject);
            if (fieldObject.GetType().GetInterface(nameof(IEnumerable)) != null){
                var enumerable = (IEnumerable) fieldObject;
                foreach (var enumerableElement in enumerable){
                    if (enumerableElement is Component component){
                        expression+="|"+ParseMemoryPrivate(component.GetType().FullName, component.gameObject.name);
                    } else{
                        //TODO 这里未来要加入递归解析的代码
                        expression+="|"+ ParseJson(enumerableElement);
                    }
                }
                return expression.Substring(1);
            }
            return ParseMemoryPrivate(otherObject,fieldName);
        }

        private static string ParseMemoryPrivate(object otherObject,string fieldName){
            // if (otherObject == null) return "[]";
            // if (otherObject is Component component){
            //     var type = component.GetType();
            //     var typeName = type.FullName;
            //     if (typeName.Contains("UnityE")){
            //         typeName = "("+type.Assembly.FullName+")"+typeName;
            //     }
            //     var expression = "Memory:" + typeName +"[" + GameObjectPath(component.gameObject) +component.gameObject.name +"]";
            //     if (fieldName == null) return expression;
            //     var fieldInfo = otherObject.GetType().GetField(fieldName);
            //     if (fieldInfo == null){
            //         Debug.LogError("找不到" +fieldName);
            //         return "[" +expression +"]";
            //     }
            //     expression += "+" + fieldInfo.Name;
            //     return "[" +expression +"]";
            // }
            return null;
        }
        
    }
}
