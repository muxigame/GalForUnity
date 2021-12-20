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
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using GalForUnity.Attributes;
using UnityEngine;


namespace GalForUnity.System.Archive{
    [Serializable]
    public class ArchiveSet :SerializeSelfable{
        public ArchiveSet(){ }
       
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected ArchiveSet(SerializationInfo info, StreamingContext context){
            configs=(List<ArchiveConfig>) info.GetValue(nameof(configs),typeof(List<ArchiveConfig>));
        }
        
        private static ArchiveSet _archive;

        public static ArchiveSet GetInstance(){
            if (_archive == null){
                return _archive = new ArchiveSet();
            }

            return _archive;
        }
    
        [SerializeField]
        [Rename(nameof(configs))]
        public List<ArchiveConfig> configs=new List<ArchiveConfig>();
        /// <summary>
        /// 允许对象进行序列化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context){
            info.AddValue(nameof(configs),configs);
        }

        public int Count => configs.Count;
        /// <summary>
        /// 向内存中添加一个新存档到存档槽末尾
        /// </summary>
        /// <param name="archiveItem"></param>
        public ArchiveConfig AddArchive(ArchiveItem archiveItem){
            return AddArchive(new ArchiveConfig(archiveItem));
        }
        /// <summary>
        /// 向内存中添加一个新存档到存档槽末尾
        /// </summary>
        /// <param name="archiveConfig"></param>
        public ArchiveConfig AddArchive(ArchiveConfig archiveConfig){
            if(!configs.Contains(archiveConfig))configs.Add(archiveConfig);
            archiveConfig.Save();
            SaveConfig(false);
            return archiveConfig;
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
            SaveConfig(false);
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
            SaveConfig(false);
        }
        /// <summary>
        /// 向内存及文件中添加一个新存档到存档槽末尾
        /// </summary>
        /// <param name="archiveConfig"></param>
        public void SaveArchive(ArchiveConfig archiveConfig){
            configs.Add(archiveConfig);
            archiveConfig.Save();
            SaveConfig(false);
        }
        public void OverrideArchive(ArchiveItem oldArchiveItem,ArchiveItem newArchiveItem){
            var archiveConfig = configs.Find((config => config.ArchiveItem == oldArchiveItem));
            archiveConfig.Delete(); //删除就存档
            var archiveConfig1 = new ArchiveConfig(newArchiveItem,archiveConfig.ArchiveIndex);//在内存中替换掉存档
            archiveConfig1.Save();//保存新存档
            SaveConfig(false);//保存配置
        }
        public ArchiveConfig OverrideArchive(int oldArchiveItemIndex,ArchiveItem newArchiveItem){
            configs[oldArchiveItemIndex].Delete();//删除旧存档
            var archiveConfig = new ArchiveConfig(newArchiveItem,oldArchiveItemIndex);
            archiveConfig.Save();//保存新存档
            SaveConfig(false);
            return configs[oldArchiveItemIndex];
        }
        public ArchiveConfig OverrideArchive(ArchiveConfig archiveConfig,ArchiveItem newArchiveItem){
            archiveConfig.Delete();
            var config = new ArchiveConfig(newArchiveItem,archiveConfig.ArchiveIndex);
            config.Save();
            SaveConfig(false);
            return config;
        }

        public ArchiveConfig this[int index]{
            get => configs[index];
            set => configs[index] = value;
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
        

        public void SaveConfig(bool includeAllArchive = true){
            if (includeAllArchive) Save(ArchiveEnvironmentConfig.GetInstance().ConfigsFile);
            else base.Save(ArchiveEnvironmentConfig.GetInstance().ConfigsFile);
        }

        public override void Save(string fileName){
            base.Save(fileName);
            configs.ForEach(config => config.Save());
        }
        
        public void LoadConfig(bool includeAllArchive = true){
            if (includeAllArchive) Load(ArchiveEnvironmentConfig.GetInstance().ConfigsFile); 
            else base.Load(ArchiveEnvironmentConfig.GetInstance().ConfigsFile);
        }
        
        public override void Load(string str){
            ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ConfigReadStart);
            var archiveConfigs = configs.ToArray();
            base.Load(str);
#if UNITY_EDITOR
            if(configs == null) throw new FileLoadException("配置文件存在，但是序列化出错");
#else
            if(configs == null) {
                Debug.LogError("配置文件存在，但是序列化出错");
                configs=new List<ArchiveConfig>();
            }
#endif
            for (var i = 0; i < configs.Count; i++){
                if(i>=archiveConfigs.Length) configs[i].ArchiveItem = new ArchiveItem(configs[i]);
                else if (configs[i].ArchiveFileName == archiveConfigs[i].ArchiveFileName){ //如果当前项没有更新的话，复制内存中的副本，否则初始化存档项从本地加载
                    if(configs[i].ArchiveTime == archiveConfigs[i].ArchiveTime && archiveConfigs[i].ArchiveImage) configs[i].ArchiveItem=archiveConfigs[i].ArchiveItem; 
                    else configs[i].ArchiveItem = new ArchiveItem(configs[i]);
                }
            }
            // configs.ForEach(config => {
            //     if(!config.ArchiveItem.Texture2D) config.ArchiveItem = new ArchiveItem(config);
            // });
            ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ConfigReadOver); //完成刷新后发送回调数据
        }
        
    }
}