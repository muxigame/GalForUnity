//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ExternalList.cs
//
//        Created by 半世癫(Roc) at 2021-08-26 14:05:06
//
//======================================================================

using System.Collections.Generic;

namespace MUX.ExternalMethods{
    public static class ExternalList{
        /// <summary>
        /// 返回逐元素转换器
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ConvertTyper<T> Typer<T>(this List<T> list){
            return new ConvertTyper<T>(list);
        }
        public static System.Type ElementType<T>(this List<T> list){
            return typeof(T);
        }
        public class ConvertTyper<T>{
            private readonly List<T> _list;

            public ConvertTyper(List<T> list){
                this._list = list;
            }

            /// <summary>
            /// 逐元素转换列表里的数据，当然前提是数据的确是T类型的
            /// </summary>
            /// <typeparam name="T">要转换到的类型</typeparam>
            /// <returns></returns>
#pragma warning disable 693
            public List<T> To<T>() where T : class{
#pragma warning restore 693
                var list = new List<T>();
                foreach (var x1 in _list){
                    list.Add(x1 as T);
                }
                return list;
            }
        }
    }
}
