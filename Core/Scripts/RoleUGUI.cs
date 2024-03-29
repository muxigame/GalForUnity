

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.Core.External;
using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.Core{
    public class RoleUGUI : MonoBehaviour, IRoleIO{
        
        public RoleAssets roleAssets;
        public Image poseImage;
        private Image[] _faceImage;
        public AudioSource audioSource;
        // public Animation animation;

        private void Reset(){
            var component = new GameObject("Component");
            var rectTransform = component.AddComponent<RectTransform>();
            rectTransform.anchorMin=Vector2.zero;
            rectTransform.anchorMax=Vector2.one;
            rectTransform.offsetMax=Vector2.zero;
            rectTransform.offsetMin=Vector2.zero;
            audioSource = component.AddComponent<AudioSource>();
            // animation = component.AddComponent<Animation>();
            component.transform.SetParent(transform);
        }

        public static RoleUGUI Create(RoleAssets roleAssets){
            var galCoreUGUI = GalCore.ActiveCore as GalCoreUGUI;
            if (galCoreUGUI == null){
                return null;
            }
            var roleGameObject = new GameObject(roleAssets.roleName);
            var roleUGUI = roleGameObject.AddComponent<RoleUGUI>();
            roleUGUI.roleAssets = roleAssets;
            roleUGUI._faceImage = new Image[roleAssets.pose.Where(x => x is SpritePose).Cast<SpritePose>().Max(x => x.bindingPoints.Count)];
            roleUGUI.poseImage = roleGameObject.AddComponent<Image>();
            roleGameObject.transform.SetParent(galCoreUGUI.interactionLayer);
            roleGameObject.transform.localPosition=new Vector3(0,0);
            return roleUGUI;
        }

        private void ShowFace(SpritePoseItem spritePoseItem,int index,Vector2 position){
            if (_faceImage[index] == null){
                var o = new GameObject(string.Concat("Anchor:",index));
                o.AddComponent<RectTransform>();
                _faceImage[index] = o.AddComponent<Image>();
                o.transform.SetParent(transform);
            }
            var transformLocalPosition = _faceImage[index].rectTransform.anchoredPosition;
            _faceImage[index].rectTransform.SetAnchor(AnchorPresets.BottomLeft);
            var rect = poseImage.rectTransform.rect;
            transformLocalPosition.x = rect.width * position.x;
            transformLocalPosition.y = rect.height * position.y;
            _faceImage[index].rectTransform.anchoredPosition = transformLocalPosition;
            _faceImage[index].sprite = spritePoseItem.sprite;
            _faceImage[index].SetNativeSize();
        }

        private void ClearFace(){
            foreach (var image in _faceImage){
                if (image != null) image.sprite = null;
            }
        }
        
        public void SetPose(string poseName, string anchorName, string faceName)
        {
            var pose = roleAssets.pose.FirstOrDefault(x => x.name == poseName);
            if (pose == null) return;
            if (!(pose is SpritePose spritePose)) return;
            if (spritePose.sprite != poseImage.sprite) ClearFace();
            var bindingPoint = spritePose.bindingPoints.FirstOrDefault(x => x.name == anchorName);
            if (bindingPoint == null) return;
            var face = bindingPoint.spritePoseItems.FirstOrDefault(x => x.name == faceName);
            poseImage.sprite = spritePose.sprite;
            poseImage.SetNativeSize();
            ShowFace(face, spritePose.bindingPoints.IndexOf(bindingPoint), bindingPoint.point);
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
            poseImage.color = color;
        }

        public void SetVoice(AudioClip audioClip){
            audioSource.clip = audioClip;
            audioSource.Play();
        }

        public void SetVisible(bool visible){
            gameObject.SetActive(visible);
        }

        public void SetAnimation(AnimationClip animationClip){
            GetComponent<Animation>().clip = animationClip;
            GetComponent<Animation>().Play();
        }
    }
}