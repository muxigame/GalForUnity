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
using GalForUnity.System.Archive.Data;
using UnityEngine;

namespace GalForUnity.System.Archive.Behavior{
    /// <summary>
    /// 附加该脚本或者继承该脚本后，对象获得可保存坐标信息到存档的能力
    /// </summary>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class TransformSavableBehaviour:SavableBehaviour{
        
        protected virtual void OnValidate(){
            savableData=new GfuTransformData(transform);
        }


        public override void Recover(){
            savableData.Recover();
        }
        [Serializable]
        public class GfuTransformData:Savable{
            public GfuTransformData(Transform transform){
                _transform = transform;
                _position = transform.position;
                _rotate = transform.eulerAngles;
                _scale = transform.localScale;
            }
            private Transform _transform;
            private Vector3 _position;
            private Vector3 _rotate;
            private Vector3 _scale;

            public override void Recover(){
                _transform.position = _position;
                _transform.eulerAngles = _rotate;
                _transform.localScale = _scale;
            }
        }
    }
}