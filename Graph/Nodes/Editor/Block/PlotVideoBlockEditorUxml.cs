//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotVideoBlockUxml.cs 2022-10-11 21:43:32
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Nodes.Editor;
using GalForUnity.Graph.Nodes.Editor.Block;

namespace GalForUnity.Graph.Block{
    [NodeEditor(typeof(GalVideoConfig))]
    public class PlotVideoBlockEditorUxml : PortableBlockEditor<GalVideoConfig>{
        public PlotVideoBlockEditorUxml(PlotNode plotNode, IGalBlock galBlock) : base(plotNode, galBlock){ }
    }
}