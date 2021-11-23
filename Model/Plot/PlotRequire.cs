//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  PlotRequire.cs 
//
//        Created by Roc
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Controller;
using GalForUnity.Model.Scene;
using GalForUnity.System;
using GalForUnity.System.Event;
using UnityEngine;
using UnityEngine.Serialization;

namespace GalForUnity.Model.Plot {
	
	public enum PlotType {
		[Rename(nameof(date))]
		date = 0,
		[Rename(nameof(plot))]
		plot = 1,
	}
	public enum TriggerType {
		[Rename(nameof(before))]
		before = 0,
		[Rename(nameof(after))]
		after = 1,
		[Rename(nameof(replace))]
		replace = 2
	}
	/// <summary>
	/// PlotRequire剧情要求，即触发剧情的前提条件，分为场景要求和角色数据要求，在引用上和PlotModel是互相引用关系
	/// </summary>
// #if UNITY_EDITOR
// 	[ExecuteInEditMode]
// #endif
// 	[RequireComponent(typeof(GfuInstance))]
	[Serializable]
	public class PlotRequire{
		[HideInInspector]
		public PlotModel plotModel;
		
		private void Awake() {
			// if (roleData == null && ! gameObject.TryGetComponent(out roleData)) {
			// 	
			// }
			// if (plotModel == null&&TryGetComponent(out PlotModel thisComponentPlotModel)) plotModel = thisComponentPlotModel;
			EventCenter.GetInstance().OnPlotWillExecuteEvent += CheckTrigger;
		}
		
		/// <summary>
		/// 剧情类型normal：正常，fiexd：固定，：special：特殊，replace，：替代
		/// </summary>
		[FormerlySerializedAs("poltType")] [Tooltip("剧情类型normal：正常，fiexd：固定，special：特殊，replace，：替代")]
		[Rename(nameof(plotType))]
		public PlotType plotType = PlotType.date;
		/// <summary>
		/// 剧情类型normal：正常，fiexd：固定，：special：特殊，replace，：替代
		/// </summary>
		[Tooltip("如何处理被触发的剧情和该剧情")]
		[Rename(nameof(triggerType))]
		[HideBy(nameof(plotType), nameof(PlotType.date))]
		public TriggerType triggerType = TriggerType.before;
		/// <summary>
		/// 触发替换剧情的前置剧情条件（先决剧情）
		/// </summary>
		[Tooltip("触发替换剧情的前置剧情条件（先决剧情）如果为空则所有剧情都会触发该剧情")]
		[HideBy(nameof(plotType),nameof(PlotType.date))]
		public PlotModel triggerPlot;
		/// <summary>
		/// 要替换掉的剧情
		/// </summary>
		[Tooltip("要替换掉的剧情，如果为空则不会替换剧情（如已经选择替换剧情，则原剧情不会触发）")]
		[ShowBy(nameof(triggerType),nameof(TriggerType.replace))]
		public PlotModel replacePlot;
		/// <summary>
		/// 剧情允许被触发的时间起点，默认为游戏开始的第一天
		/// </summary>
		[ShowBy(nameof(plotType), nameof(PlotType.date))]
		[Tooltip("剧情允许被触发的时间起点，默认为游戏开始的第一天")]
		public GameTime startTime = new GameTime();

		public bool repeatability = false;
		/// <summary>
		/// 剧情允许被触发的时间起点，默认为游戏开始三年后的第一天
		/// </summary>
		[Tooltip("剧情允许自开始时间允许被触发的持续天数，当为负值时永久持续")]
		[ShowBy("repeatability==true&&plotType==date")]
		public int duration=1;
		/// <summary>
		/// 替换剧情允许被重复多次替换的次数，1：仅一次，0：不重复，-1：无限重复，2及以上：重复指定次
		/// </summary>
		[Tooltip("替换剧情允许被重复多次替换的次数，1：仅一次，0：不重复，-1：无限重复，2及以上：重复指定次")]
		[SerializeField]
		[ShowBy("(repeatability==true)&&plotType!=date")]
		public int repetitionCount=1;
		/// <summary>
		/// 触发剧情所要求的角色数值
		/// </summary>
		[Tooltip("触发剧情所要求的角色数值")]
		//[SerializeField]
		public RoleData roleDataRequire;
		/// <summary>
		/// 触发剧情所要求的场景地点
		/// </summary>
		[Tooltip("触发剧情所要求的场景地点")]
		[RenameInEditor("sceneRequire")]
		[SerializeField]
		public SceneModel[] sceneModels=new SceneModel[1];
		/// <summary>
		/// 当有多个剧情条件被满足时的当前剧情的执行优先级，可以理解为小时，也可以理解为优先级，数字越小，越优先执行
		/// </summary>
		[Tooltip("当有多个剧情条件被满足时的当前剧情的执行优先级，可以理解为小时，也可以理解为优先级，数字越小，越优先执行")]
		public int priority = 10;
		/// <summary>
		/// 该剧情是否结束本日，如果没有任何剧情结束本日，当日会无限持续
		/// </summary>
		[Tooltip("该剧情是否结束本日，如果没有任何剧情结束本日，当日会无限持续")]
		[SerializeField]
		public bool overDay = false;
		/// <summary>
		/// 剧情的触发的概率，是以百为分母的百分概率
		/// </summary>
		
