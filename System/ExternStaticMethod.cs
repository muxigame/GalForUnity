//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ExternStaticMethod.cs
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GalForUnity.Graph.Data;
using GalForUnity.InstanceID;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Object = UnityEngine.Object;

namespace GalForUnity.System{
    public static class ExternStaticMethod{
        public static int[] Fill(this int[] array1, int value){
            for (int i = 0, length = array1.Length; i < length; i++){
                array1[i] = value;
            }

            return array1;
        }

        public static List<T> AddAll<T>(this List<T> array, List<T> array1){
            if (array == null){
                array = new List<T>();
            }

            if (array1 == null){
                return array;
            }

            for (int i = 0, length = array1.Count; i < length; i++){
                if (array1[i] != null) array.Add(array1[i]);
            }

            return array;
        }

        public static long CreateInstanceID(this Object obj){ return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0); }

        public static T FindPrefabWithInstanceID<T>(this object t, int instanceID) where T : Object{ return (T) FindPrefabWithInstanceID(t, instanceID, typeof(T)); }

        //TODO 需要优化，最佳情况下，请把所有的资源全部加载到静态资源，以提高加载速度，如果资源过大，那么就不做考虑，
        public static Object FindPrefabWithInstanceID(this object t, int instanceID, Type type){
#if UNITY_EDITOR
            var loadAll = FunAssetName();
            var replace1 = Application.dataPath.Replace("/", "\\");
            foreach (var s in loadAll){
                // var substring = s.ToString().Substring(0, s.ToString().LastIndexOf(".", StringComparison.Ordinal));
                var replace = s.ToString().Replace(replace1, "Assets");
                var loadAssetAtPath = AssetDatabase.LoadAssetAtPath(replace, type);
                if (loadAssetAtPath != null){
                    if (loadAssetAtPath.GetInstanceID() == instanceID) return (Object) loadAssetAtPath;
                }
            }
#else
            var loadAll = Resources.LoadAll("",type);
            foreach (var o in loadAll){
                if (o.GetInstanceID() == instanceID) return o;
            }
#endif
            return null;
        }

        public static List<FileInfo> FunAssetName(){
            var path = Application.dataPath;
            List<FileInfo> fileInfos = new List<FileInfo>();
            if (Directory.Exists(path)){
                DirectoryInfo direction = new DirectoryInfo(path);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++){
                    if (files[i].Name.EndsWith(".meta")){
                        continue;
                    }

                    if (files[i].Name.EndsWith(".prefab")){
                        fileInfos.Add(files[i]);
                    }
                }
            }

