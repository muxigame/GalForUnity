//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DivisionNode.cs
//
//        Created by 半世癫(Roc) at 2021-11-29 22:02:39
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation.GfuMath;

namespace GalForUnity.Graph.GFUNode.Operation.Math{
    [NodeRename("Operation/Math/" + nameof(DivisionNode), "提供相除操作")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class DivisionNode : GfuOperationNode{
        [NodeRename(nameof(A), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort A;
        [NodeRename(nameof(B), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort B;
        [NodeRename(nameof(Out), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Out;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<DivisionOperation>(otherNodeData);
            PortTypeSync(GfuPorts(),otherNodeData.InputPort !=null &&otherNodeData.InputPort.Count ==InputPortCount?otherNodeData.InputPortType(0):typeof(float));
        }
    }
}
