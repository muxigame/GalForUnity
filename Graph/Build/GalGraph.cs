//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalForUnityGraph.cs
//
//        Created by 半世癫(Roc) at 2022-04-16 00:18:12
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Build;
using GalForUnity.System;
using UnityEngine;
using MainNode = GalForUnity.Graph.Nodes.Runtime.MainNode;

namespace GalForUnity.Graph.SceneGraph{
    public class GalGraph{
        private readonly GfuGraphAsset _sourceAsset;
        private GraphProvider _graphProvider;

        public GalGraph(IGalGraph galGraph) : this(galGraph.GraphNode){ }

        public GalGraph(GfuGraphAsset gfuGraphAsset) : this(gfuGraphAsset, new GraphProvider {
            Click = () => Input.GetMouseButtonDown(0)
        }){ }

        public GalGraph(GfuGraphAsset gfuGraphAsset, GraphProvider graphProvider){
            _sourceAsset = gfuGraphAsset;
            _graphProvider = graphProvider;
            CreateRunTimeNode();
        }

        public RuntimeNode CurrentNode{ get; private set; }

        public bool IsPlay{ get; private set; }

        private RuntimeNode _rootNode=null;

        public Dictionary<long, GfuNode> RunTimeNode{ get; private set; }

        public RuntimeNode RootNode => _rootNode ?? RunTimeNode.First(x => x.Value.RuntimeNode is MainNode).Value.RuntimeNode as MainNode;

        public void Play(){
            if (IsPlay) return;
            IsPlay = true;
            ExecuteCirculation(null);
        }

        public void Reset(){
            CurrentNode = RootNode;
            IsPlay = false;
        }

        public void Stop(){
            Pause();
            Reset();
        }

        public void Pause(){
            IsPlay = false;
        }

        private void ExecuteCirculation(GfuNodeAsset gfuNodeAsset){
            CurrentNode = gfuNodeAsset.runtimeNode;
            ExecuteCirculation(CurrentNode.Execute(gfuNodeAsset));
        }

        private void CreateRunTimeNode(){
            if (_sourceAsset == null || _sourceAsset.nodes?.Count == default) return;
            if (RunTimeNode == null) RunTimeNode = new Dictionary<long, GfuNode>();
            foreach (var sourceAssetNode in _sourceAsset.nodes){
                var instance = Activator.CreateInstance(sourceAssetNode.Type) as GfuNode;
                if (sourceAssetNode.gfuNodeTypeCode == NodeCode.MainNode) CurrentNode = _rootNode = instance?.RuntimeNode as MainNode;
                if (instance                        == null) throw new NullReferenceException("node create failed");
                RunTimeNode.Add(instance.instanceID, instance);
            }
        }

        public struct GraphProvider{
            /// <summary>
            ///     每帧执行，该值指定图是否进入下一个Item或Option节点
            /// </summary>
            public Func<bool> Click;

            /// <summary>
            ///     图执行完毕回调，当图被停止（非暂停）同样会触发回调
            /// </summary>
            public Action<GfuNode> OnGraphExecuted;

            /// <summary>
            ///     节点执行完毕回调
            /// </summary>
            public Action<GfuNode> OnNodeExecuted;

            /// <summary>
            ///     节点将执行回调
            /// </summary>
            public Action<GfuNode> OnNodeWillExecuted;

            /// <summary>
            ///     PlotItemNode被执行回调
            /// </summary>
            public Action<string, string> OnSpeak;
        }
        
        public class NodeRuntimeProvider : INodeRuntimeProviderBase{
            private readonly GfuNodeAsset _gfuNodeAsset;
            private readonly GalGraph _galGraph;
            public NodeRuntimeProvider(GalGraph galGraph,GfuNodeAsset gfuNodeAsset){
                _gfuNodeAsset = gfuNodeAsset;
                _galGraph = galGraph;
            }

            public GfuNode GetInputNode(int portIndex, int connectionIndex){
                return GetNode(_gfuNodeAsset.inputPort[portIndex].connections[connectionIndex].output.node.instanceID);
            }

            public List<GfuNode> GetInputNodes(int portIndex){
                var gfuNodes = new List<GfuNode>();
                _gfuNodeAsset.inputPort[portIndex].connections.ForEach((x)=>gfuNodes.Add(GetNode(x.output.node.instanceID)));
                return gfuNodes;
            }

            public GfuNode GetOutputNode(int portIndex, int connectionIndex){
                return GetNode(_gfuNodeAsset.outputPort[portIndex].connections[connectionIndex].input.node.instanceID);
            }

            public List<GfuNode> GetOutputNodes(int portIndex){
                var gfuNodes = new List<GfuNode>();
                _gfuNodeAsset.outputPort[portIndex].connections.ForEach((x)=>gfuNodes.Add(GetNode(x.input.node.instanceID)));
                return gfuNodes;
            }

            public GfuNode GetNode(long instanceID){
                return _galGraph.RunTimeNode[instanceID];
            }

            public bool IsInputPortConnected(int portIndex){
                return _gfuNodeAsset.inputPort[portIndex].HasConnection;
            }

            public int InputPortCount(){
                return _gfuNodeAsset.inputPort.Count;
            }
            
            public bool IsOutputPortConnected(int portIndex){
                return _gfuNodeAsset.outputPort[portIndex].HasConnection;
            }

            public int OutputPortCount(){
                return _gfuNodeAsset.outputPort.Count;
            }

            public int GetInputPortConnectionCount(int portIndex){
                return _gfuNodeAsset.inputPort[portIndex].connections.Count;
            }

            public int GetOutputPortConnectionCount(int portIndex){
                return _gfuNodeAsset.outputPort[portIndex].connections.Count;
            }
            
        }
    }
    
}