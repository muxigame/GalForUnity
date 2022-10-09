using System;
using System.Collections;
using GalForUnity.System;
using MUX.Mono;
using UnityEngine;

namespace MUX.Support{
    public class SmoothTransition : InstanceManager<SmoothTransition>
    {

        public void Linear(Action<float> action,float source,float target,float speed){
            GfuMonoProxy.GetMono().StartCoroutine(new LinearClass(action,source,target,speed).Linear());
        }
        public void Linear(Action<float> action,Action<float> callback,float source,float target,float speed){
            GfuMonoProxy.GetMono().StartCoroutine(new LinearClass(action,callback,source,target,speed).Linear());
        }
        public void Range(Action<float> action,float min,float max,float speed){
            GfuMonoProxy.GetMono().StartCoroutine(new RangeClass(action,min,max,speed).Linear());
        }
        public void Range(Action<float> action,Func<bool> canstop,float min,float max,float speed){
            GfuMonoProxy.GetMono().StartCoroutine(new RangeClass(action,canstop,min,max,speed).Linear());
        }
        public void StopAll(){
            GfuMonoProxy.GetMono().StopAllCoroutines();
        }
        private class LinearClass{
            private float _source;
            private readonly float _target;
            private readonly float _speed;
            private readonly Action<float> _action;
            private readonly Action<float> _callback;
            public LinearClass(Action<float> action,float source,float target,float speed){
                this._source = source;
                this._target = target;
                this._speed = speed;
                this._action = action;
            }
            public LinearClass(Action<float> action,Action<float> callback,float source,float target,float speed){
                this._source = source;
                this._target = target;
                this._speed = speed;
                this._action = action;
                this._callback = callback;
            }
            public IEnumerator Linear() {
                while (Math.Abs(_source - _target) > 0.00001f) {
                    _action(_source = Mathf.MoveTowards(_source, _target, Time.deltaTime * _speed));
                    yield return null;
                }
                if (_callback != null)
                {
                    _callback(_source);
                }
            }
        }
        private class RangeClass{
            private readonly float _min;
            private readonly float _max;
            private float _current;
            private readonly float _speed;
            private bool _invert;
            private readonly Action<float> _action;
            private readonly Func<bool> _canstop;
            public RangeClass(Action<float> action,float min,float max,float speed){
                this._min = min;
                this._max = max;
                this._speed = speed;
                this._action = action;
            }
            public RangeClass(Action<float> action,Func<bool> canstop,float min,float max,float speed){
                this._min = min;
                this._max = max;
                this._speed = speed;
                this._action = action;
                this._canstop = canstop;
            }
            public IEnumerator Linear() {
                // Debug.Log(_min);
                // Debug.Log(_max);
                while (null==_canstop||!_canstop.Invoke()) {
                    if (_current >= _max){
                        _invert = true;
                    }
                    if (_current <= _min){
                        _invert = false;
                    }
                    if (_invert){
                        _action(_current = Mathf.MoveTowards(_current, _min, Time.deltaTime * _speed));
                    }
                    else{
                        _action(_current = Mathf.MoveTowards(_current, _max, Time.deltaTime * _speed));
                    }
                    yield return null;
                }
            }
        }
    }
}
