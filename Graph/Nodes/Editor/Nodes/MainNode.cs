//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  EnterNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-10 15:28:39
//
//======================================================================

using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.SceneGraph;
using UnityEditor.Experimental.GraphView;
using Direction = GalForUnity.Graph.SceneGraph.Direction;
using Orientation = GalForUnity.Graph.SceneGraph.Orientation;

namespace GalForUnity.Graph.Nodes.Editor{
    [NodeRename(nameof(MainNode), "主节点")]
    [NodeType(NodeCode.MainNode)]
    public class MainNode : GfuNode{
        public MainNode(){
            capabilities -= Capabilities.Deletable;
            capabilities -= Capabilities.Copiable;
        }

        public override List<GfuPort> Exit{ get; } = new List<GfuPort>{
            new GfuPort(Orientation.Horizontal, Direction.Output, Capacity.Multi, typeof(GfuNodeAsset), nameof(Exit))
        };
    }
}