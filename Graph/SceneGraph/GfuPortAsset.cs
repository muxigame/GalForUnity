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
using UnityEngine;

namespace GalForUnity.Graph.SceneGraph{
    [Serializable]
    public class GfuPortAsset:ScriptableObject{
        public GfuNodeAsset node;
        public List<GfuConnectionAsset> connections;
        public PortType portType;
        public bool HasConnection => connections != null && connections.Count != 0;
        public int Index => portType             == PortType.Input ? node.inputPort.IndexOf(this) : node.outputPort.IndexOf(this);

        public static implicit operator bool(GfuPortAsset gfuNode){
            if (gfuNode == null) return false;
            return true;
        }
    }
}