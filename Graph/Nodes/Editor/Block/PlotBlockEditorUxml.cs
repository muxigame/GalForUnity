//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotBlock.cs Created at 2022-09-30 19:54:38
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Core;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Nodes;
using GalForUnity.Graph.Nodes.Editor;
using GalForUnity.Graph.Nodes.Editor.Block.Config;
using GalForUnity.Graph.Nodes.Editor.Block.Config.ConfigAdditions;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block
{
    [NodeEditor(typeof(GalPlotConfig))]
    public class PlotBlockEditorUxml : DraggableBlockEditor
    {
        private readonly GalPlotConfig _galPlotConfig;
        private readonly NameDropdownField _nameField;
        private readonly TextField _said;

        public PlotBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock)
        {
            _galPlotConfig = (GalPlotConfig)galBlock;
            styleSheets.Add(UxmlHandler.instance.plotBlockUss);
            content.Add(_nameField = new NameDropdownField());
            content.Add(_said = new TextField
            {
                label = "Said"
            });
            _nameField.dropdownField.labelElement.AddToClassList("gal-label");
            _said.labelElement.AddToClassList("gal-label");
            _nameField.dropdownField.CreateBinder(_galPlotConfig.GetType().GetField(nameof(_galPlotConfig.name)), galBlock);
            _said.CreateBinder(_galPlotConfig.GetType().GetField(nameof(_galPlotConfig.word)), galBlock);
            content.style.flexDirection = FlexDirection.Column;
            operationButton.style.display = DisplayStyle.Flex;
            operationButton.AddManipulator(new ContextualMenuManipulator(OnContextualMenu));
            operationButton.RegisterCallback<ClickEvent>(evt => { operationButton.panel.contextualMenuManager.DisplayMenu(evt, operationButton); });
            if(string.IsNullOrEmpty(_galPlotConfig.name)) return;
            foreach (var configAddition in _galPlotConfig.configAdditions)
            {
                var additionEditor = NodeEditor.GetEditor(configAddition.GetType());
                var additionVisualElement = Activator.CreateInstance(additionEditor, new object[] { RoleDB.Instance[_galPlotConfig.name], configAddition, _galPlotConfig }) as VisualElement;
                additionalContent.Add(additionVisualElement);
            }
            
        }

        private void OnContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("附加姿势与表情", AdditionalPose, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("附加声音", AdditionalSound, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendAction("附加位置", AdditionalPosition, DropdownMenuAction.AlwaysEnabled);
        }

        private void AdditionalSound(DropdownMenuAction obj)
        {
            if (!string.IsNullOrEmpty(_galPlotConfig.name))
            {
                var additionRoleVoice = new AdditionRoleVoice();
                _galPlotConfig.configAdditions.Add(additionRoleVoice);
                additionalContent.Add(new AdditionRoleVoiceUxml(RoleDB.Instance[_galPlotConfig.name], additionRoleVoice, _galPlotConfig));
            }
              
        }

        private void AdditionalPose(DropdownMenuAction obj)
        {
          
            if (!string.IsNullOrEmpty(_galPlotConfig.name))
            {
                var additionPose = new AdditionPose();
                _galPlotConfig.configAdditions.Add(additionPose);
                additionalContent.Add(new AdditionPoseUxml(RoleDB.Instance[_galPlotConfig.name], additionPose, _galPlotConfig));
            }
               
        }

        private void AdditionalPosition(DropdownMenuAction obj)
        {
            if (!string.IsNullOrEmpty(_galPlotConfig.name))
            {
                var additionPosition = new AdditionPosition();
                _galPlotConfig.configAdditions.Add(additionPosition);
                additionalContent.Add(new AdditionPositionUxml(RoleDB.Instance[_galPlotConfig.name], additionPosition, _galPlotConfig));
            }

        }
    }
}