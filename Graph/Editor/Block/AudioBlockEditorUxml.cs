//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotAudioBlock.cs Created at 2022-09-28 00:31:49
//
//======================================================================

using GalForUnity.Core.Block;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Graph.Editor.Nodes;

namespace GalForUnity.Graph.Editor.Block{
    [NodeEditor(typeof(AudioBlock))]
    public class AudioBlockEditorUxml : PortableBlockEditor<AudioBlock>{
        public AudioBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock){ }
    }
}