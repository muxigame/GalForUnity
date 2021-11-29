//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DivisionOperation.cs
//
//        Created by 半世癫(Roc) at 2021-11-29 22:04:07
//
//======================================================================

using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Graph.Operation.GfuMath{
    public class DivisionOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Division();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Division();
            base.Update(gfuOperationData);
            
        }

        public override void OperationOver(){
            Division();
            base.OperationOver();
        }

        public void Division(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = (float) InputData[0].value * (float) InputData[1].value;
                }else{
                    var vector4 = (Vector4) InputData[0].value;
                    var value = (Vector4) InputData[1].value;
                    if (value.x == 0 || value.y == 0 || value.z == 0 || value.w == 0){
                        Debug.LogError(GfuLanguage.ParseLog("In addition to the abnormal 0"));
                    }
                    data.value = new Vector4(vector4.x /value.x,vector4.y /value.y,vector4.z /value.z,vector4.w /value.w);
                }
            }
        }
    }
}
