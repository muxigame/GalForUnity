using System.Collections.Generic;

namespace GalForUnity.Graph.AssetGraph.Data{
    public interface DataInfo{
        List<NodeData.NodeFieldInfo> jsonField{ get; set; }
        List<NodeData.NodeFieldInfo> idField{ get; set; }

        List<NodeData.ListData> listField{ get; set; }
    }
}