

using GalForUnity.Core.Block;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Graph.Editor.Nodes;

namespace GalForUnity.Graph.Editor.Block{
    [NodeEditor(typeof(AudioBlock))]
    public class AudioBlockEditorUxml : PortableBlockEditor<AudioBlock>{
        public AudioBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock){ }
    }
}