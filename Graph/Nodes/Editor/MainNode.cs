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
using UnityEditor.Experimental.GraphView;

namespace GalForUnity.Graph.Nodes.Editor{
    [NodeRename(nameof(MainNode), "主节点")]
    [NodeType(NodeCode.MainNode)]
    public class MainNode : GfuNode{
        public List<GfuPort> Exit = new List<GfuPort> {
            new GfuPort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(object), nameof(Exit))
        };

        public MainNode(){
            capabilities -= Capabilities.Deletable;
            capabilities -= Capabilities.Copiable;
        }
    }
}