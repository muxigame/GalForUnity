//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalCore.cs at 2022-09-21 23:39:58
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Model;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace GalForUnity.Core{
    public class GalCore : MonoBehaviour,ICoreIO{
        public static GalCore Instance => GalInstanceManager<GalCore>.GetInstance();
        
        public Dictionary<string,RoleModel> roleModels=new Dictionary<string, RoleModel>();
        public Text nameText;
        public Text sayText;
        public Canvas galCanvas;
        public AudioSource bgAudioSource;

        public RectTransform backgroundContent;
        public RectTransform roleContent;
        private void Awake(){
            backgroundContent = new RectTransform {
                anchorMin = Vector2.zero, anchorMax = Vector2.one, offsetMax = Vector2.zero, offsetMin = Vector2.zero
            };
            roleContent=new RectTransform() {
                anchorMin = Vector2.zero, anchorMax = Vector2.one, offsetMax = Vector2.zero, offsetMin = Vector2.zero
            };
        }

        public RoleModel GetRole(string roleName){
            return roleModels[roleName];
        }

        public void SetName(string roleName){
            nameText.text = roleName;
        }

        public void SetSay(string roleSaid){
            sayText.text = roleSaid;
        }

        public void SetRole(RoleModel roleModel){
            var instantiate = Instantiate(roleModel, roleContent);
            roleModels.Add(roleModel.Name,instantiate);
        }

        public void SetBackground(Sprite sprite){
            // backgroundContent.
        }

        public void SetBackground(VideoClip videoClip){
            
        }

        public void SetAudio(AudioClip audioClip){
            
        }
    }

    public class Role{
        public Sprite sprite;
        public Vector2 position;
    }
}