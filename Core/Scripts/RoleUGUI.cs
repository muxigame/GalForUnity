//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  Role.cs at 2022-09-22 23:51:22
//
//======================================================================

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.Core{
    public class RoleUGUI : MonoBehaviour, IRoleIO{
        
        public RoleAssets roleAssets;
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

        public void SetPose(string poseName, string anchorName, string faceName)
        {
            var pose = roleAssets.pose.FirstOrDefault(x => x.name == poseName);
            if (pose == null) return;
            if (pose is not SpritePose spritePose) return;
            var bindingPoint = spritePose.bindingPoints.FirstOrDefault(x => x.name == anchorName);
            if(bindingPoint==null) return;
            var face = bindingPoint.spritePoseItems.FirstOrDefault(x => x.name == faceName);
            image.sprite = spritePose.sprite;
            
        }

        public void SetPosition(Unit xUnit, Unit yUnit, Vector2 position){
            switch (xUnit)
            {
                case Unit.Pixel:
                    transform.localPosition = position;
                    break;
                case Unit.Percentage:
                    transform.localPosition = new Vector3(position.x * Screen.width / 100f,
                        position.y * Screen.height / 100f, 0);
                    break;
                case Unit.Decimal:
                    transform.localPosition = new Vector3(position.x * Screen.width,
                        position.y * Screen.height, 0);
                    break;
                // case null:
                //     if(position!=null)
                //         throw new ArgumentOutOfRangeException(nameof(unit), null, null);
                //     break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(xUnit), xUnit, null);
            }
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