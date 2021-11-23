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

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace GalForUnity.System.Archive{
    public class SerializeSelfable{
        public virtual void Save(string fileName){
            BinaryFormatter binaryFormatter=new BinaryFormatter();
            // binaryFormatter.Serialize();
            FileStream fileStream = null;
            if (!File.Exists(fileName)){
                fileStream = File.Create(fileName);
            }
            fileStream = fileStream??File.OpenWrite(fileName);
            binaryFormatter.Serialize(fileStream,JsonUtility.ToJson(this));
            fileStream.Close();
        }

        public virtual void Load(string fileName){
            BinaryFormatter binaryFormatter=new BinaryFormatter();
            if (File.Exists(fileName)){
                FileStream fileStream = File.OpenRead(fileName);
                var deserialize = (string) binaryFormatter.Deserialize(fileStream);
                Debug.Log(deserialize);
                JsonUtility.FromJsonOverwrite(deserialize,this);
                fileStream.Close();
            }
        }
    }
}