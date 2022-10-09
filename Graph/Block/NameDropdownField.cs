//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  NameDropdownField.cs at 2022-10-10 00:22:24
//
//======================================================================

using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public class NameDropdownField : VisualElement{
        public DropdownField dropdownField;
        public NameDropdownField(){
            Add(dropdownField=new DropdownField() {
                label = "Name"
            });
        }
    }
}
