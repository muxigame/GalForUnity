//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArchiveSystem.cs
//
//        Created by 半世癫(Roc) at 2021-02-11 17:44:12
//
//======================================================================

using System;
using System.Threading;
using GalForUnity.System.Archive.UI;
using GalForUnity.System.Event;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GalForUnity.System.Archive{
    // [CreateAssetMenu(fileName = "Archive",menuName = "GalForUnity/Archive")]
    public class ArchiveSystem : GfuMonoInstanceManager<ArchiveSystem>{

        public ScrollRect scrollRect;
        public ArchiveSlot archiveSlot;
        public Camera renderCamera;
        [SerializeField]
        private ArchiveSet archiveSet;
        public ArchiveSet ArchiveSet=>archiveSet;
        [SerializeField]
        public ArchiveEnvironmentConfig archiveEnvironmentConfig;
        // public ArchiveSet ArchiveSet=>ArchiveSet.Instance;

        public UnityEvent<ArchiveEventType> archiveEvent=new UnityEvent<ArchiveEventType>();
        public enum ArchiveEventType{
            ConfigReadStart,
            ConfigReadOver,
            ArchiveLoadStart,
            ArchiveLoadOver,
            SceneLoadStart,
            SceneLoadOver,
            ArchiveDeleteStart,
            ArchiveDeleteOver,
            ArchiveSaveStart,
            ArchiveSaveOver,
            SaveStart,
            SaveOver,
            RefreshStart,
            RefreshOver,
        }

        ArchiveSystem(){
            archiveEvent.AddListener(EventCenter.GetInstance().archiveEvent.Invoke);
        }

        private void Awake(){
            archiveSet = ArchiveSet.GetInstance();
            archiveEnvironmentConfig=ArchiveEnvironmentConfig.GetInstance();
            var archiveDirectory = archiveEnvironmentConfig.ArchiveDirectory;
            if (ArchiveSystem.GetInstance() != this){
                var obj= gameObject; //直接在Mono代理里面写Destroy(gameObject);是不可行的，因为访问不到，组件被销毁的话是不能通过组件访问游戏对象的
                // Destroy(this);       //如果直接删gameObject，那么所有实例中的archive将会丢失引用，scrollRect等mono类不会，或许是因为他们是序列化保存的原因，暂时未查明原因，
                // GfuRunOnMono.LateUpdate(() => {
                //      // 考虑改进，但是请不要因为会丢引用就去访问文件，这比较耗时。
                // });
                Destroy(obj);
            } else DontDestroyOnLoad(gameObject);
        }

        ~ArchiveSystem(){
            //archiveEvent.RemoveAllListeners();
        }
        
        public int ArchiveCount => archiveSet.Count;


        public void AddArchiveSlotUI(ArchiveConfig archiveConfig){
            var newArchiveSlot = GameObject.Instantiate(archiveSlot, scrollRect.content, false);
            newArchiveSlot.Init(archiveConfig);
        }
        public void RemoveArchiveSlot(ArchiveConfig archiveConfig){
            Destroy(archiveConfig.ArchiveSlot);
        }
        /// <summary>
        /// 异步加载默认索引的存档
        /// </summary>
        public void LoadAsync() => LoadAsync(0);
        /// <summary>
        /// 异步加载指定索引号的存档
        /// </summary>
        /// <param name="index">存档索引</param>
        public void LoadAsync(int index) => LoadAsync(index, null);

        /// <summary>
        /// 异步加载指定索引号的存档
        /// </summary>
        /// <param name="index">存档索引</param>
        /// <param name="callback">回调</param>
        public void LoadAsync(int index,UnityAction<AsyncOperation,ArchiveEventType> callback){
            new Thread(() => {
                if (callback != null){
                    void Listener(ArchiveEventType archiveEventType){
                        callback.Invoke(ArchiveEnvironmentConfig.GetInstance().asyncOperation,archiveEventType);
                        if (archiveEventType == ArchiveEventType.ArchiveLoadOver){
                            archiveEvent?.RemoveListener(Listener);
                        }
                    }
                    archiveEvent?.AddListener(Listener); //添加监听器监听文件读取完成回调
                }
                Load(index);
            }){Name = ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadName}.Start();
        }
        /// <summary>
        /// 加载默认索引的存档
        /// </summary>
        public void Load() => Load(0);
        /// <summary>
        /// 加载指定索引号的存档
        /// </summary>
        /// <param name="index">存档索引</param>
        public void Load(int index){
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            var archiveConfig = GetArchiveConfig(index);
            if (archiveConfig == null || archiveConfig.ArchiveItem == null){
                ReadArchiveConfig();
                archiveConfig = GetArchiveConfig(index);
            }
            archiveConfig.ArchiveItem.Load(archiveConfig);
        }
        /// <summary>
        /// 将图片异步保存到默认索引号，通常用于快速存档
        /// </summary>
        public void SaveAsync() => SaveAsync(0);
        /// <summary>
        /// 异步保存指定存档到指定索引号
        /// </summary>
        /// <param name="index">存档索引</param>
        public void SaveAsync(int index) => SaveAsync(index, null);
        /// <summary>
        /// 异步保存指定存档到指定索引号，同时向存档事件添加回调监听
        /// </summary>
        /// <param name="index">存档索引</param>
        /// <param name="callback">回调</param>
        public void SaveAsync(int index,UnityAction callback){
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            new Thread(() => {
                if (callback != null){
                    void Listener(ArchiveEventType archiveEventType){
                        if (archiveEventType == ArchiveEventType.ArchiveSaveOver){
                            archiveEvent?.RemoveListener(Listener);
                            callback.Invoke();
                        }
                    }
                    archiveEvent?.AddListener(Listener); //添加监听器监听文件读取完成回调
                }
                Save(index);
            }){Name = ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadName}.Start();
        }
        
        /// <summary>
        /// 将图片保存到默认索引号，通常用于快速存档
        /// </summary>
        public void Save() => Save(0);
        /// <summary>
        /// 依据指定索引保存存档，如果目标存档已经存在则会自动覆盖
        /// </summary>
        /// <param name="index">存档索引</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void Save(int index){
            archiveEvent.Invoke(ArchiveEventType.SaveStart);
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            ArchiveItem archiveItem = null;

            void SaveAndOverride(out bool isExecuted){
                int photoQuality =(int) ArchiveEnvironmentConfig.GetInstance().photoQuality;
                var quality = photoQuality / 5f;
                archiveItem = new ArchiveItem(GameSystem.GetInstance().transform, SceneManager.GetActiveScene()) {
                    Texture2D = CaptureScreen(renderCamera?renderCamera:Camera.main, new Rect(0,0,quality *Screen.width,quality*Screen.height))
                };
                isExecuted = true;
            }
            
            new ArchiveThreadTool().WaitForMono(SaveAndOverride);//等待Mono保存场景数据
            
            if (index < archiveSet.Count){
                archiveSet.OverrideArchive(index,archiveItem);
            } else{
                archiveSet.AddArchive(archiveItem);
            }
            
            // ArchiveSet.SaveConfig(false);
            
            archiveEvent.Invoke(ArchiveEventType.SaveOver);
            RefreshAllAndReadConfigAsync();
        }

        /// <summary>
        /// 异步删除指定索引的存档
        /// </summary>
        /// <param name="index">索引</param>
        public void DeleteAsync(int index = 0) => Delete(index);

        /// <summary>
        /// 异步删除指定索引的存档,并添加回调
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="callback">回调</param>
        public void DeleteAsync(int index, UnityAction callback){
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            new Thread(() => {
                if (callback != null){
                    void Listener(ArchiveEventType archiveEventType){
                        if (archiveEventType == ArchiveEventType.ArchiveDeleteOver){
                            archiveEvent?.RemoveListener(Listener);
                            callback.Invoke();
                        }
                    }
                    archiveEvent?.AddListener(Listener); //添加监听器监听文件删除完成回调
                }
                Delete(index);
            }){Name = ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadName}.Start();
        }
        /// <summary>
        /// 删除指定索引的存档
        /// </summary>
        /// <param name="index">索引</param>
        public void Delete(int index=0){
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            archiveSet.DeleteArchive(index);
            RefreshAllAndReadConfigAsync();
        }
        [Obsolete]
        public void Clear(){
            archiveSet.Clear();
        }
        /// <summary>
        /// 从内存中获得一个存档配置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ArchiveConfig GetArchiveConfig(int index=0){
            return archiveSet[index];
        }

        /// <summary>
        /// 从硬盘中读取存档配置
        /// </summary>
        public void ReadArchiveConfig(){
            archiveSet.LoadConfig();
        } 
        /// <summary>
        /// 异步读取存档配置
        /// TODO 请不要再Awake中调用此方法初步调查发现，异常为：无法将类型为“Mono.Debugger.Soft.PrimitiveValue”的对象强制转换为类型“Mono.Debugger.Soft.ObjectMirror”。
        /// 初步判定可能是Mono平台的BUG
        /// </summary>
        public void ReadArchiveConfigAsync(UnityAction callback=null){
            if (callback != null){
                void Listener(ArchiveEventType x){
                    if (x == ArchiveEventType.ConfigReadOver){
                        archiveEvent?.RemoveListener(Listener);
                        GfuRunOnMono.Update(callback.Invoke);
                    }
                }
                archiveEvent?.AddListener(Listener); //添加监听器监听文件读取完成回调
            }
            new Thread(ReadArchiveConfig){Name = ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadName}.Start();
        } 
        /// <summary>
        /// 异步读取存档配置后刷新存档槽的显示
        /// </summary>
        public void RefreshAllAndReadConfigAsync(){
            archiveEvent?.Invoke(ArchiveEventType.RefreshStart);
            void Listener(ArchiveEventType x){
                if (x == ArchiveEventType.ConfigReadOver){
                    archiveEvent?.RemoveListener(Listener);
                    GfuRunOnMono.Update(RefreshAll);
                }
            }
            archiveEvent?.AddListener(Listener); //添加监听器监听文件读取完成回调
            new Thread(ReadArchiveConfig){Name = ArchiveEnvironmentConfig.GetInstance().archiveSystemThreadName}.Start();
        }

        /// <summary>
        /// 刷新存档槽的显示
        /// </summary>
        public void RefreshAll(){
            for (int i = scrollRect.content.childCount - 1; i >= 0; i--){
                Destroy(scrollRect.content.GetChild(i).gameObject);
            }
            for (var i = 0; i < archiveSet.Count; i++){
                AddArchiveSlotUI(archiveSet[i]);
            }
            archiveEvent?.Invoke(ArchiveEventType.RefreshOver);//完成刷新后发送回调数据
        }
        
        /// <summary>
        /// 将相机内的画面截图为Texture2D，如过UI不是世界UI则不会包含在截图内
        /// </summary>
        /// <param name="came"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private Texture2D CaptureScreen(Camera came, Rect r){
            RenderTexture rt = new RenderTexture((int)r.width, (int)r.height, 0);
            came.targetTexture = rt;
            came.Render();
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(r, 0, 0);
            screenShot.Apply();
            came.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(rt);
            return screenShot;
        }


    }
}
