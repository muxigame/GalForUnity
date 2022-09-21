//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  EventCenter.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Graph;
using GalForUnity.Graph.AssetGraph;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.Operation;
using GalForUnity.Model;
using GalForUnity.Model.Plot;
using GalForUnity.Model.Scene;
using GalForUnity.System.Archive;
using UnityEngine;
using UnityEngine.Events;

namespace GalForUnity.System.Event {
	/// <summary>
	/// EventCenter顾名思义，绝大部分能监听的事件都定义在这里
	/// </summary>
	public sealed class EventCenter {
		private Dictionary<string, UnityAction> _events;//不带参委托
        private Dictionary<string, UnityAction<object>> param_events;//带参委托
        private Dictionary<string, Func<object>> return_events;//带参委托
        private Dictionary<string, Func<object,object>> return_parem_events;//带参委托
        
        /// <summary>
        /// 向事件中心添加监听
        /// </summary>
        /// <param name="key">事件名称</param>
        /// <param name="function">事件回调</param>
        public void AddEventListening(string key, UnityAction function) {//添加回调的方法
            if (key == null || function == null) {//如果空值空键
                throw new NullReferenceException();
            }
            if (_events.ContainsKey(key)) {//如果Key存在
                UnityAction action;
                _events.TryGetValue(key, out action);//取出回调
                if (action != null) {//如果回调不为空
                    _events.Remove(key);//移除原先回调
                    _events.Add(key, action += function);//重新添加回调
                } else {
                    throw new EventNotFoundExecption();
                }
            } else {
                _events.Add(key, function);
            }
        }
        /// <summary>
        /// 向事件中心添加监听
        /// </summary>
        /// <param name="key">事件名称</param>
        /// <param name="function">事件回调<事件参数></param>
        public void AddEventListeningWithParam(string key, UnityAction<object> function) {//添加回调的方法
            if (key == null || function == null) {//如果空值空键
                throw new NullReferenceException();
            }
            if (param_events.ContainsKey(key)) {//如果Key存在
                UnityAction<object> action;//带参委托
                param_events.TryGetValue(key, out action);//取出带参回调
                if (action != null) {//如果带参回调不为空
                    param_events.Remove(key);//移除原先带参回调
                    param_events.Add(key, action += function );//重新添加带参回调
                } else {
                    throw new EventNotFoundExecption();
                }
            } else {
                param_events.Add(key, function);//添加带参回调
            }
        } 
        /// <summary>
        /// 向事件中心添加监听
        /// </summary>
        /// <param name="key">事件名称</param>
        /// <param name="function">事件回调<事件参数></param>
        public void AddEventListeningWantReturn(string key, Func<object> function) {//添加回调的方法
            if (key == null || function == null) {//如果空值空键
                throw new NullReferenceException();
            }
            if (return_events.ContainsKey(key)) {//如果Key存在
                Func<object> action;//带参委托
                return_events.TryGetValue(key, out action);//取出带参回调
                if (action != null) {//如果带参回调不为空
                    return_events.Remove(key);//移除原先带参回调
                    return_events.Add(key, action += function);//重新添加带参回调
                } else {
                    throw new EventNotFoundExecption();
                }
            } else {
                return_events.Add(key, function);//添加带参回调
            }
        }
        /// <summary>
        /// 向事件中心添加监听
        /// </summary>
        /// <param name="key">事件名称</param>
        /// <param name="function">事件回调<事件参数></param>
        public void AddEventListeningWantReturnWithParam(string key, Func<object,object> function) {//添加回调的方法
            if (key == null || function == null) {//如果空值空键
                throw new NullReferenceException();
            }
            if (return_parem_events.ContainsKey(key)) {//如果Key存在
                Func<object,object> action;//带参委托
                return_parem_events.TryGetValue(key, out action);//取出带参回调
                if (action != null) {//如果带参回调不为空
                    return_parem_events.Remove(key);//移除原先带参回调
                    return_parem_events.Add(key, action += function);//重新添加带参回调
                } else {
                    throw new EventNotFoundExecption();
                }
            } else {
                return_parem_events.Add(key, function);//添加带参回调
            }
        }

