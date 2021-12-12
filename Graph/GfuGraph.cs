//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuGraph.cs
//
//        Created by 半世癫(Roc) at 2021-01-17 15:55:45
//
//======================================================================


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using GalForUnity.Controller;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.Data.Property;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.GFUNode.Plot;
using GalForUnity.System;
using GalForUnity.System.Archive;
using GalForUnity.System.Archive.Data;
using GalForUnity.System.Event;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GalForUnity.Graph{
    /// <summary>
    /// GalForUnity的图，他包含两个子类项图和流图。该类的使用方法是
    /// new GfuGraph(myGraphData).Play();或者new GfuGraph(myGraphData).Execute();
    /// 但是请不要直接new GfuGraph正确的方法是实例化他的子类
    /// 默认情况下,图会响应鼠标点击事件,自动执行,也可以改写程序响应自定义事件
    /// 这样便能在运行时创建一个图并且播放
    /// </summary>
    public class GfuGraph : EditorGraph,IDisposable{
        /// <summary>
        /// 指示图是否在执行
        /// </summary>
        public bool isPlay;
        /// <summary>
        /// 指示是否存在下一个节点
        /// </summary>
        public bool HasNext => _currentNode?.HasNext() ?? false;
        public GfuNode CurrentNode => _currentNode;
        
        private GfuNode _currentNode;
        
        private object _caller = null;
        
        private GraphCallChain graphCallChain;

        /// <summary>
        /// 用指定图数据创建一个图
        /// </summary>
        /// <param name="graphData"></param>
        public GfuGraph(Data.GraphData graphData = null){
            if (graphData) Init(graphData);
        }
        
        public GfuGraph(GraphCallChain graphCallChain){
            GfuGraph gfuGraph=null;
            if (graphCallChain != null){
                var info = graphCallChain.Pop();
                Init((GraphData) info.callerGraphData);
                gfuGraph = this;
                while (graphCallChain.Next(out CallInfo callInfo)){
                    Debug.Log(callInfo);
                    GfuGraph graph=null;
                    if(callInfo.callerGraphData is PlotItemGraphData plotItemGraphData) graph = new PlotItemGraph(plotItemGraphData);
                    if(callInfo.callerGraphData is PlotFlowGraphData plotFlowGraphData) graph = new PlotFlowGraph(plotFlowGraphData);
                    var gfuNode = graph.GetNode((NodeData) callInfo.callerNodeData);
                    graph._currentNode = gfuNode;
                    if (graph._currentNode is PlotGraphNode plotGraphNode){
                        plotGraphNode.Recover(gfuGraph);//恢复保存的数据
                        gfuGraph._caller = plotGraphNode;//保存调用者信息
                    }
                    gfuNode.OnExecuted = graph.Execute;//添加节点执行完毕后的回调
                    gfuGraph = graph;//保存当前数据进行下次迭代
                }
                Execute((NodeData) info.callerNodeData);//运行当前节点
            }else{
                Debug.LogError("graphCallChain is"+graphCallChain);
            }
        }

        public GfuGraph(string path) : base(path){ }

        ~GfuGraph(){
            ReleaseUnmanagedResources();
        }
        /// <summary>
        /// 指示图的调用者，由GalForUnity手动赋值，用户调用该值可能为空
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public bool IsCaller(object caller){ return _caller == caller; }

        /// <summary>
        /// 获得该图的调用链
        /// </summary>
        /// <returns></returns>
        public GraphCallChain GetGraphCallChain(){
            graphCallChain=new GraphCallChain();
            GfuGraph gfuGraph = this;
            graphCallChain.Add(gfuGraph.GetCallInfo());
            while (!ReferenceEquals(gfuGraph._caller, GameSystem.Data.PlotFlowController)){
                if(gfuGraph._caller==null) break;
                if (gfuGraph._caller is GfuNode gfuNode){
                    graphCallChain.Add(gfuNode.GfuGraph.GetCallInfo());
                    gfuGraph = gfuNode.GfuGraph;
                } else{
                    Debug.LogError("未知调用者");
                    break;
                }
            }
            return graphCallChain;
        }

        private CallInfo GetCallInfo(){
            return new CallInfo() {
                callerGraphData = graphData, callerNodeData = _currentNode.nodeData
            };
        }
        
        /// <summary>
        /// 指定剧情图播放某个图数据
        /// </summary>
        /// <param name="caller">播放图的调用者</param>
        public void Play(object caller = null){
            if (caller == null)
                this._caller = this;
            else{
                this._caller = caller;
            }
            graphData.isPlay = isPlay = true;
            if(Root == null) InitNode();
            Execute();
        }

        /// <summary>
        /// 开始执行当前剧情图中存在的图数据(从根节点开始执行)
        /// </summary>
        public void Execute(){
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying){
                Debug.LogError(GfuLanguage.ParseLog("It is not allowed to execute the diagram without playing, please run the scene first"));
                return;
            }
#endif
            if(Root == null) InitNode();
            if (Root != null){
                _currentNode = Root;
                Root.OnExecuted = Execute;
                Root.Executed();
            } else{
                throw new NullReferenceException("Root Node Not Found!");
            }
        }

        /// <summary>
        /// 供图内部执行下一个节点的虚方法
        /// </summary>
        /// <param name="nodeObj"></param>
        public override void Execute(GfuNode nodeObj){
            // Debug.Log(GetGraphCallChain());
            base.Execute(nodeObj);
            if (_currentNode!=null&&_currentNode!=RootNode){
                _currentNode.OnExecuted = null;
                _currentNode.Executed();
                _currentNode = null;
            }
            _currentNode = nodeObj;
            if (nodeObj != null){
                nodeObj.OnExecuted = Execute;
                // GameSystem.GraphData.currentPlotItemGraphData = this;
                GameSystem.Data.CurrentRoleModel.roleData = nodeObj.Execute(GameSystem.Data.CurrentRoleModel.roleData);
            } else{
                GfuRunOnMono.LateUpdate(Int32.MaxValue, () => {
                    graphData.isPlay = isPlay = false;
                    EventCenter.GetInstance().OnGraphExecutedEvent(this);
                });
            }
        }
        public void Execute(NodeData nodeData) => Execute(GetNode(nodeData));
        
        /// <summary>
        /// 通过默认端口号执行下一个默认节点
        /// </summary>
        /// <returns>是否存在下下一个节点</returns>
        public bool Next(){
            return Next(0);
        }
        /// <summary>
        /// 通过指定端口号执行下一个默认节点
        /// </summary>
        /// <param name="index">当前node的output的端口号</param>
        /// <returns>是否存在下下一个节点</returns>
        /// <exception cref="NullReferenceException">未找到下一个node是触发空指针异常</exception>
        public bool Next(int index){
            if (HasNext){
                GfuRunOnMono.FixedUpdate(delegate{
                    var gfuNode = _currentNode.Next(index);
                    _currentNode = gfuNode;
                    if (gfuNode==null) throw new NullReferenceException($"There is no default node to connect port {index} next");
                    gfuNode.OnExecuted = Execute;
                    //TODO 可以在这里显示剧情进度
                    GameSystem.Data.CurrentRoleModel.roleData = gfuNode.Execute(GameSystem.Data.CurrentRoleModel.roleData);
                });
            } else{
                throw new NullReferenceException("There is no default node to connect to the default port next");
            }
            return HasNext;
        }

        /// <summary>
        /// 通过gfuInstanceID获得一个节点,找不到时返回null
        /// </summary>
        public GfuNode GetNode(long gfuInstance){
            if (CreateNodeDataset.ContainsKey(gfuInstance))
                return CreateNodeDataset[gfuInstance];
            else
                return null;
        }
        /// <summary>
        /// 通过gfuInstanceID获得一个节点,找不到时返回null
        /// </summary>
        public GfuNode GetNode(NodeData nodeData){
            return GetNode(nodeData.instanceID);
        }
        /// <summary>
        /// 需要注意的是，在编辑器模式中，初始化方法会解析所有节点，但是
        /// </summary>
        public override void Init(Data.GraphData graphData){
            base.graphData = graphData;
            NodeDataset = new List<NodeData>();
            EventCenter.GetInstance().archiveEvent.AddListener((x) => {
                if (x == ArchiveSystem.ArchiveEventType.ArchiveLoadStart){
                    this.Dispose();
                }
            });
            CreateNodeDataset = new Dictionary<long, GfuNode>();
            if (graphData != null && graphData.Nodes != null && graphData.Nodes.Count != 0){
                foreach (var t in graphData.Nodes){
                    NodeDataset.Add(t);
                }
            }
            base.Init(graphData);
            //编辑器下创建空图base.Init会默认创建一个MainNode节点，但是运行下创建空图会产生ArgumentException异常。
            if(Root==null) InitNode();
        }
        /// <summary>
        /// 初始化所有的Node,初始化成功就意味着可以Play了
        /// </summary>
        /// <exception cref="ArgumentException">节点数量为空或者节点数据不存在</exception>
        /// <exception cref="NullReferenceException"></exception>
        private void InitNode(){
            if(NodeDataset == null || NodeDataset.Count == 0) throw new ArgumentException("An attempt was made to initialize all nodes, but the nodes are empty");
            foreach (var nodeData in NodeDataset){
                if(!nodeData) continue;
                if(!CreateNodeDataset.ContainsKey(nodeData.instanceID))
                    CreateNodeDataset.Add(nodeData.instanceID,(GfuNode) Activator.CreateInstance(Type.GetType(nodeData.type) ?? typeof(GfuNode)));
            }
            foreach (var keyValuePair in CreateNodeDataset){
                var nodeData = NodeDataset.Find(x => {
                    if (!x) return false;
                    return x.instanceID == keyValuePair.Key;
                });
                if (!nodeData) throw new NullReferenceException($"The node whose ID is {keyValuePair.Key} does not exist");
                if (nodeData.type == typeof(MainNode).ToString()){
                    _currentNode = Root = (MainNode) keyValuePair.Value;
                }
                keyValuePair.Value.nodeData = nodeData;
                keyValuePair.Value.GfuGraph = this;
                keyValuePair.Value.Init(nodeData);
            }
        }
        // [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        // protected GfuGraph(SerializationInfo info, StreamingContext context){
        //     Type type=GetType();
        //     type.GetField(nameof(isPlay)).SetValue(this,info.GetValue(nameof(isPlay), typeof(bool)));
        //     type.GetField(nameof(_currentNode)).SetValue(this,info.GetValue(nameof(_currentNode), typeof(object)));
        //     var value = info.GetValue(nameof(_caller), typeof(object));
        //     if (value is string memoryAddress){
        //         if (new Regex(@"\[Memory\.[\s\.]+\]").IsMatch(memoryAddress)){
        //             var substring = memoryAddress.Substring(1, memoryAddress.Length - 2);
        //         }
        //         type.GetField(nameof(_caller)).SetValue(this,typeof(string));
        //     }
        //     type.GetField(nameof(_caller)).SetValue(this,typeof(object));
        // }
        // public override void GetObjectData(SerializationInfo info, StreamingContext context){
        //     info.AddValue(nameof(isPlay),isPlay);
        //     info.AddValue(nameof(_currentNode),_currentNode);
        //     if (_caller is PlotFlowController){
        //         info.AddValue(nameof(_caller),"[Memory.GameSystem.Data.PlotFlowController]");
        //     } else{
        //         info.AddValue(nameof(_caller),_caller);
        //     }
        //     
        //     base.GetObjectData(info, context);
        // }

        private void ReleaseUnmanagedResources(){
            _currentNode.OnExecuted = null;
            NodeDataset.Clear();
            CreateNodeDataset.Clear();
        }

        public void Dispose(){
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}