using GalForUnity.Core.Block;
using GalForUnity.Graph.Editor.Builder;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block.Field
{
    public sealed class PortableFloatField : BaseField<float>
    {
        private readonly BaseBlock m_Block;
        private readonly FloatField m_FloatField;
        private readonly GalPort m_GalPort;

        public PortableFloatField(string label, BaseBlock block) : base(label, null)
        {
            m_Block = block;
            var toggle = new Toggle();
            contentContainer.Add(m_FloatField = new FloatField());
            contentContainer.Add(m_GalPort = GalPort.CreateDefault<float>());
            contentContainer.Add(toggle);
            toggle.RegisterValueChangedCallback(evt =>
            {
                var galBlock = (GalBlock)m_Block.GalBlock;
                if (evt.newValue)
                {
                    galBlock.AddPort(label);
                    m_GalPort.visible = true;
                    m_FloatField.visible = false;
                    m_Block.PortContainer.Add(this);
                }
                else
                {
                    galBlock.RemovePort(label);
                    m_GalPort.visible = false;
                    m_FloatField.visible = true;
                    m_Block.FieldContainer.Add(this);
                }
                m_Block.plotNode.GraphView.Record();
            });
            m_FloatField.RegisterValueChangedCallback(Callback);
        }

        ~PortableFloatField()
        {
            m_FloatField.UnregisterValueChangedCallback(Callback);
        }
        
        private void Callback(ChangeEvent<float> evt)
        {
            value = evt.newValue;
            m_Block.plotNode.GraphView.Record();
        }
        
    }
}