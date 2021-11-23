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
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GalForUnity.Graph.Data.Property;
using GalForUnity.Model.Plot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GalForUnity.System.Archive{
    [CreateAssetMenu(fileName = "Archive",menuName = "GalForUnity/Archive")]
    public class ArchiveSystem : ScriptableObject{

        public ArchiveSet Archive;
        public ScrollRect ScrollRect;
        public PlotItemGraphData PlotItemGraphData;
    
        public UnityEvent<ArchiveEventType> loadCallBack;
        public enum ArchiveEventType{
            LoadStart,
            LoadOver,
            SaveStart,
            SaveOver,
        }
    

        // [SerializeField]
        // public List<ArchiveItem> archiveItems;

        // [HideInInspector]
        public static string Path = "";
        public static string photoSuffix = ".png";
        public static string archiveSuffix = ".txt";
        public static string configsFileName = "Archive.txt";

        private void Reset(){
#if UNITY_EDITOR
            Path = "Assets/Archive/";
#else
        Path = Application.persistentDataPath+"/Archive/";
#endif
            Archive = ArchiveSet.Instance;
            // archiveItems = Archive.Data;
        }

        private void OnValidate(){
            if (string.IsNullOrEmpty(Path)){
            #if UNITY_EDITOR
                Path = "Assets/Archive/";
#else
            Path = Application.persistentDataPath+"/Archive/";
#endif
            }
        }

        public void Add(){
            // StartCoroutine(AddItem());
        }
        // public IEnumerator AddItem(PlotFlow plotFlow){
        //     var plotFlowPlotItemGraph = plotFlow.PlotItemGraph;
        //     yield return new WaitForEndOfFrame();
        //     Archive.AddArchive(new ArchiveItem(){
        //         Texture2D =  ScreenCapture.CaptureScreenshotAsTexture()
        //     });
        //     Debug.Log(Archive.Count);
        //     var showArchive = ShowArchive();
        //     showArchive.GetComponent<ArchiveImage>().ShowImage(Archive.Last.Texture2D);
        // }
        // public IEnumerator AddItem(){
        //     yield return new WaitForEndOfFrame();
        //     Archive.AddArchive(new ArchiveItem(){
        //         Texture2D =  ScreenCapture.CaptureScreenshotAsTexture(),
        //         PlotItemGraphData = PlotItemGraphData
        //     });
        //     Debug.Log(Archive.Count);
        //     var showArchive = ShowArchive();
        //     showArchive.GetComponent<ArchiveImage>().ShowImage(Archive.Last.Texture2D);
        // }

        public void Sub(){
            Archive.DeleteArchive(Archive.Count-1);
            Debug.Log(Archive.Count);
        }
        public void Clear(){
            Archive.Clear();
            Debug.Log(Archive);
        }
        public void Save(){
            Archive.SaveAll();
        }
        public void LoadAsync(){
            // StartCoroutine(LoadCoroutine());
        }
        public IEnumerator LoadCoroutine(){
            // loadCallBack.Invoke(null);
            yield return new WaitForSeconds(1);
            Load(configsFileName,Archive);
            yield return null;
            for (var i = 0; i < Archive.Count; i++){
                // loadCallBack.Invoke(Archive[i]);
                yield return new WaitForSeconds(0.25f);
            }
        }
        public void Load(){
            Archive.LoadAll();
            // Load(FileName,Archive);
            for (var i = 0; i < Archive.Count; i++){
                var showArchive = ShowArchive();
                showArchive.GetComponent<ArchiveImage>().ShowImage(Archive[i].Texture2D);
            }
        }

        public GameObject ShowArchive(){
            var scrollRectContent = ScrollRect.content;
            var load = Resources.Load<GameObject>("image");
            return GameObject.Instantiate(load, scrollRectContent.transform, true);
        }
    
        public static void Save(string fileName,object obj){
            BinaryFormatter binaryFormatter=new BinaryFormatter();
            // binaryFormatter.Serialize();
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            // if (!File.Exists(path + fileName)) File.Create(path + fileName);
            FileStream fileStream = File.Create(Path + fileName);
            binaryFormatter.Serialize(fileStream,JsonUtility.ToJson(obj));
            fileStream.Close();
        }
        public void Load(string fileName,out ArchiveSet obj){
            obj = ArchiveSet.Instance;
            Load(fileName, obj);
        }
        public void Load(string fileName, ArchiveSet obj){
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            BinaryFormatter binaryFormatter=new BinaryFormatter();
            if (File.Exists(Path + fileName)){
                FileStream fileStream = File.OpenRead(Path + fileName);
                var deserialize = (string) binaryFormatter.Deserialize(fileStream);
                Debug.Log(deserialize);
                JsonUtility.FromJsonOverwrite(deserialize,obj);
                fileStream.Close();
            };
        }

   
        public static void SaveTextureToFile(string file, Texture2D tex)
        {
            byte[] bytes = tex.EncodeToPNG();
            SaveToFile(file, bytes);
        }
 
        public static void SaveToFile(string file, byte[] data){
            FileStream fs = null;
            if (!File.Exists(file)){
                fs = File.Create(file);
            }
            fs=fs?? new FileStream(file, FileMode.Create, FileAccess.Write);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }

        public static byte[] GetTextureByte(string textureFile)
        {
            FileStream files = new FileStream(textureFile, FileMode.Open);
            byte[] texByte = new byte[files.Length];
            files.Read(texByte, 0, texByte.Length);
            files.Close();
            return texByte;
        }

        public Texture2D CaptureScreen(Camera came, Rect r)
        {
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
            // byte[] bytes = screenShot.EncodeToPNG();
            // string filename = Application.streamingAssetsPath + "/ScreenShot.png";
            // System.IO.File.WriteAllBytes(filename, bytes);
            return screenShot;
        }


    }
}
