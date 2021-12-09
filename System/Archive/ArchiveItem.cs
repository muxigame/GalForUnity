using System;
using System.Collections.Generic;
using System.IO;
using GalForUnity.Graph.Data.Property;
using GalForUnity.InstanceID;
using GalForUnity.Model;
using GalForUnity.System.Archive.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace GalForUnity.System.Archive{
    /// <summary>
    /// 存档内容及底层文件操作
    /// </summary>
    [Serializable]
    public class ArchiveItem : SerializeSelfable{
        [SerializeField] 
        public long instanceID;
        [NonSerialized] 
        public Texture2D Texture2D;
        [SerializeField]
        public List<ScriptData> scriptData=new List<ScriptData>();

        public void Save(string dir,string fileName,string archiveSuffix,string photoSuffix){
            var transform = GameSystem.GetInstance().transform;
            SaveHierarchy(transform);
            SaveMonoScript(transform);
            SavePhoto(dir, fileName, photoSuffix);
            base.Save(dir + fileName + archiveSuffix);
            
        }
        public Texture2D SavePhoto(string photoPath,string fileName,string photoSuffix){
            var file = photoPath + fileName + photoSuffix;
            if (!Directory.Exists(photoPath)) Directory.CreateDirectory(photoPath);
            if (!File.Exists(file) && Texture2D != null) SaveTextureToFile(file, Texture2D);
            return Texture2D;
        }
        
        public void Load(string dir,string fileName,string archiveSuffix,string photoSuffix){
            base.Load(dir + fileName + archiveSuffix);
            if(!Texture2D)LoadPhoto(dir, fileName, photoSuffix);
            foreach (var saveable in scriptData){
                saveable.Recover();
            }
        }

        public Texture2D LoadPhoto(string photoPath,string fileName,string photoSuffix){
            if (!File.Exists(photoPath)) return null;
            if (!Texture2D){
                Texture2D = new Texture2D(Screen.width, Screen.height);
                Texture2D.LoadImage(GetTextureByte(photoPath +fileName +photoSuffix));
            }
            return Texture2D;
        }
        
        public void Delete(string dir,string fileName,string archiveSuffix,string photoSuffix){
            var file = dir + fileName + archiveSuffix;
            if (File.Exists(file)) File.Delete(file);
            DeletePhoto(dir,fileName,photoSuffix);
        }

        public void DeletePhoto(string dir,string fileName,string photoSuffix){
            var photoPath = dir + fileName + photoSuffix;
            if (File.Exists(photoPath)) File.Delete(photoPath);
        }

        public void Override(string dir,string oldFileName,string newFileName,string archiveSuffix,string photoSuffix){
            Delete(dir,oldFileName,archiveSuffix,photoSuffix);
            Save(dir,newFileName,archiveSuffix,photoSuffix);
        }
        
        private static void SaveTextureToFile(string file, Texture2D tex){
            byte[] bytes = tex.EncodeToPNG();
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
            fs=fs ?? new FileStream(file, FileMode.Create, FileAccess.Write);
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }
        private void SaveHierarchy(Transform transform){
            for (int i = 0; i < transform.childCount; i++){
                SaveHierarchy(transform.GetChild(i));
            }

            if (transform.GetComponent<GfuInstance>()){
                var savable = new ScriptData(transform.gameObject);
                scriptData.Add(savable);
            }
        }
        private void SaveMonoScript(Transform transform){
            for (int i = 0; i < transform.childCount; i++){
                SaveMonoScript(transform.GetChild(i));
            }
            var components = transform.GetComponents<MonoBehaviour>();
            
            // var gfuInstances = transform.GetComponent<GfuInstance>();
            foreach (var component in components){
                if (component.gameObject.hideFlags == HideFlags.HideInHierarchy | component.gameObject.hideFlags == HideFlags.HideInInspector) continue;
                var savable = new ScriptData(component);
                if (!string.IsNullOrEmpty(savable.ObjectAddressExpression)){
                    scriptData.Add(savable);
                }
            }
        }
        
    }
}