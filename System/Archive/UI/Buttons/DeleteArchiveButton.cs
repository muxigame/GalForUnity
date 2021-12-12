//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DeleteArchiveButton.cs
//
//        Created by 半世癫(Roc) at 2021-12-10 14:19:57
//
//======================================================================

using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.System.Archive.UI.Buttons{
    public class DeleteArchiveButton : MonoBehaviour{
        void Start(){
            if (!TryGetComponent(out Button button)) button=gameObject.AddComponent<Button>();
            button.onClick.AddListener(DeleteArchive);
        }

        private void DeleteArchive(){
            ArchiveSystem.GetInstance().Delete(ArchiveSlot.GetArchiveSlot(this).Index);
        }

    }
}
