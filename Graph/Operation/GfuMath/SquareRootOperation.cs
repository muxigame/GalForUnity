//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SquareRootOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 16:17:21
//
//======================================================================


using GalForUnity.Graph.Operation;
using UnityEngine;

namespace GalForUnity.Graph.Operation.GfuMath{
    public class SquareRootOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Sqrt();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Sqrt();
            base.Update(gfuOperationData);
        }

        public override void OperationOver(){
            Sqrt();
            base.OperationOver();
        }

        public void Sqrt(){
            foreach (var data in OutPutData){
                if (InputData[0].Type == typeof(float)){
                    data.value = Mathf.Sqrt((float) InputData[0].value);
                } else{
                    var vectorInput1 = (Vector4) InputData[0].value;
                    data.value = new Vector4(Mathf.Sqrt(vectorInput1.x),
                        Mathf.Sqrt(vectorInput1.y),
                        Mathf.Sqrt(vectorInput1.z),
                        Mathf.Sqrt(vectorInput1.w));
                }
            }
        }
    }
}
