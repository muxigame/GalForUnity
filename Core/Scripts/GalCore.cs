//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalCore.cs  at 2022-09-22 23:24:00
//
//======================================================================

using UnityEngine;
using UnityEngine.Video;

namespace GalForUnity.Core{
    public abstract class GalCore : MonoBehaviour, ICoreIO{
        public static GalCore ActiveCore;
        public VideoPlayer mainVideoPlayer;
        public AudioSource mainAudioSource;

        public GalCore(){ ActiveCore = this; }

        public abstract IRoleIO GetRole(string roleName);
        public abstract void SetName(string roleName);
        public abstract void SetSay(string roleSaid);
        public abstract void SetRole(string roleName, IRoleIO roleModel);
        public abstract void SetBackground(Sprite sprite);
        public abstract void SetBackground(VideoClip videoClip);
        public abstract void SetAudio(AudioClip audioClip);
    }
}