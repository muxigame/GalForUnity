//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotItemNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-27 15:59:45
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Controller;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.GFUNode.Operation;
using GalForUnity.Graph.Operation;
using GalForUnity.Model;
using GalForUnity.Model.Plot;
using GalForUnity.System;
using GalForUnity.System.Archive;
using GalForUnity.System.Event;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GalForUnity.Graph.GFUNode.Plot{
    [NodeRename("Node/" + nameof(PlotItemNode), "剧情项编辑节点")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class PlotItemNode : BaseFieldNode{
        [NodeRename(nameof(Exit), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit;

        [NodeRename(nameof(Operation), typeof(GfuOperation), NodeDirection.Input, NodeCapacity.Multi)]
        public GfuPort Operation;

        [NodeFieldType(typeof(string), nameof(name))]
        public new string name;

        [NodeFieldType(typeof(string), nameof(speak))]
        public string speak;

        // [NodeFieldType(typeof(bool),nameof(isCanJump))]
        [Rename(nameof(isCanJump))] public bool isCanJump = true;

        // [NodeFieldType(typeof(bool),nameof(isAutoHighLight))]
        [Rename(nameof(isAutoHighLight))] public bool isAutoHighLight = true;
        [NonSerialized] private bool _isExecuting = false;

        [NodeFieldType(typeof(PlotScript), nameof(Script))]
        public PlotScript Script;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            _isExecuting = false;
#if UNITY_EDITOR
            TextField Name = InitObject(nameof(name), (attribute => new TextField() {
                label = attribute.Name,
                value = name,
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft,
                    }
                }
            }));
            Name.RegisterValueChangedCallback((delegate(ChangeEvent<string> evt){ name = evt.newValue; }));
            TextField Speak = InitObject(nameof(speak), (attribute => new TextField() {
                label = attribute.Name,
                value = speak,
                multiline = true,
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft,
                    }
                }
            }));
            Speak.RegisterValueChangedCallback((delegate(ChangeEvent<string> evt){ speak = evt.newValue; }));
            Toggle IsCanJump = InitObject(nameof(isCanJump), attribute => new Toggle() {
                label = attribute.Name,
                tooltip = "当所有节点完成操作后，才可跳过当前剧情项，通常用于播放CG或者一段剧情时使用，当取消勾选此项后，请不要连接时间节点，因为时间节点不会停止执行",
                value = isCanJump,
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft,
                    }
                }
            });
            Toggle IsAutoHighLight = InitObject(nameof(isAutoHighLight), attribute => new Toggle() {
                label = attribute.Name,
                tooltip = "当连接了RoleNode的颜色接口后，该属性不一定生效",
                value = isAutoHighLight,
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft,
                    }
                }
            });
            ObjectField Script = InitObject(nameof(this.Script), obj => new ObjectField() {
                label = obj.Name,
                tooltip = "附加运行的脚本",
                value = this.Script,
                objectType = typeof(PlotScript),
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft,
                    }
                }
            });
            IsCanJump.RegisterValueChangedCallback((delegate(ChangeEvent<bool> evt){ isCanJump = evt.newValue; }));
            IsAutoHighLight.RegisterValueChangedCallback((delegate(ChangeEvent<bool> evt){ isAutoHighLight = evt.newValue; }));
            Script.RegisterValueChangedCallback((delegate(ChangeEvent<Object> evt){ this.Script = (PlotScript) evt.newValue; }));
            RefreshExpandedState();
