//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  AddOperation.cs
//
//        Created by Roc(半世癫) at 2021-11-28 20:47:39
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.GfuMath{
    public class AddOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Add();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Add();
            base.Update(gfuOperationData);
            
        }

        public override void OperationOver(){
            Add();
            base.OperationOver();
        }

        public void Add(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = (float) InputData[0].value + (float) InputData[1].value;
                }else{
                    data.value = ((Vector4) InputData[0].value) + ((Vector4) InputData[1].value);
                }
            }
        }
    }
}
