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

using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Model;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif


namespace GalForUnity.Graph.GFUNode.Base{
    [NodeRename(nameof(MainNode),"主节点")]
    [NodeAttributeUsage(NodeAttributeTargets.FlowGraph | NodeAttributeTargets.ItemGraph)]
    public class MainNode : GfuNode{
        [NodeRename(nameof(Exit), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit;
#if UNITY_EDITOR
        public MainNode(){
            capabilities -= Capabilities.Deletable;
            capabilities -= Capabilities.Copiable;
        }
#endif

        public override RoleData Execute(RoleData roleData){ return base.Execute(roleData); }
    }
}