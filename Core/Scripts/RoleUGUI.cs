//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  Role.cs at 2022-09-22 23:51:22
//
//======================================================================

using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.Core{
    public class RoleUGUI : MonoBehaviour, IRoleIO{
        
        public string roleName="Qiyue";
        public Image image;
        public AudioSource audioSource;
        public Animation animation;

        private void Reset(){
            var component = new GameObject("Component");
            var rectTransform = component.AddComponent<RectTransform>();
            rectTransform.anchorMin=Vector2.zero;
            rectTransform.anchorMax=Vector2.one;
            rectTransform.offsetMax=Vector2.zero;
            rectTransform.offsetMin=Vector2.zero;
            audioSource = component.AddComponent<AudioSource>();
            animation = component.AddComponent<Animation>();
            image = component.AddComponent<Image>();
            component.transform.parent = transform;
        }

        public void SetSprite(Sprite sprite){
            image.sprite = sprite;
        }

        public void SetPosition(Vector3 position){
            transform.position = position;
        }

        public void SetRotate(Vector3 rotate){
            transform.rotation=Quaternion.Euler(rotate); 
        }

        public void SetScale(Vector3 scale){
            transform.localScale = scale;
        }

        public void SetColor(Color color){
            image.color = color;
        }

        public void SetVoice(AudioClip audioClip){
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public void SetVisible(bool visible){
            gameObject.SetActive(visible);
        }

        public void SetAnimation(AnimationClip animationClip){
            animation.clip = animationClip;
            animation.Play();
        }
    }
}