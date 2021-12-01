//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SplitNode.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 13:18:41
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation.Channel;

namespace GalForUnity.Graph.GFUNode.Operation.Channel{
    [NodeRename("Operation/Channel/" + nameof(SplitNode), "将向量拆分成浮点数")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class SplitNode : GfuOperationNode{
        [NodeRename(nameof(x), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort x;
        [NodeRename(nameof(y), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort y;
        [NodeRename(nameof(z), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort z; 
        [NodeRename(nameof(w), typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort w;
        [NodeRename(nameof(In), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort In;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<SplitOperation>(otherNodeData);
            PortTypeSync(GetGfuInput(),otherNodeData.InputPort !=null &&otherNodeData.InputPort.Count ==InputPortCount?otherNodeData.InputPortType(0):typeof(float),false);
        }
    }
}
