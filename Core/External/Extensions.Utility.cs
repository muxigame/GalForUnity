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
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace GalForUnity.Core.External{
    public class InstanceIDUtil{
        public static long CreateInstanceID(){ return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0); }
    }

    public struct SelectCap<T1, T2> : IComparable<SelectCap<T1, T2>> where T1 : struct, IComparable where T2 : class{
        public T1 Value;
        public T2 Obj;

        public SelectCap(T1 value, T2 obj){
            Value = value;
            Obj = obj;
        }
        
        public int CompareTo(SelectCap<T1, T2> other){ return Value.CompareTo(other.Value); }
    }

    public static partial class Extension{
        public static List<T> AddAll<T>(this List<T> array, List<T> array1){
            if (array == null) array = new List<T>();

            if (array1 == null) return array;

            for (int i = 0, length = array1.Count; i < length; i++)
                if (array1[i] != null)
                    array.Add(array1[i]);

            return array;
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

        /// <summary>
        ///     创建引用副本
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<ListGenericAdapterReference<T>> ToCap<T>(this List<T> obj){
            var list = new List<ListGenericAdapterReference<T>>();
            for (var i = 0; i < obj.Count; i++) list.Add(new ListGenericAdapterReference<T>(obj, i));
            return list;
        }

        /// <summary>
        ///     创建引用副本
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public static List<MapGenericAdapterReference<T, T2>> ToCap<T, T2>(this Dictionary<T, T2> obj){
            var list = new List<MapGenericAdapterReference<T, T2>>();
            foreach (var keyValuePair in obj) list.Add(new MapGenericAdapterReference<T, T2>(obj, keyValuePair.Key, keyValuePair.Value));
            return list;
        }

        /// <summary>
        ///     创建拷贝副本
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<GenericAdapter<T>> ToCapCopy<T>(this List<T> obj){
            var list = new List<GenericAdapter<T>>();
            foreach (var o in obj) list.Add(new GenericAdapter<T>(o));
            return list;
        }

        /// <summary>
        ///     移除list中的指定元素
        /// </summary>
        /// <param name="list"></param>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        public static void Remove<T>(this List<T> list, ListGenericAdapterReference<T> obj){ obj.RemoveSelf(); }

        /// <summary>
        ///     移除list中的指定元素
        /// </summary>
        /// <param name="list"></param>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        public static void Remove<T, T2>(this Dictionary<T, T2> list, MapGenericAdapterReference<T, T2> obj){ obj.RemoveSelf(); }

        public class ListGenericAdapterReference<T>{
            private readonly int _index;
            private readonly List<T> _list;
            private readonly T _obj;

            public ListGenericAdapterReference(List<T> list, int index){
                _list = list;
                _index = index;
                _obj = _list[_index];
            }

            public T Obj{
                set => _list[_index] = value;
                get => _obj;
            }

            public void RemoveSelf(){ _list.Remove(_obj); }
        }

        public class MapGenericAdapterReference<T, T2>{
            private readonly Dictionary<T, T2> _dictionary;
            private readonly T2 _value;

            public MapGenericAdapterReference(Dictionary<T, T2> dictionary, T key, T2 value){
                _dictionary = dictionary;
                Key = key;
                _value = value;
            }

            public T2 this[T key]{
                get => _dictionary[key];
                set => _dictionary[key] = value;
            }

            public T2 Value{
                get => _value;
                set => _dictionary[Key] = value;
            }

            public T Key{ get; }

            public void RemoveSelf(){ _dictionary.Remove(Key); }
        }

        public class GenericAdapter<T>{
            public T obj;

            public GenericAdapter(T obj){ this.obj = obj; }
        }
    }
}