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

using GalForUnity.System.Archive.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GalForUnity.System.Archive{
    // [CreateAssetMenu(fileName = "Archive",menuName = "GalForUnity/Archive")]
    public class ArchiveSystem : GfuMonoInstanceManager<ArchiveSystem>{

        public ScrollRect scrollRect;
        public ArchiveSlot archive;
        public ArchiveSet Archive;

        public UnityEvent<ArchiveEventType> loadCallBack;
        public enum ArchiveEventType{
            LoadStart,
            LoadOver,
            SaveStart,
            SaveOver,
        }

        public int _archiveCount = 0;
            

        public void Add(){
            // scrollRect.
            var rectTransform = GameObject.Instantiate(archive, scrollRect.content, false);
            rectTransform.index = _archiveCount++;
            // scrollRect.
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
        public void Load(){
            Archive.LoadAll();
            // Load(FileName,Archive);
            for (var i = 0; i < Archive.Count; i++){
                var showArchive = ShowArchive();
                showArchive.GetComponent<ArchiveImage>().ShowImage(Archive[i].Texture2D);
            }
        }

        public GameObject ShowArchive(){
            // var scrollRectContent = ScrollRect.content;
            // var load = Resources.Load<GameObject>("image");
            // return GameObject.Instantiate(load, scrollRectContent.transform, true);
            return null;
        }
        
        
        public Texture2D CaptureScreen(Camera came, Rect r){
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
