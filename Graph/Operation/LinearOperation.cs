//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  LinearOperation.cs
//
//        Created by 半世癫(Roc) at 2021-02-04 19:49:33
//
//======================================================================

using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GalForUnity.Graph.Operation{
    public class LinearOperation : GfuOperation{
        public override Task Execute(GfuOperationData gfuOperationData){
            Input.Data[2].IsOver = false;
            startTime = Time.time;
            return base.Execute(gfuOperationData);
        }

        public override void Start(GfuOperationData gfuOperationData){
            
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    float from = (float) InputData[0].value;
                    float to = (float) InputData[1].value;
                    data.value = from;
                    // Debug.LogError(data.value);
                    if (Math.Abs(from - to) < 0.01f) Input.Data[2].IsOver = true;
                }else{
                    Debug.LogError("一个不合理的输出类型，原因可能是连接了不同类型的接口");
                    Debug.LogError("An unreasonable output type, possibly because a different type of interface is connected");
                }
            }
            base.Start(gfuOperationData);
        }

        private float startTime;
        public override void Update(GfuOperationData gfuOperationData){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    float from = (float) InputData[0].value;
                    float to = (float) InputData[1].value;
                    float time = (float) InputData[2].value;
                    float timeScale = ((Time.time - startTime) / time);
                    float value = from + (to - from) * (timeScale > 1 ? 1 : timeScale);
                    data.value = value;
                    // Debug.LogError(data.value);
                    if (Math.Abs(value - to) < 0.01f) Input.Data[2].IsOver = true;
                }else{
                    Debug.LogError("一个不合理的输出类型，原因可能是连接了不同类型的接口");
                    Debug.LogError("An unreasonable output type, possibly because a different type of interface is connected");
                }
            }
            base.Update(gfuOperationData);
        }
        public override void OperationOver(){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value = (float)InputData[1].value;
                }else{
                    Debug.LogError("一个不合理的输出类型，原因可能是连接了不同类型的接口");
                    Debug.LogError("An unreasonable output type, possibly because a different type of interface is connected");
                }
            }
            base.OperationOver();
        }
    }
}
