//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotBlock.cs Created at 2022-09-30 19:54:38
//
//======================================================================

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public class PlotBlockUxml : DraggableBlock{
        private NameDropdownField _nameField;
        private TextField _said;

        public PlotBlockUxml(){
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/GalForUnity/Graph/Block/PlotBlock.uss");
            styleSheets.Add(styleSheet);
            content.Add(_nameField = new NameDropdownField());
            content.Add(_said = new TextField {
                label = "Said"
            });
            content.style.flexDirection = FlexDirection.Column;
        }

        public class PlotBlockUxmlFactory : UxmlFactory<PlotBlockUxml, UxmlTraits>{
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc){ return new PlotBlockUxml(); }
        }
    }
}