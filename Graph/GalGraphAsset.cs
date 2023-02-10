using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalForUnity.Graph{
    [Serializable]
    public class GalGraphAsset{
        public long instanceID = -1;

        [SerializeReference] public List<GalNodeAsset> nodes;

        [NonSerialized] private Dictionary<long, GalNodeAsset> _nodeKeyMap = new Dictionary<long, GalNodeAsset>();

        public GalNodeAsset GetNodeByInstanceID(long paramInstanceID){
            if (_nodeKeyMap == null || _nodeKeyMap.Count == 0){
                if (nodes == null) return null;
                _nodeKeyMap = new Dictionary<long, GalNodeAsset>();
            }

            if (_nodeKeyMap.Count != nodes.Count){
                _nodeKeyMap.Clear();
                foreach (var gfuNodeAsset in nodes){
                    if (gfuNodeAsset == null) continue;
                    _nodeKeyMap.Add(gfuNodeAsset.instanceID, gfuNodeAsset);
                }
            }

            if (!_nodeKeyMap.ContainsKey(paramInstanceID)) return null;
            return _nodeKeyMap[paramInstanceID];
        }

        public static implicit operator bool(GalGraphAsset galGraphAsset){
            if (galGraphAsset != null) return true;
            return false;
        }
    }
}