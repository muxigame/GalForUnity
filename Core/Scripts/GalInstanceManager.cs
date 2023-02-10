

using UnityEngine;

namespace GalForUnity.Core{
    public class GalInstanceManager<T> where T : class, new(){
        private static volatile T _instance;

        public static T GetInstance(){
            if (_instance == null){
                lock (typeof(T)){
                    if (_instance == null){
                        if (typeof(T).IsSubclassOf(typeof(Object))){
                            if (_instance == null) _instance = Object.FindObjectOfType(typeof(T)) as T;
                            if (_instance == null) _instance = new GameObject(typeof(T).Name).AddComponent(typeof(T)) as T;
                        } else if (_instance == null){
                            _instance = new T();
                        }
                    }
                }
            }
            return _instance;
        }
        public static void ClearInstance(){ _instance = null; }
    }
}