        /// <summary>
        /// 向事件中心发送事件，请求参数
        /// </summary>
        /// <param name="_event">事件名称</param>
        public T RequestData<T>(string _event)
        {
            if (_event == null) {//如果空键
                throw new NullReferenceException();
            }
            if (return_events.ContainsKey(_event)) {//如果key存在
                Func<object> action;
                return_events.TryGetValue(_event, out action);//尝试获得无参回调
                if (action != null) {
                    return (T)action.Invoke();//回调
                } 
            }
            throw new EventNotFoundExecption();
        }
        /// <summary>
        /// 向事件中心发送事件，请求参数
        /// </summary>
        /// <param name="_event">事件名称</param>
        public T RequestData<T>(string _event, object param)
        {
            if (_event == null) {//如果空键
                throw new NullReferenceException();
            }
            if (return_parem_events.ContainsKey(_event)) {//如果key存在
                Func<object,object> action;
                return_parem_events.TryGetValue(_event, out action);//尝试获得无参回调
                if (action != null) {
                    return (T)action.Invoke(param);//回调
                } 
            }
            throw new EventNotFoundExecption();
        }
        
        /// <summary>
        /// 向事件中心发送事件，并回调监听对象
        /// </summary>
        /// <param name="_event">事件名称</param>
        public void SendEvent(string _event) {
            if (_event == null) {//如果空键
                throw new NullReferenceException();
            }
            if (_events.ContainsKey(_event)) {//如果key存在
                UnityAction action;
                _events.TryGetValue(_event, out action);//尝试获得无参回调
                if (action != null) {
                    action.Invoke();//回调
                } else {
                    throw new EventNotFoundExecption();
                }
            }
        }
        /// <summary>
        /// 向事件中心发送事件，并回调监听对象
        /// </summary>
        /// <param name="_event">事件名称</param>
        /// <param name="param">事件携带的参数</param>
        public void SendEvent(string _event, object param) {
            if (_event == null || param == null) {//如果空键
                throw new NullReferenceException();
            }
            if (param_events.ContainsKey(_event)) {//如果key存在
                UnityAction<object> action;
                param_events.TryGetValue(_event, out action);//尝试获得带参回调
                if (action != null) {
                    action.Invoke(param);//回调
                } else {
                    throw new EventNotFoundExecption();
                }
            }
        }
        /// <summary>
        /// 向事件中心取消注册的监听
        /// </summary>
        /// <param name="key">要取消的事件</param>
        public void CancelRegister(string key) {//移除监听
            if (key == null) {//如果空键
                throw new NullReferenceException();
            }
            if (_events.ContainsKey(key)) {
                _events.Remove(key);
            }
            if (param_events.ContainsKey(key)) {
                param_events.Remove(key);
            }
            if (return_events.ContainsKey(key)) {
                return_events.Remove(key);
            }
            if (return_parem_events.ContainsKey(key)) {
                return_parem_events.Remove(key);
            }

        }
        /// <summary>
        /// 清除所有事件回调
        /// </summary>
        public void ClearEventCallBack() {
            _events.Clear();
            param_events.Clear();// 清空所有注册的事件
            return_events.Clear();
            return_parem_events.Clear();
        }

        public class EventNotFoundExecption : Exception { }

    
		// ReSharper disable all FieldCanBeMadeReadOnly.Global
		private static EventCenter _eventCenter = new EventCenter();

