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
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GalForUnity.External{
    public interface IInstanceIDAble{ }

    public static class ExternStaticMethod{
        public static int[] Fill(this int[] array1, int value){
            for (int i = 0, length = array1.Length; i < length; i++) array1[i] = value;

            return array1;
        }

        public static List<T> AddAll<T>(this List<T> array, List<T> array1){
            if (array == null) array = new List<T>();

            if (array1 == null) return array;

            for (int i = 0, length = array1.Count; i < length; i++)
                if (array1[i] != null)
                    array.Add(array1[i]);

            return array;
        }

        public static long CreateInstanceID(this Object obj){ return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0); }
        public static long CreateInstanceID(this IInstanceIDAble obj){ return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0); }

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
                if (loadAssetAtPath != null)
                    if (loadAssetAtPath.GetInstanceID() == instanceID)
                        return loadAssetAtPath;
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
            var fileInfos = new List<FileInfo>();
            if (Directory.Exists(path)){
                var direction = new DirectoryInfo(path);
                var files = direction.GetFiles("*", SearchOption.AllDirectories);
                for (var i = 0; i < files.Length; i++){
                    if (files[i].Name.EndsWith(".meta")) continue;

                    if (files[i].Name.EndsWith(".prefab")) fileInfos.Add(files[i]);
                }
            }

            return fileInfos;
        }

        public static bool IsNullablePrimitive(this Type type){
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType == null) return type.IsPrimitive;
            return underlyingType.IsPrimitive;
        }

        public static bool IsGfuInstance(this Object obj){
            if (obj is GameObject gameObject) return true;

            if (obj is MonoBehaviour monoBehaviour) return true;

            if (obj is Component component) return true;

            return false;
        }

        public static FieldInfo[] GetFields<T>(this Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic){
            var fieldInfos = type.GetFields(bindingFlags);
            var fieldInfosList = new List<FieldInfo>();
            foreach (var fieldInfo in fieldInfos)
                if (fieldInfo.FieldType == typeof(T) || fieldInfo.FieldType.IsSubclassOf(typeof(T)))
                    fieldInfosList.Add(fieldInfo);
            return fieldInfosList.ToArray();
        }

        private static T[] ObjectOfType<T>() where T : Object{ return Object.FindObjectsOfType<T>(); }
        private static T[] AllOfType<T>() where T : Object{ return Resources.FindObjectsOfTypeAll<T>(); }


        public static Object FindObjectWithInstanceID(this object t, int instance){ return Array.Find(Object.FindObjectsOfType<Object>(), x => x.GetInstanceID()           == instance); }
        public static T FindObjectWithInstanceID<T>(this object t, int instance) where T : Object{ return Array.Find(Object.FindObjectsOfType<T>(), x => x.GetInstanceID() == instance); }
        public static T FindObjectWithInstanceID<T>(this T t, int instance) where T : Object{ return Array.Find(Object.FindObjectsOfType<T>(), x => x.GetInstanceID()      == instance); }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable{ return listToClone.Select(item => (T) item.Clone()).ToList(); }


        public static Dictionary<T1, T2> Clone<T1, T2>(this Dictionary<T1, T2> listToClone) where T2 : ICloneable{
            var newDictionary = new Dictionary<T1, T2>();
            foreach (var kCloneable in listToClone) newDictionary.Add(kCloneable.Key, (T2) kCloneable.Value.Clone());

            return newDictionary;
        }
#if UNITY_EDITOR

#region Simple string path based extensions

        /// <summary>
        ///     Returns the path to the parent of a SerializedProperty
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static string ParentPath(this SerializedProperty prop){
            var lastDot = prop.propertyPath.LastIndexOf('.');
            if (lastDot == -1) // No parent property
                return "";

            return prop.propertyPath.Substring(0, lastDot);
        }

        /// <summary>
        ///     Returns the parent of a SerializedProperty, as another SerializedProperty
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static SerializedProperty GetParentProp(this SerializedProperty prop){
            var parentPath = prop.ParentPath();
            return prop.serializedObject.FindProperty(parentPath);
        }

