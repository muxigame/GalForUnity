//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  TimeNode.cs
//
//        Created by 半世癫(Roc) at 2021-02-09 16:07:23
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.Operation;
using GalForUnity.Graph.Attributes;
using NodeData = GalForUnity.Graph.Build.NodeData;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Operation{
    [NodeRename("Operation/" + nameof(TimeNode), "时间节点，提供Unity系统的时间信息")]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    [NodeType(NodeCode.TimeNode)]
    public class TimeNode : GfuOperationNode{
        [NodeRename(nameof(Time), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Time;

        [NodeRename(nameof(SineTime), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort SineTime;

        [NodeRename(nameof(CosineTime), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort CosineTime;

        [NodeRename(nameof(DeltaTime), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort DeltaTime;

        [NodeRename(nameof(SmoothDeltaTime), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort SmoothDeltaTime;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<TimeOperation>(otherNodeData);
        }
    }
}