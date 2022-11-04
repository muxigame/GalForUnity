//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  RoleNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-28 16:56:49
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.Operation;
using GalForUnity.Graph.Attributes;
using GalForUnity.Model;
using GalForUnity.System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using NodeData = GalForUnity.Graph.Build.NodeData;

#if UNITY_EDITOR
#endif

namespace GalForUnity.Graph.AssetGraph.GFUNode.Operation{
    [NodeRename("Operation/" + nameof(RoleNode), "角色节点，能够修改角色的状态")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    [NodeType(NodeCode.RoleNode)]
    public class RoleNode : GfuOperationNode{
        public RoleModel RoleModel;

        [NodeRename(nameof(TransformNode), typeof(Transform), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Enter;

        [NodeRename(nameof(Animation), typeof(AnimationClip), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Animation;

        [NodeRename(nameof(Opacity), typeof(float), NodeDirection.Input, NodeCapacity.Single)] [DefaultValue(1)]
        public GfuPort Opacity;

        [NodeRename(nameof(Color), typeof(Color), NodeDirection.Input, NodeCapacity.Single)] [DefaultValue(1, 1, 1, 1)]
        public GfuPort Color;

        [NodeRename(nameof(Operation), typeof(GfuOperation), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;

        public enum RoleOperationType{
            None,
            ToStage,
            StepDown
        }

        public RoleOperationType roleOperationType;

        public RoleNode(){
            GfuOperation = new RoleOperation(RoleModel);
            GfuOperation.OnInit += (x) => {
                x.ContainerData = new List<AssetGraph.Operation.Data> {
                    new AssetGraph.Operation.Data(typeof(RoleModel), RoleModel), new AssetGraph.Operation.Data(typeof(RoleOperationType), roleOperationType),
                };
                if (RoleModel){
                    x.InputData = new List<AssetGraph.Operation.Data> {
                        new AssetGraph.Operation.Data(RoleModel.transform), new AssetGraph.Operation.Data(typeof(AnimationClip)), new AssetGraph.Operation.Data(typeof(float)), new AssetGraph.Operation.Data(typeof(Color))
                    };
                }
            };
        }

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort(otherNodeData);
#if UNITY_EDITOR
            ObjectField objectField = new ObjectField() {
                label = GfuLanguage.Parse(nameof(RoleModel)),
                objectType = typeof(RoleModel),
                tooltip = "要操作的角色",
                value = RoleModel,
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft
                    }
                }
            };
            objectField.RegisterValueChangedCallback(evt => {
                RoleModel = evt.newValue as RoleModel;
                GfuOperation.ContainerData[0] = evt.newValue;
            });
            EnumField enumField = new EnumField(roleOperationType) {
                label = GfuLanguage.Parse(nameof(RoleOperationType)),
                value = roleOperationType,
                tooltip = "角色该登场还是下场或者不执行相关操作",
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft
                    }
                }
            };
            enumField.RegisterValueChangedCallback(evt => { GfuOperation.ContainerData[1].value = roleOperationType = (RoleOperationType) evt.newValue; });
            extensionContainer.Add(objectField);
            extensionContainer.Add(enumField);
            RefreshExpandedState();
            style.width = 210;
#endif
        }
        
    }
}