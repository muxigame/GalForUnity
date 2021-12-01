//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  TangentNode.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 22:56:18
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation.Geometry;

namespace GalForUnity.Graph.GFUNode.Operation.Geometry{
    [NodeRename("Operation/Geometry/" + nameof(TangentNode), "得到输入值的正切函数值")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class TangentNode : GfuOperationNode{
        [NodeRename(nameof(In), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort In;
        [NodeRename(nameof(Out), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Out;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<TangentOperation>(otherNodeData);
            PortTypeSync(GfuPorts(),otherNodeData.InputPort !=null &&otherNodeData.InputPort.Count == InputPortCount?otherNodeData.InputPortType(0):typeof(float));
        }
    }
}
