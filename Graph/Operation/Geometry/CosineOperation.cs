//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  CosineOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 22:54:17
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.Geometry{
    public class CosineOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Cosine();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Cosine();
            base.Update(gfuOperationData);

        }

        public override void OperationOver(){
            Cosine();
            base.OperationOver();
        }

        public void Cosine(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = Mathf.Cos((float) InputData[0].value);
                } else{
                    var vector4 = (Vector4) InputData[0].value;
                    data.value = new Vector4(Mathf.Cos(vector4.x),Mathf.Cos(vector4.y),
                        Mathf.Cos(vector4.z),Mathf.Cos(vector4.w));
                }
            }
        }
    }
}
