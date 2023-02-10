//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuNodeHandler.cs
//
//        Created by 半世癫(Roc) at 2022-09-19 23:38:34
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Nodes;
using UnityEngine;

namespace GalForUnity.Graph{
    [Serializable]
    public class GfuNodeHandler : ScriptableObject{
        public GalNodeAsset galNode;
    }

    [Serializable]
    public class GalNodeAsset{
        public long instanceID;
        public int gfuNodeTypeCode;

        public Vector4 position;

        // [HideInInspector]
        [SerializeReference] public List<GalPortAsset> inputPort;

        // 
        [SerializeReference] public List<GalPortAsset> outputPort;

        // [NonSerialized]
        [SerializeReference] public RuntimeNode runtimeNode;
        public Type Type => NodeType.GetTypeByCode(gfuNodeTypeCode);
        public bool HasInputPort => inputPort   != null && inputPort.Count  != 0;
        public bool HasOutputPort => outputPort != null && outputPort.Count != 0;

        public GalNodeAsset NextNode(int portIndex, int connectIndex = 0){
            if (!HasOutputPort) return null;
            if (outputPort.Count <= portIndex) throw new ArgumentOutOfRangeException($"index: {portIndex} out of range: {outputPort.Count}");
            var connectionts = outputPort[portIndex].connections;
            if (connectionts       == null) throw new NullReferenceException("the port not connected");
            if (connectionts.Count <= connectIndex) throw new ArgumentOutOfRangeException($"index: {portIndex} out of range: {outputPort.Count}");
            return connectionts[connectIndex]?.input.node ?? null;
        }
    }


    [Serializable]
    public class GfuConnectionAsset{
        [SerializeReference] [HideInInspector] public GalPortAsset input;
        [SerializeReference] [HideInInspector] public GalPortAsset output;
    }
    
}