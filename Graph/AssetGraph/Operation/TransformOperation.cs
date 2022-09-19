//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  TransformOperation.cs
//
//        Created by 半世癫(Roc) at 2021-01-27
//
//======================================================================

using UnityEngine;

namespace GalForUnity.Graph.AssetGraph.Operation{
    public class TransformOperation:GfuOperation{
        public TransformOperation(){ }
        public TransformOperation(GfuOperationData input){
            Input =  new GfuOperationData(input.Data);
        }

        /// <summary>
        /// 将从入口处得到的参数赋值给输出
        /// </summary>
        /// <param name="gfuOperationData"></param>
        public override void Update(GfuOperationData gfuOperationData){
            foreach (var data in OutPutData){
                if (InputData.Count == 3 && data.value is Transform outputData){
                    outputData.position = InputData[0].value as Vector3? ?? (Vector4)InputData[0].value;
                    outputData.eulerAngles = InputData[1].value as Vector3? ?? (Vector4)InputData[1].value;
                    // Debug.Log(InputData[2].value);
                    outputData.localScale = InputData[2].value as Vector3? ?? (Vector4)InputData[2].value;
                }
                else if (data.Type==typeof(Vector3)||data.Type==typeof(Quaternion)){
                    data.value = InputData[data.Index].value;
                }
            }

            base.Update(gfuOperationData);
        }

        public override void OperationOver(){
            foreach (var data in OutPutData){
                if (InputData.Count == 3 && data.value is Transform outputData){
                    outputData.position = InputData[0].value as Vector3? ?? (Vector4)InputData[0].value;//从输入端口一获得position
                    outputData.eulerAngles = InputData[1].value as Vector3? ?? (Vector4)InputData[1].value;//从输入端口二获得欧拉角
                    outputData.localScale = InputData[2].value as Vector3? ?? (Vector4) InputData[2].value;//从输入端口三获得scale
                }
                else if (data.Type==typeof(Vector3)||data.Type==typeof(Quaternion)){
                    data.value = InputData[data.Index].value;
                }
            }
            base.OperationOver();
        }
        
    }

    public class TransformOperationData : GfuOperationData{
        
        public class TransformData:Data{
            // public Transform Transform;
            
            // protected TransformData(){}
            
            public TransformData(Transform transform) : base(transform){
                value = transform;
            }
        }

    }
}