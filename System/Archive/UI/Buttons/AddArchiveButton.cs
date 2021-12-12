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

using UnityEngine;
using UnityEngine.UI;

namespace GalForUnity.System.Archive.UI.Buttons{
    public class AddArchiveButton : MonoBehaviour{
        
        private ArchiveSystem _archiveSystem;
        
        private RectTransform _rectTransform;

        public void Start(){
            if(!_archiveSystem) _archiveSystem=ArchiveSystem.GetInstance();
            _rectTransform = GetComponent<RectTransform>();
            if (!TryGetComponent(out Button button)) button=gameObject.AddComponent<Button>();
            if(_archiveSystem)button.onClick.AddListener(Save);
        }

        private void Update(){
            _rectTransform.anchoredPosition = new Vector2(0,(_archiveSystem.ArchiveCount) * -200);
            _rectTransform.SetAsLastSibling();
        }

        private void Save(){
            _archiveSystem.SaveAsync(_archiveSystem.ArchiveCount);
        }
    }
}
