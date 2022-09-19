//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuGraphAsset.cs
//
//        Created by 半世癫(Roc) at 2022-04-14 00:42:08
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.External;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using UnityEngine;
using GalForUnity.Graph.Attributes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace GalForUnity.Graph.SceneGraph{
    [Serializable]
    public class GfuGraphAsset : ScriptableObject{
        public long instanceID = -1;
        public List<GfuNodeAsset> nodes;

        private Dictionary<long, GfuNodeAsset> _nodeKeyMap = new Dictionary<long, GfuNodeAsset>();

        public GfuNodeAsset GetNodeByInstanceID(long paramInstanceID){
            if (_nodeKeyMap == null || _nodeKeyMap.Count == 0){
                if (nodes == null) return null;
                _nodeKeyMap = new Dictionary<long, GfuNodeAsset>();
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
    }

    public static class GfuNodeStaticMethod{
        public static bool HasConnection(this List<GfuPortAsset> gfuPortAssets){ return !gfuPortAssets.TrueForAll(x => !x.HasConnection); }
#if UNITY_EDITOR

        public static void Save(this GfuGraphAsset gfuGraphAsset, GfuSceneGraphView graphView){
            if (gfuGraphAsset.instanceID == -1) gfuGraphAsset.instanceID = gfuGraphAsset.CreateInstanceID();
            var portMap = new Dictionary<Port, GfuPortAsset>();
            foreach (var graphViewNode in graphView.Nodes){
                if (gfuGraphAsset.nodes == null) gfuGraphAsset.nodes = new List<GfuNodeAsset>();
                var nodeByInstanceID = gfuGraphAsset.GetNodeByInstanceID(graphViewNode.Key);
                GfuNodeAsset gfuNodeAsset;
                if (nodeByInstanceID != null){
                    gfuNodeAsset = nodeByInstanceID;
                } else{
                    gfuNodeAsset = ScriptableObject.CreateInstance<GfuNodeAsset>();
                    gfuGraphAsset.nodes.Add(gfuNodeAsset);
                    AssetDatabase.AddObjectToAsset(gfuNodeAsset, gfuGraphAsset);
                }

                gfuNodeAsset.Save(graphViewNode.Value, portMap);
                gfuNodeAsset.instanceID = graphViewNode.Key;
            }

            foreach (var portKeyValuePair in portMap){
                portKeyValuePair.Value.connections=new List<GfuConnectionAsset>();
                foreach (var keyConnection in portKeyValuePair.Key.connections){
                    if (portKeyValuePair.Value.connections == null) portKeyValuePair.Value.connections = new List<GfuConnectionAsset>();
                    portKeyValuePair.Value.connections.Add(new GfuConnectionAsset().Save(portMap[keyConnection.input], portMap[keyConnection.output]));
                }
            }

            AssetDatabase.SetMainObject(gfuGraphAsset, AssetDatabase.GetAssetPath(gfuGraphAsset));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void Save(this GfuNodeAsset gfuNodeAsset, GfuNode gfuNode, Dictionary<Port, GfuPortAsset> portMap){
            gfuNodeAsset.position = gfuNode.GetPosition().position;
            gfuNodeAsset.gfuNodeTypeCode = gfuNode.GetTypeByCode();
            gfuNodeAsset.name = gfuNode.GetType().Name;
            if (gfuNodeAsset.inputPort  == null) gfuNodeAsset.inputPort = new List<GfuPortAsset>();
            if (gfuNodeAsset.outputPort == null) gfuNodeAsset.outputPort = new List<GfuPortAsset>();
            var gfuPorts = gfuNode.GetGfuInput();
            for (var i = 0; i < gfuPorts.Count; i++){
                if (!gfuNodeAsset.HasInputPort) gfuNodeAsset.inputPort = new List<GfuPortAsset>();
                if (gfuNodeAsset.inputPort.Count > i){
                    gfuNodeAsset.inputPort[i].Save(gfuPorts[i], gfuNodeAsset);
                } else{
                    var gfuPortAsset = ScriptableObject.CreateInstance<GfuPortAsset>();
                    gfuPortAsset.Save(gfuPorts[i], gfuNodeAsset);
                    gfuNodeAsset.inputPort.Add(gfuPortAsset);
                    gfuPortAsset.name = gfuNodeAsset.name + nameof(Port);
                    gfuPortAsset.hideFlags = HideFlags.None;
                    AssetDatabase.AddObjectToAsset(gfuPortAsset, gfuNodeAsset);
                }

                portMap.Add(gfuPorts[i], gfuNodeAsset.inputPort[i]);
            }

            gfuPorts = gfuNode.GetGfuOutPut();
            for (var i = 0; i < gfuPorts.Count; i++){
                if (!gfuNodeAsset.HasOutputPort) gfuNodeAsset.outputPort = new List<GfuPortAsset>();
                if (gfuNodeAsset.outputPort.Count > i){
                    gfuNodeAsset.outputPort[i].Save(gfuPorts[i], gfuNodeAsset);
                } else{
                    var gfuPortAsset = ScriptableObject.CreateInstance<GfuPortAsset>();
                    gfuPortAsset.Save(gfuPorts[i], gfuNodeAsset);
                    gfuNodeAsset.outputPort.Add(gfuPortAsset);
                    gfuPortAsset.name = gfuNodeAsset.name + nameof(Port);
                    gfuPortAsset.hideFlags = HideFlags.None;
                    AssetDatabase.AddObjectToAsset(gfuPortAsset, gfuNodeAsset);
                }

                portMap.Add(gfuPorts[i], gfuNodeAsset.outputPort[i]);
            }
        }

        private static void Save(this GfuPortAsset gfuPortAsset, GfuPort gfuPort, GfuNodeAsset gfuNodeAsset){
            if (gfuPort.direction == Direction.Input)
                gfuPortAsset.portType = PortType.Input;
            else if (gfuPort.direction == Direction.Output) 
                gfuPortAsset.portType = PortType.OutPut;
            gfuPortAsset.node = gfuNodeAsset;
        }

        private static GfuConnectionAsset Save(this GfuConnectionAsset gfuConnectionAsset, GfuPortAsset input, GfuPortAsset output){
            gfuConnectionAsset.input = input;
            gfuConnectionAsset.output = output;
            return gfuConnectionAsset;
        }
#endif
    }
}