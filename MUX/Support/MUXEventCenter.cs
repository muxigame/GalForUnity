// using System;
// using System.Collections.Generic;
// using _Project.Script;
// using UnityEngine.Events;
//
// namespace MUX.Support {
//     /// <summary>
// 	/// CopyRight © MUXI Studio 
// 	/// Author Roc
// 	/// 事件中心类，通过此类可以监听指定事件或者发送指定事件的类可以使用GetInstance()获取对象实例，类似于单例，但是对象本身依旧是可New的
// 	/// 类设计思路是观察者模式，通过委托来实现解耦
//     /// 使用此类需要先添加监听事件，再发送事件，事件分为带参事件和不带参事件，他们是完全隔离的，各自响应对应的事件
//     /// </summary>
//     public class EventCenter : InstanceManager<EventCenter> {
//
//
//         private Dictionary<Enum, UnityAction> _events;//不带参委托
//         private Dictionary<Enum, UnityAction<object>> param_events;//带参委托
//
//
//         public EventCenter() {
//             _events = new Dictionary<Enum, UnityAction>();
//             param_events = new Dictionary<Enum, UnityAction<object>>();
//         }
//
//         /// <summary>
//         /// 向事件中心添加监听
//         /// </summary>
//         /// <param name="key">事件名称</param>
//         /// <param name="function">事件回调</param>
//         public void AddEventListening(Enum key, UnityAction function) {//添加回调的方法
//             if (key == null || function == null) {//如果空值空键
//                 throw new NullReferenceException();
//             }
//             if (_events.ContainsKey(key)) {//如果Key存在
//                 UnityAction action;
//                 _events.TryGetValue(key, out action);//取出回调
//                 if (action != null) {//如果回调不为空
//                     _events.Remove(key);//移除原先回调
//                     _events.Add(key, action += function);//重新添加回调
//                 } else {
//                     throw new EventNotFoundExecption();
//                 }
//             } else {
//                 _events.Add(key, function);
//             }
//         }
//         /// <summary>
//         /// 向事件中心添加监听
//         /// </summary>
//         /// <param name="key">事件名称</param>
//         /// <param name="function">事件回调<事件参数></param>
//         public void AddEventListeningWithParam(Enum key, UnityAction<object> function) {//添加回调的方法
//             if (key == null || function == null) {//如果空值空键
//                 throw new NullReferenceException();
//             }
//             if (param_events.ContainsKey(key)) {//如果Key存在
//                 UnityAction<object> action;//带参委托
//                 param_events.TryGetValue(key, out action);//取出带参回调
//                 if (action != null) {//如果带参回调不为空
//                     param_events.Remove(key);//移除原先带参回调
//                     param_events.Add(key, action += function);//重新添加带参回调
//                 } else {
//                     throw new EventNotFoundExecption();
//                 }
//             } else {
//                 param_events.Add(key, function);//添加带参回调
//             }
//         }
//
//         /// <summary>
//         /// 向事件中心发送事件，并回调监听对象
//         /// </summary>
//         /// <param name="_event">事件名称</param>
//         public void SendEvent(Enum _event) {
//             if (_event == null) {//如果空键
//                 throw new NullReferenceException();
//             }
//             if (_events.ContainsKey(_event)) {//如果key存在
//                 UnityAction action;
//                 _events.TryGetValue(_event, out action);//尝试获得无参回调
//                 if (action != null) {
//                     action.Invoke();//回调
//                 } else {
//                     throw new EventNotFoundExecption();
//                 }
//             }
//         }
//         /// <summary>
//         /// 向事件中心发送事件，并回调监听对象
//         /// </summary>
//         /// <param name="_event">事件名称</param>
//         /// <param name="param">事件携带的参数</param>
//         public void SendEventWithParam(Enum _event, object param) {
//             if (_event == null || param == null) {//如果空键
//                 throw new NullReferenceException();
//             }
//             if (param_events.ContainsKey(_event)) {//如果key存在
//                 UnityAction<object> action;
//                 param_events.TryGetValue(_event, out action);//尝试获得带参回调
//                 if (action != null) {
//                     action.Invoke(param);//回调
//                 } else {
//                     throw new EventNotFoundExecption();
//                 }
//             }
//         }
//         /// <summary>
//         /// 向事件中心取消注册的监听
//         /// </summary>
//         /// <param name="key">要取消的事件</param>
//         public void CancelRegister(Enum key) {//移除监听
//             if (key == null) {//如果空键
//                 throw new NullReferenceException();
//             }
//             if (_events.ContainsKey(key)) {
//                 _events.Remove(key);
//             }
//             if (param_events.ContainsKey(key)) {
//                 param_events.Remove(key);
//             }
//
//         }
//         /// <summary>
//         /// 清除所有事件回调
//         /// </summary>
//         public void ClearEventCallBack() {
//             _events.Clear();
//             param_events.Clear();// 清空所有注册的事件
//         }
//
//         #region IDisposable Support
//         private bool _disposedValue = false; // 要检测冗余调用
//         /// <summary>
//         /// 清理此对象
//         /// </summary>
//         /// <param name="disposing"></param>
//         protected sealed override void Dispose(bool disposing) {
//             base.Dispose(disposing);
//             if (!_disposedValue) {
//                 if (disposing) {
//                     ClearEventCallBack();
//                 }
//
//                 // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
//                 // TODO: 将大型字段设置为 null。
//
//                 _disposedValue = true;
//             }
//         }
//         #endregion
//
//         public class EventNotFoundExecption : Exception { }
//
//     }
//
// }