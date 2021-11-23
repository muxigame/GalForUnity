//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ActionModel.cs
//
//        Created by 半世癫(Roc) at 2021-01-03 12:25:07
//
//======================================================================
using System;
using GalForUnity.Attributes;
using GalForUnity.Model.Plot;
using GalForUnity.Model.Scene;
using GalForUnity.System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace GalForUnity.Model{
    /// <summary>
    /// ActionModel游戏中剧情流结束后的活动类，负责各种游戏逻辑的跳转和调用
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    [Obsolete]
    public class ActionModel : MonoBehaviour
    {
        public enum ActionModelType{
            [Rename(nameof(None))]
            None,
            [Rename(nameof(GotoScene))]
            GotoScene,
            [Rename(nameof(JumpPlot))]
            JumpPlot,
            [Rename(nameof(GrowUp))]
            GrowUp,
            [Rename(nameof(ChangePlotProbability))]
            ChangePlotProbability,
            [Rename(nameof(Custom))]
            Custom,
        }
        [Rename(nameof(actionModelType))]
        public ActionModelType actionModelType = ActionModelType.None;
        [RenameInEditor(nameof(directionSceneModel))]
        public SceneModel directionSceneModel;
        [RenameInEditor(nameof(directionPlotModel))]
        public PlotModel directionPlotModel;
        [RenameInEditor(nameof(roleData))]
        public RoleData roleData;
        [RenameInEditor(nameof(customEvent))]
        public UnityEvent<PlotModel,SceneModel> customEvent=new UnityEvent<PlotModel, SceneModel>();
        [RenameInEditor(nameof(customEventNoParam))]
        public UnityEvent customEventNoParam=new UnityEvent();
        [RenameInEditor(nameof(changeProbability))]
        public int changeProbability;
        
        private void Awake(){
            customEvent = customEvent ?? new UnityEvent<PlotModel, SceneModel>();
            customEventNoParam = customEventNoParam ?? new UnityEvent();
        }
        
        /// <summary>
        /// 自定义方法的执行不依赖于枚举，当有事件注册那必定会执行
        /// </summary>
        /// <param name="plotModel"></param>
        /// <param name="sceneModel"></param>
        public virtual void DoAction(PlotModel plotModel,SceneModel sceneModel){
            switch (actionModelType){
                case ActionModelType.GotoScene:
                    GotoScene();
                    break;
                case ActionModelType.JumpPlot:
                    JumpPlot(ref plotModel);
                    break;
                case ActionModelType.GrowUp:
                    GrowUp(roleData);
                    break; 
                case ActionModelType.ChangePlotProbability:
                    ChangeProbability(directionPlotModel,changeProbability);
                    break;
            }
        
            if (customEvent != null||customEventNoParam != null){
                ExecuteCustom(GameSystem.Data.PlotFlowController.currentPlotModel,GameSystem.Data.CurrentSceneModel);
            }
        }
        
        public virtual void DoAction(PlotModel plotModel){
            DoAction(plotModel,GameSystem.Data.CurrentSceneModel);
        }
        
        /// <summary>
        /// 以默认操作执行活动
        /// </summary>
        public virtual void DoAction(){
            DoAction(GameSystem.Data.PlotFlowController.currentPlotModel);
        }
        
        public void GotoScene(){
            GameSystem.Data.CurrentSceneModel = directionSceneModel ? directionSceneModel : GameSystem.Data.CurrentSceneModel;
        }
        
        public void GrowUp(RoleData otherRoleData){
            GameSystem.Data.CurrentRoleModel.Add(otherRoleData);
        }
        
        public void JumpPlot([NotNull] ref PlotModel plotModel){
            if (plotModel == null) throw new ArgumentNullException(nameof(plotModel));
            plotModel = directionPlotModel;
        }
        
        public void ChangeProbability(PlotModel plotModel,float probability){
            if (plotModel == null) throw new ArgumentNullException(nameof(plotModel));
            plotModel.plotRequire.plotProbability+=probability;
        }
        public void ExecuteCustom([NotNull] PlotModel plotModel,[NotNull] SceneModel sceneModel){
            customEvent?.Invoke(plotModel,sceneModel);
            customEventNoParam?.Invoke();
        }
        
    }
}