            return fileInfos;
        }

        public static bool IsGfuInstance(this Object obj){
            if (obj is GameObject gameObject){
                return true;
            }

            if (obj is MonoBehaviour monoBehaviour){
                return true;
            }

            if (obj is Component component){
                return true;
            }

            return false;
        }

        public static bool TryGetGfuInstance(this Object obj, out GfuInstance gfuInstance){
            if (obj is GameObject gameObject){
                gfuInstance = gameObject.GetComponent<GfuInstance>();
                return gfuInstance;
            }

            if (obj is MonoBehaviour monoBehaviour){
                gfuInstance = monoBehaviour.GetComponent<GfuInstance>();
                return gfuInstance;
            }

            if (obj is Component component){
                gfuInstance = component.GetComponent<GfuInstance>();
                return gfuInstance;
            }

            gfuInstance = null;
            return false;
        }
        public static bool TryAddGfuInstance(this Object obj, out GfuInstance gfuInstance){
            if (obj is GameObject gameObject){
                gfuInstance = gameObject.AddComponent<GfuInstance>();
                return gfuInstance;
            }

            if (obj is MonoBehaviour monoBehaviour){
                gfuInstance = monoBehaviour.gameObject.AddComponent<GfuInstance>();
                return gfuInstance;
            }

            if (obj is Component component){
                gfuInstance = component.gameObject.AddComponent<GfuInstance>();
                return gfuInstance;
            }

            gfuInstance = null;
            return false;
        }

        public static T FindObjectsWithGfuInstanceID<T>(this Object obj, long gfuInstanceID) where T : Object{
            if (GfuInstance.GfuInstances.ContainsKey(gfuInstanceID)) return GfuInstance.GfuInstances[gfuInstanceID].GetComponent<T>();
            foreach (var gfuInstance in ObjectOfType<GfuInstance>()){
                if (gfuInstanceID == gfuInstance.instanceID){
                    GfuInstance.GfuInstances.Add(gfuInstanceID, gfuInstance);
                    return gfuInstance.GetComponent<T>();
                }
            }

            return null;
        }

        public static T FindAllWithGfuInstanceID<T>(this Object obj, long gfuInstanceID) where T : Object{
            if (GfuInstance.GfuInstances.ContainsKey(gfuInstanceID)) return GfuInstance.GfuInstances[gfuInstanceID].GetComponent<T>();
            foreach (var gfuInstance in AllOfType<GfuInstance>()){
                if (gfuInstanceID == gfuInstance.instanceID){
                    GfuInstance.GfuInstances.Add(gfuInstanceID, gfuInstance);
                    return gfuInstance.GetComponent<T>();
                }
            }

            return null;
        }

        private static T[] ObjectOfType<T>() where T : Object{ return Object.FindObjectsOfType<T>(); }
        private static T[] AllOfType<T>() where T : Object{ return Resources.FindObjectsOfTypeAll<T>(); }


        public static Object FindObjectWithInstanceID(this object t, int instance){ return Array.Find(Object.FindObjectsOfType<Object>(), (x) => x.GetInstanceID()           == instance); }
        public static T FindObjectWithInstanceID<T>(this object t, int instance) where T : Object{ return Array.Find(Object.FindObjectsOfType<T>(), (x) => x.GetInstanceID() == instance); }
        public static T FindObjectWithInstanceID<T>(this T t, int instance) where T : Object{ return Array.Find(Object.FindObjectsOfType<T>(), (x) => x.GetInstanceID()      == instance); }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable{ return listToClone.Select(item => (T) item.Clone()).ToList(); }

        /// <summary>
        /// Editor Method 解析节点数据中的值
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <param name="fieldValue"></param>
        /// <param name="fieldInfo"></param>
        public static void ParseField(this DataInfo dataInfo, object fieldValue, FieldInfo fieldInfo){
            if(fieldValue ==null) return;
#if UNITY_EDITOR
            if (fieldInfo.GetCustomAttribute<NonSerializedAttribute>() != null) return;
            //保存List等字段，字典不支持
            if (fieldInfo.FieldType!=typeof(string)&&fieldInfo.FieldType.GetInterface(nameof(IEnumerable))!=null){
                var enumerable = fieldValue as IEnumerable;
                var nodeFieldInfos = new List<NodeData.NodeFieldInfo>();
                var listData = new NodeData.ListData();
                if (enumerable != null){
                    foreach (var obj in enumerable){
                        if (fieldValue is Object){
                            ParseIDField(nodeFieldInfos, obj, obj.GetType());
                        }

                        if (!fieldInfo.IsNotSerialized){ //可序列化的非VisualElement字段可被序列化，并且在之后被反序列化为对象，赋值给Node节点
                            ParseJSONField(nodeFieldInfos, obj, obj.GetType());
                        }
                    }
                } else{
                    Debug.Log("This list is empty");
                }
                listData.type = fieldInfo.FieldType.ToString();
                listData.assembly = fieldInfo.FieldType.Assembly.FullName;
                listData.name = fieldInfo.Name;
                if (fieldValue is Object){
                    listData.idField = nodeFieldInfos;
                    dataInfo.listField.Add(listData);
                }
                if (!fieldInfo.IsNotSerialized){ //可序列化的非VisualElement字段可被序列化，并且在之后被反序列化为对象，赋值给Node节点
                    listData.jsonField = nodeFieldInfos;
                    dataInfo.listField.Add(listData);
                }
                return;
            }
            //保存普通字段
            if (fieldValue is Object o){ //如果是mono字段包括GameObject和MonoBehavior及其子类，那么则保存其的InstanceID，此InstanceID为GfuInstance中的ID。
                ParseIDField(dataInfo.idField, fieldValue, fieldInfo);
                return;
            }

            if (!fieldInfo.IsNotSerialized){ //可序列化的非VisualElement字段可被序列化，并且在之后被反序列化为对象，赋值给Node节点
                ParseJSONField(dataInfo.jsonField, fieldValue, fieldInfo);
            }

#endif
        }

        /// <summary>
        /// Editor Method 解析节点数据中的值
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <param name="fieldValue"></param>
        /// <param name="fieldInfo"></param>
        public static void ParseIDField(List<NodeData.NodeFieldInfo> fieldinfo, object fieldValue, FieldInfo fieldInfo){
            if(fieldValue ==null) return;
#if UNITY_EDITOR
            if (fieldValue is Object o){ //如果是mono字段包括GameObject和MonoBehavior及其子类，那么则保存其的InstanceID，此InstanceID为GfuInstance中的ID。
                if(!o) return;
                if (AssetDatabase.IsNativeAsset(o)){
                    fieldinfo.Add(new NodeData.NodeFieldInfo() {
                        name = fieldInfo.Name, scriptableObject = o, type = o.GetType().FullName, assembly = o.GetType().Assembly.FullName
                    });
                } else if (o.TryGetGfuInstance(out GfuInstance gfuInstance)){
                    fieldinfo.Add(new NodeData.NodeFieldInfo() {
                        name = fieldInfo.Name, instanceID = gfuInstance.instanceID, type = o.GetType().FullName, assembly = o.GetType().Assembly.FullName
                    });
                } else{
                    Debug.LogError("没有找到GfuInstanceID,且这不是一个本地资源对象,这意味着这个对象没有被保存：" + fieldValue);
                }
            }
#endif
        }

        /// <summary>
        /// Editor Method 解析节点数据中的值
        /// </summary>
        /// <param name="fieldinfo"></param>
        /// <param name="fieldValue"></param>
        /// <param name="type"></param>
        public static void ParseIDField(List<NodeData.NodeFieldInfo> fieldinfo, object fieldValue, Type type){
            if(fieldValue ==null) return;
#if UNITY_EDITOR
            if (fieldValue is Object o){ //如果是mono字段包括GameObject和MonoBehavior及其子类，那么则保存其的InstanceID，此InstanceID为GfuInstance中的ID。
                if (AssetDatabase.IsNativeAsset(o)){
                    fieldinfo.Add(new NodeData.NodeFieldInfo() {
                        name = type.Name, scriptableObject = o, type = o.GetType().FullName, assembly = o.GetType().Assembly.FullName
                    });
                } else if (o.TryGetGfuInstance(out GfuInstance gfuInstance)){
                    fieldinfo.Add(new NodeData.NodeFieldInfo() {
                        name = type.Name, instanceID = gfuInstance.instanceID, type = o.GetType().FullName, assembly = o.GetType().Assembly.FullName
                    });
                } else{
                    Debug.LogError("没有找到GfuInstanceID,且这不是一个本地资源对象,这意味着这个对象没有被保存：" + fieldValue);
                }
            }
#endif
        }

        /// <summary>
        /// Editor Method 解析节点数据中的值
        /// </summary>
        /// <param name="fieldinfo"></param>
        /// <param name="fieldValue"></param>
        /// <param name="fieldInfo"></param>
        public static void ParseJSONField(List<NodeData.NodeFieldInfo> fieldinfo, object fieldValue, FieldInfo fieldInfo){
#if UNITY_EDITOR
            if (!(fieldValue is VisualElement)){
                Type fieldType = fieldValue != null ? fieldValue.GetType() : fieldInfo.FieldType;
                fieldinfo.Add(new NodeData.NodeFieldInfo() {
                    name = fieldInfo.Name, data = fieldType.IsPrimitive || fieldType == typeof(string) ? Convert.ToString(fieldValue) : JsonUtility.ToJson(fieldValue), type = fieldType.FullName, assembly = fieldType.Assembly.FullName
                });
            }
#endif
        }

        /// <summary>
        /// Editor Method 解析节点数据中的值
        /// </summary>
        /// <param name="fieldinfo"></param>
        /// <param name="fieldValue"></param>
        /// <param name="type"></param>
        public static void ParseJSONField(List<NodeData.NodeFieldInfo> fieldinfo, object fieldValue, Type type){
#if UNITY_EDITOR
            if (!(fieldValue is VisualElement)){
                Type fieldType = fieldValue != null ? fieldValue.GetType() : type;
                fieldinfo.Add(new NodeData.NodeFieldInfo() {
                    name = type.Name, data = fieldType.IsPrimitive || fieldType == typeof(string) ? Convert.ToString(fieldValue) : JsonUtility.ToJson(fieldValue), type = fieldType.FullName, assembly = fieldType.Assembly.FullName
                });
            }
#endif
        }
        /// <summary>
        /// Editor Method 解析节点数据中的值
        /// </summary>
        /// <param name="dataInfo"></param>
        /// <param name="fieldValue"></param>
        /// <param name="fieldInfo"></param>
        public static void ParseField(this DataInfo dataInfo, object fieldValue, PropertyInfo fieldInfo){
#if UNITY_EDITOR
            if (fieldValue != null){
                if (fieldValue is Object o){ //如果是mono字段包括GameObject和MonoBehavior及其子类，那么则保存其的InstanceID，此InstanceID为GfuInstance中的ID。
                    if (AssetDatabase.IsNativeAsset(o)){
                        dataInfo.idField.Add(new NodeData.NodeFieldInfo() {
                            name = fieldInfo.Name, scriptableObject = o, type = o.GetType().FullName, assembly = o.GetType().Assembly.FullName
                        });
                    } else if (o.TryGetGfuInstance(out GfuInstance gfuInstance)){
                        dataInfo.idField.Add(new NodeData.NodeFieldInfo() {
                            name = fieldInfo.Name, instanceID = gfuInstance.instanceID, type = o.GetType().FullName, assembly = o.GetType().Assembly.FullName
                        });
                    } else{
                        if (o.TryAddGfuInstance(out GfuInstance beAddedGfuInstance)){
                            dataInfo.idField.Add(new NodeData.NodeFieldInfo() {
                                name = fieldInfo.Name, instanceID = beAddedGfuInstance.instanceID, type = o.GetType().FullName, assembly = o.GetType().Assembly.FullName
                            });
                        } else{
                            Debug.LogError("The GfuInstanceID was not found and cannot be added, and this is not a local resource object, meaning the object is not saved"+fieldValue);
                        }
                    }
                    return;
                }
                // Debug.Log(fieldInfo.PropertyType.IsSerializable);
                if (!(fieldValue is VisualElement)){
                    Type fieldType = fieldValue.GetType();
                    dataInfo.jsonField.Add(new NodeData.NodeFieldInfo() {
                        name = fieldInfo.Name, data = fieldType.IsPrimitive || fieldType == typeof(string) ? Convert.ToString(fieldValue) : JsonUtility.ToJson(fieldValue), type = fieldType.FullName, assembly = fieldType.Assembly.FullName
                    });
                }
            }

#endif
        }

        public static Dictionary<T1, T2> Clone<T1, T2>(this Dictionary<T1, T2> listToClone) where T2 : ICloneable{
            Dictionary<T1, T2> newDictionary = new Dictionary<T1, T2>();
            foreach (var kCloneable in listToClone){
                newDictionary.Add(kCloneable.Key, (T2) kCloneable.Value.Clone());
            }

            return newDictionary;
        }
