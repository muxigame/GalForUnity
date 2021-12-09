//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArchiveSlot.cs
//
//        Created by 半世癫(Roc) at 2021-12-09 19:07:37
//
//======================================================================

using UnityEngine;
using UnityEngine.Serialization;

namespace GalForUnity.System.Archive.UI{
    public class ArchiveSlot : MonoBehaviour{
        [FormerlySerializedAs("ArchiveImage")] 
        public ArchiveImage archiveImage;
        public ArchiveInfo archiveInfo;
        public int index;

        internal static ArchiveSlot GetArchiveSlot(MonoBehaviour monoBehaviour){
            return monoBehaviour.GetComponentInParent<ArchiveSlot>();
        }

        private void Start(){
            SetPosition();
        }

        public void Init(ArchiveConfig archiveItem){
            archiveImage.ShowImage(archiveItem.ArchiveImage);
            archiveInfo.SetArchiveName(archiveItem.ArchiveFileName);
            archiveInfo.SetArchiveName(archiveItem.ArchiveTime);
        }

        public void SetPosition(){
            GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-index *200);
        }
        
    }
}