#endregion

        /// <summary>
        ///     Set isExpanded of the SerializedProperty and propogate the change up the hierarchy
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="expand">isExpanded value</param>
        public static void ExpandHierarchy(this SerializedProperty prop, bool expand = true){
            prop.isExpanded = expand;
            var parent = GetParentProp(prop);
            if (parent != null) ExpandHierarchy(parent);
        }

#region Reflection based extensions

        /// <summary>
        ///     Use reflection to get the actual data instance of a SerializedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetValue<T>(this SerializedProperty prop){
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
                if (element.Contains("[")){
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                } else{
                    obj = GetValue(obj, element);
                }

            if (obj is T) return (T) obj;
            return null;
        }

        public static Type GetTypeReflection(this SerializedProperty prop){
            var obj = GetParent<object>(prop);
            if (obj == null) return null;

            var objType = obj.GetType();
            const BindingFlags bindingFlags = BindingFlags.GetField
                                              | BindingFlags.GetProperty
                                              | BindingFlags.Instance
                                              | BindingFlags.NonPublic
                                              | BindingFlags.Public;
            var field = objType.GetField(prop.name, bindingFlags);
            if (field == null) return null;
            return field.FieldType;
        }

        /// <summary>
        ///     Uses reflection to get the actual data instance of the parent of a SerializedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T GetParent<T>(this SerializedProperty prop){
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
                if (element.Contains("[")){
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                } else{
                    obj = GetValue(obj, element);
                }

            return (T) obj;
        }

        private static object GetValue(object source, string name){
            if (source == null) return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null){
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
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
        ///     Use reflection to check if SerializedProperty has a given attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this SerializedProperty prop){
            var attributes = GetAttributes<T>(prop);
            if (attributes != null) return attributes.Length > 0;

            return false;
        }

        /// <summary>
        ///     Use reflection to get the attributes of the SerializedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object[] GetAttributes<T>(this SerializedProperty prop){
            var obj = GetParent<object>(prop);
            if (obj == null) return new object[0];

            var attrType = typeof(T);
            var objType = obj.GetType();
            const BindingFlags bindingFlags = BindingFlags.GetField
                                              | BindingFlags.GetProperty
                                              | BindingFlags.Instance
                                              | BindingFlags.NonPublic
                                              | BindingFlags.Public;
            var field = objType.GetField(prop.name, bindingFlags);
            if (field != null) return field.GetCustomAttributes(attrType, true);
            return new object[0];
        }

        /// <summary>
        ///     Use reflection to get the attributes of the SerializedProperty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetAttribute<T>(this SerializedProperty prop){
            var obj = GetParent<object>(prop);
            if (obj == null) return null;
            var attrType = typeof(T);
            var objType = obj.GetType();
            const BindingFlags bindingFlags = BindingFlags.GetField
                                              | BindingFlags.GetProperty
                                              | BindingFlags.Instance
                                              | BindingFlags.NonPublic
                                              | BindingFlags.Public;
            var field = objType.GetField(prop.name, bindingFlags);
            if (field != null) return field.GetCustomAttribute(attrType);
            return null;
        }

        /// <summary>
        ///     Find properties in the serialized object of the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="enterChildren"></param>
        /// <returns></returns>
        public static SerializedProperty[] FindPropsOfType<T>(this SerializedObject obj, bool enterChildren = false){
            var foundProps = new List<SerializedProperty>();
            var propType = typeof(T);

            var iterProp = obj.GetIterator();
            iterProp.Next(true);

            if (iterProp.NextVisible(enterChildren))
                do{
                    var propValue = iterProp.GetValue<T>();
                    if (propValue == null){
                        if (iterProp.propertyType == SerializedPropertyType.ObjectReference)
                            if (iterProp.objectReferenceValue != null && iterProp.objectReferenceValue.GetType() == propType)
                                foundProps.Add(iterProp.Copy());
                    } else{
                        foundProps.Add(iterProp.Copy());
                    }
                } while (iterProp.NextVisible(enterChildren));

            return foundProps.ToArray();
        }

#endregion

#endif
    }
}