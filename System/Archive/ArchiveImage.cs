using UnityEngine;
using UnityEngine.UI;

public class ArchiveImage : MonoBehaviour{
    public static int Count = 0;
    
    public void ShowImage(Texture2D texture){
        transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
    
    public void Start(){
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMin=new Vector2(0,rectTransform.offsetMin.y);
        rectTransform.offsetMax=new Vector2(0,rectTransform.offsetMax.y);
        var rectTransformParent = (RectTransform) rectTransform.parent;
        rectTransformParent.sizeDelta=new Vector2(rectTransformParent.sizeDelta.x,-rectTransform.rect.y*++Count);
        rectTransform.anchoredPosition=new Vector2(rectTransform.anchoredPosition.x,rectTransform.rect.y*(Count-1));
    }
}
