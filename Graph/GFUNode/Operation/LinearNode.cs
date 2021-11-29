//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  LinearNode.cs
//
//        Created by 半世癫(Roc) at 2021-02-24 13:57:29
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation;

namespace GalForUnity.Graph.GFUNode.Operation{
    [NodeRename("Operation/" + nameof(LinearNode), "提供时间维度的线性操作")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class LinearNode : GfuOperationNode{
        [NodeRename(nameof(From), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort From;

        [NodeRename(nameof(To), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort To;

        [NodeRename(nameof(Time), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Time;

        [NodeRename("Vector1", typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<LinearOperation>(otherNodeData);
        }
    }
}