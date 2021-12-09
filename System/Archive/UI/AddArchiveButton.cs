//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  AddArchiveButton.cs
//
//        Created by 半世癫(Roc) at 2021-12-09 23:31:28
//
//======================================================================

using System;
using UnityEngine;

namespace GalForUnity.System.Archive.UI{
    public class AddArchiveButton : MonoBehaviour{
        [SerializeField]
        private ArchiveSystem archiveSystem;
        
        private RectTransform _rectTransform;

        public void Start(){
            if(!archiveSystem) archiveSystem=ArchiveSystem.GetInstance();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update(){
            _rectTransform.anchoredPosition = new Vector2(0,(archiveSystem._archiveCount) * -200);
            _rectTransform.SetAsLastSibling();
        }
    }
}
