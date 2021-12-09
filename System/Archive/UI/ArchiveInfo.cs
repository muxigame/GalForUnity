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

using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.System.Archive.UI{
    public class ArchiveInfo : MonoBehaviour{
        [SerializeField]
        private Text archiveName;
        [SerializeField]
        private Text archiveTime;
        void Start(){
            SetInfoPosition();
        }

        internal void SetInfoPosition(){
            var archiveSlot = ArchiveSlot.GetArchiveSlot(this);
            var anchoredPositionX = archiveSlot.archiveImage.GetComponent<RectTransform>().rect.width;
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition=new Vector2(anchoredPositionX,rectTransform.anchoredPosition.y);
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