#endif
        }
        void ArchiveListener(ArchiveSystem.ArchiveEventType arg0){
            if (arg0 == ArchiveSystem.ArchiveEventType.ArchiveLoadStart){
                EventCenter.GetInstance().OnMouseDown.RemoveListener(OnMouseDown);
                EventCenter.GetInstance().archiveEvent.RemoveListener(ArchiveListener);
            }
        }
        
        public override RoleData Execute(RoleData roleData){
            InitScript(roleData);
            if (!_isExecuting){
                _isExecuting = true; //指示正在执行
                //TODO 还要写是否节点允许跳过的逻辑
                var gfuNodes = GetInputNodes(0);
                EventCenter.GetInstance().OnMouseDown.AddListener(OnMouseDown);
                EventCenter.GetInstance().archiveEvent.AddListener(ArchiveListener);
                GameSystem.Data.PlotFlowController.ExecutePoltItem(new PlotItem() {
                    name = this.name, speak = this.speak,
                });
                foreach (var gfuNode in gfuNodes){
                    var gfuOperationNode = ((GfuOperationNode) gfuNode);
                    if (gfuOperationNode is RoleNode roleNode){
                        roleNode.GfuOperation.OnStart += delegate(GfuOperation operation){
                            //自动高光部分代码
                            if (isAutoHighLight){
                                RoleController.AutoHighLight_S(name);
                                if (!roleNode.IsInputConnected(3)){ //如果角色节点的颜色属性已经连接了，不去尝试强行篡改，
                                    //TODO 这里以后要进行重构，RoleNode的OutPutData要转移到ContainerData中去
                                    // Debug.Log(operation.OutPutData[0].value);
                                    operation.InputData[3].value = (operation.ContainerData[0].value as RoleModel)?.Name == name ? RoleModel.HighLightColor : RoleModel.UnHighLightColor;
                                }
                            }
                        };
                    }

                    if (Script){
                        gfuOperationNode.GfuOperation.OnStart += Script.OnGfuOperationStart;
                        gfuOperationNode.GfuOperation.OnUpdate += Script.OnGfuOperationUpdate;
                    }
                    gfuOperationNode.GfuOperation.Start(1, null, gfuNode);
                    gfuOperationNode.GfuOperation.RunAllNode();
                }

                if (isAutoHighLight){
                    GfuRunOnMono.LateUpdate(-1, delegate{ RoleController.AutoHighLight_S(name); });
                }
            } else{
                // if (isCanJump){
                //     var gfuNodes =  GetInputNodes(0);
                //     foreach (var gfuNode in gfuNodes){
                //         var gfuOperationNode = ((GfuOperationNode)gfuNode);
                //         gfuOperationNode.Executed();
                //     }
                // }
            }

            return roleData;
        }

        /// <summary>
        /// 初始化附加在PlotItemNode剧情项节点的脚本，并调用节点将运行回调
        /// </summary>
        /// <param name="roleData"></param>
        private void InitScript(RoleData roleData){
            if (Script){
                if (Script.GfuNode == null) Script.GfuNode = this;
                Script.RoleData = roleData;
                Script.NodeData = this.nodeData;
                Script.PlotModel = GameSystem.Data.PlotFlowController.currentPlotModel;
                Script.GraphData = this.GfuGraph.graphData;
                Script.GfuGraph = this.GfuGraph;
                Script.OnNodeWillExecute();
            }
        }

        public void OnMouseDown(Vector2 vector2){
            if (isCanJump){
                Executed();
            } else{
                var gfuNodes = GetInputNodes(0);
                bool isOver = gfuNodes.TrueForAll(node => ((GfuOperationNode) node).GfuOperation.IsOver);
                Debug.Log(isOver);
                if (isOver){
                    Executed();
                }
            }
        }

        public override void Executed(){
            if (Script){
                Script.OnNodeExecuted();
#pragma warning disable 612
                Script.OnPlotItemExecuted();
#pragma warning restore 612
            }
            _isExecuting = false;
            EventCenter.GetInstance().archiveEvent.RemoveListener(ArchiveListener);
            EventCenter.GetInstance().OnMouseDown.RemoveListener(OnMouseDown);
            EventCenter.GetInstance().OnPlotItemExecutedEvent.Invoke();
            base.Executed();
        }

        public override void Save(){ }
    }
}