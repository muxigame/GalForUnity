//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalCore.cs at 2022-09-21 23:39:58
//
//======================================================================

using System.Collections.Generic;
using GalForUnity.Model;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace GalForUnity.Core{
    [RequireComponent(typeof(Canvas))]
    public class GalCoreUGUI : GalCore{
        public Dictionary<string, RoleModel> roleModels = new Dictionary<string, RoleModel>();
        public Text nameText;
        public Text sayText;
        public Canvas galCanvas;
        public AudioSource bgAudioSource;

        public VideoPlayer videoPlayer;
        public Image image;
        public RectTransform roleContent;
        public static GalCoreUGUI Instance => GalInstanceManager<GalCoreUGUI>.GetInstance();

        public override RoleModel GetRole(string roleName){
            return roleModels[roleName];
        }

        public override void SetName(string roleName){
            nameText.text = roleName;
        }

        public override void SetSay(string roleSaid){
            sayText.text = roleSaid;
        }

        public override void SetRole(string roleName, RoleModel roleModel){
            if(roleModels.ContainsKey(roleName)) Destroy(roleModels[roleName]);
            if (!roleModel&&roleModels.ContainsKey(roleName)){
                roleModels.Remove(roleName);
                return;
            }
            var instantiate = Instantiate(roleModel, roleContent);
            roleModels[roleName]=instantiate;
        }

        public override void SetBackground(Sprite sprite){
            videoPlayer.gameObject.SetActive(false);
            image.sprite = sprite;
        }

        public override void SetBackground(VideoClip videoClip){
            videoPlayer.gameObject.SetActive(true);
            if(!videoPlayer.targetTexture) videoPlayer.targetTexture=new RenderTexture(Screen.width,Screen.height,32);
            videoPlayer.clip = videoClip;
            videoPlayer.Play();
        }

        public override void SetAudio(AudioClip audioClip){
            bgAudioSource.clip = audioClip;
            bgAudioSource.Play();
        }
    }

    public class Role{
        public Vector2 position;
        public Sprite sprite;
    }
}