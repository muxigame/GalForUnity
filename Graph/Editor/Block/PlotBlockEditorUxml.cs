//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotBlock.cs Created at 2022-09-30 19:54:38
//
//======================================================================

using System;
using GalForUnity.Core;
using GalForUnity.Core.Block;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Graph.Editor.ConfigAdditions;
using GalForUnity.Graph.Editor.Nodes;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block
{
    [NodeEditor(typeof(PlotBlock))]
    public class PlotBlockEditorUxml : DraggableBlockEditor
    {
        private readonly PlotBlock _plotBlock;
        private readonly NameDropdownField _nameField;
        private readonly TextField _said;

        public PlotBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock)
        {
            _plotBlock = (PlotBlock)galBlock;
            styleSheets.Add(UxmlHandler.instance.plotBlockUss);
            content.Add(_nameField = new NameDropdownField());
            content.Add(_said = new TextField
            {
                label = "Said"
            });
            _nameField.dropdownField.labelElement.AddToClassList("gal-label");
            _said.labelElement.AddToClassList("gal-label");
            _nameField.dropdownField.CreateBinder(_plotBlock.GetType().GetField(nameof(_plotBlock.name)), galBlock);
            _said.CreateBinder(_plotBlock.GetType().GetField(nameof(_plotBlock.word)), galBlock);
            content.style.flexDirection = FlexDirection.Column;
            operationButton.style.display = DisplayStyle.Flex;
            operationButton.AddManipulator(new ContextualMenuManipulator(OnContextualMenu));
            operationButton.RegisterCallback<ClickEvent>(evt => { operationButton.panel.contextualMenuManager.DisplayMenu(evt, operationButton); });
            if(string.IsNullOrEmpty(_plotBlock.name)) return;
            foreach (var configAddition in _plotBlock.configAdditions)
            {
                var additionEditor = NodeEditor.GetEditor(configAddition.GetType());
                var additionVisualElement = Activator.CreateInstance(additionEditor, new object[] { RoleDB.Instance[_plotBlock.name], configAddition, _plotBlock }) as VisualElement;
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
            if (!string.IsNullOrEmpty(_plotBlock.name))
            {
                var additionRoleVoice = new AdditionRoleVoice();
                _plotBlock.configAdditions.Add(additionRoleVoice);
                additionalContent.Add(new AdditionRoleVoiceUxml(RoleDB.Instance[_plotBlock.name], additionRoleVoice, _plotBlock));
            }
              
        }

        private void AdditionalPose(DropdownMenuAction obj)
        {
          
            if (!string.IsNullOrEmpty(_plotBlock.name))
            {
                var additionPose = new AdditionPose();
                _plotBlock.configAdditions.Add(additionPose);
                additionalContent.Add(new AdditionPoseUxml(RoleDB.Instance[_plotBlock.name], additionPose, _plotBlock));
            }
               
        }

        private void AdditionalPosition(DropdownMenuAction obj)
        {
            if (!string.IsNullOrEmpty(_plotBlock.name))
            {
                var additionPosition = new AdditionPosition();
                _plotBlock.configAdditions.Add(additionPosition);
                additionalContent.Add(new AdditionPositionUxml(RoleDB.Instance[_plotBlock.name], additionPosition, _plotBlock));
            }

        }
    }
}