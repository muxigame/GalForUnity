using System.Threading.Tasks;

namespace GalForUnity.Graph.Nodes{
    public class PlayGraphNode:RuntimeNode{
        public SceneGraph SceneGraph;

        public override async Task<GalNodeAsset> OnNodeEnter(GalNodeAsset galNodeAsset){
            var galGraph = new GalGraph(SceneGraph.GraphNode);
            bool isGraphExecuted = false;
            galGraph.GraphProvider.OnGraphExecuted += x => {
                isGraphExecuted = true;
            };
            galGraph.Play();
            while (!isGraphExecuted){
                await Task.Yield();
            }
            return galNodeAsset.NextNode(0);
        }
    }
}