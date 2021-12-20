//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  SceneController.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.InstanceID;
using GalForUnity.Model.Scene;
using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Controller {
    // ReSharper disable all MemberCanBePrivate.Global
    public class SceneController : GfuMonoInstanceManager<SceneController>{
        [Attributes.Title("场景集合")]
        [Rename("场景索引 ")]
        [Tooltip("场景控制器会自动寻找场景内的所有场景并附加，您也可以手动附加，来获得可控的索引，然后通过场景控制器的索引来访问场景对象")]
        [SerializeField]
        // [ButtonAttribute]
        private List<SceneModel> sceneModels=new List<SceneModel>();

        public List<SceneModel> SceneModels{
            get => sceneModels;
        }

        private void Awake(){
            InitialSceneController();
        }
        /// <summary>
        /// 初始化场景控制器
        /// </summary>
        public void InitialSceneController(){
            for (int i = sceneModels.Count - 1; i >= 0; i--){
                sceneModels.RemoveAt(i);
            }
            var allSceneModel = FindObjectsOfType(typeof(SceneModel));
            foreach (var obj in allSceneModel){
                if (obj is SceneModel sceneModel){
                    if (!sceneModels.Contains(sceneModel)) sceneModels.Add(sceneModel);
                    if (sceneModel != this) sceneModel.transform.parent = transform;
                }
            }
            
        }
        /// <summary>
        /// 通过GfuInstanceID查找角色数据对象,该查找不通过GfuinstanceID系统，只查找SceneModel列表
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public SceneModel FindRoleModelWithInstanceID(long instanceID){
            foreach (var roleModel in sceneModels){
                if(roleModel.GetComponent<GfuInstance>().instanceID ==instanceID) return roleModel;
            }
            return null;
        }
        /// <summary>
        /// 向场景控制器添加场景
        /// </summary>
        /// <param name="sceneModel"></param>
        public void Add(SceneModel sceneModel){
            for (int i = sceneModels.Count - 1; i >= 0; i--){
                if (sceneModels[0] == null){
                    sceneModels.RemoveAt(i);
                }
            }
            sceneModels.Add(sceneModel);
        }
        /// <summary>
        /// 向场景控制器移除场景
        /// </summary>
        /// <param name="sceneModel"></param>
        public void Remove(SceneModel sceneModel){
            for (int i = sceneModels.Count - 1; i >= 0; i--){
                if (sceneModels[0] == null){
                    sceneModels.RemoveAt(i);
                }
            }
            sceneModels.Remove(sceneModel);
        }
        /// <summary>
        /// 场景是否存在于场景控制器中
        /// </summary>
        /// <param name="sceneModel">要检查的场景</param>
        /// <returns></returns>
        public bool HasScene(SceneModel sceneModel){
            return sceneModels.Contains(sceneModel);
        }

        /// <summary>
        /// 前往场景的方法，执行后前往场景，触发场景变更事件
        /// </summary>
        /// <param name="directionSceneModel"></param>
        /// <exception cref="SceneModelNotFoundException">当场景不存在于场景控制器中时触发异常，但是不会影响场景变更的事实</exception>
        public void GoToScene(SceneModel directionSceneModel){
            GameSystem.Data.CurrentSceneModel = directionSceneModel;
            GameSystem.Data.ShowPlotView.ShowSceneModel(directionSceneModel);
            if (!HasScene(directionSceneModel)){
                throw new SceneModelNotFoundException();
            }
        }
        /// <summary>
        /// 通过索引访问场景控制器
        /// </summary>
        /// <param name="index">索引</param>
        /// <exception cref="ArgumentOutOfRangeException">当索引超出边界时触发</exception>

        public SceneModel this[int index]{
            get{
                if (index >= 0 && index < sceneModels.Count){
                    return sceneModels[index];
                }
                throw new ArgumentOutOfRangeException();
            }
            set{
                if (index >= 0 && index < sceneModels.Count){
                    sceneModels[index] = value;
                }
                throw new ArgumentOutOfRangeException();
            }
        }
        
        public SceneModel[] this[string gameObjectName]{
            get{
                List<SceneModel> list=new List<SceneModel>();
                foreach (var sceneModel in sceneModels){
                    if (sceneModel.gameObject.name == gameObjectName){
                        list.Add(sceneModel);
                    }
                }
                return list.ToArray();
            }
        }

        private class SceneModelNotFoundException:Exception{ }
    }
}
