//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SubtractOperation.cs
//
//        Created by Roc(半世癫) at 2021-01-28 18:01:49
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.AssetGraph.Operation.GfuMath{
    public class SubtractOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Subtract();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Subtract();
            base.Update(gfuOperationData);

        }

        public override void OperationOver(){
            Subtract();
            base.OperationOver();
        }

        public void Subtract(){
            foreach (var data in OutPutData){
                if (InputData[0].Type == typeof(float)){
                    data.value = (float) InputData[0].value - (float) InputData[1].value;
                } else{
                    data.value = ((Vector4) InputData[0].value) - ((Vector4) InputData[1].value);
                }
            }
        }
    }
}
