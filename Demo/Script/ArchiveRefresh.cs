//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArchiveRefresh.cs
//
//        Created by 半世癫(Roc) at 2021-12-10 14:44:49
//
//======================================================================

using GalForUnity.System;
using GalForUnity.System.Archive;
using GalForUnity.System.Archive.UI.Buttons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GalForUnity.Demo.Script{
    public class ArchiveRefresh : MonoBehaviour{
        [FormerlySerializedAs("ArchiveButton")] 
        public AddArchiveButton archiveButton;
        void Awake(){
            ArchiveSystem.GetInstance().archiveEvent.AddListener(RefreshButtonAndArchiveSlot);
        }

        //由刷新完成事件调用和保存完成事件调用
        void RefreshButtonAndArchiveSlot(ArchiveSystem.ArchiveEventType archiveEventType){
            if(archiveEventType == ArchiveSystem.ArchiveEventType.RefreshOver)
                Instantiate(archiveButton, ArchiveSystem.GetInstance().scrollRect.content, false);
            else if (archiveEventType == ArchiveSystem.ArchiveEventType.SaveOver) LoadConfig(Refresh);
        }
        

        //由隐藏按钮,保存存档和游戏初次打开时调用
        public void LoadConfig(UnityAction action=null){
            ArchiveSystem.GetInstance().ReadArchiveConfigAsync(action);
        }
        
        //由存档按钮调用
        public void Refresh(){
            GfuRunOnMono.LateUpdate(ArchiveSystem.GetInstance().RefreshAll);
        }
        
    }
}
