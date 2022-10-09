//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  Timer.cs
//
//        Created by 半世癫(Roc) at 2021-08-18 14:27:19
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace MUX.Support{
    public class Timer
    {
        private static readonly Dictionary<object,float> timeKey=new Dictionary<object, float>();

        /// <summary>
        /// 通过反射类名来区别调用对象，以此进行的计时非常消耗性能，且只能对调用位置进行区分
        /// </summary>
        /// <param name="time">指定的秒数</param>
        /// <returns></returns>
        [Obsolete]
        public static bool TrueUntil_(int time){
            StackTrace stackTrace = new StackTrace();
            var runtimeMethodHandle = stackTrace.GetFrame(1).GetMethod().MethodHandle.Value.ToInt32();
            var fileLineNumber = stackTrace.GetFrame(1).GetFileLineNumber();
            var fileColumnNumber = stackTrace.GetFrame(1).GetFileColumnNumber();
            var obJ= new TimeKey(runtimeMethodHandle,fileLineNumber,fileColumnNumber);
            var ftime = Time.time;
            if (!timeKey.ContainsKey(obJ)) timeKey[obJ] = ftime;
            if (timeKey[obJ] + time >= ftime){
                timeKey[obJ] = ftime;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 直到指定的秒数后返回True
        /// </summary>
        /// <param name="obJ">区分是何对象调用的计时器，请传入this</param>
        /// <param name="time">指定的秒数</param>
        /// <returns></returns>
        public static bool TrueUntil(object obJ,int time){
            var ftime = Time.time;
            if (!timeKey.ContainsKey(obJ)) timeKey[obJ] = ftime;
            if (timeKey[obJ] + time >= ftime){
                timeKey[obJ] = ftime;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 直到指定的秒数之后返回True
        /// </summary>
        /// <param name="time">指定的秒数</param>
        /// <returns></returns>
        public bool TrueUntil(int time){
            var ftime = Time.time;
            if (!timeKey.ContainsKey(this)) timeKey[this] = ftime;
            if (timeKey[this] + time >= ftime){
                timeKey[this] = ftime;
                return true;
            }
            Timer.Until(1).AddCallBack(()=>{});
            return false;
            //获取调用此方法的方法类名
            
        }

        /// <summary>
        /// 直到指定的秒数后执行回调
        /// </summary>
        /// <param name="second">倒计时</param>
        /// <param name="loop">是否持续回调</param>
        /// <returns></returns>
        public static TimerCallBack Until(int second,bool loop=false){
            return new TimerCallBack(second,loop);
            //获取调用此方法的方法类名
        }

        /// <summary>
        /// 直到表达式为True时执行回调
        /// </summary>
        /// <param name="match">表达式</param>
        /// <param name="loop">是否持续回调</param>
        /// <returns></returns>
        public static TrueCallBack Until(Func<bool> match,bool loop=false){
            return new TrueCallBack(match,loop);
            //获取调用此方法的方法类名
        }
        private class TimeKey{
            private bool Equals(TimeKey other){
                return _handle == other._handle && _line == other._line && _row == other._row;
            }

            public override bool Equals(object obj){
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TimeKey) obj);
            }

            public override int GetHashCode(){
                unchecked{
                    var hashCode = _handle;
                    hashCode = (hashCode * 397) ^ _line;
                    hashCode = (hashCode * 397) ^ _row;
                    return hashCode;
                }
            }

            private readonly int _handle;
            private readonly int _line;
            private readonly int _row;

            public TimeKey(int handle,
                int line,
                int row){
                this._handle = handle;
                this._line = line;
                this._row = row;
            }
            
            // public int CompareTo(object obj){
            //    
            // }

            public static bool operator ==(TimeKey timeKey1,TimeKey timeKe2){
                return !(timeKe2 is null) && !(timeKey1 is null) && timeKey1._handle == timeKe2._handle && timeKey1._line == timeKe2._line && timeKey1._row == timeKe2._row;
            }

            public static bool operator !=(TimeKey timeKey1, TimeKey timeKe2){
                return !(timeKey1 == timeKe2);
            }
        }
        
        public class TimerCallBack{
            public TimerCallBack(float second,bool loop){
                new Thread(() => {
                    while (loop){
                        Thread.Sleep((int) (second * 1000));
                        if (_cancel.Invoke()){
                            return;
                        }
                        _callBack.Invoke();
                    }
                }).Start();
            }

            private Action _callBack;
            private Func<bool> _cancel;
            /// <summary>
            /// 添加回调
            /// </summary>
            /// <param name="callBack">回调函数</param>
            /// <returns></returns>
            public TimerCallBack AddCallBack(Action callBack){
                 _callBack += callBack;
                return this;
            }
            /// <summary>
            /// 当表达式为True时取消回调，终止计时器
            /// </summary>
            /// <param name="callBack">表达式</param>
            /// <returns></returns>
            public TimerCallBack CancelWhen(Func<bool> callBack){
                _cancel = callBack;
                return this;
            }
        }
        public class TrueCallBack{
            public TrueCallBack(Func<bool> match,bool loop){
                new Thread(() => {
                    while (loop){
                        Thread.Sleep(2);
                        if (match.Invoke()){
                            _callBack.Invoke();
                        }
                        if(_cancel.Invoke()) return;
                    }
                }).Start();
            }

            private Action _callBack;
            private Func<bool> _cancel;
            /// <summary>
            /// 添加回调
            /// </summary>
            /// <param name="callBack">回调函数</param>
            /// <returns></returns>
            public TrueCallBack AddCallBack(Action callBack){
                _callBack += callBack;
                return this;
            }
            /// <summary>
            /// 当表达式为True时取消回调，终止计时器
            /// </summary>
            /// <param name="callBack">表达式</param>
            /// <returns></returns>
            public TrueCallBack CancelWhen(Func<bool> callBack){
                _cancel = callBack;
                return this;
            }

        }
    }
}
