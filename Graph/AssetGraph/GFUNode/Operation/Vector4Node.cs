//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  Vector4Node.cs
//
//        Created by 半世癫(Roc) at 2021-02-03 20:00:08
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.Operation;
using GalForUnity.Graph.Attributes;
using UnityEngine;
using NodeData = GalForUnity.Graph.Build.NodeData;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Operation{
    [NodeRename("Operation/" + nameof(Vector4Node), "4维向量节点")]
    [NodeType(NodeCode.Vector4Node)]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class Vector4Node : GfuOperationNode{
        [NodeRename("X", typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort X;

        [NodeRename("Y", typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Y;

        [NodeRename("Z", typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Z;

        [NodeRename("W", typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort W;

        [NodeRename(nameof(Vector4), typeof(Vector4), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;

        public Vector4Node(){
            // GfuOperation = new Vector4Operation(
            //     GfuOperationData.CreateInstance(
            //         typeof(float),
            //         typeof(float),
            //         typeof(float),
            //         typeof(float)
            //     ));
        }

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<Vector4Operation>(otherNodeData);
        }

        // public override object GetDefaultValue(int portIndex){
        //     var gfuPort = (GfuPort)inputContainer[portIndex];
        //     return gfuPort.GetDefaultValue();
        // }
    }
}