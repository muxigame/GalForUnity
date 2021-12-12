//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  OverrideArchiveButton.cs
//
//        Created by 半世癫(Roc) at 2021-12-10 14:12:08
//
//======================================================================

using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.System.Archive.UI.Buttons{
    public class OverrideArchiveButton : MonoBehaviour{
        void Start(){
            if (!TryGetComponent(out UnityEngine.UI.Button button)) button=gameObject.AddComponent<Button>();
            button.onClick.AddListener(OverrideArchive);
        }

        private void OverrideArchive(){
            ArchiveSystem.GetInstance().SaveAsync(ArchiveSlot.GetArchiveSlot(this).Index);
        }
        
        
    }
}