#if UNITY_EDITOR

#region Simple string path based extensions

        /// <summary>
        /// Returns the path to the parent of a SerializedProperty
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static string ParentPath(this SerializedProperty prop){
            int lastDot = prop.propertyPath.LastIndexOf('.');
            if (lastDot == -1) // No parent property
                return "";

            return prop.propertyPath.Substring(0, lastDot);
        }

        /// <summary>
        /// Returns the parent of a SerializedProperty, as another SerializedProperty
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static SerializedProperty GetParentProp(this SerializedProperty prop){
            string parentPath = prop.ParentPath();
            return prop.serializedObject.FindProperty(parentPath);
        }

#endregion

        /// <summary>
        /// Set isExpanded of the SerializedProperty and propogate the change up the hierarchy
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="expand">isExpanded value</param>
        public static void ExpandHierarchy(this SerializedProperty prop, bool expand = true){
            prop.isExpanded = expand;
            SerializedProperty parent = GetParentProp(prop);
            if (parent != null) ExpandHierarchy(parent);
        }

#region Reflection based extensions

        /// <summary>
        /// Use reflection to get the actual data instance of a SerializedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetValue<T>(this SerializedProperty prop){
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements){
                if (element.Contains("[")){
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                } else{
                    obj = GetValue(obj, element);
                }
            }

            if (obj is T) return (T) obj;
            return null;
        }

        public static Type GetTypeReflection(this SerializedProperty prop){
            object obj = GetParent<object>(prop);
            if (obj == null) return null;

            Type objType = obj.GetType();
            const BindingFlags bindingFlags = global::System.Reflection.BindingFlags.GetField
                                              | global::System.Reflection.BindingFlags.GetProperty
                                              | global::System.Reflection.BindingFlags.Instance
                                              | global::System.Reflection.BindingFlags.NonPublic
                                              | global::System.Reflection.BindingFlags.Public;
            FieldInfo field = objType.GetField(prop.name, bindingFlags);
            if (field == null) return null;
            return field.FieldType;
        }

        /// <summary>
        /// Uses reflection to get the actual data instance of the parent of a SerializedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T GetParent<T>(this SerializedProperty prop){
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1)){
                if (element.Contains("[")){
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                } else{
                    obj = GetValue(obj, element);
                }
            }

            return (T) obj;
        }

        private static object GetValue(object source, string name){
            if (source == null) return null;
            Type type = source.GetType();
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null){
                PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null) return null;
                return p.GetValue(source, null);
            }

            return f.GetValue(source);
        }

        private static object GetValue(object source, string name, int index){
            var enumerable = GetValue(source, name) as IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0) enm.MoveNext();
            return enm.Current;
        }

        /// <summary>
        /// Use reflection to check if SerializedProperty has a given attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this SerializedProperty prop){
            object[] attributes = GetAttributes<T>(prop);
            if (attributes != null){
                return attributes.Length > 0;
            }

            return false;
        }

        /// <summary>
        /// Use reflection to get the attributes of the SerializedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object[] GetAttributes<T>(this SerializedProperty prop){
            object obj = GetParent<object>(prop);
            if (obj == null) return new object[0];

            Type attrType = typeof(T);
            Type objType = obj.GetType();
            const BindingFlags bindingFlags = global::System.Reflection.BindingFlags.GetField
                                              | global::System.Reflection.BindingFlags.GetProperty
                                              | global::System.Reflection.BindingFlags.Instance
                                              | global::System.Reflection.BindingFlags.NonPublic
                                              | global::System.Reflection.BindingFlags.Public;
            FieldInfo field = objType.GetField(prop.name, bindingFlags);
            if (field != null) return field.GetCustomAttributes(attrType, true);
            return new object[0];
        }
        /// <summary>
        /// Use reflection to get the attributes of the SerializedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetAttribute<T>(this SerializedProperty prop){
            object obj = GetParent<object>(prop);
            if (obj == null) return null;
            Type attrType = typeof(T);
            Type objType = obj.GetType();
            const BindingFlags bindingFlags = global::System.Reflection.BindingFlags.GetField
                                              | global::System.Reflection.BindingFlags.GetProperty
                                              | global::System.Reflection.BindingFlags.Instance
                                              | global::System.Reflection.BindingFlags.NonPublic
                                              | global::System.Reflection.BindingFlags.Public;
            FieldInfo field = objType.GetField(prop.name, bindingFlags);
            if (field != null) return field.GetCustomAttribute(attrType);
            return null;
        }
        /// <summary>
        /// Find properties in the serialized object of the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="enterChildren"></param>
        /// <returns></returns>
        public static SerializedProperty[] FindPropsOfType<T>(this SerializedObject obj, bool enterChildren = false){
            List<SerializedProperty> foundProps = new List<SerializedProperty>();
            Type propType = typeof(T);

            var iterProp = obj.GetIterator();
            iterProp.Next(true);

            if (iterProp.NextVisible(enterChildren)){
                do{
                    var propValue = iterProp.GetValue<T>();
                    if (propValue == null){
                        if (iterProp.propertyType == SerializedPropertyType.ObjectReference){
                            if (iterProp.objectReferenceValue != null && iterProp.objectReferenceValue.GetType() == propType) foundProps.Add(iterProp.Copy());
                        }
                    } else{
                        foundProps.Add(iterProp.Copy());
                    }
                } while (iterProp.NextVisible(enterChildren));
            }

            return foundProps.ToArray();
        }

#endregion

#endif
    }
}