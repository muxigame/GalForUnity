//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotVideoBlockUxml.cs 2022-10-11 21:43:32
//
//======================================================================

using GalForUnity.Core.Block;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Graph.Editor.Nodes;

namespace GalForUnity.Graph.Editor.Block{
    [NodeEditor(typeof(VideoBlock))]
    public class VideoBlockEditorUxml : PortableBlockEditor<VideoBlock>{
        public VideoBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock){ }
    }
}