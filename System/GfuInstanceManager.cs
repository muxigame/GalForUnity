//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuInstanceManager.cs
//
//        Created by 半世癫(Roc) at 2021-11-24 22:30:41
//
//======================================================================
using System;
using UnityEngine;

namespace GalForUnity.System{
    /// <summary>
    /// CopyRight © MUXI Studio 
    /// Author Roc
    /// 实例管理器类，继承了此类的类可以使用GetInstance()获取对象实例，类似于单例，但是对象本身依旧是可New的
    /// 使用此类需要传入一个要单例使用的类
    /// </summary>
    /// <typeparam name="T">要创建实例的泛型类</typeparam>
    public class GfuInstanceManager<T> : IDisposable where T : class /*new() 允许子类存在私有构造，并不会出现编译器错误，但运行时依旧会出现无法new的异常*/{
        private static object Lock = new object();
        private static volatile T instance;

        /// <summary>
        /// 或得类实例，无需担心空指针问题
        /// </summary>
        /// <returns></returns>
        public static T GetInstance(){
            if (instance == null){
                lock (Lock){
                    if (instance == null){
                        instance = Activator.CreateInstance<T>() as T; //避免子类必须为可new类型，所以这里使用工厂
                    }
                }
            }

            return instance;
        }

#region IDisposable Support

        public bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing){ }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~InstanceManager()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public virtual void Dispose(){ Dispose(true); }

#endregion
    }

    public class GfuInstanceManagerForMono<T> : MonoBehaviour, IDisposable
        where T : MonoBehaviour /*new() 允许子类存在私有构造，并不会出现编译器错误，但运行时依旧会出现无法new的异常*/{
        private static object Lock = new object();
        private static volatile T instance;

        /// <summary>
        /// 或得类实例，无需担心空指针问题
        /// </summary>
        /// <returns></returns>
        public static T GetInstance(){
            if (instance == null){
                lock (Lock){
                    if (instance == null){
                        instance = FindObjectOfType<T>(); //避免子类必须为可new类型，所以这里使用工厂
                    }
                }
            }

            return instance;
        }

#region IDisposable Support

        protected virtual void Dispose(bool disposing){ }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~InstanceManager()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public virtual void Dispose(){ Dispose(true); }

#endregion
    }
}