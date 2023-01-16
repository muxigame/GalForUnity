using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GalForUnity.Core{
    public static class GalSynchronizationContextUtility{
        public static SynchronizationContext UnitySynchronizationContext;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init(){
            UnitySynchronizationContext = SynchronizationContext.Current;
        }
        public static void AsyncStart(Func<bool> onComplied){
            UnitySynchronizationContext.Post(async x=> {
                while (!onComplied.Invoke()){
                    await Task.Yield();
                }
            },null);
        }
    }
}