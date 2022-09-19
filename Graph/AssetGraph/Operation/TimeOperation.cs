//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  TimeOperation.cs
//
//        Created by 半世癫(Roc) at 2021-02-09 16:08:11
//
//======================================================================


using UnityEngine;

namespace GalForUnity.Graph.AssetGraph.Operation{
    public class TimeOperation : GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            Operation();
            base.Start(gfuOperationData);
        }

        public override void Update(GfuOperationData gfuOperationData){
            Operation();
            base.Update(gfuOperationData);
        }

        public override void OperationOver(){//永远不会停止的节点
            Operation();
            base.OperationOver();
        }

        private void Operation(){
            foreach (var data in OutPutData){
                switch (data.outportIndex){
                    case 0:data.value = Time.time;
                        break;
                    case 1:data.value = Mathf.Sin(Time.time);
                        break;
                    case 2:data.value = Mathf.Cos(Time.time);
                        break;
                    case 3:data.value = Time.deltaTime;
                        break;
                    case 4:data.value = Time.smoothDeltaTime;
                        break;
                    default:Debug.LogError("未知的端口号！Unknown Port Number"+data.outportIndex);
                        break;
                }
            }
            
        }
    }
}
