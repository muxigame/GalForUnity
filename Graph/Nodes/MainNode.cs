

using System.Threading.Tasks;

namespace GalForUnity.Graph.Nodes{
    public class MainNode : RuntimeNode{
        public override Task<GalNodeAsset> OnNodeEnter(GalNodeAsset galNodeAsset){ return Task.FromResult(galNodeAsset.outputPort[0].connections[0].input.node); }
    }
}