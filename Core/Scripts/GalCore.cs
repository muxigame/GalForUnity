//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalCore.cs  at 2022-09-22 23:24:00
//
//======================================================================

using System.Threading;
using GalForUnity.Model;
using UnityEngine;
using UnityEngine.Video;

namespace GalForUnity.Core{
    public abstract class GalCore : MonoBehaviour , ICoreIO{
        public GalCore(){
            ActiveCore = this;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init(){
            // SynchronizationContext.Current;
        }
        public static GalCore ActiveCore;
        public abstract RoleModel GetRole(string roleName);
        public abstract void SetName(string roleName);
        public abstract void SetSay(string roleSaid);
        public abstract void SetRole(string roleName,  RoleModel roleModel);
        public abstract void SetBackground(Sprite sprite);
        public abstract void SetBackground(VideoClip videoClip);
        public abstract void SetAudio(AudioClip audioClip);
    }
}
