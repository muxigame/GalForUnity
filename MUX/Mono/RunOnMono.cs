using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MUX.Mono{
    /// <summary>
    /// CopyRight © MUXI Studio 
    /// Author Roc
    /// RunOnMono顾名思义，这个类能让你将一段代码放到Mono的生命周期里去执行
    /// </summary>
    [ExecuteAlways]
    public class RunOnMono : MonoBehaviour{
        private static Dictionary<MethodType, List<MonoAction>> DelegateType = new Dictionary<MethodType, List<MonoAction>>();

        private static Dictionary<MethodType, List<MonoAction>> AlwaysDelegateType = new Dictionary<MethodType, List<MonoAction>>();

        // private static Action actions;
        private static MethodType _methodType = MethodType.Awake;

        public static bool IsUpdate => _methodType     == MethodType.LateUpdate;
        public static bool IsLateUpdate => _methodType == MethodType.LateUpdate;

        public static bool IsFixedUpdate => _methodType == MethodType.FixedUpdate;

        private void Awake(){ SceneManager.sceneLoaded += (x, y) => { DelegateType.Clear(); }; }

        // Unity回调
        private void Update(){ RunOnMono.ExecuteCallBack(MethodType.Update); }
        private void LateUpdate(){ RunOnMono.ExecuteCallBack(MethodType.LateUpdate); }
        private void FixedUpdate(){ RunOnMono.ExecuteCallBack(MethodType.FixedUpdate); }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次Update时执行
        /// </summary>
        /// <param name="action">要提交的回调</param>
        /// <param name="always"></param>
        public static void Update(Action action, bool always = false){ Update(100, action, always); }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次LateUpdate时执行
        /// </summary>
        /// <param name="action">要提交的回调</param>
        public static void LateUpdate(Action action){ LateUpdate(100, action); }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次FixedUpdate时执行
        /// </summary>
        /// <param name="action">要提交的回调</param>
        public static void FixedUpdate(Action action){ FixedUpdate(100, action); }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次Update时执行
        /// </summary>
        /// ///
        /// <param name="priority"></param>
        /// <param name="action">要提交的回调</param>
        /// <param name="always"></param>
        public static void Update(int priority, Action action, bool always = false){
            if (always)
                RegisterAlwaysCallBack(action, MethodType.Update, priority);
            else
                RegisterCallBack(action, MethodType.Update, priority);
        }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次LateUpdate时执行
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="action">要提交的回调</param>
        public static void LateUpdate(int priority, Action action){ RegisterCallBack(action, MethodType.LateUpdate, priority); }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次FixedUpdate时执行
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="action">要提交的回调</param>
        public static void FixedUpdate(int priority, Action action){ RegisterCallBack(action, MethodType.FixedUpdate, priority); }

        /// <summary>
        /// 负责执行回调的方法
        /// </summary>
        /// <param name="methodType">方法类型</param>
        public static void ExecuteCallBack(MethodType methodType){
            _methodType = methodType;
            if (!AlwaysDelegateType.TryGetValue(methodType, out List<MonoAction> alwaysMonoActions)) return;
            if (alwaysMonoActions == null || alwaysMonoActions.Count <= 0) return;
            alwaysMonoActions.Sort();
            foreach (var alwaysMonoAction in alwaysMonoActions){
                alwaysMonoAction.Action.Invoke();
            }

            if (!DelegateType.TryGetValue(methodType, out List<MonoAction> monoActions)) return;
            if (monoActions == null || monoActions.Count <= 0) return;
            monoActions.Sort();
            foreach (var monoAction in monoActions){
                try{
                    monoAction.Action.Invoke();
                } catch (Exception e){
                    Debug.LogError("Action execution failed:" + e);
                }
            }

            DelegateType[methodType].Clear();
        }

        /// <summary>
        /// 注册委托的方法
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="callback">回调方法</param>
        /// <param name="methodType">回调方法类型</param>
        private static void RegisterCallBack(Action callback, MethodType methodType, int priority){
            lock (DelegateType){
                // RunOnMono.actions += callback;
                if (RunOnMono.DelegateType.ContainsKey(methodType)){
                    RunOnMono.DelegateType[methodType].Add(new MonoAction(callback, priority));
                } else{
                    RunOnMono.DelegateType.Add(methodType, new List<MonoAction>());
                    RunOnMono.DelegateType[methodType].Add(new MonoAction(callback, priority));
                }
            }
        }

        /// <summary>
        /// 注册委托的方法
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="callback">回调方法</param>
        /// <param name="methodType">回调方法类型</param>
        private static void RegisterAlwaysCallBack(Action callback, MethodType methodType, int priority){
            lock (AlwaysDelegateType){
                // RunOnMono.actions += callback;
                if (RunOnMono.AlwaysDelegateType.ContainsKey(methodType)){
                    RunOnMono.AlwaysDelegateType[methodType].Add(new MonoAction(callback, priority));
                } else{
                    RunOnMono.AlwaysDelegateType.Add(methodType, new List<MonoAction>());
                    RunOnMono.AlwaysDelegateType[methodType].Add(new MonoAction(callback, priority));
                }
            }
        }

        /// <summary>
        /// 清理委托的方法
        /// </summary>
        /// <param name="methodType">回调方法类型</param>
        public static void Clear(MethodType methodType){
            // RunOnMono.actions += callback;
            if (RunOnMono.DelegateType.ContainsKey(methodType)){
                RunOnMono.DelegateType[methodType].Clear();
            }
        }
    }

    public class MonoAction : IComparable<MonoAction>{
        public MonoAction(Action action, int priority){
            this.Action = action;
            this.Priority = priority;
        }

        /// <summary>
        /// 要执行的委托
        /// </summary>
        public readonly Action Action;

        /// <summary>
        /// 委托的帧内优先级
        /// </summary>
        public readonly int Priority;

        public int CompareTo(MonoAction other){
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return -1;
            return -Priority.CompareTo(other.Priority);
        }
    }

    public enum MethodType{
        Awake,
        Update,
        LateUpdate,
        FixedUpdate
    }
}