//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  TransformNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-28 15:50:32
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation;
using GalForUnity.Model;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

using UnityEngine;

namespace GalForUnity.Graph.GFUNode.Operation{
    [NodeRename("Operation/"+nameof(TransformNode),"变换操作节点，能够修改Transform")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class TransformNode : GfuOperationNode{
        
        [NodeRename(nameof(Position),typeof(Vector3),NodeDirection.Input,NodeCapacity.Single)]
        public GfuPort Position;
        [NodeRename(nameof(Rotation),typeof(Vector4),NodeDirection.Input,NodeCapacity.Single)]
        public GfuPort Rotation;
        [NodeRename(nameof(Scale),typeof(Vector3),NodeDirection.Input,NodeCapacity.Single)]
        [DefaultValue(1,1,1)]
        public GfuPort Scale;
        [NodeRename(nameof(TransformOperation),typeof(Transform),NodeDirection.Output,NodeCapacity.Multi)]
        public GfuPort Exit;

        // public TransformNode(){
        //     GfuOperation=new TransformOperation(
        //         GfuOperationData.CreateInstance(
        //             typeof(Vector3),
        //             typeof(Vector3),
        //             typeof(Vector3))
        //     );
        // }
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<TransformOperation>(otherNodeData);
        }
        

        // public override object GetDefaultValue(int portIndex){
        //     var gfuPort = (GfuPort)inputContainer[portIndex];
        //     return gfuPort.GetDefaultValue();
        // }
    }
}
