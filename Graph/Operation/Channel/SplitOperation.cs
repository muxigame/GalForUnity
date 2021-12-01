//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SplitOperation.cs
//
//        Created by 半世癫(Roc) at 2021-12-01 13:28:28
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.Operation.Channel{
    public class SplitOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Split();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Split();
            base.Update(gfuOperationData);
            
        }

        public override void OperationOver(){
            Split();
            base.OperationOver();
        }

        public void Split(){
            var value = (Vector4)InputData[0].value;
            foreach (var data in OutPutData){
                switch (data.outportIndex){
                    case 0: data.value = value.x;
                        break;
                    case 1: data.value = value.y;
                        break;
                    case 2: data.value = value.z;
                        break;
                    case 3: data.value = value.w;
                        break;
                    default: Debug.LogError("未知的端口号！Unknown Port Number" +data.outportIndex);
                        break;
                }
            }
        }
    }
}
