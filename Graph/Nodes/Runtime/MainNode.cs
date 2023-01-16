//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  MainNode.cs at 2022-11-04 23:36:34
//
//======================================================================

using System.Threading.Tasks;
using GalForUnity.Graph.Build;
using GalForUnity.Graph.SceneGraph;

namespace GalForUnity.Graph.Nodes.Runtime{
    public class MainNode : RuntimeNode{
        public override Task<GfuNodeAsset> OnNodeEnter(GfuNodeAsset gfuNodeAsset){ return Task.FromResult(gfuNodeAsset.outputPort[0].connections[0].input.node); }
    }
}