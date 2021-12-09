//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArchiveSet.cs
//
//        Created by 半世癫(Roc) at 2021-02-12 14:21:22
//
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalForUnity.System.Archive{
    [Serializable]
    public class ArchiveSet :SerializeSelfable{
        
        public static ArchiveSet Instance=new ArchiveSet();
    
        [SerializeField]
        public List<ArchiveConfig> configs=new List<ArchiveConfig>();

        public int Count => configs.Count;
        /// <summary>
        /// 向内存中添加一个新存档到存档槽末尾
        /// </summary>
        /// <param name="archiveItem"></param>
        public void AddArchive(ArchiveItem archiveItem){
            configs.Add(new ArchiveConfig(archiveItem));
        }
        /// <summary>
        /// 向内存中添加一个新存档到存档槽末尾
        /// </summary>
        /// <param name="archiveConfig"></param>
        public void AddArchive(ArchiveConfig archiveConfig){
            configs.Add(archiveConfig);
        }
        /// <summary>
        /// 在内存及文件中删除一个存档
        /// </summary>
        /// <param name="archiveConfig"></param>
        public void DeleteArchive(ArchiveConfig archiveConfig){
            configs.Remove(archiveConfig);
            archiveConfig.Delete();
        }
        /// <summary>
        /// 在内存及文件中删除一个存档
        /// </summary>
        /// <param name="archiveIndex"></param>
        public void DeleteArchive(int archiveIndex){
            configs[archiveIndex].Delete();
            configs.RemoveAt(archiveIndex);
        }
        /// <summary>
        /// 在内存及文件中删除一个存档
        /// </summary>
        /// <param name="archiveItem"></param>
        public void DeleteArchive(ArchiveItem archiveItem){
            configs.RemoveAll((config => {
                if (config.ArchiveItem == archiveItem){
                    config.Delete();
                    return true;
                }
                return false;
            }));
        }
        /// <summary>
        /// 向内存及文件中添加一个新存档到存档槽末尾
        /// </summary>
        /// <param name="archiveItem"></param>
        public void SaveArchive(ArchiveItem archiveItem){
            var archiveConfig = new ArchiveConfig(archiveItem);
            configs.Add(archiveConfig);
            archiveConfig.Save();
            SaveConfig();
        }
        /// <summary>
        /// 向内存及文件中添加一个新存档到存档槽末尾
        /// </summary>
        /// <param name="archiveConfig"></param>
        public void SaveArchive(ArchiveConfig archiveConfig){
            configs.Add(archiveConfig);
            archiveConfig.Save();
            SaveConfig();
        }
        public void OverrideArchive(ArchiveItem oldArchiveItem,ArchiveItem newArchiveItem){
            var archiveConfig = configs.Find((config => config.ArchiveItem == oldArchiveItem));
            var archiveConfig1 = configs[archiveConfig.ArchiveIndex] = new ArchiveConfig(newArchiveItem);//在内存中替换掉存档
            archiveConfig.Delete();//删除就存档
            archiveConfig1.Save();//保存新存档
            SaveConfig();//保存配置
        }
        public void OverrideArchive(int oldArchiveItemIndex,ArchiveItem newArchiveItem){
            configs[oldArchiveItemIndex].Delete();//删除旧存档
            configs[oldArchiveItemIndex]=new ArchiveConfig(newArchiveItem);//在内存中替换掉
            configs[oldArchiveItemIndex].Save();//保存新存档
            SaveConfig();
        }
        public void OverrideArchive(ArchiveConfig archiveConfig,ArchiveItem newArchiveItem){
            var config = new ArchiveConfig(newArchiveItem);
            configs[archiveConfig.ArchiveIndex] = config;
            archiveConfig.Delete();
            config.Save();
            SaveConfig();
        }

        public ArchiveItem this[int index]{
            get => configs[index].ArchiveItem;
            set => configs[index].ArchiveItem = value;
        }
        
        public ArchiveItem GetArchive(int index){
            return configs[index].ArchiveItem;
        }
        public ArchiveItem GetArchiveWithInstanceID(long instanceID){
            return configs.Find(item => {
                var itemArchiveItem = item.ArchiveItem;
                if (itemArchiveItem != null){
                    return itemArchiveItem.instanceID == instanceID;
                }
                return false;
            }).ArchiveItem;
        }
        public ArchiveItem Last => configs[configs.Count - 1].ArchiveItem;
        public void Clear(){
            configs.Clear();
        }
        public void SaveAll(){
            configs.ForEach(config => config.Save());
            SaveConfig();
        }

        public void SaveConfig() => base.Save(ArchiveEnvironmentConfig.GetInstance().ConfigsFile);
        
        public void LoadAll(){
            base.Load( ArchiveEnvironmentConfig.GetInstance().ConfigsFile);
            configs.ForEach(config => config.Load());
        }

        public void LoadConfig() => base.Load( ArchiveEnvironmentConfig.GetInstance().ConfigsFile);
    }
}