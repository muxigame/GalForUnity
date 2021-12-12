//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  LoadArchiveButton.cs
//
//        Created by 半世癫(Roc) at 2021-12-10 15:19:08
//
//======================================================================

using UnityEngine;

namespace GalForUnity.System.Archive.UI.Buttons{
    public class LoadArchiveButton : MonoBehaviour{
        void Start(){
            if (!TryGetComponent(out UnityEngine.UI.Button button)) button = gameObject.AddComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(LoadArchive);
        }

        private void LoadArchive(){
            ArchiveSlot.GetArchiveSlot(this).Load();
        }
    }
}
