//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  PlotFlowController.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.Graph;
using GalForUnity.Model;
using GalForUnity.Model.Plot;
using GalForUnity.System;
using GalForUnity.System.Event;
#if LIVE2D
using Live2D.Cubism.Framework.Motion;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using GraphData = GalForUnity.Graph.Data.GraphData;

namespace GalForUnity.Controller{
    // ReSharper disable all MemberCanBePrivate.Global
    /// <summary>
    /// 剧情流控制器，控制着剧情的流程，执行着剧情的流程，剧情逻辑的核心
    /// </summary>
    public class PlotFlowController : MonoBehaviour{
        // [Rename(nameof(plotMode))] public PlotMode plotMode;
        // [Rename(nameof(SceneGraph))] 
        /// <summary>
        /// 当前接受剧情流控制器管控的剧情图数据，并不是所有的剧情图在在剧情流控制器中都可以访问到
        /// </summary>
        [FormerlySerializedAs("SceneGraph")] [HideInInspector]
        public GraphData currentGraphData;
        /// <summary>
        /// 当前接受剧情流控制器管控的剧情图，并不是所有的剧情图在在剧情流控制器中都可以访问到
        /// </summary>
        public GfuGraph CurrentGraph;
        /// <summary>
        /// 当前正在执行的剧情模型
        /// </summary>
        [Rename(nameof(currentPlotModel))]
        public PlotModel currentPlotModel;

        /// <summary>
        /// 所有待执行剧情的列表引用
        /// </summary>
        public PlotModelList ReadyExecutePlotModelSet;

        /// <summary>
        /// 所有剧情的列表引用
        /// </summary>
        // public DailyStateChecker AllPlotModelSet;
        public Dictionary<string, List<PlotModel>> PlotModels = new Dictionary<string, List<PlotModel>>();

        private void OnEnable(){
            PlotModels.Add("" + PlotType.date, new List<PlotModel>());
            PlotModels.Add("" + PlotType.plot, new List<PlotModel>());
            EventCenter.GetInstance().AddEventListening("date", Date);
            EventCenter.GetInstance().OnPlotWillExecuteEvent = Plot;
            EventCenter.GetInstance().OnGraphExecutedEvent = OnGraphExecuted;
            EventCenter.GetInstance().OnGraphExecutedEvent += (x) => {
            };
            ReadyExecutePlotModelSet = PlotModelList.GetInstance();
        }

        private void Update(){
            //无时无刻检查剧情是否通过日期检查
            DailyStateChecker.GetInstance().Check(GameSystem.Data.CurrentRoleModel, GameSystem.Data.CurrentSceneModel);
            if (!currentGraphData){
                ExecuteGraph();
            }

            if (currentGraphData && !currentGraphData.isPlay){
                Debug.Log("进入剧情");
                ExecuteGraph();
            }
        }

        private void ExecuteGraph(){
            var plotModel = ReadyExecutePlotModelSet.GetPlot();
            if (plotModel){
                var invoke = EventCenter.GetInstance().OnPlotWillExecuteEvent.GetInvocationList();
                List<bool> replace = new List<bool>();
                foreach (var @delegate in invoke){
                    replace.Add( (bool)@delegate.Method.Invoke(@delegate.Target, new object[] {
                        plotModel, GameSystem.Data.CurrentRoleModel
                    }));
                }
                if (!replace.TrueForAll((b => !b))){
                    ReadyExecutePlotModelSet.PopPlot();
                } else{
                    PlotModel plotModel2 = ReadyExecutePlotModelSet.PopPlot();
                    currentPlotModel = plotModel2;
                    ExecuteGraph(plotModel2.SceneGraph.graphData);
                }
            }
        }

