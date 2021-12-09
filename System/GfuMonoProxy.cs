using System;
using System.Collections.Generic;
using MUX.Mono;

namespace GalForUnity.System{
    public class GfuMonoProxy : GfuMonoInstanceManager<GfuMonoProxy>{
        private static GfuMonoProxy _gfuMonoProxy;
    
        private static Dictionary<MethodType,Action> actions=new Dictionary<MethodType,Action>();

        public static void Update(Action action){
            if (actions.ContainsKey(MethodType.Update)){
                actions[MethodType.Update]+=action;//修改注册的方法
            }
            else{
                actions.Add(MethodType.Update,action);//增加注册的方法
            }
        
        }
        void Start(){
            _gfuMonoProxy = this;
        }

        // Update is called once per frame
        void Update(){
            Action action;
            if (actions.TryGetValue(MethodType.Update,out action)){
                action.Invoke();
            }
        }

        public static void CancelRegister(Action action,MethodType methodType){
            if (actions.TryGetValue(methodType, out Action _)){
                // ReSharper disable once DelegateSubtraction
                if (action != null) actions[methodType] -= action;
            }
        }
        public static GfuMonoProxy GetMono(){
            return _gfuMonoProxy ? _gfuMonoProxy : throw new NullReferenceException("Mono doesn't Ready!");
        }
        
    }

}
