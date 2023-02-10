//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  MainNode.cs at 2022-11-04 23:36:34
//
//======================================================================

using System.Threading.Tasks;

namespace GalForUnity.Graph.Nodes{
    public class MainNode : RuntimeNode{
        public override Task<GalNodeAsset> OnNodeEnter(GalNodeAsset galNodeAsset){ return Task.FromResult(galNodeAsset.outputPort[0].connections[0].input.node); }
    }
}