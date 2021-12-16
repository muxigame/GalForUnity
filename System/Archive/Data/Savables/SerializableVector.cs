//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SerializableVector.cs
//
//        Created by 半世癫(Roc) at 2021-12-15 15:42:13
//
//======================================================================

using System;
using UnityEngine;

namespace GalForUnity.System.Archive.Data.Savables{
    [Serializable]
    public struct SerializableVector{
        [SerializeField]
        public float x;
        [SerializeField]
        public float y;
        [SerializeField]
        public float z;
        [SerializeField]
        public float w;

        public SerializableVector(float x = 0,float y=0,float z=0,float w=0){
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Vector2(SerializableVector serializableVector){
            return new Vector2(serializableVector.x,serializableVector.y);
        } 
        public static implicit operator Vector3(SerializableVector serializableVector){
            return new Vector3(serializableVector.x,serializableVector.y,serializableVector.z);
        }
        public static implicit operator Vector4(SerializableVector serializableVector){
            return new Vector4(serializableVector.x,serializableVector.y,serializableVector.z,serializableVector.w);
        } 
        public static implicit operator SerializableVector(Vector2 vector2){
            return new SerializableVector(vector2.x,vector2.y);
        }
        public static implicit operator SerializableVector(Vector3 vector2){
            return new SerializableVector(vector2.x,vector2.y,vector2.z);
        }
        public static implicit operator SerializableVector(Vector4 vector2){
            return new SerializableVector(vector2.x,vector2.y,vector2.z,vector2.w);
        }
        public static implicit operator SerializableVector(Color color){
            return new SerializableVector(color.r,color.g,color.b);
        } 
        public static implicit operator Color(SerializableVector serializableVector){
            return new Color(serializableVector.x,serializableVector.y,serializableVector.z);
        }

    }
}
