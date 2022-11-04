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
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using UnityEngine;
using GalForUnity.Graph.Attributes;
using Unity.Plastic.Newtonsoft.Json;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

namespace GalForUnity.Graph.SceneGraph{
    public interface IGalGraph{
        public GfuGraphAsset GraphNode{ get; set; }
        public GraphData GraphData{ get; set; }
        public int GetInstanceID();
        // ReSharper disable once InconsistentNaming
        public string name{ get; set; }
        
    }
    [CreateAssetMenu(menuName = "GalForUnity/AssetGraph" ,fileName = "AssetGraph.asset", order = 2)]
    public class AssetGraph: ScriptableObject,IGalGraph{
        [SerializeField]
        private GfuGraphAsset graphNode;
        [SerializeField]
        private GraphData graphData;
        public GfuGraphAsset GraphNode{
            get => graphNode;
            set => graphNode = value;
        }

        public GraphData GraphData{
            get => graphData;
            set => graphData = value;
        }
        
    }
    [Serializable]
    public class GfuGraphAsset :IInstanceIDAble{
        public long instanceID = -1;
        [SerializeReference]
        public List<GfuNodeAsset> nodes;
        public string nodeDataJson;
        public List<Object> unityReference;
        [NonSerialized]
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

        public static implicit operator bool(GfuGraphAsset gfuGraphAsset){
            if (gfuGraphAsset != null) return true;
            return false;
        }
    }

    public static class GfuNodeStaticMethod{
        public static bool HasConnection(this List<GfuPortAsset> gfuPortAssets){ return !gfuPortAssets.TrueForAll(x => !x.HasConnection); }
#if UNITY_EDITOR
        
        internal static void Save(this SceneGraph sceneGraph,GfuSceneGraphView graphView){
            sceneGraph.GraphNode.Save(graphView);
            sceneGraph.GraphData.Save(graphView);
        }
        internal static void Save(this AssetGraph assetGraph, GfuSceneGraphView graphView){
            assetGraph.GraphNode=new GfuGraphAsset();
            assetGraph.GraphNode.Save(graphView);
            assetGraph.GraphData.Save(graphView);
            // TODO 使用子资产持有引用，方便预览
            // foreach (var graphNodeNode in assetGraph.GraphNode.nodes){
            //     var gfuNodeHandler = ScriptableObject.CreateInstance<GfuNodeHandler>();
            //     gfuNodeHandler.gfuNode = graphNodeNode;
            //     gfuNodeHandler.name = graphNodeNode.GetType().Name;
            //     AssetDatabase.AddObjectToAsset(gfuNodeHandler,assetGraph);
            // }
            AssetDatabase.SetMainObject(assetGraph,AssetDatabase.GetAssetPath(assetGraph));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        internal static void Save(this GfuGraphAsset assetGraph, GfuSceneGraphView graphView){
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
                gfuNodeAsset.instanceID = graphViewNode.Key;
            }
            graphView.edges.ForEach(x => {
                if (portMap.ContainsKey(x.input) && portMap.ContainsKey(x.output)) 
                    new GfuConnectionAsset().Save(portMap[x.input], portMap[x.output]);
            });

        }

        internal static void Save(this GfuNodeAsset gfuNodeAsset, GfuNode gfuNode, Dictionary<Port, GfuPortAsset> portMap){
            gfuNodeAsset.position = gfuNode.GetPosition().position;
            gfuNodeAsset.gfuNodeTypeCode = gfuNode.GetTypeByCode();
            gfuNodeAsset.inputPort = new List<GfuPortAsset>();
            gfuNodeAsset.outputPort = new List<GfuPortAsset>();
            var gfuPorts = gfuNode.GetGfuInput();
            for (var i = 0; i < gfuPorts.Count; i++){
                var gfuPortAsset = new GfuPortAsset();
                gfuPortAsset.Save(gfuPorts[i], gfuNodeAsset);
                gfuNodeAsset.inputPort.Add(gfuPortAsset);
                portMap.Add(gfuPorts[i], gfuNodeAsset.inputPort[i]);
            }
            gfuPorts = gfuNode.GetGfuOutPut();
            for (var i = 0; i < gfuPorts.Count; i++){
                var gfuPortAsset =new GfuPortAsset();
                gfuPortAsset.Save(gfuPorts[i], gfuNodeAsset);
                gfuNodeAsset.outputPort.Add(gfuPortAsset);
                portMap.Add(gfuPorts[i], gfuNodeAsset.outputPort[i]);
            }
        }

        internal static void Save(this GfuPortAsset gfuPortAsset, GfuPort gfuPort, GfuNodeAsset gfuNodeAsset){
            if (gfuPort.direction == Direction.Input)
                gfuPortAsset.portType = PortType.Input;
            else if (gfuPort.direction == Direction.Output) 
                gfuPortAsset.portType = PortType.OutPut;
            gfuPortAsset.node = gfuNodeAsset;
        }

        internal static GfuConnectionAsset Save(this GfuConnectionAsset gfuConnectionAsset, GfuPortAsset input, GfuPortAsset output){
            if(input.connections==null)input.connections=new List<GfuConnectionAsset>();
            if(output.connections==null)output.connections=new List<GfuConnectionAsset>();
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