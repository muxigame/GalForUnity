//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  DailyStateChecker.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Model;
using GalForUnity.Model.Plot;
using GalForUnity.Model.Scene;
using GalForUnity.System;
using GalForUnity.System.Event;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GalForUnity.Controller {
	/// <summary>
	/// 所有剧情的集合，场景中所有的PlotModel都会被注册到这里进行验证，检测器会每天检查一次剧情将满足条件的剧情提交到待执行列表。除此之外，当系统角色，系统场景被修改时，也会触发检查
	/// </summary>
	[Serializable]
	public sealed class DailyStateChecker :IDisposable{
		private static DailyStateChecker CreateInstance()
		{
			return new DailyStateChecker();
		}

		private static DailyStateChecker _dailyStateChecker;
		[SerializeField]
		private Dictionary<SceneModel, List<PlotModel>> plotsModel;

		public static DailyStateChecker GetInstance()
		{
			return _dailyStateChecker ?? (_dailyStateChecker = CreateInstance());
		}

		private DailyStateChecker(){
			//EventCenter.GetInstance().DayOverEvent=null;
			plotsModel = new Dictionary<SceneModel, List<PlotModel>>();
			EventCenter.GetInstance().DayOverEvent += (x) => {
				Check(GameSystem.Data.CurrentRoleModel, GameSystem.Data.CurrentSceneModel);//当一天结束触发检测
			};
			EventCenter.GetInstance().RoleStateChangeEvent += (currentRoleData) => {//当角色数据变更触发检测
				Check(currentRoleData, GameSystem.Data.CurrentSceneModel);
			};
			EventCenter.GetInstance().SceneStateChangeEvent += (currentSceneModel) => {
				Check(GameSystem.Data.CurrentRoleModel, currentSceneModel);//当场景数据变更触发检测
			};
		}
		
		public Dictionary<SceneModel, List<PlotModel>> GetPlotsModel(){
			return plotsModel;
		}

		public void RegisterPlotModel(SceneModel sceneModel, PlotModel plotModel){
			GameSystem.Data.PlotFlowController.PlotModels[plotModel.plotRequire.plotType+""].Add(plotModel);
			// Debug.Log("事件注册:" + plotModel.plotRequire.plotType+" 来自游戏对象："+plotModel.gameObject.name);
			if(plotsModel.ContainsKey(sceneModel)){
				plotsModel[sceneModel].Add(plotModel);
			}else{
				plotsModel.Add(sceneModel,new List<PlotModel>());
				plotsModel[sceneModel].Add(plotModel);
			}
			//Check(GameSystem.Data.CurrentRoleModel,GameSystem.Data.CurrentSceneModel);//用当前角色数值和场景数值判断是否触发剧情
		}
		public void Check(RoleData roleData,SceneModel sceneModel){
			Check(roleData.RoleModel, sceneModel ? sceneModel : SceneModel.NULL);
		}
		public void Check(RoleModel roleModel,SceneModel sceneModel){
			// Debug.Log("DailyStateCheckerSize:"+_plotsModel.Count);
			if (plotsModel.TryGetValue(sceneModel,out List<PlotModel> plotModels)){//读取当前场景的剧情并验证
				for (int i = plotModels.Count - 1; i >= 0; i--){
					if (Random.Range(0, 100) <= plotModels[i].plotRequire.plotProbability){
						plotModels[i].CheckWhthExecuteList(roleModel,sceneModel);
					}
				}
			}
			if (plotsModel.TryGetValue(SceneModel.NULL, out List<PlotModel> nullScenePlotsModel)) {//读取所有场景都允许触发的剧情并验证
				foreach (PlotModel plotModel in nullScenePlotsModel) {
					if (Random.Range(0, 100) <= plotModel.plotRequire.plotProbability){
						plotModel.CheckWhthExecuteList(roleModel, sceneModel);
					}
				}
			}
		}
		public void CancelRegister(SceneModel sceneModel, PlotRequire plotRequire) {
			if (plotsModel.ContainsKey(sceneModel)) {
				PlotModel[] plotModels = plotsModel[sceneModel].ToArray();
				foreach(PlotModel plotModel in plotModels){
					if(plotModel.plotRequire == plotRequire&&plotRequire.plotModel == plotModel) {
						plotsModel[sceneModel].Remove(plotModel);
					}
				}
			}
		}
		public void CancelRegister(PlotRequire plotRequire) {
			if(plotRequire.sceneModels==null||plotRequire.sceneModels.Length==0) return;
			for (var i = plotRequire.sceneModels.Length - 1; i >= 0; i--){
				if(plotRequire.sceneModels[i]==null) continue;
				if (plotsModel.ContainsKey(plotRequire.sceneModels[i])) {
					PlotModel[] plotModels = plotsModel[plotRequire.sceneModels[i]].ToArray();
					for (int j = plotModels.Length - 1; j >= 0; j--){
						if(plotModels[i].plotRequire == plotRequire && plotRequire.plotModel == plotModels[i]) {
							plotsModel[plotRequire.sceneModels[i]].Remove(plotModels[i]);
						}
					}
				}
			}
		}
		public void Dispose()
		{
			plotsModel?.Clear();
		}
	}
}
