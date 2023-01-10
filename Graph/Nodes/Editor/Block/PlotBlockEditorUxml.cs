//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotBlock.cs Created at 2022-09-30 19:54:38
//
//======================================================================

using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Nodes.Editor;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public class PlotBlockEditorUxml : DraggableBlockEditor{
        private NameDropdownField _nameField;
        private TextField _said;

        public PlotBlockEditorUxml(PlotNode plotNode,Config.IGalBlock galBlock):base(plotNode,galBlock){
            styleSheets.Add(UxmlHandler.instance.plotBlockUss);
            content.Add(_nameField = new NameDropdownField());
            content.Add(_said = new TextField {
                label = "Said"
            });
            content.style.flexDirection = FlexDirection.Column;
        }
        
    }
}