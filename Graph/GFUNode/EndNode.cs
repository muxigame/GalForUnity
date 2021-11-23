//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  EndNode.cs
//
//        Created by 半世癫(Roc) at 2021-11-19 11:18:26
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.System.Event;
using UnityEngine;

namespace GalForUnity.Graph.GFUNode{
    [NodeRename("Node/" +nameof(EndNode),"剧情项编辑节点")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph|NodeAttributeTargets.FlowGraph)]
    public class EndNode : EnterNode{
        public override void Executed(int index){
            var invocationList = EventCenter.GetInstance().OnNodeExecutedEvent.GetInvocationList();
            for (var i = invocationList.Length - 1; i >= 0; i--){
                invocationList[i].Method.Invoke(invocationList[i].Target,new object[]{this});
            }
            OnExecuted?.Invoke(null);
        }
    }
}
