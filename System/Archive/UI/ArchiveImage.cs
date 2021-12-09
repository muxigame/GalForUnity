using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.System.Archive.UI{
    public class ArchiveImage : MonoBehaviour{
        public static int Count = 0;
    
        public void ShowImage(Texture2D texture){
            transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            SetImagePosition();
        }
    
        public void Start(){
            SetImagePosition();
        }

        private void SetImagePosition(){
            var rectTransform = GetComponent<RectTransform>();
            var image = rectTransform.GetComponent<Image>();
            if(!image ||!image.sprite) return;
            var sprite = image.sprite;
            var textureHeight = sprite.texture.width /sprite.texture.height;
            var sizeDelta = rectTransform.sizeDelta;
            var width = rectTransform.rect.height * textureHeight;
            sizeDelta=new Vector2(width,sizeDelta.y);
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchoredPosition=new Vector2(width /2f,rectTransform.anchoredPosition.y);
            ArchiveSlot.GetArchiveSlot(this).archiveInfo.SetInfoPosition();
        }
    }
}
