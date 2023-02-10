using GalForUnity.Core;
using GalForUnity.Core.Block;
using GalForUnity.Core.Editor.Attributes;
using UnityEditor.UIElements;
using UnityEngine;

namespace GalForUnity.Graph.Editor.ConfigAdditions
{
    [NodeEditor(typeof(AdditionRoleVoice))]
    public class AdditionRoleVoiceUxml : ConfigAdditionUxml<AdditionRoleVoice>
    {
        private readonly ObjectField _objectField;

        public AdditionRoleVoiceUxml(RoleAssets roleAssets, AdditionRoleVoice additionRoleVoice, PlotBlock plotBlock) : base(roleAssets, additionRoleVoice, plotBlock)
        {
            _objectField = new ObjectField()
            {
                label = "声音",
            };
            _objectField.labelElement.AddToClassList("gal-label");
            _objectField.objectType = typeof(AudioClip);
            _objectField.CreateBinder(typeof(AdditionRoleVoice).GetField(nameof(additionRoleVoice.audioClip)),
                additionRoleVoice);
            var fieldContainer = new FieldContainer();
            fieldContainer.Add(_objectField);
            Content.Add(fieldContainer);
        }
    }
}