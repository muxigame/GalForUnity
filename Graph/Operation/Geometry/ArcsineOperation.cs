//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ArcsineOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-02 13:10:23
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.Geometry{
    
    public class ArcsineOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Arcsine();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Arcsine();
            base.Update(gfuOperationData);

        }

        public override void OperationOver(){
            Arcsine();
            base.OperationOver();
        }

        public void Arcsine(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = Mathf.Asin((float) InputData[0].value);
                } else{
                    var vector4 = (Vector4) InputData[0].value;
                    data.value = new Vector4(Mathf.Asin(vector4.x),Mathf.Asin(vector4.y),
                        Mathf.Asin(vector4.z),Mathf.Asin(vector4.w));
                }
            }
        }
    }
}
