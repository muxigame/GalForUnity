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
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation.GfuMath;

namespace GalForUnity.Graph.GFUNode.Operation.Math{
    [NodeRename("Operation/Math/" + nameof(SubtractNode), "提供相减操作")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class SubtractNode : GfuOperationNode{
        [NodeRename(nameof(Vector4), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Vector4;
        [NodeRename(nameof(Vector4), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Vector4Second;
        [NodeRename(nameof(Vector4), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<SubtractOperation>(otherNodeData);
            PortTypeSync(GfuPorts(),otherNodeData.InputPort!=null&&otherNodeData.InputPort.Count==InputPortCount?otherNodeData.InputPortType(0):typeof(float));
        }
    }
}
