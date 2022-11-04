//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArccosineNode.cs
//
//        Created by 半世癫(Roc) at 2021-12-02 13:15:58
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.Operation.Geometry;
using GalForUnity.Graph.Attributes;
using NodeData = GalForUnity.Graph.Build.NodeData;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Operation.Geometry{
    [NodeRename("Operation/Geometry/" + nameof(ArccosineNode), "得到输入值的反余弦函数值")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    [NodeType(NodeCode.ArccosineNode)]
    public class ArccosineNode : GfuOperationNode{
        [NodeRename(nameof(In), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort In;
        [NodeRename(nameof(Out), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Out;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<ArccosineOperation>(otherNodeData);
            PortTypeSync(GfuPorts(),otherNodeData.InputPort !=null &&otherNodeData.InputPort.Count == InputPortCount?otherNodeData.InputPortType(0):typeof(float));
        }
    }
}