        /// <summary>
        /// 执行指定剧情图
        /// </summary>
        /// <param name="graphData"></param>
        /// <exception cref="ArgumentException">剧情图不是一个已经支持的类型</exception>
        public void ExecuteGraph(GraphData graphData){
            currentGraphData = graphData;
            if (currentGraphData is Graph.Data.Property.PlotItemGraphData){
                CurrentGraph = new PlotItemGraph(graphData);
                CurrentGraph.Play(this);
            } else if (currentGraphData is Graph.Data.Property.PlotFlowGraphData){
                CurrentGraph = new PlotFlowGraph(graphData);
                CurrentGraph.Play(this);
            } else{
                throw new ArgumentException("Unknown Graph,Support Type is PlotItemGraphData and PlotFlowGraphData");
            }
        }

        private void OnGraphExecuted(GfuGraph gfuGraph){
            if (gfuGraph.IsCaller(this)){
                if (currentPlotModel.plotRequire.overDay) GameSystem.Data.CurrentTime=GameSystem.Data.CurrentTime.NextDay();
                currentPlotModel = null;
                currentGraphData = null;
            }
        }
        /// <summary>
        /// 初始化剧情流控制器
        /// </summary>
        public void InitialPlowFlowController(){ }

        
        private void Date(){
            foreach (var plotModel in PlotModels["date"]){
                if (plotModel.Check(GameSystem.Data.CurrentRoleModel.roleData, GameSystem.Data.CurrentSceneModel)){
                    ReadyExecutePlotModelSet.Add(plotModel);
                }
            }
        }

        private bool Plot(PlotModel model, RoleModel roleModel){
            foreach (var plotModel in PlotModels["plot"]){
                if (plotModel != model && plotModel.plotRequire.triggerPlot == model){
                    if (plotModel.plotRequire.NormalCheck(GameSystem.Data.CurrentRoleModel.roleData, GameSystem.Data.CurrentSceneModel)){
                        CanRepetitionCheck(plotModel);
                        if (plotModel.plotRequire.triggerType == TriggerType.before){
                            ReadyExecutePlotModelSet.Insert(0, plotModel);
                            return false;
                        } else if (plotModel.plotRequire.triggerType == TriggerType.after){
                            ReadyExecutePlotModelSet.Insert(1, plotModel);
                            return false;
                        } else{
                            ReadyExecutePlotModelSet.Insert(1, plotModel);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        private void CanRepetitionCheck(PlotModel model){
            if (!model.plotRequire.repeatability){
                PlotModels["plot"].Remove(model);
                DailyStateChecker.GetInstance().CancelRegister(model.plotRequire);
            }
            if (model.plotRequire.repetitionCount != 0){
                model.plotRequire.repetitionCount--;
            } else{
                PlotModels["plot"].Remove(model);
                DailyStateChecker.GetInstance().CancelRegister(model.plotRequire);
            }
        }

        /// <summary>
        /// 执行单个剧情项，
        /// </summary>
        /// <param name="plotItem">剧情项</param>
        public bool ExecutePoltItem(PlotItem plotItem){
            // bool Played = false;
            GameSystem.Data.ShowPlotView.ShowAll(plotItem.name, plotItem.speak, plotItem.background, plotItem.audioClip); //显示所有内容
            if (plotItem.Animations!=null){
#if LIVE2D
                var cubism = GameSystem.Data.CurrentRoleModel.gameObject.GetComponent<CubismMotionController>(); //找到动画的所属的模型控制器
                for (int i = 0, length = plotItem.Animations.Count; i < length; i++){
                    if (plotItem?.Animations[i] != null){
                        if (cubism.IsPlayingAnimation(i)){ //当播放新动画时，如果没有新动画，继续之前的动画停止之前在播放的动画
                            cubism.StopAllAnimation();
                        }
                        cubism.PlayAnimation(plotItem.Animations[i], i, 3, true, 1); //并行播放所有动画
                        // Played = true;
                    }
                }
#endif
                EventCenter.GetInstance().OnPlotItemExecutedEvent.Invoke();
                return true;
            }
            return true;
        }
        
    }
}