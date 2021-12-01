//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  AddNode.cs
//
//        Created by Roc(半世癫) at 2021-11-28 20:44:36
//
//======================================================================

using System;
using GalForUnity.Graph.Operation.GfuMath;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;

namespace GalForUnity.Graph.GFUNode.Operation.Math{
    [NodeRename("Operation/Math/" + nameof(AddNode), "提供相加操作")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class AddNode : GfuOperationNode{
        [NodeRename(nameof(A), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort A;
        [NodeRename(nameof(B), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort B;
        [NodeRename(nameof(Out), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Out;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<AddOperation>(otherNodeData);
            PortTypeSync(GfuPorts(),otherNodeData.InputPort !=null &&otherNodeData.InputPort.Count ==InputPortCount?otherNodeData.InputPortType(0):typeof(float));
        }
    }
}
