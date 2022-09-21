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
        public void SetRole(RoleModel roleModel);
        public void SetBackground(Sprite sprite);
        public void SetBackground(VideoClip videoClip);
        public void SetAudio(AudioClip audioClip);
    } 
    public interface IRoleIO{
        public void SetSprite(string roleName,Sprite sprite);
        public void SetPosition(string roleName,Vector3 position);
        public void SetRotate(string roleName,Vector3 rotate);
        public void SetScale(string roleName,Vector3 scale);
        public void SetColor(string roleName, Color color);
        public void SetVoice(string roleName, AudioClip audioClip);
        public void SetVisible(string roleName, bool visible);
        public void SetAnimation(string roleName, Animation animation);
    }
}
