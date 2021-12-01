//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  CombineOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 13:13:22
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.Channel{
    public class CombineOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Combine();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Combine();
            base.Update(gfuOperationData);
            
        }

        public override void OperationOver(){
            Combine();
            base.OperationOver();
        }

        public void Combine(){
            foreach (var data in OutPutData){
                data.value = new Vector4((float) InputData[0].value,(float) InputData[1].value,(float) InputData[2].value,(float) InputData[3].value); 
                // if (data.Type == typeof(Vector2)){
                //     
                // }else{
                //     var vector4 = (Vector4) InputData[0].value;
                //     var value = (Vector4) InputData[1].value;
                //     if (value.x == 0 || value.y == 0 || value.z == 0 || value.w == 0){
                //         Debug.LogError(GfuLanguage.ParseLog("In addition to the abnormal 0"));
                //     }
                //     data.value = new Vector4(vector4.x /value.x,vector4.y /value.y,vector4.z /value.z,vector4.w /value.w);
                // }
            }
        }
    }
}