		private static EventCenter eventCenter{
			get{
				if (null == _eventCenter.RoleChangeEvent
				    || null == _eventCenter.RoleStateChangeEvent
				    || null == _eventCenter.SceneStateChangeEvent
				    || null == _eventCenter.OnPlotWillExecuteEvent
#pragma warning disable 612
				    // || null == _eventCenter.PlotItemExecuteEvent
#pragma warning restore 612
				    // || null == _eventCenter.PlotExecuteOverEvent
				    || null == _eventCenter.PlotRequireCheckOkEvent
				    || null == _eventCenter.DayOverEvent
				    || null == _eventCenter.OnNodeExecutedEvent
				    // || null == _eventCenter.OnGraphExecutedEvent
				    )
				{
					return _eventCenter = new EventCenter();
				}
				return _eventCenter;
			}
		}
		/// <summary>
		/// 获得事件中心的实例
		/// </summary>
		/// <returns></returns>
		public static EventCenter GetInstance(){
			return eventCenter;
		}
		/// <summary>
		/// 当鼠标落下时触发
		/// </summary>
		public UnityEvent<Vector2> OnMouseDown;
		/// <summary>
		/// 当某个角色的数值被改变是触发
		/// </summary>
		public UnityAction<int, string, int> RoleChangeEvent;
		/// <summary>
		/// 当某个角色的状态发生改变时触发
		/// </summary>
		public UnityAction<RoleModel> RoleStateChangeEvent;
		/// <summary>
		/// 当系统场景的状态发生改变时触发
		/// </summary>
		public UnityAction<SceneModel> SceneStateChangeEvent;
		/// <summary>
		/// 当角色说话时触发
		/// </summary>
		public Func<string,string> OnSpeak;
		/// <summary>
		/// 当剧情开始执行时触发
		/// </summary>
		public Func<PlotModel, RoleModel, bool> OnPlotWillExecuteEvent;
		// /// <summary>
		// /// 当剧情流中的某一项开始执行时触发
		// /// </summary>
		// [Obsolete]
		// public Func<PlotModel, RoleModel, PlotItem, bool> PlotItemExecuteEvent;
		/// <summary>
		/// 当剧情流执行完毕时触发
		/// </summary>
		[Obsolete]
		public UnityAction<PlotModel, RoleModel> PlotExecuteOverEvent;
		/// <summary>
		/// 当剧情流通过检查器的检查时触发
		/// </summary>
		public UnityAction<PlotModel, RoleModel> PlotRequireCheckOkEvent;
		/// <summary>
		/// 当一天结束时触发
		/// </summary>
		public UnityAction<GameTime> DayOverEvent;
		/// <summary>
		/// 当某一个剧情节点执行完毕时触发
		/// </summary>
		
#if UNITY_EDITOR
		public Action<GfuNode> OnNodeExecutedEvent;
#else
		public Action<GfuNode> OnNodeExecutedEvent;
#endif
		/// <summary>
		/// 当剧情流通过检查器的检查时触发
		/// </summary>
		// public UnityAction<GfuGraph> OnGraphExecutedEvent;
		
		/// <summary>
		/// 当剧情项执行完毕时触发
		/// </summary>
		public Action OnPlotItemExecutedEvent;
		/// <summary>
		/// 当剧情操作执行完毕执行完毕时触发
		/// </summary>
		public UnityEvent<GfuOperation> OnOperationExecutedEvent;
		/// <summary>
		/// 当存档系统活动时回调
		/// </summary>
		public UnityEvent<ArchiveSystem.ArchiveEventType> archiveEvent;
		
		public float time;

		public static int limitCount =
#if UNITY_EDITOR
				1000
#else
				2000
#endif
			;
		
		public static int recursionCount = 0;


		private void LimitCheck(GfuNode gfuNode){
			recursionCount++;
			if (recursionCount > limitCount){
				OnNodeExecutedEvent = null;
				throw new StackOverflowException();
			}

		}
		

		public EventCenter(){
			//初始化所有委托
			RoleChangeEvent = (x,y,z) => { };
			// OnSpeak = (x) => { return x;};
			// OnSpeak=new Func<string, string>();
			OnMouseDown = new UnityEvent<Vector2>();
			archiveEvent=new UnityEvent<ArchiveSystem.ArchiveEventType>();
			RoleStateChangeEvent = (x)=> { };
			SceneStateChangeEvent = new UnityAction<SceneModel>((x)=>{});
			OnPlotWillExecuteEvent = (x,y) => false;
#pragma warning disable 612
			// PlotItemExecuteEvent = (x,y,z) => false;
#pragma warning restore 612
			// PlotExecuteOverEvent = (x, y) => { recursionCount = 0; };
			PlotRequireCheckOkEvent = (x,y)=> { };
			DayOverEvent = (x) => { Debug.Log("当日结束,新的时间是："+x); };
			OnNodeExecutedEvent+=LimitCheck;
			// OnGraphExecutedEvent = (x) => { recursionCount = 0; };
			OnPlotItemExecutedEvent = () => { };
			OnOperationExecutedEvent = new UnityEvent<GfuOperation>();
			_events = new Dictionary<string, UnityAction>();
			param_events = new Dictionary<string, UnityAction<object>>();
			return_events=new Dictionary<string, Func<object>>();
			return_parem_events=new Dictionary<string, Func<object, object>>();
		}
	}
}
