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
    /// <summary>
    /// 存档槽，存档的UI显示类，包含存档信息和存档图片
    /// </summary>
    public class ArchiveSlot : MonoBehaviour{
        [FormerlySerializedAs("ArchiveImage")] 
        public ArchiveImage archiveImage;
        public ArchiveInfo archiveInfo;

        private ArchiveConfig _archiveConfig;
        public int Index=>_archiveConfig.ArchiveIndex;

        internal static ArchiveSlot GetArchiveSlot(MonoBehaviour monoBehaviour){
            if (monoBehaviour.TryGetComponent(out ArchiveSlot archiveSlot)) return archiveSlot;
            return monoBehaviour.GetComponentInParent<ArchiveSlot>();
        }

        private void Start(){
            SetPosition();
        }

        public void Init(ArchiveConfig archiveConfig){
            _archiveConfig = archiveConfig;
            _archiveConfig.ArchiveSlot = this;
            archiveImage.ShowImage(archiveConfig.ArchiveImage);
            archiveInfo.SetArchiveName(archiveConfig.ArchiveFileName);
            archiveInfo.SetArchiveTime(archiveConfig.ArchiveTime);
        }

        public void SetPosition(){
            GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-Index *200);
        }

        public void Load(){
            _archiveConfig.ArchiveItem.Load(_archiveConfig);
        }
        
    }
}
