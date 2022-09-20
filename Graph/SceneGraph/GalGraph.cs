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
using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Graph.SceneGraph{
    public class GalGraph{
        private readonly Dictionary<long, GfuNodeData> _graphData;
        public const string PortViewName = "______PortData";
        private readonly GfuGraphAsset _sourceAsset;
        private GraphProvider _graphProvider;

        public GalGraph(IGalGraph galGraph) : this(galGraph.GraphNode, galGraph.GraphData.nodeDatas){ }

        public GalGraph(GfuGraphAsset gfuGraphAsset, Dictionary<long, GfuNodeData> graphData) : this(gfuGraphAsset, graphData, new GraphProvider {
            Click = () => Input.GetMouseButtonDown(0)
        }){ }

        public GalGraph(GfuGraphAsset gfuGraphAsset, Dictionary<long, GfuNodeData> graphData, GraphProvider graphProvider){
            _sourceAsset = gfuGraphAsset;
            _graphData = graphData;
            _graphProvider = graphProvider;
            CreateRunTimeNode();
        }

        public GfuNode CurrentNode{ get; private set; }

        public bool IsPlay{ get; private set; }

        private MainNode _rootNode=null;

        public Dictionary<long, GfuNode> RunTimeNode{ get; private set; }

        public MainNode RootNode => _rootNode ?? RunTimeNode.First(x => x.Value is MainNode).Value as MainNode;

        public void Play(){
            if (IsPlay) return;
            IsPlay = true;
            ExecuteCirculation(CurrentNode);
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
            CurrentNode.OnExecuted = null;
        }

        private void ExecuteCirculation(GfuNode gfuNode){
            CurrentNode = gfuNode;
            CurrentNode.OnExecuted += x => GfuRunOnMono.Update(() => ExecuteCirculation(x));
            GameSystem.Data.CurrentRoleModel.roleData = CurrentNode.Execute(GameSystem.Data.CurrentRoleModel.roleData);
        }

        private void CreateRunTimeNode(){
            if (_sourceAsset == null || _sourceAsset.nodes?.Count == default) return;
            if (RunTimeNode == null) RunTimeNode = new Dictionary<long, GfuNode>();
            foreach (var sourceAssetNode in _sourceAsset.nodes){
                var instance = Activator.CreateInstance(sourceAssetNode.Type) as GfuNode;
                if (sourceAssetNode.gfuNodeTypeCode == NodeCode.MainNode) CurrentNode = _rootNode = instance as MainNode;
                if (instance                        == null) throw new NullReferenceException("node create failed");
                instance.InitWithGfuNodeData(sourceAssetNode,_graphData[sourceAssetNode.instanceID], new NodeRuntimeProvider(this,sourceAssetNode));
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