//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArccosineOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-02 13:15:19
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.Geometry{
    public class ArccosineOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Arccosine();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Arccosine();
            base.Update(gfuOperationData);

        }

        public override void OperationOver(){
            Arccosine();
            base.OperationOver();
        }

        public void Arccosine(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = Mathf.Acos((float) InputData[0].value);
                } else{
                    var vector4 = (Vector4) InputData[0].value;
                    data.value = new Vector4(Mathf.Acos(vector4.x),Mathf.Acos(vector4.y),
                        Mathf.Acos(vector4.z),Mathf.Acos(vector4.w));
                }
            }
        }
    }
}
