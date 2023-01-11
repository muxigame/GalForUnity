//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuPortAsset.cs
//
//        Created by 半世癫(Roc) at 2022-04-16 17:07:38
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.Graph.Build;
using UnityEngine;

namespace GalForUnity.Graph.SceneGraph{
    [Serializable]
    public class GfuPortAsset{
        [SerializeField]
        public string portName;
        [HideInInspector]
        [SerializeReference] public GfuNodeAsset node;
        [SerializeReference] public List<GfuConnectionAsset> connections;
        public PortType portType;
        [SerializeField]
        public PortValue value;

        public (T value, bool over) GetValueIfExist<T>(){
            if (portType == PortType.Input){
                if (value.Value != null) return ((T)value.Value,true);
                return connections?.FirstOrDefault()?.output?.GetValueIfExist<T>() ?? default;
            }
            if (node.runtimeNode is OperationNode operationNode) return operationNode.GetValue<T>(Index);
            return default;
        }
        public bool HasConnection => connections != null && connections.Count != 0;

        /// <summary>
        /// Get port index from ports of node
        /// Ports in the block always return -1
        /// </summary>
        public int Index => portType == PortType.Input ? node.inputPort.IndexOf(this) : node.outputPort.IndexOf(this);

        public static implicit operator bool(GfuPortAsset gfuNode){
            if (gfuNode == null) return false;
            return true;
        }
    }
}