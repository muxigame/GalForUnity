//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  TransformSavableBehaviour.cs
//
//        Created by 半世癫(Roc) at 2021-12-03 10:10:50
//
//======================================================================


using System;
using GalForUnity.System.Address.Addresser;
using GalForUnity.System.Archive.Attributes;
using GalForUnity.System.Archive.Data;
using GalForUnity.System.Archive.Data.Savables;
using UnityEngine;

namespace GalForUnity.System.Archive.Behavior{
    /// <summary>
    /// 附加该脚本或者继承该脚本后，对象获得可保存坐标信息到存档的能力
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class TransformSavableBehaviour:SavableBehaviour{

        [SaveFlag]
        [SerializeField]
        [HideInInspector]
        private GfuTransformData gfuTransformData;
        
        [Serializable]
        public class GfuTransformData:Savable{
            public GfuTransformData(Transform transform){
                _transform = transform;
            }
            [NonSerialized]
            private Transform _transform;
            [SerializeField]
            private SerializableVector position;
            [SerializeField]
            private SerializableVector rotate;
            [SerializeField]
            private SerializableVector scale;

            public override void Save(){
                address = InstanceIDAddresser.GetInstance().Parse(_transform);
                position = _transform.position;
                rotate = _transform.eulerAngles;
                scale = _transform.localScale;
            }

            public override void Recover(){
                base.Recover();
                if (!string.IsNullOrEmpty(address)){
                    if (InstanceIDAddresser.GetInstance().Get(address,out object obj)){
                        _transform = (Transform) obj;
                        _transform.position = position;
                        _transform.eulerAngles = rotate;
                        _transform.localScale = scale;
                    }
                }
            }
        }

        public override void GetObjectData(){
            gfuTransformData=new GfuTransformData(transform); //在父类序列化自身之前保存transform中的数据
        }

        public override void Recover(){
            gfuTransformData.Recover();//gfuTransformData会被反序列化赋值
        }
    }
}