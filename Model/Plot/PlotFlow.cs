//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  PlotFlow.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.Graph;
using GalForUnity.Graph.Data.Property;
using GalForUnity.InstanceID;
using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Model.Plot{
	/// <summary>
	/// 剧情流本身，管理着剧情的始终，由剧情项列表组成
	/// </summary>
	[Serializable]
	[RequireComponent(typeof(GfuInstance))]
	[Obsolete]
	public class PlotFlow : MonoBehaviour {
		// ReSharper disable all MemberCanBePrivate.Global
		

		public PlotFlowType PlotFlowType;
		[HideInInspector]
		public PlotModel PlotModel;
		[Tooltip("每一次点击屏幕，都会自动播放下一项剧情项")]
		[SerializeField]
		public List<PlotItem> plotItems = new List<PlotItem>();
		[SerializeField]
		public Graph.Data.Property.PlotItemGraphData PlotItemGraph;

		private GfuGraph _gfuGraph;
		
		[Tooltip("参与该项剧情的角色")]
		[Rename(nameof(role))]
		public List<RoleModel> role = new List<RoleModel>();
		[Tooltip("当前剧情开始的索引，索引默认从第0项开始，指定索引以跳过剧情")]
		public int startIndex = 0;
		[Rename("动作集", "列表中的所有动画都会被播放，且动画相互覆盖，请保证动画没有重叠部分")]
		public PlotAnimationSet animationSet;
		public ActionModel ActionModel{
			get{
				if (PlotModel){
					if (TryGetComponent(out ActionModel actionModel)){
						return actionModel;
					} else{
						PlotModel.actionModel = gameObject.AddComponent<ActionModel>();
						return PlotModel.actionModel;
					}
				}
				if (TryGetComponent(out ActionModel actionModel2)){
					return actionModel2;
				} else{
					return gameObject.AddComponent<ActionModel>();
				}
			}
		}

		public void TryInit(){
			if (_gfuGraph == null){
				Init();
			}
		}
		public void Init(){
			_gfuGraph = new PlotItemGraph(PlotItemGraph);
			// _gfuGraph.InitNode();
		}
		

		/// <summary>
		/// 反正是否存在下一项
		/// </summary>
		/// <returns></returns>
		public bool HasNext(){
			if(PlotFlowType==PlotFlowType.Item) return startIndex < plotItems.Count && startIndex >= 0;
			else return _gfuGraph.HasNext;
		}
		/// <summary>
		/// 返回下一个剧情项，倘若没有，则返回空
		/// </summary>
		/// <param name="plotItem">要赋值的剧情项</param>
		/// <returns>反正是否存在下一项</returns>
		public bool Next(out PlotItem plotItem){
			if (HasNext()) {
				if (PlotFlowType==PlotFlowType.Item){
					plotItem = plotItems[startIndex];
					return startIndex++ < plotItems.Count;
				}else{
					//TODO 这里应该尝试获得PlotItem的值
					//不过在图模式中拿不到PlotItem似乎又是情理之中
					plotItem = null;
					if (_gfuGraph != null){
						return _gfuGraph.HasNext;
					}
					return false;
				}
			}
			plotItem = null;
			return false;
		}
		/// <summary>
		/// 返回下一个剧情项，倘若没有，则返回空
		/// </summary>
		/// <param name="plotItem">要赋值的剧情项</param>
		/// <returns>反正是否存在下一项</returns>
		// public bool Next(PlotItem plotItem){
		// 	if (HasNext()) {
		// 		if (PlotFlowType==PlotFlowType.Item){
		// 			plotItem = plotItems[startIndex];
		// 			return startIndex++ < plotItems.Count;
		// 		}else{
		// 			//TODO 这里应该尝试获得PlotItem的值
		// 			//不过在图模式中拿不到PlotItem似乎又是情理之中
		// 			plotItem = null;
		// 			if (_gfuGraph != null){
		// 				return _gfuGraph.HasNext;
		// 			}
		// 			return false;
		// 		}
		// 	}
		// 	plotItem = null;
		// 	return false;
		// }

		/// <summary>
		/// 模拟执行一个下一个剧情项
		/// </summary>
		/// <returns>返回是否成功执行</returns>
		public bool ExecuteNext(){
			if(Next(out PlotItem plotItem)){
				// if (PlotFlowType == PlotFlowType.Item) return GameSystem.Data.PlotFlowController.ExecutePoltItem(plotItem);
				// else{
				// 	return _gfuGraph.Next();
				// 	//当此方法执行以后，剧情流便不在接受剧情流控制器管辖，转而进入图系统控制
				// 	//但是通过图系统模拟的返回值依旧能判断是否执行完成了
				// }
			}
			return false;
		}
	}
	
	[Serializable]
	public class PlotItem {
		[SerializeField]
		[Rename("姓名","指定要出场改剧情的角色，指定的名字需要在剧情流包含的出场角色中出现")]
		public string name;
		[Rename("说的话", "当前剧情项出场角色说的话")]
		public string speak;
		public List<AnimationClip> Animations => animationSet +new[]{_animationClip1,_animationClip2,_animationClip3,_animationClip4};

		[Rename("配音", "当前剧情项播放的背景音乐，一般建议使用配音，将背景音乐放置到场景模型SceneModel中")]
		public AudioClip audioClip;
		[Rename("背景","背景图片，如果未空，则保持上一项的背景，如果当前项是第一项，则显示场景图片")]
		public Sprite background;
		
		[Rename("动作集", "列表中的所有动画都会被播放，且动画相互覆盖，请保证动画没有重叠部分")]
		public PlotAnimationSet animationSet;

		public AnimationClip _animationClip1;
		public AnimationClip _animationClip2;
		public AnimationClip _animationClip3;
		public AnimationClip _animationClip4;
	}
	public enum PlotFlowType{
		Item,
		Graph
	}
}
