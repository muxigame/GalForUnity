//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArchiveConfig.cs
//
//        Created by 半世癫(Roc) at 2021-02-12 15:21:11
//
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalForUnity.System.Archive{
    /// <summary>
    /// ArchiveConfig不应该是一个可配置的类，而是一个自动配置的类
    /// </summary>
    [Serializable]
    public class ArchiveConfig{
        
        [SerializeField]
        private string archiveFileName;
        [SerializeField]
        private string archiveTime;
        [NonSerialized]
        private ArchiveItem _archiveItem;

        public ArchiveItem ArchiveItem{
            get => _archiveItem;
            set => _archiveItem = value;
        }

        public string ArchiveTime{
            get => archiveTime;
            set => archiveTime = value;
        }

        public string ArchiveFileName{
            get => archiveFileName;
            set => archiveFileName = value;
        }

        public string ArchiveDirectory=>ArchiveEnvironmentConfig.GetInstance().archiveDirectory;
        public string ArchiveSuffix=>ArchiveEnvironmentConfig.GetInstance().archiveSuffix;
        public string PhotoSuffix=>ArchiveEnvironmentConfig.GetInstance().photoSuffix;
        
        public Texture2D ArchiveImage => _archiveItem.Texture2D ? _archiveItem.Texture2D : _archiveItem.LoadPhoto(ArchiveDirectory,archiveFileName,PhotoSuffix);

        private static List<ArchiveConfig> _configs = ArchiveSet.Instance.configs;
        
        public ArchiveConfig(string name){
            archiveFileName = name;
            archiveTime = ArchiveEnvironmentConfig.GetInstance().Time;
            _configs.Add(this);
        }
        public ArchiveConfig(ArchiveItem archiveItem){
            this._archiveItem = archiveItem;
            _configs.Add(this);
        }
        public static ArchiveConfig Create(string name){
            var archiveConfig = new ArchiveConfig(name);
            _configs.Add(archiveConfig);
            return archiveConfig;
        }
        /// <summary>
        /// 存档配置的总数，可以理解为存档槽的总数
        /// </summary>
        public static int ArchiveCount => _configs.Count;
        /// <summary>
        /// 任何对象的所以索引均是访问在集合中的值
        /// </summary>
        public int ArchiveIndex=>_configs.IndexOf(this);


        public void Save(){
            if (string.IsNullOrEmpty(archiveFileName)){
                var dateTimeOffset = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Local);
                archiveTime = $"{dateTimeOffset:yyyy-MM-dd HH-mm-ss}";
            }
            _archiveItem?.Save(ArchiveDirectory,archiveFileName,ArchiveSuffix,PhotoSuffix);
        }
        public void SaveAll(){
            _configs.ForEach((config => config.Save()));
        }
        public void Load(){
            if (!string.IsNullOrEmpty(archiveFileName)){
                if(_archiveItem==null) _archiveItem=new ArchiveItem();
                _archiveItem.Load(ArchiveDirectory,archiveFileName,ArchiveSuffix,PhotoSuffix);
            }
        }
        public void Delete(){
            if (!string.IsNullOrEmpty(archiveFileName))
                _archiveItem?.Delete(ArchiveDirectory,archiveFileName,ArchiveSuffix,PhotoSuffix);
        }
        
        public void LoadAll(){
            _configs.ForEach(config => config.Load());
        }
        
    }
}