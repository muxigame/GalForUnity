//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SerializeSelfable.cs
//
//        Created by 半世癫(Roc) at 2021-02-12 18:58:17
//
//======================================================================

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;

namespace GalForUnity.System.Archive{
    [Serializable]
    public class SerializeSelfable{
        public virtual void Save(string fileName){
            
            BinaryFormatter binaryFormatter=new BinaryFormatter();
            // binaryFormatter.Serialize();
            FileStream fileStream = null;
            if (!File.Exists(fileName)){
                fileStream = File.Create(fileName);
            }
            fileStream = fileStream??File.OpenWrite(fileName);
            // JsonUtility.ToJson(this)
            binaryFormatter.Serialize(fileStream,this);
            fileStream.Close();
        }

        public virtual void Load(string fileName){
            BinaryFormatter binaryFormatter=new BinaryFormatter();
            if (File.Exists(fileName)){
                FileStream fileStream = File.OpenRead(fileName);
                var deserialize = binaryFormatter.Deserialize(fileStream);
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                var fieldInfos = this.GetType().GetFields(bindingFlags);
                var type = deserialize.GetType();
                foreach (var fieldInfo in fieldInfos){
                    fieldInfo.SetValue(this,type.GetField(fieldInfo.Name,bindingFlags)?.GetValue(deserialize));
                }
                // JsonUtility.FromJsonOverwrite(deserialize,this);
                fileStream.Close();
            }
        }
    }
}