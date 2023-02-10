//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalCore.cs at 2022-09-21 23:39:58
//
//======================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace GalForUnity.Core{
    [RequireComponent(typeof(Canvas))]
    public class GalCoreUGUI : GalCore{
        public Dictionary<string, RoleUGUI> roleModels = new Dictionary<string, RoleUGUI>();
        public Text nameText;
        public Text sayText;
        public Canvas galCanvas;
        public Image image;
        public RectTransform interactionLayer;
        public RectTransform backgroundLayer;
        public RectTransform uiLayer;
        public static GalCoreUGUI Instance => GalInstanceManager<GalCoreUGUI>.GetInstance();

        private void Awake(){
            ActiveCore = this;
        }

        public override IRoleIO GetRole(string roleName){
            if (!roleModels.ContainsKey(roleName)) return RoleUGUI.Create(RoleDB.Instance[roleName]);
            return roleModels[roleName];
        }

        public override void SetName(string roleName){
            nameText.text = roleName;
        }

        public override void SetSay(string roleSaid){
            sayText.text = roleSaid;
        }

        public override void SetRole(string roleName, IRoleIO roleModel){
            // if(roleModels.ContainsKey(roleName)) Destroy(roleModels[roleName]);
            // if (!roleModel&&roleModels.ContainsKey(roleName)){
            //     roleModels.Remove(roleName);
            //     return;
            // }
            // var instantiate = Instantiate(roleModel, roleContent);
            // roleModels[roleName]=instantiate;
        }

        public override void SetBackground(Sprite sprite){
            mainVideoPlayer.gameObject.SetActive(false);
            image.sprite = sprite;
        }

        public override void SetBackground(VideoClip videoClip){
            mainVideoPlayer.gameObject.SetActive(true);
            // galVideoConfig.Process(videoPlayer);
            if(!mainVideoPlayer.targetTexture) mainVideoPlayer.targetTexture=new RenderTexture(Screen.width,Screen.height,32);
            mainVideoPlayer.clip = videoClip;
            mainVideoPlayer.Play();
        }

        public override void SetAudio(AudioClip audioClip){
            // galAudioConfig.Process(bgAudioSource);
            mainAudioSource.clip = audioClip;
            mainAudioSource.Play();
        }
    }

    public class Role{
        public Vector2 position;
        public Sprite sprite;
    }
}