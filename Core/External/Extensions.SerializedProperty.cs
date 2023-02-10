using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace GalForUnity.Core.External{
    public static partial class Extension{
#if UNITY_EDITOR

#region Simple string path based extensions

        /// <summary>
        ///     Returns the path to the parent of a SerializedProperty
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        internal static string ParentPath(this SerializedProperty prop){
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
        internal static SerializedProperty GetParentProp(this SerializedProperty prop){
            var parentPath = prop.ParentPath();
            return prop.serializedObject.FindProperty(parentPath);
        }

#endregion

        /// <summary>
        ///     Set isExpanded of the SerializedProperty and propogate the change up the hierarchy
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="expand">isExpanded value</param>
        internal static void ExpandHierarchy(this SerializedProperty prop, bool expand = true){
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
        internal static object GetValue<T>(this SerializedProperty prop){
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

        internal static Type GetTypeReflection(this SerializedProperty prop){
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
        internal static T GetParent<T>(this SerializedProperty prop){
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

        internal static object GetValue(object source, string name){
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

        internal static object GetValue(object source, string name, int index){
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
        internal static bool HasAttribute<T>(this SerializedProperty prop){
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
        internal static object[] GetAttributes<T>(this SerializedProperty prop){
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
        internal static object GetAttribute<T>(this SerializedProperty prop){
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
        internal static SerializedProperty[] FindPropsOfType<T>(this SerializedObject obj, bool enterChildren = false){
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