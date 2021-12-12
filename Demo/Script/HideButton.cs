//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  HideButton.cs
//
//        Created by 半世癫(Roc) at 2021-12-11 01:58:47
//
//======================================================================

using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.Demo.Script{
    public class HideButton : MonoBehaviour
    {
        
#pragma warning disable 0649
        public bool hideAwake = false;
        [SerializeField]
        private RectTransform rectTransform;
#pragma warning disable 0649
        void Awake(){
            if(TryGetComponent(out Button button)) button.onClick.AddListener(Hide);
            if(hideAwake) Hide();
        }

        public void Hide(){
            if (rectTransform) rectTransform.gameObject.SetActive(false);
        }
    }
}
