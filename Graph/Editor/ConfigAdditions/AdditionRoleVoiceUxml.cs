using GalForUnity.Core;
using GalForUnity.Core.Block;
using GalForUnity.Core.Editor;
using UnityEditor.UIElements;
using UnityEngine;

namespace GalForUnity.Graph.Editor.ConfigAdditions
{
    [NodeEditor(typeof(AdditionRoleVoice))]
    public class AdditionRoleVoiceUxml : ConfigAdditionUxml<AdditionRoleVoice>
    {
        private readonly ObjectField _objectField;

        public AdditionRoleVoiceUxml(GalObject galObject, AdditionRoleVoice additionRoleVoice, PlotBlock plotBlock) : base(galObject, additionRoleVoice, plotBlock)
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