//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SceneNode.cs
//
//        Created by 半世癫(Roc) at 2021-11-19 15:39:06
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Model;
using GalForUnity.Model.Scene;
using GalForUnity.System;

namespace GalForUnity.Graph.GFUNode{
    [NodeRename("Node/" +nameof(SceneNode),"场景节点")]
    [NodeFieldType("Scene")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph|NodeAttributeTargets.FlowGraph)]
    public class SceneNode :ObjectFieldNode<SceneModel>{
        [NodeRename(nameof(Exit), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit;

        public override RoleData Execute(RoleData roleData){
            if(objectReference) GameSystem.Data.SceneController.GoToScene(objectReference);
            return base.Execute(roleData);;
        }
    }
}
