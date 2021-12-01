//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PowerOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 16:09:56
//
//======================================================================

using GalForUnity.Graph.Operation;
using UnityEngine;

namespace Assets.GalForUnity.Graph.Operation.GfuMath{
    public class PowerOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Power();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Power();
            base.Update(gfuOperationData);
        }

        public override void OperationOver(){
            Power();
            base.OperationOver();
        }

        public void Power(){
            foreach (var data in OutPutData){
                if (InputData[0].Type == typeof(float)){
                    data.value = Mathf.Pow((float) InputData[0].value, (float) InputData[1].value);
                } else{
                    var vectorInput1 = (Vector4) InputData[0].value;
                    var vectorInput2 = (Vector4) InputData[1].value;
                    data.value = new Vector4(Mathf.Pow(vectorInput1.x,vectorInput2.x),
                        Mathf.Pow(vectorInput1.y,vectorInput2.y),
                        Mathf.Pow(vectorInput1.z,vectorInput2.z),
                        Mathf.Pow(vectorInput1.w,vectorInput2.w));
                }
            }
        }
    }
}
