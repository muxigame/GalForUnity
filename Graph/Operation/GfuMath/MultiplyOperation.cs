//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  MultiplyOperation.cs
//
//        Created by 半世癫(Roc) at 2021-11-29 21:59:45
//
//======================================================================

using GalForUnity.Graph.Operation;
using UnityEngine;

namespace Assets.GalForUnity.Graph.Operation.GfuMath{
    public class MultiplyOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Multiply();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Multiply();
            base.Update(gfuOperationData);
            
        }

        public override void OperationOver(){
            Multiply();
            base.OperationOver();
        }

        public void Multiply(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = (float) InputData[0].value * (float) InputData[1].value;
                }else{
                    var vector4 = (Vector4) InputData[0].value;
                    var value = (Vector4) InputData[1].value;
                    data.value = new Vector4(vector4.x*value.x,vector4.y*value.y,vector4.z*value.z,vector4.w*value.w);
                }
            }
        }
    }
}
