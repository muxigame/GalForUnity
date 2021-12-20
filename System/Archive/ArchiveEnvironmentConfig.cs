//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArchiveEnvironmentConfig.cs
//
//        Created by 半世癫(Roc) at 2021-12-09 21:55:30
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.Controller;
using GalForUnity.View;
using UnityEditor;
using UnityEngine;

namespace GalForUnity.System.Archive{
    [Serializable]
    public class ArchiveEnvironmentConfig:GfuInstanceManager<ArchiveEnvironmentConfig>{
        public ArchiveEnvironmentConfig(){
            
        }

        [NonSerialized] 
        private string archiveDirectory;

        /// <summary>
        /// 存档路径，这个字段不能静态调用，不能从构造中调用，尽可在Mono生命周期内调用，因为Application不允许从这些地方调用。
        /// </summary>
        public string ArchiveDirectory{
            get{
                if (string.IsNullOrEmpty(archiveDirectory))
                    return archiveDirectory =
#if UNITY_EDITOR
                        Application.dataPath + "/";
#else
                        Application.persistentDataPath+"/";
#endif
                return archiveDirectory;
            }
        }

        /// <summary>
        /// 指定使用的存档算法
        /// AddresserAndUnityJson适用于原型开发阶段，以及不打算进行更新的游戏
        /// 该算法使用Unity自带的Json工具，能够序列化Unity内部的引用类型，使用该算无需进行特殊操作，只需要对象上存在GfuInstance组件脚本就会被序列化。
        /// 游戏打包之后Unity内置的InstanceID便不会改变，因此能够完成存档。但是不能二次打包安装。
        /// ReflectionSerialization适用于自定义开发
        /// 该算法使用二进制序列化算法，无法序列化Unity内部类型，使用改算法应继承SavableBehaviour，并用SaveFlagAttribute标记要序列化的字段
        /// </summary>
        [SerializeField]
        [Rename(nameof(archiveAlgorithm))]
        [Tooltip("当这个值为False时，存档系统不会尝试保存层级信息，但是已保存层级的存档仍能正常解析")]
        public ArchiveAlgorithmType archiveAlgorithm=ArchiveAlgorithmType.UnityJson;
        /// <summary>
        /// 当这个值为False时，存档系统不会尝试保存层级信息，但是已保存层级的存档仍能正常解析
        /// </summary>
        [SerializeField]
        [Tooltip("当这个值为False时，存档系统不会尝试保存层级信息，但是已保存层级的存档仍能正常解析")]
        [Rename(nameof(saveHierarchy))]
        public bool saveHierarchy=true;
        /// <summary>
        /// 当这个值为False时，存档系统不会尝试保存游戏数据，但是已保存游戏数据的存档仍能正常解析
        /// </summary>
        [SerializeField]
        [Tooltip("当这个值为False时，存档系统不会尝试保存游戏数据，但是已保存游戏数据的存档仍能正常解析")]
        [Rename(nameof(saveData))]
        public bool saveData=true;
        /// <summary>
        /// 存档后缀
        /// </summary>
        [SerializeField]
        [Tooltip("存档后缀")]
        [Rename(nameof(archiveSuffix))]
        public string archiveSuffix=".dat";
        /// <summary>
        /// 存档截图后缀
        /// </summary>
        [SerializeField]
        [Tooltip("存档截图后缀")]
        [Rename(nameof(photoSuffix))]
        public string photoSuffix=".png";
        /// <summary>
        /// 存档默认名称
        /// </summary>
        [SerializeField]
        [Tooltip("存档默认名称")]
        [Rename(nameof(archiveDefaultName))]
        public string archiveDefaultName="存档";
        /// <summary>
        /// 存档图片的质量
        /// </summary>
        [SerializeField]
        [Tooltip("存档图片的质量")]
        [Rename(nameof(photoQuality))]
        public PhotoQuality photoQuality=PhotoQuality.Low;
        /// <summary>
        /// 存档配置的文件名
        /// </summary>
        [SerializeField]
        [Tooltip("存档配置的文件名")]
        [Rename(nameof(configsFileName))]
        private string configsFileName="archiveConfig";
        /// <summary>
        /// 存档配置的后缀名
        /// </summary>
        [SerializeField]
        [Tooltip("存档配置的后缀名")]
        [Rename(nameof(configsSuffix))]
        private string configsSuffix=".config";
        /// <summary>
        /// 存档系统使用的异步线程名
        /// </summary>
        [SerializeField]
        [Tooltip("存档系统使用的异步线程名")]
        [Rename(nameof(archiveSystemThreadName))]
        public string archiveSystemThreadName="ArchiveSystemThread";
        [SerializeField]
        public AsyncOperation asyncOperation;
        /// <summary>
        /// 存档线程异步等待主线程的时间间隔，单位毫秒
        /// </summary>
        [SerializeField]
        [Tooltip("存档线程异步等待主线程的时间间隔，单位毫秒")]
        [Rename(nameof(archiveSystemThreadWaitingTime))]
        public int archiveSystemThreadWaitingTime=10;
        /// <summary>
        /// 存档线程异步等待主线程的时间上限，单位秒
        /// </summary>
        [SerializeField]
        [Tooltip("存档线程异步等待主线程的时间上限，单位秒")]
        [Rename(nameof(archiveSystemThreadWaitingMaxSecond))]
        public int archiveSystemThreadWaitingMaxSecond=500;
        /// <summary>
        /// 类型的序列化优先级
        /// </summary>
        [SerializeField]
        public Dictionary<Type,int> recoverPriority=new Dictionary<Type, int>() {
            {typeof(GameSystem),0},
            {typeof(RoleController),-10},
            {typeof(SceneController),-10},
            {typeof(ShowPlotView),-10},
            {typeof(OptionController),-20},
            {typeof(PlotFlowController),-100},
        };

        /// <summary>
        /// 配置文件的完整路径和文件名
        /// </summary>
        public string ConfigsFile => ArchiveDirectory + configsFileName + configsSuffix;
        /// <summary>
        /// 当前本地时间
        /// </summary>
        public string Time{
            get{
                var dateTimeOffset = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Local);
                return $"{dateTimeOffset:yyyy-MM-dd HH:mm:ss}"; 
            }
        }
        /// <summary>
        /// 指定图像质量的枚举
        /// </summary>
        public enum PhotoQuality{
            [Rename(nameof(None))]
            None=0,
            [Rename(nameof(Low))]
            Low=1,
            [Rename(nameof(Middle))]
            Middle=3,
            [Rename(nameof(High))]
            High=5,
        }
        /// <summary>
        /// 指定使用何种存档算法的枚举
        /// </summary>
        public enum ArchiveAlgorithmType{
            [Rename(nameof(UnityJson))]
            UnityJson,
            [Rename(nameof(ReflectionSerialization))]
            ReflectionSerialization
        }

        
    }
}
