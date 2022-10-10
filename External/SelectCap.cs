//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName : SelectCap.cs   Time : 2022-05-29 00:48:41
//
//======================================================================

using System;

namespace GalForUnity.External{
    /// <summary>
    /// 一个为了实现MinBy maxBy的小包装，TODO 把用到这坨屎的实现全部换成 more linq
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public struct SelectCap<T1,T2>:IComparable<SelectCap<T1,T2>> where T1:struct,IComparable where T2:class {
    
        public T1 Value;
        public T2 Obj;
    
        public SelectCap(T1 value,T2 obj){
            this.Value = value;
            this.Obj = obj;
        }
    

        public int CompareTo(SelectCap<T1, T2> other){
            return Value.CompareTo(other.Value);
        }
    }
}