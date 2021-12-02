//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuRunOnMono.cs
//
//        Created by 半世癫(Roc) at 2021-11-24 22:30:41
//
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalForUnity.System{
    /// <summary>
    /// CopyRight © MUXI Studio 
    /// Author Roc
    /// RunOnMono顾名思义，这个类能让你将一段代码放到Mono的生命周期里去执行
    /// </summary>
    [ExecuteAlways]
    public class GfuRunOnMono : MonoBehaviour{
        private static Dictionary<GfuMethodType, List<MonoAction>> DelegateType = new Dictionary<GfuMethodType, List<MonoAction>>();
        private static Dictionary<GfuMethodType, List<MonoAction>> AlwaysDelegateType = new Dictionary<GfuMethodType, List<MonoAction>>();
        // private static Action actions;
        private static GfuMethodType _gfuMethodType = GfuMethodType.Awake;

        public static bool IsUpdate => _gfuMethodType     == GfuMethodType.LateUpdate;
        public static bool IsLateUpdate => _gfuMethodType == GfuMethodType.LateUpdate;

        public static bool IsFixedUpdate => _gfuMethodType == GfuMethodType.FixedUpdate;

        private void Awake(){ SceneManager.sceneLoaded += (x, y) => { DelegateType.Clear(); }; }

        // Unity回调
        private void Update(){
            GfuRunOnMono.ExecuteCallBack(GfuMethodType.Update);
            GfuRunOnMono.ExecuteAlwaysCallBack(GfuMethodType.Update);
        }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次Update时执行
        /// </summary>
        /// <param name="action">要提交的回调</param>
        public static void Update(Action action){ Update(100, action); }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次Update时执行
        /// </summary>
        /// /// <param name="priority"></param>
        /// <param name="action">要提交的回调</param>
        /// <param name="always"></param>
        public static void Update(int priority, Action action, bool always = false){
            if(always) RegisterAlwaysCallBack(action, GfuMethodType.Update, priority);
            else RegisterCallBack(action, GfuMethodType.Update, priority);
        }

        private void FixedUpdate(){
            GfuRunOnMono.ExecuteCallBack(GfuMethodType.FixedUpdate);
            GfuRunOnMono.ExecuteAlwaysCallBack(GfuMethodType.FixedUpdate);
        }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次FixedUpdate时执行
        /// </summary>
        /// <param name="action">要提交的回调</param>
        public static void FixedUpdate(Action action){ FixedUpdate(100, action); }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次FixedUpdate时执行
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="action">要提交的回调</param>
        /// <param name="always"></param>
        public static void FixedUpdate(int priority, Action action, bool always = false){
            if(always) RegisterAlwaysCallBack(action, GfuMethodType.FixedUpdate, priority);
            else RegisterCallBack(action, GfuMethodType.FixedUpdate, priority);
        }

        private void LateUpdate(){
            GfuRunOnMono.ExecuteCallBack(GfuMethodType.LateUpdate);
            GfuRunOnMono.ExecuteAlwaysCallBack(GfuMethodType.LateUpdate);
        }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次LateUpdate时执行
        /// </summary>
        /// <param name="action">要提交的回调</param>
        public static void LateUpdate(Action action){ LateUpdate(100, action); }

        /// <summary>
        /// 向RunOnMono提交回调直到下一次LateUpdate时执行
        /// </summary>
        /// <param name="priority">优先级</param>
        /// <param name="action">要提交的回调</param>
        /// <param name="always"></param>
        public static void LateUpdate(int priority, Action action, bool always = false){
            if(always) RegisterAlwaysCallBack(action, GfuMethodType.LateUpdate, priority);
            else RegisterCallBack(action, GfuMethodType.LateUpdate, priority);
        }

        /// <summary>
        /// 负责执行回调的方法
        /// </summary>
        /// <param name="gfuMethodType">方法类型</param>
        public static void ExecuteCallBack(GfuMethodType gfuMethodType){
            _gfuMethodType = gfuMethodType;
            if (!DelegateType.TryGetValue(gfuMethodType, out List<MonoAction> monoActions)) return;
            if (monoActions == null || monoActions.Count <= 0) return;
            monoActions.Sort();
            monoActions.Reverse();
            //TODO 请前往检索GfuOperation，中的Update方法，由于使用了线程，所以GfuOperation会在遍历中途加入数据，尽管已经采用倒叙遍历和锁，但是还是期望其他解决方案
            lock (DelegateType){
                for (int i = monoActions.Count - 1; i >= 0; i--){
                    try{
                        monoActions[i].Action.Invoke();
                    } catch (Exception e){
                        Debug.LogError("Action execution failed:" + e);
                    }
                }
            }
            DelegateType[gfuMethodType].Clear();
        }
        public static void ExecuteAlwaysCallBack(GfuMethodType gfuMethodType){
            _gfuMethodType = gfuMethodType;
            if (!AlwaysDelegateType.TryGetValue(gfuMethodType, out List<MonoAction> alwaysMonoActions)) return;
            if (alwaysMonoActions == null || alwaysMonoActions.Count <= 0) return;
            alwaysMonoActions.Sort();
            alwaysMonoActions.Reverse();
            for (int i = alwaysMonoActions.Count - 1; i >= 0; i--){
                try{
                    alwaysMonoActions[i].Action.Invoke();
                } catch (Exception e){
                    Debug.LogError("Action execution failed:" + e);
                }
            }
        }

        /// <summary>
        /// 注册委托的方法
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="callback">回调方法</param>
        /// <param name="gfuMethodType">回调方法类型</param>
        private static void RegisterCallBack(Action callback, GfuMethodType gfuMethodType, int priority){
            lock (DelegateType){
                // RunOnMono.actions += callback;
                if (GfuRunOnMono.DelegateType.ContainsKey(gfuMethodType)){
                    GfuRunOnMono.DelegateType[gfuMethodType].Add(new MonoAction(callback, priority));
                } else{
                    GfuRunOnMono.DelegateType.Add(gfuMethodType, new List<MonoAction>());
                    GfuRunOnMono.DelegateType[gfuMethodType].Add(new MonoAction(callback, priority));
                }
            }
        } 
        /// <summary>
        /// 注册委托的方法
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="callback">回调方法</param>
        /// <param name="gfuMethodType">回调方法类型</param>
        private static void RegisterAlwaysCallBack(Action callback, GfuMethodType gfuMethodType, int priority){
            lock (AlwaysDelegateType){
                // RunOnMono.actions += callback;
                if (GfuRunOnMono.AlwaysDelegateType.ContainsKey(gfuMethodType)){
                    GfuRunOnMono.AlwaysDelegateType[gfuMethodType].Add(new MonoAction(callback, priority));
                } else{
                    GfuRunOnMono.AlwaysDelegateType.Add(gfuMethodType, new List<MonoAction>());
                    GfuRunOnMono.AlwaysDelegateType[gfuMethodType].Add(new MonoAction(callback, priority));
                }
            }
        }

        /// <summary>
        /// 清理委托的方法
        /// </summary>
        /// <param name="gfuMethodType">回调方法类型</param>
        public static void Clear(GfuMethodType gfuMethodType){
            // RunOnMono.actions += callback;
            if (GfuRunOnMono.DelegateType.ContainsKey(gfuMethodType)){
                GfuRunOnMono.DelegateType[gfuMethodType].Clear();
            }
        }
    }

    public class MonoAction : IComparable<MonoAction>{
        /// <summary>
        /// 要执行的委托
        /// </summary>
        public readonly Action Action;

        /// <summary>
        /// 委托的帧内优先级
        /// </summary>
        public readonly int Priority;

        public MonoAction(Action action, int priority){
            this.Action = action;
            this.Priority = priority;
        }

        public int CompareTo(MonoAction other){
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return -1;
            return -Priority.CompareTo(other.Priority);
        }
    }

    public enum GfuMethodType{
        Awake,
        Update,
        LateUpdate,
        FixedUpdate
    }
}