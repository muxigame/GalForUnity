//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  CombineNode.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 13:07:15
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation.Channel;
using UnityEngine;

namespace GalForUnity.Graph.GFUNode.Operation.Channel{
    [NodeRename("Operation/Channel/" + nameof(CombineNode), "将浮点数组合成向量")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class CombineNode : GfuOperationNode{
        [NodeRename(nameof(x), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort x;
        [NodeRename(nameof(y), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort y;
        [NodeRename(nameof(z), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort z; 
        [NodeRename(nameof(w), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort w;
        [NodeRename(nameof(Vector4), typeof(Vector4), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;
        [NodeRename(nameof(Vector3), typeof(Vector3), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit1;
        [NodeRename(nameof(Vector2), typeof(Vector2), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit2;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<CombineOperation>(otherNodeData);
        }
    }
}
