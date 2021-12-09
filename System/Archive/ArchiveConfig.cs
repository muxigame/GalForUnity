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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GalForUnity.Graph;
using UnityEngine;

namespace GalForUnity.System.Archive{
    /// <summary>
    /// ArchiveConfig不应该是一个可配置的类，而是一个自动配置的类
    /// </summary>
    [Serializable]
    public class ArchiveConfig{

        private static List<ArchiveConfig> _configs = ArchiveSet.Instance.configs;
        /// <summary>
        /// 任何对象的所以索引均是访问在集合中的值
        /// </summary>
        // public ArchiveItem this[int index]{
        //     set=>_configs[index].archiveItem=value;
        //     get=>_configs[index].archiveItem;
        // }
        public ArchiveConfig(){
            _configs.Add(this);
        }
        public ArchiveConfig(ArchiveItem archiveItem){
            this.archiveItem = archiveItem;
            _configs.Add(this);
        }
        public static ArchiveConfig Create(){
            var archiveConfig = new ArchiveConfig();
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
        
        [SerializeField]
        public string archiveFileName;
        [NonSerialized]
        public ArchiveItem archiveItem;
        

        public void Save(){
            if (string.IsNullOrEmpty(archiveFileName)){
                var dateTimeOffset = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Local);
                archiveFileName = $"{dateTimeOffset:yyyy-MM-dd HH-mm-ss}";
            }
            archiveItem?.Save(archiveFileName);
        }
        public void SaveAll(){
            _configs.ForEach((config => config.Save()));
        }
        public void Load(){
            if (!string.IsNullOrEmpty(archiveFileName)){
                if(archiveItem==null) archiveItem=new ArchiveItem();
                archiveItem.Load(archiveFileName);
            }
        }
        public void Delete(){
            if (!string.IsNullOrEmpty(archiveFileName))
                archiveItem?.Delete(archiveFileName);
        }
        
        public void LoadAll(){
            _configs.ForEach(config => config.Load());
        }

        public void SaveGraph(){
            Debug.Log(Application.persistentDataPath);
            FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
            BinaryFormatter bf = new BinaryFormatter();
            //序列化储存
            GfuRunOnMono.LateUpdate(() => {
                Debug.Log(GameSystem.Data.PlotFlowController.CurrentGraph);
                bf.Serialize(file, GameSystem.Data.PlotFlowController.CurrentGraph); //将data序列化为file文件（即playerInfo.dat）储存
                file.Close();                                                        //关闭流操作
            });
        }

        public void LoadGraph(){
            // Debug.Log(Application.persistentDataPath);
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            PlotItemGraph plotItem = bf.Deserialize(file) as PlotItemGraph;     
            file.Close();
            Debug.Log(plotItem.RootNode);
        }
        // private GameObject loadImage;
        // public void ShowLoad(ArchiveItem archiveItem){
        //     
        //     if (archiveItem == null){
        //         var load = Resources.Load<GameObject>("LoadingImage");
        //         loadImage = Instantiate(load,GetComponent<ArchiveSystem>().ScrollRect.transform,false);
        //         var rectTransform = loadImage.GetComponent<RectTransform>();
        //         // transform.tr
        //     }
        //     else{
        //          if(loadImage)
        //             Destroy(loadImage);
        //          var showArchive = ShowArchive();
        //          showArchive.GetComponent<ArchiveImage>().ShowImage(archiveItem.Texture2D);
        //     }
        // }
        // public GameObject ShowArchive(){
        //     var scrollRectContent = GetComponent<ArchiveSystem>().ScrollRect.content;
        //     var load = Resources.Load<GameObject>("image");
        //     return GameObject.Instantiate(load, scrollRectContent.transform, true);
        // }
    }
}