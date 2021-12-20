//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ReflectionSerialization.cs
//
//        Created by 半世癫(Roc) at 2021-12-13 17:13:46
//
//======================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.InstanceID;
using GalForUnity.System.Archive.Attributes;
using GalForUnity.System.Archive.Behavior;
using GalForUnity.System.Archive.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using NotImplementedException = System.NotImplementedException;

namespace GalForUnity.System.Archive.SaveAlgorithm{
    [Serializable]
    public class ReflectionSerialization : SavableAlgorithm{
        
        [SerializeField]
        public List<ComponentValue> ComponentValues=new List<ComponentValue>();
        public ReflectionSerialization(Transform transform, Scene scene):base(transform,scene){
            if (transform){
                if(ArchiveEnvironmentConfig.GetInstance().saveHierarchy)GfuRunOnMono.GetInstance().StartCoroutine(SaveHierarchy(transform));
                if(ArchiveEnvironmentConfig.GetInstance().saveData)GfuRunOnMono.GetInstance().StartCoroutine(SaveMonoScript(transform, 0));
            }
        }

        public override void OnSaveStart(){ }

        public override void Loaded(){
            foreach (var data in scriptData){
                data.Recover();//层级恢复
            }

            HashSet<SavableBehaviour> hashSet=new HashSet<SavableBehaviour>();
            List<SavableBehaviour> list=new List<SavableBehaviour>();
            var recoverPriority = ArchiveEnvironmentConfig.GetInstance().recoverPriority;
            ComponentValues.Sort((x,y) => {
                int xp = x.priority;
                int yp = y.priority;
                var typeX = x.Value().GetType();
                var typeY = y.Value().GetType();
                if (recoverPriority.ContainsKey(typeX)) xp = recoverPriority[typeX];
                if (recoverPriority.ContainsKey(typeY)) yp = recoverPriority[typeY];
                return -xp.CompareTo(yp);
            });
            foreach (var componentValue in ComponentValues){
                Debug.Log(componentValue);
                var component = componentValue.Set();
                var fieldInfo = componentValue.Field();
                if(fieldInfo!=null&&fieldInfo.FieldType.IsSubclassOf(typeof(Savable))) (fieldInfo.GetValue(component) as Savable)?.Recover();//如果一个有保存标记的组件是是一个可保存数据对象，就调用Recover
                if (component is SavableBehaviour savableBehaviour){
                    if (!hashSet.Contains(savableBehaviour)){
                        hashSet.Add(savableBehaviour);
                        list.Add(savableBehaviour);
                    }
                }
                    
            }
            foreach (var component in list){
                if(component is SavableBehaviour savableBehaviour) savableBehaviour.Recover();
            }
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

        //反射保存含有SaveFlag标记的数据，仅支持基本数据类型
        private IEnumerator SaveMonoScript(Transform transform, int priority){
            if (transform.TryGetComponent(out GfuInstance gfuInstance)){
                var savableBehaviours = gfuInstance.GetComponents<SavableBehaviour>();
                foreach (var savableBehaviour in savableBehaviours){
                    var fieldInfos = savableBehaviour.GetType().GetFields(BindingFlags.Instance |BindingFlags.NonPublic |BindingFlags.Public |BindingFlags.Static);
                    bool hasSaveFlag= false;
                    foreach (var fieldInfo in fieldInfos){
                        if (fieldInfo.GetCustomAttribute<SaveFlagAttribute>() != null){
                            hasSaveFlag = true;
                            savableBehaviour.GetObjectData();
                            if(fieldInfo.GetValue(savableBehaviour) is Savable savable) savable.Save();//也就是说可保存对象没有SaveFlag标记则不会尝试保存
                            ComponentValues.Add(new ComponentValue(fieldInfo.Name,savableBehaviour,fieldInfo.GetValue(savableBehaviour)){priority = priority});
                        }
                    }

                    if (!hasSaveFlag){
                        ComponentValues.Add(new ComponentValue("",savableBehaviour,""){priority = priority});
                    }
                }
            }
            for (int i = 0; i < transform.childCount; i++){
                yield return SaveMonoScript(transform.GetChild(i),priority-10);
            }
            if (priority == 0) parsed = true;
        }
    }
}
