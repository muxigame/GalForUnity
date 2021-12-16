//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ForEachChangeSupport.cs
//
//        Created by 半世癫(Roc) at 2021-12-15 09:13:42
//
//======================================================================

using System.Collections.Generic;
using UnityEngine;

namespace GalForUnity.External{
    /// <summary>
    /// 为ForEach提供遍历修改支持
    /// </summary>
    public static class ForEachChangeSupport{
        public class ListGenericAdapterReference<T>{
            private readonly List<T> _list;
            private readonly int _index;
            private readonly T _obj;
            public ListGenericAdapterReference(List<T> list, int index){
                _list = list;
                _index = index;
                _obj = _list[_index];
            }

            public T Obj{
                set => _list[_index] = value;
                get=>_obj;
            }

            public void RemoveSelf(){
                _list.Remove(_obj);
            }
        }
        public class MapGenericAdapterReference<T,T2>{
            private readonly Dictionary<T,T2> _dictionary;
            private readonly T _key;
            private readonly T2 _value;
            public MapGenericAdapterReference(Dictionary<T,T2> dictionary, T key,T2 value){
                _dictionary = dictionary;
                _key = key;
                _value = value;
            }

            public T2 this[T key]{
                get => _dictionary[key];
                set => _dictionary[key] = value;
            }
        
            public T2 Value{
                get => _value;
                set => _dictionary[_key] = value;
            }
            public T Key{
                get => _key;
            }
            public void RemoveSelf(){
                _dictionary.Remove(_key);
            }
        }
        public class GenericAdapter<T>{
            public GenericAdapter(T obj){
                this.obj = obj;
            }
            public T obj;
        }
        /// <summary>
        /// 创建引用副本
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<ListGenericAdapterReference<T>> ToCap<T>(this List<T> obj){
            var list = new List<ListGenericAdapterReference<T>>();
            for (var i = 0; i < obj.Count; i++){
                list.Add(new ListGenericAdapterReference<T>(obj,i));
            }
            return list;
        }

        /// <summary>
        /// 创建引用副本
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public static List<MapGenericAdapterReference<T,T2>> ToCap<T,T2>(this Dictionary<T,T2> obj){
            var list = new List<MapGenericAdapterReference<T,T2>>();
            foreach (var keyValuePair in obj){
                list.Add(new MapGenericAdapterReference<T,T2>(obj,keyValuePair.Key,keyValuePair.Value));
            }
            return list;
        }
        /// <summary>
        ///创建拷贝副本
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<GenericAdapter<T>> ToCapCopy<T>(this List<T> obj){
            var list = new List<GenericAdapter<T>>();
            foreach (var o in obj){
                list.Add(new GenericAdapter<T>(o));
            }
            return list;
        }
        /// <summary>
        /// 移除list中的指定元素
        /// </summary>
        /// <param name="list"></param>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        public static void Remove<T>(this List<T> list,ListGenericAdapterReference<T> obj){
            obj.RemoveSelf();
        }

        /// <summary>
        /// 移除list中的指定元素
        /// </summary>
        /// <param name="list"></param>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        public static void Remove<T,T2>(this Dictionary<T,T2> list,MapGenericAdapterReference<T,T2> obj){
            obj.RemoveSelf();
        }
    }
}