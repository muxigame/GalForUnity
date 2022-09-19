//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  AnimationOperation.cs
//
//        Created by 半世癫(Roc) at 2021-02-08 13:37:31
//
//======================================================================

namespace GalForUnity.Graph.AssetGraph.Operation{
    public class AnimationOperation:GfuOperation{
        public override void Start(GfuOperationData gfuOperationData){
            foreach (var data in OutPutData){
                if (ContainerData[0].value != null){
                    data.value = ContainerData[0].value;
                }
                else if (ContainerData[1].value != null){
                    data.value = ContainerData[1].value;
                }
            }
            base.Start(gfuOperationData);
        }
    }
}