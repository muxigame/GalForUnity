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
using GalForUnity.Graph.Nodes.Runtime;
using UnityEngine;

namespace GalForUnity.Graph.SceneGraph{
    public class GalGraph{
        private readonly GfuGraphAsset _sourceAsset;
        private readonly GraphProvider _graphProvider;
        private readonly GfuNodeAsset _rootNode = null;

        public GalGraph(IGalGraph galGraph) : this(galGraph.GraphNode){ }

        public GalGraph(GfuGraphAsset gfuGraphAsset) : this(gfuGraphAsset, new GraphProvider{
            Click = () => Input.GetMouseButtonDown(0)
        }){ }

        public GalGraph(GfuGraphAsset gfuGraphAsset, GraphProvider graphProvider){
            _sourceAsset = gfuGraphAsset;
            _graphProvider = graphProvider;
            CreateRunTimeNode();
        }

        public GfuNodeAsset CurrentNode{ get; private set; }

        public bool IsPlay{ get; private set; }

        public Dictionary<long, GfuNodeAsset> RunTimeNode{ get; private set; }

        public GfuNodeAsset RootNode => _rootNode ?? RunTimeNode.First(x => x.Value.runtimeNode is MainNode).Value;

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

        public void Pause(){ IsPlay = false; }

        private void ExecuteCirculation(GfuNodeAsset gfuNodeAsset){
            if (gfuNodeAsset == null){
                _graphProvider.OnGraphExecuted.Invoke(CurrentNode);
                return;
            }
            CurrentNode = gfuNodeAsset;
            _graphProvider.OnNodeWillExecuted.Invoke(gfuNodeAsset);
            var nodeAsset = CurrentNode.runtimeNode.Execute(gfuNodeAsset);
            _graphProvider.OnNodeExecuted.Invoke(gfuNodeAsset);
            ExecuteCirculation(nodeAsset);
        }

        private void CreateRunTimeNode(){
            if (_sourceAsset?.nodes?.Count == null) return;
            if (RunTimeNode                == null) RunTimeNode = new Dictionary<long, GfuNodeAsset>();
            foreach (var sourceAssetNode in _sourceAsset.nodes) // var instance = Activator.CreateInstance(sourceAssetNode.Type) as GfuNode;
                // if (sourceAssetNode.gfuNodeTypeCode == NodeCode.MainNode) CurrentNode = _rootNode = instance?.RuntimeNode as MainNode;
                // if (instance == null) throw new NullReferenceException("node create failed");
                RunTimeNode.Add(sourceAssetNode.instanceID, sourceAssetNode);
        }

        public struct GraphProvider{
            /// <summary>
            ///     每帧执行，该值指定图是否进入下一个Item或Option节点
            /// </summary>
            public Func<bool> Click;

            /// <summary>
            ///     图执行完毕回调，当图被停止（非暂停）同样会触发回调
            /// </summary>
            public Action<GfuNodeAsset> OnGraphExecuted;

            /// <summary>
            ///     节点执行完毕回调
            /// </summary>
            public Action<GfuNodeAsset> OnNodeExecuted;

            /// <summary>
            ///     节点将执行回调
            /// </summary>
            public Action<GfuNodeAsset> OnNodeWillExecuted;

            /// <summary>
            ///     PlotItemNode被执行回调
            /// </summary>
            public Action<string, string> OnSpeak;
        }

        public class NodeRuntimeProvider : INodeRuntimeProviderBase{
            private readonly GalGraph _galGraph;
            private readonly GfuNodeAsset _gfuNodeAsset;

            public NodeRuntimeProvider(GalGraph galGraph, GfuNodeAsset gfuNodeAsset){
                _gfuNodeAsset = gfuNodeAsset;
                _galGraph = galGraph;
            }

            public GfuNodeAsset GetInputNode(int portIndex, int connectionIndex){ return GetNode(_gfuNodeAsset.inputPort[portIndex].connections[connectionIndex].output.node.instanceID); }

            public List<GfuNodeAsset> GetInputNodes(int portIndex){
                var gfuNodes = new List<GfuNodeAsset>();
                _gfuNodeAsset.inputPort[portIndex].connections.ForEach(x => gfuNodes.Add(GetNode(x.output.node.instanceID)));
                return gfuNodes;
            }

            public GfuNodeAsset GetOutputNode(int portIndex, int connectionIndex){ return GetNode(_gfuNodeAsset.outputPort[portIndex].connections[connectionIndex].input.node.instanceID); }

            public List<GfuNodeAsset> GetOutputNodes(int portIndex){
                var gfuNodes = new List<GfuNodeAsset>();
                _gfuNodeAsset.outputPort[portIndex].connections.ForEach(x => gfuNodes.Add(GetNode(x.input.node.instanceID)));
                return gfuNodes;
            }

            public GfuNodeAsset GetNode(long instanceID){ return _galGraph.RunTimeNode[instanceID]; }

            public bool IsInputPortConnected(int portIndex){ return _gfuNodeAsset.inputPort[portIndex].HasConnection; }

            public int InputPortCount(){ return _gfuNodeAsset.inputPort.Count; }

            public bool IsOutputPortConnected(int portIndex){ return _gfuNodeAsset.outputPort[portIndex].HasConnection; }

            public int OutputPortCount(){ return _gfuNodeAsset.outputPort.Count; }

            public int GetInputPortConnectionCount(int portIndex){ return _gfuNodeAsset.inputPort[portIndex].connections.Count; }

            public int GetOutputPortConnectionCount(int portIndex){ return _gfuNodeAsset.outputPort[portIndex].connections.Count; }
        }
    }
}