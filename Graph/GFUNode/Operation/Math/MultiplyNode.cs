//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  MultiplyNode.cs
//
//        Created by 半世癫(Roc) at 2021-11-29 21:57:54
//
//======================================================================

using System;
using Assets.GalForUnity.Graph.Operation.GfuMath;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;

namespace GalForUnity.Graph.GFUNode.Operation.Math{
    [NodeRename("Operation/Math/" + nameof(MultiplyNode), "提供相乘操作")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class MultiplyNode : GfuOperationNode{
        [NodeRename(nameof(Vector4), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Vector4;
        [NodeRename(nameof(Vector4), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Vector4Second;
        [NodeRename(nameof(Vector4), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<MultiplyOperation>(otherNodeData);
            PortTypeSync(GfuPorts(),otherNodeData.InputPort !=null &&otherNodeData.InputPort.Count ==InputPortCount?otherNodeData.InputPortType(0):typeof(float));
        }
    }
}
