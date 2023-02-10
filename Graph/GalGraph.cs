

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.Graph.Editor.Builder;
using GalForUnity.Graph.Nodes;
using UnityEngine;

namespace GalForUnity.Graph{
    public class GalGraph{
        private readonly GalGraphAsset _sourceAsset;
        public readonly GraphProvider GraphProvider;
        private readonly GalNodeAsset _rootNode = null;

        public GalGraph(IGalGraph galGraph) : this(galGraph.GraphNode){ }

        public GalGraph(GalGraphAsset galGraphAsset) : this(galGraphAsset, new GraphProvider{
            Next = () => Input.GetMouseButtonDown(0)
        }){ }

        public GalGraph(GalGraphAsset galGraphAsset, GraphProvider graphProvider){
            _sourceAsset = galGraphAsset;
            GraphProvider = graphProvider;
            CreateRunTimeNode();
            Reset();
        }

        public GalNodeAsset CurrentNode{ get; private set; }

        public bool IsPlay{ get; private set; }

        public Dictionary<long, GalNodeAsset> RunTimeNode{ get; private set; }

        public GalNodeAsset RootNode => _rootNode ?? RunTimeNode.First(x => x.Value.runtimeNode is MainNode).Value;

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

        private async void ExecuteCirculation(GalNodeAsset galNodeAsset){
            if (galNodeAsset == null){
                IsPlay = false;
                GraphProvider.OnGraphExecuted?.Invoke(CurrentNode);
                return;
            }
            CurrentNode = galNodeAsset;
            GraphProvider.OnNodeWillExecuted?.Invoke(galNodeAsset);
            CurrentNode.runtimeNode.GalGraph = this;
            var nodeAsset = await CurrentNode.runtimeNode.OnNodeEnter(galNodeAsset);
            GraphProvider.OnNodeExecuted?.Invoke(galNodeAsset);
            ExecuteCirculation(nodeAsset);
        }

        private void CreateRunTimeNode(){
            if (_sourceAsset?.nodes==null||_sourceAsset?.nodes?.Count == 0) return;
            if (RunTimeNode                == null) RunTimeNode = new Dictionary<long, GalNodeAsset>();
            foreach (var sourceAssetNode in _sourceAsset.nodes) // var instance = Activator.CreateInstance(sourceAssetNode.Type) as GfuNode;
                // if (sourceAssetNode.gfuNodeTypeCode == NodeCode.MainNode) CurrentNode = _rootNode = instance?.RuntimeNode as MainNode;
                // if (instance == null) throw new NullReferenceException("node create failed");
                RunTimeNode.Add(sourceAssetNode.instanceID, sourceAssetNode);
        }
        

        public class NodeRuntimeProvider : INodeRuntimeProviderBase{
            private readonly GalGraph _galGraph;
            private readonly GalNodeAsset _galNodeAsset;

            public NodeRuntimeProvider(GalGraph galGraph, GalNodeAsset galNodeAsset){
                _galNodeAsset = galNodeAsset;
                _galGraph = galGraph;
            }

            public GalNodeAsset GetInputNode(int portIndex, int connectionIndex){ return GetNode(_galNodeAsset.inputPort[portIndex].connections[connectionIndex].output.node.instanceID); }

            public List<GalNodeAsset> GetInputNodes(int portIndex){
                var gfuNodes = new List<GalNodeAsset>();
                _galNodeAsset.inputPort[portIndex].connections.ForEach(x => gfuNodes.Add(GetNode(x.output.node.instanceID)));
                return gfuNodes;
            }

            public GalNodeAsset GetOutputNode(int portIndex, int connectionIndex){ return GetNode(_galNodeAsset.outputPort[portIndex].connections[connectionIndex].input.node.instanceID); }

            public List<GalNodeAsset> GetOutputNodes(int portIndex){
                var gfuNodes = new List<GalNodeAsset>();
                _galNodeAsset.outputPort[portIndex].connections.ForEach(x => gfuNodes.Add(GetNode(x.input.node.instanceID)));
                return gfuNodes;
            }

            public GalNodeAsset GetNode(long instanceID){ return _galGraph.RunTimeNode[instanceID]; }

            public bool IsInputPortConnected(int portIndex){ return _galNodeAsset.inputPort[portIndex].HasConnection; }

            public int InputPortCount(){ return _galNodeAsset.inputPort.Count; }

            public bool IsOutputPortConnected(int portIndex){ return _galNodeAsset.outputPort[portIndex].HasConnection; }

            public int OutputPortCount(){ return _galNodeAsset.outputPort.Count; }

            public int GetInputPortConnectionCount(int portIndex){ return _galNodeAsset.inputPort[portIndex].connections.Count; }

            public int GetOutputPortConnectionCount(int portIndex){ return _galNodeAsset.outputPort[portIndex].connections.Count; }
        }
    }
    public class GraphProvider{
        /// <summary>
        ///     每帧执行，该值指定图是否进入下一个Item或Option节点
        /// </summary>
        public Func<bool> Next;

        /// <summary>
        ///     图执行完毕回调，当图被停止（非暂停）同样会触发回调
        /// </summary>
        public Action<GalNodeAsset> OnGraphExecuted;

        /// <summary>
        ///     节点执行完毕回调
        /// </summary>
        public Action<GalNodeAsset> OnNodeExecuted;

        /// <summary>
        ///     节点将执行回调
        /// </summary>
        public Action<GalNodeAsset> OnNodeWillExecuted;

        /// <summary>
        ///     PlotItemNode被执行回调
        /// </summary>
        public Action<string, string> OnSpeak;
    }
}