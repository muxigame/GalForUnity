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

        
        [SerializeField]
        public string archiveSuffix=".dat";
        [SerializeField]
        public string photoSuffix=".png";
        [SerializeField]
        public string archiveDefaultName="存档";
        [SerializeField]
        public PhotoQuality photoQuality=PhotoQuality.Low;
        [SerializeField]
        private string configsFileName="archiveConfig";
        [SerializeField]
        private string configsSuffix=".config";
        [SerializeField]
        public string archiveSystemThreadName="ArchiveSystemThread";
        [SerializeField]
        public AsyncOperation asyncOperation;
        [SerializeField]
        public int archiveSystemThreadWaitingTime=10;
        [SerializeField]
        public int archiveSystemThreadWaitingMaxSecond=500;

        public string ConfigsFile => ArchiveDirectory + configsFileName + configsSuffix;
        public string Time{
            get{
                var dateTimeOffset = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Local);
                return $"{dateTimeOffset:yyyy-MM-dd HH:mm:ss}"; 
            }
        }
        public enum PhotoQuality{
            None=0,
            Low=1,
            Meddle=3,
            Height=5,
        }
    }
}
