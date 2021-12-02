//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  CompareNode.cs
//
//        Created by 半世癫(Roc) at 2021-02-08 21:16:12
//
//======================================================================

using System;
using System.Reflection;
using GalForUnity.Graph.Operation.Logic;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine.UIElements;


namespace GalForUnity.Graph.GFUNode.Operation{
    [NodeRename("Operation/LogicOperation/" + nameof(CompareNode), "布尔节点，能进行逻辑判断")]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class CompareNode : GfuOperationNode{
        [NodeRename(nameof(Value1), typeof(object), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Value1;

        [NodeRename(nameof(Value2), typeof(object), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Value2;

        [NodeRename(nameof(Boolean), typeof(bool), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Boolean;

        /// <summary>
        /// Container 0
        /// </summary>
        public Enum CompareType = ObjectCompareType.Equal;

        /// <summary>
        /// Container 1
        /// </summary>
        public string Type;

        /// <summary>
        /// Container 2
        /// </summary>
        public string assembly;

#if UNITY_EDITOR
        /// <summary>
        /// No Container
        /// </summary>
        private EnumField objType;

        /// <summary>
        /// No Container
        /// </summary>
        private EnumField valType;
#endif

        // public ValueCompareType CompareType = (int)ValueCompareType.Equal;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            Type type = typeof(object);
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(assembly)){
                type
#if UNITY_EDITOR
                    = Value1.portType = Value2.portType
#endif
                        = Assembly.Load(assembly).GetType(Type);
            }

            InitDefaultValuePort<CompareOperation>(otherNodeData);
            GfuOperation.OnPostInput += (x) => {
                x.InputData[0].Type = type;
                x.InputData[1].Type = type;
                x.ContainerData[0].value = CompareType;
            };
#if UNITY_EDITOR
            valType = new EnumField(ValueCompareType.Equal);
            objType = new EnumField(CompareType);
            objType.RegisterValueChangedCallback(evt => { CompareType = (ObjectCompareType) Enum.Parse(typeof(ObjectCompareType), evt.newValue.ToString()); });
            valType.RegisterValueChangedCallback(evt => { CompareType = (ValueCompareType) Enum.Parse(typeof(ValueCompareType), evt.newValue.ToString()); });
            Value1.OnConnected += (x) => {
                if (Value2.portType == typeof(object)){
                    Value1.portType = Value2.portType = x.portType;
                    assembly = Assembly.GetAssembly(x.portType).FullName;
                    Type = x.portType.ToString();
                    CheckEnum(x.portType);
                }
            };
            Value2.OnConnected += (x) => {
                if (Value1.portType == typeof(object)){
                    Value1.portType = Value2.portType = x.portType;
                    assembly = Assembly.GetAssembly(x.portType).FullName;
                    Type = x.portType.ToString();
                    CheckEnum(x.portType);
                }
            };
            Value1.OnDisConnected += () => {
                if (!Value2.connected){
                    Value1.portType = Value2.portType = typeof(object);
                    assembly = Type = "";
                }
            };
            Value2.OnDisConnected += () => {
                if (!Value1.connected){
                    Value1.portType = Value2.portType = typeof(object);
                    assembly = Type = "";
                }
            };
            CheckEnum(type);
#endif
        }
#if UNITY_EDITOR
        public void CheckEnum(Type type){
            if (type.IsValueType){
                if (mainContainer.Contains(objType)){
                    mainContainer.Remove(objType);
                }

                mainContainer.Add(valType);
            } else{
                if (mainContainer.Contains(valType)){
                    mainContainer.Remove(valType);
                }

                mainContainer.Add(objType);
            }
        }
#endif
    }

    public enum ValueCompareType{
        Equal,
        NotEqual,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual
    }

    public enum ObjectCompareType : byte{
        Equal,
        NotEqual
    }

    // op_Addition
}