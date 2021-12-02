//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArctangentOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-02 13:21:52
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.Geometry{
    public class ArctangentOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Arctangent();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Arctangent();
            base.Update(gfuOperationData);

        }

        public override void OperationOver(){
            Arctangent();
            base.OperationOver();
        }

        public void Arctangent(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = Mathf.Atan((float) InputData[0].value);
                } else{
                    var vector4 = (Vector4) InputData[0].value;
                    data.value = new Vector4(Mathf.Atan(vector4.x),Mathf.Atan(vector4.y),
                        Mathf.Atan(vector4.z),Mathf.Atan(vector4.w));
                }
            }
        }
    }
}
