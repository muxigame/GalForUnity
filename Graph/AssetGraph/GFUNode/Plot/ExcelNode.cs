//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ExcelNode.cs
//
//        Created by 半世癫(Roc) at 2022-05-18 22:47:53
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Model;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Plot{
    [NodeRename("Node/" + nameof(PlotItemNode), "剧情项编辑节点")]
    [Serializable]
    [NodeType(NodeCode.PlotItemNode)]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class ExcelNode : GfuNode
    {
        [NodeRename(nameof(Exit), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit;   
        [NodeRename(nameof(Enter), typeof(RoleData), NodeDirection.Input, NodeCapacity.Multi)]
        public GfuPort Enter;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
        }
    }
}
