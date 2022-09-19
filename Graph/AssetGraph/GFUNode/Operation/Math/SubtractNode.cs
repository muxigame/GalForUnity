//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SubtractNode.cs
//
//        Created by Roc(半世癫) at 2021-11-29 17:52:11
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.Operation.GfuMath;
using GalForUnity.Graph.Attributes;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Operation.Math{
    [NodeRename("Operation/Math/" + nameof(SubtractNode), "提供相减操作")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    [NodeType(NodeCode.SubtractNode)]
    public class SubtractNode : GfuOperationNode{
        [NodeRename("A", typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort In;
        [NodeRename("B", typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Vector4Second;
        [NodeRename(nameof(Out), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Out;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<SubtractOperation>(otherNodeData);
            PortTypeSync(GfuPorts(),otherNodeData.InputPort!=null&&otherNodeData.InputPort.Count==InputPortCount?otherNodeData.InputPortType(0):typeof(float));
        }
    }
}
