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
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Editor.Builder;
using UnityEditor.Experimental.GraphView;
using Direction = GalForUnity.Graph.Direction;
using Orientation = GalForUnity.Graph.Orientation;

namespace GalForUnity.Graph.Editor.Nodes{
    [NodeRename(nameof(MainNode), "主节点")]
    [NodeType(NodeCode.MainNode)]
    public class MainNode : GfuNode{
        public MainNode(){
            capabilities -= Capabilities.Deletable;
            capabilities -= Capabilities.Copiable;
        }

        public override List<GalPort> Exit{ get; } = new List<GalPort>{
            new GalPort(Orientation.Horizontal, Direction.Output, Capacity.Multi, typeof(GalNodeAsset), nameof(Exit))
        };
    }
}