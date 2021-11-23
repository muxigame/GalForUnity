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
using GalForUnity.Graph.Data.Property;
using GalForUnity.Model;
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
                config.Delete();
                return config.archiveItem == archiveItem;
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
            var archiveConfig = configs.Find((config => config.archiveItem == oldArchiveItem));
            var archiveConfig1 = configs[archiveConfig.ArchiveIndex] = new ArchiveConfig(newArchiveItem);//在内存中替换掉存档
            oldArchiveItem.Delete(archiveConfig.archiveFileName);//删除就存档
            archiveConfig1.Save();//保存新存档
            SaveConfig();//保存配置
        }
        public void OverrideArchive(int oldArchiveItemIndex,ArchiveItem newArchiveItem){
            configs[oldArchiveItemIndex].Delete();//删除旧存档
            configs[oldArchiveItemIndex]=new ArchiveConfig(newArchiveItem);//在内存中替换掉
            configs[oldArchiveItemIndex].Save();//保存新存档
        }
        public void OverrideArchive(ArchiveConfig archiveConfig,ArchiveItem newArchiveItem){
            var config = new ArchiveConfig(newArchiveItem);
            configs[archiveConfig.ArchiveIndex] = config;
            archiveConfig.Delete();
            config.Save();
            SaveConfig();
        }

        public ArchiveItem this[int index]{
            get => configs[index].archiveItem;
            set => configs[index].archiveItem = value;
        }
        
        public ArchiveItem GetArchive(int index){
            return configs[index].archiveItem;
        }
        public ArchiveItem GetArchiveWithInstanceID(long instanceID){
            return configs.Find(item => {
                var itemArchiveItem = item.archiveItem;
                if (itemArchiveItem != null){
                    return itemArchiveItem.instanceID == instanceID||
                           itemArchiveItem.plotFlowGraphDataInstanceID == instanceID||
                           itemArchiveItem.plotItemGraphDataInstanceID == instanceID;
                }
                return false;
            }).archiveItem;
        }
        public ArchiveItem Last => configs[configs.Count - 1].archiveItem;
        public void Clear(){
            configs.Clear();
        }
        public void SaveAll(){
            configs.ForEach(config => config.Save());
            base.Save(ArchiveSystem.Path+ArchiveSystem.configsFileName);
        }

        public void SaveConfig() => base.Save(ArchiveSystem.Path+ArchiveSystem.configsFileName);
        
        public void LoadAll(){
            base.Load(ArchiveSystem.Path+ArchiveSystem.configsFileName);
            configs.ForEach(config => config.Load());
        }

        public void LoadConfig() => base.Load(ArchiveSystem.Path + ArchiveSystem.configsFileName);
    }

    /// <summary>
    /// 存档内容及底层文件操作
    /// </summary>
    [Serializable]
    public class ArchiveItem : SerializeSelfable{
        [SerializeField] public long instanceID;
        [SerializeField] public long plotItemGraphDataInstanceID;
        [SerializeField] public long plotFlowGraphDataInstanceID;
        [SerializeField] public long startIndex;
        [SerializeField] public RoleData roleData;
        [NonSerialized] public Texture2D Texture2D;
        [SerializeField] public string name;
        [SerializeField] public string speak;

        public PlotItemGraphData PlotItemGraphData{
            get{
                var findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll<PlotItemGraphData>();
                foreach (var itemGraphData in findObjectsOfTypeAll){
                    if (itemGraphData.instanceID == plotItemGraphDataInstanceID){
                        return itemGraphData;
                    }
                }

                throw new NullReferenceException("InstanceID not exit in Resources");
            }
            set => plotItemGraphDataInstanceID = value.instanceID;
        }

        public PlotFlowGraphData PlotFlowGraphData{
            get{
                var findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll<PlotFlowGraphData>();
                foreach (var itemGraphData in findObjectsOfTypeAll){
                    if (itemGraphData.instanceID == plotFlowGraphDataInstanceID){
                        return itemGraphData;
                    }
                }

                throw new NullReferenceException("InstanceID not exit in Resources");
            }
            set => plotFlowGraphDataInstanceID = value.instanceID;
        }
        public override void Save(string fileName){
            var photoPath=ArchiveSystem.Path + fileName + ArchiveSystem.photoSuffix;
            if (!Directory.Exists(ArchiveSystem.Path)) Directory.CreateDirectory(ArchiveSystem.Path);
            if (!File.Exists(photoPath) && Texture2D != null) ArchiveSystem.SaveTextureToFile(photoPath, Texture2D);
            base.Save(ArchiveSystem.Path + fileName + ArchiveSystem.archiveSuffix);
        }
        public override void Load(string fileName){
            base.Load(ArchiveSystem.Path + fileName + ArchiveSystem.archiveSuffix);
            var photoPath=ArchiveSystem.Path + fileName + ArchiveSystem.photoSuffix;
            if (!File.Exists(photoPath)) return;
            if (!Texture2D) Texture2D = new Texture2D(500, 500);
            Texture2D.LoadImage(ArchiveSystem.GetTextureByte(photoPath));
        }
        public void Delete(string fileName){
            var photoPath = ArchiveSystem.Path + fileName + ArchiveSystem.photoSuffix;
            var archivePath = ArchiveSystem.Path + fileName + ArchiveSystem.archiveSuffix;
            if (File.Exists(photoPath)) File.Delete(photoPath);
            if (File.Exists(archivePath)) File.Delete(archivePath);
        }
        
        public void Override(string oldFileName,string newFileName){
            Delete(oldFileName);
            Save(newFileName);
        }
    }
}