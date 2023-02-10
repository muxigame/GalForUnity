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
using GalForUnity.Core.External;
using GalForUnity.Graph.Editor.Builder;
using GalForUnity.Graph.Editor.Nodes;
using GalForUnity.Graph.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace GalForUnity.Graph{
    public interface IGalGraph{
        public GalGraphAsset GraphNode{ get; set; }

        // ReSharper disable once InconsistentNaming
        public string name{ get; set; }
        public int GetInstanceID();
    }

    [CreateAssetMenu(menuName = "GalForUnity/AssetGraph", fileName = "AssetGraph.asset", order = 2)]
    public class AssetGraph : ScriptableObject, IGalGraph{
        [SerializeField] private GalGraphAsset graphNode;

        [SerializeField]
        public GalGraphAsset GraphNode{
            get => graphNode;
            set => graphNode = value;
        }
    }

    public static class GfuNodeStaticMethod{
        public static bool HasConnection(this List<GalPortAsset> gfuPortAssets){ return !gfuPortAssets.TrueForAll(x => !x.HasConnection); }
#if UNITY_EDITOR

        internal static void Save(this SceneGraph sceneGraph, GalGraphView graphView){
            var save = sceneGraph.GraphNode.Save(graphView).ToList();
        }

        internal static void Save(this AssetGraph assetGraph, GalGraphView graphView){
            var save = assetGraph.GraphNode.Save(graphView);
            AssetDatabase.SetMainObject(assetGraph, AssetDatabase.GetAssetPath(assetGraph));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static IEnumerable<RuntimeNode> Save(this GalGraphAsset assetGraph, GalGraphView graphView){
            if (assetGraph.instanceID == -1) assetGraph.instanceID = InstanceIDUtil.CreateInstanceID();
            var portMap = new Dictionary<Port, GalPortAsset>();
            if (assetGraph.nodes == null) assetGraph.nodes = new List<GalNodeAsset>();
            assetGraph.nodes.RemoveAll(x => !graphView.Nodes.ContainsKey(x.instanceID));
            foreach (var graphViewNode in graphView.Nodes){
                var nodeByInstanceID = assetGraph.GetNodeByInstanceID(graphViewNode.Key);
                GalNodeAsset galNodeAsset;
                if (nodeByInstanceID != null){
                    galNodeAsset = nodeByInstanceID;
                } else{
                    galNodeAsset = new GalNodeAsset();
                    assetGraph.nodes.Add(galNodeAsset);
                }
                galNodeAsset.Save(graphViewNode.Value, portMap);
                yield return galNodeAsset.runtimeNode = graphViewNode.Value.RuntimeNode;
                galNodeAsset.instanceID = graphViewNode.Key;
            }
            graphView.edges.ForEach(x => {
                if (portMap.ContainsKey(x.input) && portMap.ContainsKey(x.output)) new GfuConnectionAsset().Save(portMap[x.input], portMap[x.output]);
            });
        }

        internal static void Save(this GalNodeAsset galNodeAsset, GfuNode gfuNode, Dictionary<Port, GalPortAsset> portMap){
            foreach (var keyValuePair in gfuNode.OnSavePort(galNodeAsset)) portMap.Add(keyValuePair.port, keyValuePair.gfuPortAsset);
        }

        internal static void Save(this GalPortAsset galPortAsset, GalPort galPort, GalNodeAsset galNodeAsset){
            galPortAsset.portName = galPort.name;
            if (galPort.direction == UnityEditor.Experimental.GraphView.Direction.Input)
                galPortAsset.Direction = Direction.Input;
            else if (galPort.direction == UnityEditor.Experimental.GraphView.Direction.Output) 
                galPortAsset.Direction = Direction.Output;
            galPortAsset.node = galNodeAsset;
            galPortAsset.portType = galPort.portType;
            switch (galPort.capacity)
            {
                case Port.Capacity.Single:
                    galPortAsset.Capacity = Capacity.Single;
                    break;
                case Port.Capacity.Multi:
                    galPortAsset.Capacity = Capacity.Multi;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (galPort.orientation)
            {
                case UnityEditor.Experimental.GraphView.Orientation.Horizontal:
                    galPortAsset.Orientation = Orientation.Horizontal;
                    break;
                case UnityEditor.Experimental.GraphView.Orientation.Vertical:
                    galPortAsset.Orientation = Orientation.Vertical;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static GfuConnectionAsset Save(this GfuConnectionAsset gfuConnectionAsset, GalPortAsset input, GalPortAsset output){
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