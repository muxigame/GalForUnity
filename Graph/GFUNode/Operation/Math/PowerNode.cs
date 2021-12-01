//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PowerNode.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 16:05:24
//
//======================================================================


using System;
using Assets.GalForUnity.Graph.Operation.GfuMath;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;

namespace GalForUnity.Graph.GFUNode.Operation.Math{
    [NodeRename("Operation/Math/" + nameof(PowerNode), "提供幂指函数操作")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class PowerNode : GfuOperationNode{
        [NodeRename(nameof(A), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort A;
        [NodeRename(nameof(B), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort B;
        [NodeRename(nameof(Out), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Out;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<PowerOperation>(otherNodeData);
            PortTypeSync(GfuPorts(),otherNodeData.InputPort !=null &&otherNodeData.InputPort.Count ==InputPortCount?otherNodeData.InputPortType(0):typeof(float));
        }
    }
}
