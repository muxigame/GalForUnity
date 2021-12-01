//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  BooleanOperation.cs
//
//        Created by 半世癫(Roc) at 2021-02-08 17:32:16
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.Logic{
    
    public class BooleanOperation:GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Operation();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Operation();
            base.Update(gfuOperationData);
        }
        public override void OperationOver(){
            Operation();
            base.OperationOver();
        }

        public void Operation(){
            if ((InputData[2].value is bool boolean)){
                foreach (var data in OutPutData){
                    if (boolean){
                        data.value=InputData[0].value;
                    }else{
                        data.value=InputData[1].value;
                    }
                }
            }else{
                Debug.LogError("端口篡改，或一个未知异常，端口的类型不是bool");
            }
                
        }
    }
}