		[Tooltip("剧情的触发的概率，是以百为分母的百分概率")]
		[SerializeField]
		public float plotProbability = 100;
		///// <summary>
		///// 触发条件被满足时要触发的剧情
		///// </summary>
		//[Tooltip("触发条件被满足时要触发的剧情")]
		// private bool _isTrigger = false;
		/// <summary>
		/// 提交监听，以拦截替换事件
		/// </summary>
		/// <param name="trrigerPlotModel">剧情模型</param>
		/// <param name="roleModel">当前角色模型</param>
		/// <returns>是否拦截该替换事件</returns>
		private bool CheckTrigger(PlotModel trrigerPlotModel,RoleModel roleModel){
			if(plotType==PlotType.plot){
				if (repetitionCount == 0) {
					this.Destroy();
					return false;
				}
				if (trrigerPlotModel == triggerPlot) {
					// Debug.Log("CheckTriggerAddReplacePolt:" + this.plotModel);
					PlotModelList.GetInstance().Add(this.plotModel);
					repetitionCount--; 
					// _isTrigger = true;
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 检查任务条件是否被满足
		/// 特殊事件与常规事件以及替代事件均需要场景和角色数值验证，引用为空默认为所有条件皆可
		/// 固定事件仅验证任务开始事件
		/// </summary>
		/// <param name="roleData">角色数据</param>
		/// <param name="sceneModel">场景数据</param>
		/// <returns>验证结果</returns>
		public bool Check(RoleData roleData, SceneModel sceneModel) {
			// if (plotType == PlotType.plot) return _isTrigger;//对于固定事件仅需要时间验证
			if (TimeCheck(GameSystem.Data.CurrentTime)) return NormalCheck(roleData,sceneModel);
			return false;
		}
		
		public bool NormalCheck(RoleData otherRoleData, SceneModel otherSceneModel){
			var modelCheck = ModelCheck(otherSceneModel);
			var roleDataCheck = RoleDataCheck(otherRoleData);
			//当存在场景列表，检查场景列表里是否有null要求，上方的条件通过将置空的条件当成不需要条件即可触发
			return modelCheck && roleDataCheck;
		}
		/// <summary>
		/// 不可重复时，检查时间是不是相等,响应剧情的事件不会通过时间检查
		/// 可重复时,检查时间是不是在周期内
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		private bool TimeCheck(GameTime gameTime){
			if (plotType == PlotType.plot) return false;
			if (!repeatability) return startTime == gameTime;
			if (duration < 0) return true;
			return startTime <= gameTime && startTime + duration-new GameTime(0,0,1) >= gameTime;
		}
		private bool RoleDataCheck(RoleData roleModel) {
			if (!roleModel){
				Debug.Log("The role data is empty, please assign a value to the role data of the game system");
				return true;
			}
			if (!roleDataRequire) return true;
			return roleDataRequire <= roleModel;
		}
		private bool ModelCheck(SceneModel sceneModel){
			if (sceneModels == null || sceneModels.Length == 0) return true;
			foreach(var scene in sceneModels){
				if (!scene ||sceneModel == SceneModel.NULL) return true;
				if(scene==sceneModel){
					return true;
				}
			}
			return false;
		}

		private bool _destroyFlag = false;

		private void Destroy()
		{
			if (!_destroyFlag)
			{
				_destroyFlag = true;
				foreach (SceneModel sceneModel in sceneModels)
				{
					DailyStateChecker.GetInstance()
						.CancelRegister(sceneModel == null ? SceneModel.NULL : sceneModel, this);
				}

				var poltExecuteEvent = EventCenter.GetInstance().OnPlotWillExecuteEvent;
				if (poltExecuteEvent != null)
					poltExecuteEvent -= CheckTrigger;

				if (this != null)
				{
					// GameObject.Destroy(gameObject);
				}
			}

		}

	}

}
