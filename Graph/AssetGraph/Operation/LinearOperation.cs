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
using UnityEngine;

namespace GalForUnity.Graph.AssetGraph.Operation{
    public class LinearOperation : GfuOperation{
        public override void Execute(GfuOperationData gfuOperationData){
            Input.Data[2].IsOver = false;
            startTime = Time.time;
            base.Execute(gfuOperationData);
        }
        
        public override void Start(GfuOperationData gfuOperationData){
            loopTime = (float) ContainerData[1].value;
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    float from = (float) InputData[0].value;
                    float to = (float) InputData[1].value;
                    data.value = from;
                    // Debug.LogError(data.value);
                    if (Math.Abs(from - to) < 0.000001f) Input.Data[2].IsOver = true;
                }else{
                    Debug.LogError("一个不合理的输出类型，原因可能是连接了不同类型的接口");
                    Debug.LogError("An unreasonable output type, possibly because a different type of interface is connected");
                }
            }
            base.Start(gfuOperationData);
        }

        private float startTime;
        private float loopTime;
        private bool reverse;
        public override void Update(GfuOperationData gfuOperationData){
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    float from = (float) (reverse?InputData[1].value:InputData[0].value);
                    float to = (float) (reverse?InputData[0].value:InputData[1].value);
                    float time = (float) InputData[2].value;
                    float timeScale = ((Time.time - startTime) / time);
                    float value = from + (to - from) * (timeScale > 1 ? 1 : timeScale);
                    data.value = value;
                    if ((bool) ContainerData[0].value){
                        if (loopTime - (Time.time - startTime) <= 0){
                            IsOver = true;
                        }
                        if (timeScale >= 1){
                            loopTime = loopTime - time;
                            startTime = Time.time;
                            reverse = !reverse;
                        }
                    } else{
                        if (Math.Abs(value - to) < 0.00001f){
                            IsOver = true;
                            // Input.Data[2].IsOver = true;
                        }
                    }
                    // Debug.LogError(data.value);
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
                    if ((bool) ContainerData[0].value){
                        float from = (float) InputData[0].value;
                        float to = (float) InputData[1].value;
                        float time = (float) InputData[2].value;
                        int count = (int) ((float) ContainerData[1].value / time);
                        float remainder = ((float) ContainerData[1].value % time);
                        float timeScale = (remainder / time);
                        if (count % 2 == 0){
                            float value = from + (to - from) * (timeScale > 1 ? 1 : timeScale);
                            data.value = value;
                        } else{
                            float value = to -  (to - from) * (timeScale > 1 ? 1 : timeScale);
                            data.value = value;
                        }
                    } else{
                        data.value = (float)InputData[1].value;
                    }
                }else{
                    Debug.LogError("一个不合理的输出类型，原因可能是连接了不同类型的接口");
                    Debug.LogError("An unreasonable output type, possibly because a different type of interface is connected");
                }
            }
            base.OperationOver();
        }
    }
}
