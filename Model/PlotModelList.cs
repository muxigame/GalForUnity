//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  PlotModelList.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.Model.Plot;
using UnityEngine;

namespace GalForUnity.Model{
	/// <summary>
	/// 所有的待执行事件列表
	/// </summary>
	///
	[Serializable]
	public class PlotModelList :IDisposable{
		
		private static PlotModelList _plotModelList = new PlotModelList();
		// private readonly Dictionary<PlotType, List<PlotModel>> _plotsModel = new Dictionary<PlotType, List<PlotModel>>();
		private readonly List<PlotModel> _plotsModel = new List<PlotModel>();
		/// <summary>
		/// 获得待执行剧情模型列表的实例
		/// </summary>
		/// <returns></returns>
		public static PlotModelList GetInstance(){ return _plotModelList ?? (_plotModelList = CreateInstance()); }

		private static PlotModelList CreateInstance() => new PlotModelList();
		/// <summary>
		/// 向待执行剧情列表添加剧情模型
		/// </summary>
		/// <param name="plotModel">指定要添加的剧情模型</param>
		public void Add(PlotModel plotModel) {
			// Debug.Log("对象被添加"+plotModel);
			_plotsModel.Add(plotModel);
		}
		/// <summary>
		/// 向指定索引插入待执行任务
		/// </summary>
		/// <param name="index"></param>
		/// <param name="plotModel"></param>
		public void Insert(int index,PlotModel plotModel) {
			_plotsModel.Insert(index,plotModel);
		}

		/// <summary>
		/// 获取待执行剧情列表
		/// </summary>
		/// <param name="index"></param>
		/// <returns>剧情列表，如果没有需要执行的剧情，返回null</returns>
		public PlotModel GetPlot(int index = 0) {
			if (_plotsModel.Count == 0) return null;
			var plotModel = _plotsModel[index];
			return plotModel;
		}

		/// <summary>
		/// 获取待执行剧情列表
		/// </summary>
		/// <param name="index"></param>
		/// <returns>剧情列表，如果没有需要执行的剧情，返回null</returns>
		public PlotModel PopPlot(int index) {
			var plotModel = _plotsModel[index];
			_plotsModel.Remove(plotModel);
			return plotModel;
		}
		public PlotModel PopPlot() {
			return PopPlot(0);
		}
		/// <summary>
		/// 是否有该剧情类型的剧情
		/// </summary>
		/// <param name="plotType">指定要查找的剧情类型</param>
		/// <returns>剧情类型是否存在</returns>
		public bool HasPlot(PlotModel plotModel) {
			if (_plotsModel != null) {
				return _plotsModel.Contains(plotModel);
			}
			return false;
		}

		// /// <summary>
		// /// 是否有该剧情类型的剧情
		// /// </summary>
		// /// <param name="plotType">指定要查找的剧情类型</param>
		// /// <param name="plotModel"></param>
		// /// <returns>剧情类型是否存在</returns>
		// public bool HasPlot(PlotType plotType,PlotModel plotModel) {
		// 	if (_plotsModel.ContainsKey(plotType)) {
		// 		return _plotsModel[plotType].Contains(plotModel);
		// 	}
		// 	return false;
		// }
		// public void Dispose()
		// {
		// 	_plotsModel.Clear();
		// }
		public void Dispose(){
			_plotsModel.Clear();
		}
	}

}
