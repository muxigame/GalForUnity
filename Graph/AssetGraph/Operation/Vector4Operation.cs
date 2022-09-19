using UnityEngine;

namespace GalForUnity.Graph.AssetGraph.Operation{
    public class Vector4Operation:GfuOperation{
        public Vector4Operation(){}
        public Vector4Operation(GfuOperationData input){
            Input = input;
        }

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
            foreach (var data in OutPutData){
                if (data.Type == typeof(float)){
                    data.value=InputData[0].value;
                }else{
                    data.value = new Vector4(
                        (float)InputData[0].value,
                        (float)InputData[1].value,
                        (float)InputData[2].value,
                        (float)InputData[3].value
                    );
                }
            }
        }
    }
}