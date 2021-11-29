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
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation;
using GalForUnity.Model;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.GFUNode.Operation{
    [NodeRename("Operation/" + nameof(RoleNode), "角色节点，能够修改角色的状态")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
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
                x.ContainerData = new List<Graph.Operation.Data> {
                    new Graph.Operation.Data(typeof(RoleModel), RoleModel), new Graph.Operation.Data(typeof(RoleOperationType), roleOperationType),
                };
                if (RoleModel){
                    x.InputData = new List<Graph.Operation.Data> {
                        new Graph.Operation.Data(RoleModel.transform), new Graph.Operation.Data(typeof(AnimationClip)), new Graph.Operation.Data(typeof(float)), new Graph.Operation.Data(typeof(Color))
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