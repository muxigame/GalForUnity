//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ICoreOutput.cs 2022-09-22 00:07:10
//
//======================================================================

using GalForUnity.Model;
using UnityEngine;
using UnityEngine.Video;

namespace GalForUnity.Core{
    public interface ICoreIO{
        public RoleModel GetRole(string roleName);
        
        public void SetName(string roleName);
        public void SetSay(string roleSaid);
        public void SetRole(string roleName, RoleModel roleModel);
        public void SetBackground(Sprite sprite);
        public void SetBackground(VideoClip videoClip);
        public void SetAudio(AudioClip audioClip);
    } 
    public interface IRoleIO{
        public void SetSprite(Sprite sprite);
        public void SetPosition(Vector3 position);
        public void SetRotate(Vector3 rotate);
        public void SetScale(Vector3 scale);
        public void SetColor(Color color);
        public void SetVoice(AudioClip audioClip);
        public void SetVisible(bool visible);
        public void SetAnimation(AnimationClip animationClip);
    }
}
