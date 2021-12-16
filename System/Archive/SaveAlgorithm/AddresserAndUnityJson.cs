//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  AddresserAndUnityJson.cs
//
//        Created by 半世癫(Roc) at 2021-12-12 23:04:51
//
//======================================================================

using System;
using System.Collections;
using GalForUnity.InstanceID;
using GalForUnity.System.Archive.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalForUnity.System.Archive.SaveAlgorithm{
    [Serializable]
    public class AddresserAndUnityJson :SavableAlgorithm{
        public bool IsLoaded=>scriptData.Count!=0;

        public AddresserAndUnityJson(Transform transform, Scene scene) : base(transform, scene){
            if (transform){
                if(ArchiveEnvironmentConfig.GetInstance().saveHierarchy) GfuRunOnMono.GetInstance().StartCoroutine(SaveHierarchy(transform));
                if(ArchiveEnvironmentConfig.GetInstance().saveData)GfuRunOnMono.GetInstance().StartCoroutine(SaveMonoScript(transform, 0));
            }
        }
        // public AddresserAndUnityJson() : base(){}
        
        public override void OnSaveStart(){ }

        public override void Loaded(){
            //如果实在Edit模式不会尝试加载场景，因为Edit中切换场景会导致存档无效，游戏中则不会
            void InitOnScene(){
                scriptData.Sort((x, y) => -x.priority.CompareTo(y.priority));
                foreach (var saveable in scriptData){
                    saveable.Recover();
                }
                ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ArchiveLoadOver); 
            }
#if !UNITY_EDITOR
            void InitOnSceneLoaded(Scene scene, LoadSceneMode loadMode){
                ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.SceneLoadOver);
                scriptData.Sort((x, y) => -x.priority.CompareTo(y.priority));
                foreach (var saveable in scriptData){
                    saveable.Recover();
                }
                ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ArchiveLoadOver);
                if(scene != default) SceneManager.sceneLoaded -= InitOnSceneLoaded;
            }
            if (sceneIndex != SceneManager.GetActiveScene().buildIndex){
                SceneManager.sceneLoaded += InitOnSceneLoaded;
                var asyncOperation = ArchiveEnvironmentConfig.GetInstance().asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
                ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.SceneLoadStart); 
            } else{
                InitOnScene();
            }
            // asyncOperation.allowSceneActivation = false;//如果在Edit模式就不加载场景了，因为这会初始化InstanceID导致存档不可用，Unity打包后使用的是FileID，只要文件不移动位置不会造成引用问题
#else
            InitOnScene();
#endif
        }
        private IEnumerator SaveHierarchy(Transform transform){
            if (transform.TryGetComponent(out GfuInstance gfuInstance)){
                var savable = new ScriptData(transform.gameObject);
                scriptData.Add(savable);
                savable.priority = Int32.MaxValue;
            }

            for (int i = 0; i < transform.childCount; i++){
                yield return SaveHierarchy(transform.GetChild(i));
            }
        }

        private IEnumerator SaveMonoScript(Transform transform, int priority){
            var components = transform.GetComponents<MonoBehaviour>();

            foreach (var component in components){
                if (component.gameObject.hideFlags == HideFlags.HideInHierarchy | component.gameObject.hideFlags == HideFlags.HideInInspector) continue;
                var savable = new ScriptData(component);
                savable.priority = savable.priority == Int32.MinValue ? priority : savable.priority;
                if (!string.IsNullOrEmpty(savable.ObjectAddressExpression)){
                    scriptData.Add(savable);
                }
            }

            for (int i = 0; i < transform.childCount; i++){
                yield return SaveMonoScript(transform.GetChild(i), priority - 10);
            }

            if (priority == 0) parsed = true;
        }

        
    }
}
