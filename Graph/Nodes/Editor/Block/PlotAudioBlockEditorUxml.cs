//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotAudioBlock.cs Created at 2022-09-28 00:31:49
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Nodes.Editor;
using GalForUnity.Graph.Nodes.Editor.Block;

namespace GalForUnity.Graph.Block{
    [NodeEditor(typeof(GalAudioConfig))]
    public class PlotAudioBlockEditorUxml : PortableBlockEditor<GalAudioConfig>{
        public PlotAudioBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock){ }
    }
}