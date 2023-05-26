

using GalForUnity.Core.Block;
using GalForUnity.Core.Editor;
using GalForUnity.Graph.Editor.Nodes;

namespace GalForUnity.Graph.Editor.Block{
    [NodeEditor(typeof(VideoBlock))]
    public class VideoBlockEditorUxml : PortableBlockEditor<VideoBlock>{
        public VideoBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock){ }
    }
}