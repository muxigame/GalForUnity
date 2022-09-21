//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  PlotModel.cs 
//
//        Created by 半世癫(Roc) at 2021-01-01 10:20:33
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Controller;
using GalForUnity.Graph;
using GalForUnity.Graph.AssetGraph;
using GalForUnity.InstanceID;
using GalForUnity.Model.Scene;
using GalForUnity.System;
using GalForUnity.System.Event;
using UnityEngine;
using UnityEngine.Serialization;

namespace GalForUnity.Model.Plot {
    /// <summary>
    /// PlotModel即剧情模型，管理着剧情要求和剧情流，在某种意义上说是剧情本身，需要剧情细节请访问PlotFlow，需要知道剧情要求请访问PlotRequire，在引用上和PlotRequire是互相引用关系
    /// </summary>
    // [RequireComponent(typeof(PlotRequire))]
    public class PlotModel : MonoBehaviour,IComparable {
        // ReSharper disable all MemberCanBePrivate.Global
        [Rename(nameof(plotRequire))]
        [Tooltip("默认挂载当前对象存在的剧情要求（PlotRequire）如果当前对象不存在剧情要求，则会自动创建")]
        [SerializeField]
        public PlotRequire plotRequire;
        // [Rename("剧情流(已废弃)")] 
        // [Tooltip("默认挂载剧情流（PlotFlow），如果当前对象不存在剧情流，则会自动创建")]
        // public PlotFlow plotFlow;
        // [FormerlySerializedAs("PlotFlowGraph")]
        // [Rename(nameof(SceneGraph))] 
        // [Tooltip("当剧情流图存在时，默认执行剧情流图中的内容")]
        // public GfuSceneGraph SceneGraph;
        [Rename(nameof(actionModel))]
        [Tooltip("剧情结束完后，执行指定的操作")]
#pragma warning disable 612
        public ActionModel actionModel;
#pragma warning restore 612
        
        
        /// <summary>
        /// 在开始时，向剧情酷注册本剧情，如果剧情列表的角色要求里存在null要求，则所有场景中，只要角色数据满足要求。都会触发本事件；
        /// </summary>
        void Start(){
            // if (!SceneGraph &&gameObject.TryGetComponent(out GfuSceneGraph gfuSceneGraph)) SceneGraph = gfuSceneGraph;
            plotRequire.plotModel  = this;
            if (plotRequire.sceneModels == null || plotRequire.sceneModels.Length == 0){
                DailyStateChecker.GetInstance().RegisterPlotModel(SceneModel.NULL, this); //将空场景进行注册，所有场景均可以触发当前剧情
            } else{
                for (int i = 0, length = plotRequire.sceneModels.Length; i < length; i++){
                    SceneModel sceneModel;
                    if ((sceneModel = plotRequire.sceneModels[i]) == null){
                        sceneModel = plotRequire.sceneModels[i] = SceneModel.NULL;
                        DailyStateChecker.GetInstance()
                                         .RegisterPlotModel(SceneModel.NULL,
                                             this); //一旦发现未装填的空场景，将会立刻将剧情置为不需要剧情要求，所有场景均可以触发当前剧情，并取消已注册的剧情
                        return;
                    }
                    // Debug.Log("DailyStateChecker.GetInstance().RegisterPoltModel(sceneModel, this)");
                    DailyStateChecker.GetInstance().RegisterPlotModel(sceneModel, this); //将所有场景进行注册
                }
            }
        }
        public void ToExecuteList(){
            _gameTime = GameSystem.Data.CurrentTime;
            if (!PlotModelList.GetInstance().HasPlot(this)){
                PlotModelList.GetInstance().Add(this);
            }
        }

        private GameTime _gameTime;
        public bool CheckWhthExecuteList(RoleData roleData, SceneModel sceneModel) {
            if (_gameTime == GameSystem.Data.CurrentTime) return false;
            return CheckWhthExecuteList(roleData.RoleModel, sceneModel);
        }
        public bool CheckWhthExecuteList(RoleModel roleModel, SceneModel sceneModel){
            if (_gameTime == GameSystem.Data.CurrentTime) return false;
            if (!plotRequire.Check(roleModel.RoleData, sceneModel)) return false;
            EventCenter.GetInstance().PlotRequireCheckOkEvent.Invoke(this, roleModel); //触发剧情前提条件验证完成事件
            ToExecuteList();
            //如果剧情是不可重复的
            if(!plotRequire.repeatability) DailyStateChecker.GetInstance().CancelRegister(sceneModel,plotRequire);
            else if(!plotRequire.Check(roleModel.RoleData, sceneModel)) DailyStateChecker.GetInstance().CancelRegister(sceneModel,plotRequire);
            return true;
        }
        /// <summary>
        /// 检查剧情是否满足触发要求
        /// </summary>
        /// <param name="roleData">角色数据</param>
        /// <param name="sceneModel">场景数据</param>
        /// <returns></returns>
        public bool Check(RoleData roleData, SceneModel sceneModel) {
            return plotRequire.Check(roleData, sceneModel);
        }
        
		public int Compare(PlotModel x, PlotModel y)
        {
            if (x is null) throw new NullReferenceException();
            if (!(y is null))
                return x.plotRequire.priority - y.plotRequire.priority;
            throw new NullReferenceException();
        }

        public int CompareTo(object obj)
        {
            if (!(obj is null))
                if(obj is PlotModel p)
                    return plotRequire.priority - p.plotRequire.priority;
            throw new NullReferenceException();
        }
    }
}