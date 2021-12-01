//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SineOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 22:21:27
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.Geometry{
    public class SineOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Sine();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Sine();
            base.Update(gfuOperationData);

        }

        public override void OperationOver(){
            Sine();
            base.OperationOver();
        }

        public void Sine(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = Mathf.Sin((float) InputData[0].value);
                } else{
                    var vector4 = (Vector4) InputData[0].value;
                    data.value = new Vector4(Mathf.Sin(vector4.x),Mathf.Sin(vector4.y),
                        Mathf.Sin(vector4.z),Mathf.Sin(vector4.w));
                }
            }
        }
    }
}
