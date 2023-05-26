using GalForUnity.Core.Block;
using GalForUnity.Graph.Editor.Nodes;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block
{
    public class BaseBlock : DraggableBlockEditor
    {
        public VisualElement PortContainer => m_PortContainer;

        public VisualElement FieldContainer => m_FieldContainer;

        private readonly VisualElement m_PortContainer;
        private readonly VisualElement m_FieldContainer;
        
        public BaseBlock(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock)
        {

        }
    }
}