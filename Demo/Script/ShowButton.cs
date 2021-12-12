//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ShowButton.cs
//
//        Created by 半世癫(Roc) at 2021-12-11 01:55:30
//
//======================================================================

using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.Demo.Script{
    public class ShowButton : MonoBehaviour{
#pragma warning disable 0649
        [SerializeField]
        private RectTransform rectTransform;
#pragma warning disable 0649
        void Start(){
            if(TryGetComponent(out Button button)) button.onClick.AddListener(Show);
        }

        public void Show(){
            if (rectTransform) rectTransform.gameObject.SetActive(true);
        }
    }
}
