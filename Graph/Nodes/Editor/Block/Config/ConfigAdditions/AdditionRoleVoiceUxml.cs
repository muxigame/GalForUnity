using GalForUnity.Attributes;
using GalForUnity.Core;
using UnityEditor.UIElements;
using UnityEngine;
namespace GalForUnity.Graph.Nodes.Editor.Block.Config.ConfigAdditions
{
    [NodeEditor(typeof(AdditionRoleVoice))]
    public class AdditionRoleVoiceUxml : ConfigAdditionUxml<AdditionRoleVoice>
    {
        private readonly ObjectField _objectField;

        public AdditionRoleVoiceUxml(RoleAssets roleAssets, AdditionRoleVoice additionRoleVoice, GalPlotConfig galPlotConfig) : base(roleAssets, additionRoleVoice, galPlotConfig)
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