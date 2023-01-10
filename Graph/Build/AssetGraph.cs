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
using System.Linq;
using GalForUnity.External;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using UnityEngine;
#if UNITY_EDITOR
using GalForUnity.Graph.Build;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace GalForUnity.Graph.SceneGraph{
    public interface IGalGraph{
        public GfuGraphAsset GraphNode{ get; set; }

        // ReSharper disable once InconsistentNaming
        public string name{ get; set; }
        public int GetInstanceID();
    }

    [CreateAssetMenu(menuName = "GalForUnity/AssetGraph", fileName = "AssetGraph.asset", order = 2)]
    public class AssetGraph : ScriptableObject, IGalGraph{
        [SerializeField] private GfuGraphAsset graphNode;

        [SerializeField]
        public GfuGraphAsset GraphNode{
            get => graphNode;
            set => graphNode = value;
        }
    }

    [Serializable]
    public class GfuGraphAsset : IInstanceIDAble{
        public long instanceID = -1;

        [SerializeReference] public List<GfuNodeAsset> nodes;

        [NonSerialized] private Dictionary<long, GfuNodeAsset> _nodeKeyMap = new Dictionary<long, GfuNodeAsset>();

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

        public static implicit operator bool(GfuGraphAsset gfuGraphAsset){
            if (gfuGraphAsset != null) return true;
            return false;
        }
    }

    public static class GfuNodeStaticMethod{
        public static bool HasConnection(this List<GfuPortAsset> gfuPortAssets){ return !gfuPortAssets.TrueForAll(x => !x.HasConnection); }
#if UNITY_EDITOR

        internal static void Save(this SceneGraph sceneGraph, GfuSceneGraphView graphView){
            var save = sceneGraph.GraphNode.Save(graphView).ToList();
        }

        internal static void Save(this AssetGraph assetGraph, GfuSceneGraphView graphView){
            var save = assetGraph.GraphNode.Save(graphView);
            AssetDatabase.SetMainObject(assetGraph, AssetDatabase.GetAssetPath(assetGraph));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static IEnumerable<RuntimeNode> Save(this GfuGraphAsset assetGraph, GfuSceneGraphView graphView){
            if (assetGraph.instanceID == -1) assetGraph.instanceID = assetGraph.CreateInstanceID();
            var portMap = new Dictionary<Port, GfuPortAsset>();
            foreach (var graphViewNode in graphView.Nodes){
                if (assetGraph.nodes == null) assetGraph.nodes = new List<GfuNodeAsset>();
                var nodeByInstanceID = assetGraph.GetNodeByInstanceID(graphViewNode.Key);
                GfuNodeAsset gfuNodeAsset;
                if (nodeByInstanceID != null){
                    gfuNodeAsset = nodeByInstanceID;
                } else{
                    gfuNodeAsset = new GfuNodeAsset();
                    assetGraph.nodes.Add(gfuNodeAsset);
                }
                gfuNodeAsset.Save(graphViewNode.Value, portMap);
                yield return gfuNodeAsset.runtimeNode = graphViewNode.Value.RuntimeNode;
                gfuNodeAsset.instanceID = graphViewNode.Key;
            }
            graphView.edges.ForEach(x => {
                if (portMap.ContainsKey(x.input) && portMap.ContainsKey(x.output)) new GfuConnectionAsset().Save(portMap[x.input], portMap[x.output]);
            });
        }

        internal static void Save(this GfuNodeAsset gfuNodeAsset, GfuNode gfuNode, Dictionary<Port, GfuPortAsset> portMap){
            foreach (var keyValuePair in gfuNode.OnSavePort(gfuNodeAsset)) portMap.Add(keyValuePair.port, keyValuePair.gfuPortAsset);
        }

        internal static void Save(this GfuPortAsset gfuPortAsset, GfuPort gfuPort, GfuNodeAsset gfuNodeAsset){
            gfuPortAsset.portName = gfuPort.name;
            if (gfuPort.direction == Direction.Input)
                gfuPortAsset.portType = PortType.Input;
            else if (gfuPort.direction == Direction.Output) gfuPortAsset.portType = PortType.OutPut;
            gfuPortAsset.node = gfuNodeAsset;
        }

        internal static GfuConnectionAsset Save(this GfuConnectionAsset gfuConnectionAsset, GfuPortAsset input, GfuPortAsset output){
            if (input.connections  == null) input.connections = new List<GfuConnectionAsset>();
            if (output.connections == null) output.connections = new List<GfuConnectionAsset>();
            if (gfuConnectionAsset == null) return null;
            input.connections.Add(gfuConnectionAsset);
            output.connections.Add(gfuConnectionAsset);
            gfuConnectionAsset.input = input;
            gfuConnectionAsset.output = output;
            return gfuConnectionAsset;
        }
#endif
    }
}