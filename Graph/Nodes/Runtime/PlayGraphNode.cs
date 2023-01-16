using System.Threading.Tasks;
using GalForUnity.Graph.Build;
using GalForUnity.Graph.SceneGraph;
using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.Graph.Nodes.Runtime{
    public class PlayGraphNode:RuntimeNode{
        public SceneGraph.SceneGraph SceneGraph;

        public override async Task<GfuNodeAsset> OnNodeEnter(GfuNodeAsset gfuNodeAsset){
            var galGraph = new GalGraph(SceneGraph.GraphNode);
            bool isGraphExecuted = false;
            galGraph.GraphProvider.OnGraphExecuted += x => {
                isGraphExecuted = true;
            };
            galGraph.Play();
            while (!isGraphExecuted){
                await Task.Yield();
            }
            return gfuNodeAsset.NextNode(0);
        }
    }
}