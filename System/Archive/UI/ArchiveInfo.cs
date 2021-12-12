//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArchiveInfo.cs
//
//        Created by 半世癫(Roc) at 2021-12-09 19:23:20
//
//======================================================================

using System;
using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.System.Archive.UI{
    public class ArchiveInfo : MonoBehaviour{
        
#pragma warning disable 0649
        [SerializeField]
        private ArchiveSlot archiveSlot;
        [SerializeField]
        private Text archiveName;
        [SerializeField]
        private Text archiveTime;

        private RectTransform _rectTransform;
        private RectTransform _imageRectTransform;
        
#pragma warning disable 0649

        private void Awake(){
            if(!archiveSlot) archiveSlot= ArchiveSlot.GetArchiveSlot(this);
            if (!_imageRectTransform) _imageRectTransform = archiveSlot.archiveImage.GetComponent<RectTransform>();
            if(!_rectTransform) _rectTransform = GetComponent<RectTransform>();
        }

        void Start(){
            SetInfoPosition();
        }

        internal void SetInfoPosition(){
            if (!_imageRectTransform) Awake();
            var anchoredPositionX = _imageRectTransform.rect.width;
            _rectTransform.anchoredPosition=new Vector2(anchoredPositionX,_rectTransform.anchoredPosition.y);
        }

        public ArchiveInfo SetArchiveName(string str){
            archiveName.text = str;
            return this;
        }
        public ArchiveInfo SetArchiveTime(string str){
            archiveTime.text = str;
            return this;
        }
        
    }
}
