//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  TangentOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 22:59:49
//
//======================================================================


using UnityEngine;

namespace GalForUnity.Graph.Operation.Geometry{
    public class TangentOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Tangent();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Tangent();
            base.Update(gfuOperationData);

        }

        public override void OperationOver(){
            Tangent();
            base.OperationOver();
        }

        public void Tangent(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = Mathf.Tan((float) InputData[0].value);
                } else{
                    var vector4 = (Vector4) InputData[0].value;
                    data.value = new Vector4(Mathf.Tan(vector4.x),Mathf.Tan(vector4.y),
                        Mathf.Tan(vector4.z),Mathf.Tan(vector4.w));
                }
            }
        }
    }
}
