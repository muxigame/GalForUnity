//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotBlock.cs Created at 2022-09-30 19:54:38
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Nodes;
using GalForUnity.Graph.Nodes.Editor;
using GalForUnity.Graph.Nodes.Editor.Block.Config;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    [NodeEditor(typeof(GalPlotConfig))]
    public class PlotBlockEditorUxml : DraggableBlockEditor{
        private readonly NameDropdownField _nameField;
        private readonly TextField _said;

        public PlotBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock){
            var galPlotConfig = (GalPlotConfig) galBlock;
            styleSheets.Add(UxmlHandler.instance.plotBlockUss);
            content.Add(_nameField = new NameDropdownField());
            content.Add(_said = new TextField{
                label = "Said"
            });
            _nameField.dropdownField.CreateBinder(galPlotConfig.GetType().GetField(nameof(galPlotConfig.name)), galBlock);
            _said.CreateBinder(galPlotConfig.GetType().GetField(nameof(galPlotConfig.word)), galBlock);
            content.style.flexDirection = FlexDirection.Column;
            operationButton.style.display = DisplayStyle.Flex;
            operationButton.clickable = new Clickable(() =>
            {

            });
        }
    }
}