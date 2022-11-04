//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ChangeRoleDataNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-15 15:05:45
//
//======================================================================

#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Model;
using GalForUnity.System;
using UnityEngine;
using UnityEngine.UIElements;
using NodeData = GalForUnity.Graph.Build.NodeData;
using Object = UnityEngine.Object;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Operation{
    [NodeRename("Operation/" + nameof(ChangeRoleDataNode))]
    [NodeAttributeUsage(NodeAttributeTargets.FlowGraph | NodeAttributeTargets.ItemGraph)]
    [NodeType(NodeCode.ChangeRoleDataNode)]
    public class ChangeRoleDataNode : BaseFieldNode{
        [NodeRename(nameof(Exit), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit;

        public enum ChangeType{
            Equal,
            Add,
            Subtract,
        }


        public ChangeType operationType;


        public RoleData objectReference;

        public override RoleData Execute(RoleData roleData){
            // Debug.Log(objectReference);
            // Debug.Log(operationType);
            if (objectReference){
                switch (operationType){
                    case ChangeType.Add:
                        return Executed(0, roleData.Parse(roleData + (RoleData) objectReference));
                    case ChangeType.Subtract:
                        return Executed(0, roleData.Parse(roleData - (RoleData) objectReference));
                    case ChangeType.Equal:
                        return Executed(0, (RoleData) objectReference);
                }
            }

            return Executed(0, roleData);
        }

#if UNITY_EDITOR
        [NodeFieldType(typeof(RoleData), nameof(RoleData))]
        public ObjectField ObjectField;

        [NodeFieldType(typeof(ChangeType), nameof(operationType))]
        public EnumField enumField;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitObject(nameof(enumField), (customAttribute) => {
                enumField = new EnumField() {
                    value = operationType,
                    label = GfuLanguage.Parse(customAttribute.Name),
                    style = {
                        marginTop = 5
                    },
                    labelElement = {
                        style = {
                            minWidth = 0, fontSize = 12, unityTextAlign = TextAnchor.MiddleLeft
                        }
                    }
                };
                enumField.Init(operationType);
                return enumField;
            });
            InitObject(nameof(ObjectField), (customAttribute) => new ObjectField() {
                value = objectReference,
                objectType = typeof(RoleData),
                label = GfuLanguage.Parse(customAttribute.Name),
                labelElement = {
                    style = {
                        minWidth = 0, fontSize = 12, unityTextAlign = TextAnchor.MiddleLeft
                    }
                }
            });
            RegisterValueChangedCallback<BaseField<Enum>, Enum>(this);
            RegisterValueChangedCallback<BaseField<Object>, Object>(this);
        }

        protected override void OnValueChangedCallback<T, T2>(T field, ChangeEvent<T2> changeEvent){
            // Debug.Log(field);
            Debug.Log(changeEvent.newValue);
            switch (changeEvent.newValue){
                case ChangeType changeEventType:
                    this.operationType = changeEventType;
                    break;
                case RoleData roleData:
                    this.objectReference = roleData;
                    break;
            }
        }

        public override void Save(){
            operationType = (ChangeType) enumField.value;
            objectReference = (RoleData) ObjectField.value;
        }
#endif
    }
}