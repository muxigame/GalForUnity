

using GalForUnity.Core;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block{
    public class NameDropdownField : VisualElement{
        public DropdownField dropdownField;
        public string Value => dropdownField.value;
        public NameDropdownField()
        {
            Add(dropdownField=new DropdownField("Name",RoleDB.Keys(),0) {
                // label = "",
            });
        }
    }
}
