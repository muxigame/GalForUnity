//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ListenerArchiveLoaded.cs
//
//        Created by 半世癫(Roc) at 2021-12-12 16:51:30
//
//======================================================================

using GalForUnity.System;
using GalForUnity.System.Archive;
using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.Demo.Script{
    public class ListenerArchiveLoaded : MonoBehaviour{
        public ScrollRect archiveList;
        public Button archiveButton;
        void Awake(){
            ArchiveSystem.GetInstance().archiveEvent.AddListener((x) => {
                if (x == ArchiveSystem.ArchiveEventType.ArchiveLoadStart){
                    GfuRunOnMono.Update(() => {
                        archiveButton.gameObject.SetActive(true);
                        archiveList.gameObject.SetActive(false);
                    });
                }
            });
        }
    }
}
