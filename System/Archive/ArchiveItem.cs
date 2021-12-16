using System;
using System.IO;
using GalForUnity.System.Archive.SaveAlgorithm;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalForUnity.System.Archive{
    /// <summary>
    /// 存档内容及底层文件操作,如非必要不应该直接操作这个类,因为手动调用API进行的存档会绕过存档配置系统，存档系统不一定能识别该存档
    /// </summary>
    [Serializable]
    public class ArchiveItem : SerializeSelfable{
        [SerializeField] public long instanceID;
        [NonSerialized] public Texture2D Texture2D;
        
        public SavableAlgorithm savableAlgorithm;

        /// <summary>
        /// 该构造一般由存档方法调用，负责解析场景
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="scene"></param>
        public ArchiveItem(Transform transform, Scene scene){
            savableAlgorithm=SavableAlgorithm.Create(ArchiveEnvironmentConfig.GetInstance().archiveAlgorithm,transform,scene);
        }
        /// <summary>
        /// 初始化存档，将存档读取到内存，但是不加载,该构造一般由加载存档的方法调用
        /// </summary>
        /// <param name="archiveConfig">存档的配置信息</param>
        public ArchiveItem(ArchiveConfig archiveConfig){ 
            savableAlgorithm=SavableAlgorithm.Create(ArchiveEnvironmentConfig.GetInstance().archiveAlgorithm);
            LoadPhoto(archiveConfig);
        }
        
        /// <summary>
        /// 依据指定的信息保存存档
        /// </summary>
        /// <param name="dir">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="archiveSuffix">存档后缀</param>
        /// <param name="photoSuffix">图片后缀</param>
        public void Save(string dir, string fileName, string archiveSuffix, string photoSuffix){
            ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ArchiveSaveStart);
            new ArchiveThreadTool().Wait((out bool isExecuted) => {//不进行等待的话可能会出现存档存一半的问题，因为解析mono层级时利用协程异步进行的，如果同步进行可能卡帧，也不可以放到线程里解析，
                                                                   //因为解析mono类型需要在主线程操作，暂时不考虑通过Unity作业系统实现
                isExecuted = false;
                if (savableAlgorithm.parsed){
                    savableAlgorithm.Save(dir + fileName + archiveSuffix);
                    SavePhoto(dir, fileName, photoSuffix);
                    // base.Save(dir        + fileName + archiveSuffix);
                    Debug.Log("存档已经保存到：" + dir + fileName + archiveSuffix);
                    Debug.Log("提示：编辑期状态下的存档切换场景或者重启Unity后无效，打包运行后可切换场景，但是移动文件位置后依旧会导致文件找不到的异常");
                    ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ArchiveSaveOver);
                    isExecuted = true;
                }
            },true);
        }

        /// <summary>
        /// 依据指定的信息保存存档图片
        /// </summary>
        /// <param name="photoPath">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="photoSuffix">图片后缀</param>
        public Texture2D SavePhoto(string photoPath, string fileName, string photoSuffix){
            var file = photoPath + fileName + photoSuffix;
            if (!Directory.Exists(photoPath)) Directory.CreateDirectory(photoPath);
            if (!File.Exists(file) && Texture2D != null) SaveTextureToFile(file, Texture2D);
            return Texture2D;
        }

        /// <summary>
        /// 依据指定的信息读取并且加载存档,在游戏中当前场景和存档场景不一致时会加载场景如果场景一致或者在Edit模式中会尝试直接初始化
        /// </summary>
        /// <param name="dir">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="archiveSuffix">存档后缀</param>
        /// <param name="photoSuffix">图片后缀</param>
        public void Load(string dir, string fileName, string archiveSuffix, string photoSuffix){
            new ArchiveThreadTool().WaitForMono((out bool b) => {
                ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ArchiveLoadStart);
                if (!Texture2D) LoadPhoto(dir, fileName, photoSuffix);
                savableAlgorithm.Load(dir + fileName + archiveSuffix);
                // savableAlgorithm.Load(dir + fileName + archiveSuffix);
                b = true;
            });
        }

        /// <summary>
        /// 依据存档配置信息读取并且加载存档
        /// </summary>
        /// <param name="archiveConfig">存档的配置信息</param>
        /// <returns></returns>
        public void Load(ArchiveConfig archiveConfig){ Load(archiveConfig.ArchiveDirectory, archiveConfig.ArchiveFileName, archiveConfig.ArchiveSuffix, archiveConfig.PhotoSuffix); }

        /// <summary>
        /// 依据指定的信息读取存档图片
        /// </summary>
        /// <param name="photoPath">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="photoSuffix">图片后缀</param>
        public Texture2D LoadPhoto(string photoPath, string fileName, string photoSuffix){
            if (!File.Exists(photoPath + fileName + photoSuffix)) return null;

            void GetTexture(out bool isExecuted){
                if (!Texture2D){
                    Texture2D = new Texture2D(Screen.width, Screen.height);
                    Texture2D.LoadImage(GetTextureByte(photoPath + fileName + photoSuffix));
                }

                isExecuted = true;
            }

            new ArchiveThreadTool().WaitForMono(GetTexture);

            return Texture2D;
        }

        /// <summary>
        /// 依据存档配置信息读取存档图片
        /// </summary>
        /// <param name="archiveConfig">存档的配置信息</param>
        /// <returns></returns>
        public Texture2D LoadPhoto(ArchiveConfig archiveConfig){ return LoadPhoto(archiveConfig.ArchiveDirectory, archiveConfig.ArchiveFileName, archiveConfig.PhotoSuffix); }

        /// <summary>
        /// 依据指定的信息删除存档
        /// </summary>
        /// <param name="dir">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="archiveSuffix">存档后缀</param>
        /// <param name="photoSuffix">图片后缀</param>
        public void Delete(string dir, string fileName, string archiveSuffix, string photoSuffix){
            ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ArchiveDeleteStart);
            var file = dir + fileName + archiveSuffix;
            if (File.Exists(file)) File.Delete(file);
            DeletePhoto(dir, fileName, photoSuffix);
            ArchiveSystem.GetInstance().archiveEvent?.Invoke(ArchiveSystem.ArchiveEventType.ArchiveDeleteOver);
        }

        /// <summary>
        /// 依据指定的信息删除存档图片
        /// </summary>
        /// <param name="dir">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="photoSuffix">图片后缀</param>
        public void DeletePhoto(string dir, string fileName, string photoSuffix){
            var photoPath = dir + fileName + photoSuffix;
            if (File.Exists(photoPath)) File.Delete(photoPath);
        }

        /// <summary>
        /// 依据指定的信息替换存档
        /// </summary>
        /// <param name="dir">路径</param>
        /// <param name="oldFileName">旧文件名</param>
        /// <param name="newFileName">新文件名</param>
        /// <param name="archiveSuffix">存档后缀</param>
        /// <param name="photoSuffix">图片后缀</param>
        public void Override(string dir, string oldFileName, string newFileName, string archiveSuffix, string photoSuffix){
            Delete(dir, oldFileName, archiveSuffix, photoSuffix);
            Save(dir, newFileName, archiveSuffix, photoSuffix);
        }

        private static void SaveTextureToFile(string file, Texture2D tex){
            byte[] bytes = null;

            void Encode(out bool isExecuted){
                var ordinalIgnoreCase = StringComparison.OrdinalIgnoreCase;
                if (file.EndsWith(".png", ordinalIgnoreCase))
                    bytes = tex.EncodeToPNG();
                else if (file.EndsWith(".exp", ordinalIgnoreCase))
                    bytes = tex.EncodeToEXR();
                else if (file.EndsWith(".jpg", ordinalIgnoreCase) || file.EndsWith(".jpeg", ordinalIgnoreCase))
                    bytes = tex.EncodeToJPG();
                else if (file.EndsWith(".tag", ordinalIgnoreCase))
                    bytes = tex.EncodeToTGA();
                else{
                    bytes = tex.EncodeToPNG();
                    Debug.LogError("未知的后缀名，现已使用png编码");
                }

                isExecuted = true;
            }

            new ArchiveThreadTool().WaitForMono(Encode); //等待Mono解码完Texture2D后才保存图片
            SaveToFile(file, bytes);
        }

        private static byte[] GetTextureByte(string textureFile){
            FileStream files = new FileStream(textureFile, FileMode.Open);
            byte[] texByte = new byte[files.Length];
            files.Read(texByte, 0, texByte.Length);
            files.Close();
            return texByte;
        }

        private static void SaveToFile(string file, byte[] data){
            FileStream fs = null;
            if (!File.Exists(file)){
                fs = File.Create(file);
            }

            fs = fs ?? new FileStream(file, FileMode.Create, FileAccess.Write);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }

        
    }
}