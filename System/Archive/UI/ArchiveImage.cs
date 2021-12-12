using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.System.Archive.UI{
    public class ArchiveImage : MonoBehaviour{
        
#pragma warning disable 649
        [SerializeField]
        private ArchiveSlot archiveSlot;
        private Image _image;
        private RectTransform _rectTransform;
#pragma warning restore 649

        private void Awake(){
            _image=GetComponent<Image>();
            _rectTransform=GetComponent<RectTransform>();
        }

        public void ShowImage(Texture2D texture){
            if (!_image || !_rectTransform) Awake();
            _image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            SetImagePosition();
        }
    
        public void Start(){
            SetImagePosition();
        }

        private void SetImagePosition(){
            if(!_image ||!_image.sprite) return;
            var sprite = _image.sprite;
            var textureHeight = sprite.bounds.size.x /sprite.bounds.size.y;
            var sizeDelta = _rectTransform.sizeDelta;
            var width = _rectTransform.rect.height * textureHeight;
            sizeDelta=new Vector2(width,sizeDelta.y);
            _rectTransform.sizeDelta = sizeDelta;
            _rectTransform.anchoredPosition=new Vector2(width /2f,_rectTransform.anchoredPosition.y);
            if(archiveSlot) archiveSlot.archiveInfo.SetInfoPosition();
            else ArchiveSlot.GetArchiveSlot(this).archiveInfo.SetInfoPosition();
        }
    }
}
