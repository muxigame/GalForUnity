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
using UnityEngine;

namespace GalForUnity.System.Archive{
    [Serializable]
    public class ArchiveEnvironmentConfig:GfuInstanceManager<ArchiveEnvironmentConfig>{
        [NonSerialized] 
        public string archiveDirectory =
#if UNITY_EDITOR
            Application.dataPath;
#else
            Application.persistentDataPath;
#endif
        [SerializeField]
        public string archiveSuffix="dat";
        [SerializeField]
        public string photoSuffix=".png";
        [SerializeField]
        private string configsFileName="archiveConfig";
        [SerializeField]
        private string configsSuffix=".config";

        public string ConfigsFile => archiveDirectory + configsFileName + configsSuffix;
        public string Time{
            get{
                var dateTimeOffset = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Local);
                return $"{dateTimeOffset:yyyy-MM-dd HH-mm-ss}"; 
            }
        }
        
    }
}
