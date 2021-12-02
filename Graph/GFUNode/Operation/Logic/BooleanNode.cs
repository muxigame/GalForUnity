//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  BooleanNode.cs
//
//        Created by 半世癫(Roc) at 2021-02-08 15:44:36
//
//======================================================================

using System;
using System.Reflection;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation.Logic;

namespace GalForUnity.Graph.GFUNode.Operation{
    [NodeRename("Operation/LogicOperation/" + nameof(BooleanNode), "布尔节点，能进行逻辑判断")]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class BooleanNode : GfuOperationNode{
        [NodeRename(nameof(True), typeof(object), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort True;

        [NodeRename(nameof(False), typeof(object), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort False;

        [NodeRename(nameof(Boolean), typeof(bool), NodeDirection.Input, NodeCapacity.Single)] [DefaultValue(false)]
        public GfuPort Boolean;

        [NodeRename(nameof(Value), typeof(object), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Value;

        public string Type;
        public string assembly;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<BooleanOperation>(otherNodeData);

#if UNITY_EDITOR
            True.OnConnected += (x) => {
                if (True.portType == typeof(object)){
                    False.portType = Value.portType = True.portType = x.portType;
                    assembly = Assembly.GetAssembly(x.portType).FullName;
                    Type = x.portType.ToString();
                }
            };
            False.OnConnected += (x) => {
                if (False.portType == typeof(object)){
                    True.portType = Value.portType = False.portType = x.portType;
                    assembly = Assembly.GetAssembly(x.portType).FullName;
                    Type = x.portType.ToString();
                }
            };
            Value.OnConnected += (x) => {
                if (Value.portType == typeof(object)){
                    True.portType = Value.portType = False.portType = x.portType;
                    assembly = Assembly.GetAssembly(x.portType).FullName;
                    Type = x.portType.ToString();
                }
            };
            True.OnDisConnected += () => {
                if (!False.connected && !Value.connected){
                    False.portType = Value.portType = True.portType = typeof(object);
                    assembly = Type = "";
                }
            };
            False.OnDisConnected += () => {
                if (!True.connected && !Value.connected){
                    True.portType = Value.portType = False.portType = typeof(object);
                    assembly = Type = "";
                }
            };
            Value.OnDisConnected += () => {
                if (!True.connected && !False.connected){
                    True.portType = Value.portType = False.portType = typeof(object);
                    assembly = Type = "";
                }
            };
            Type type = default;
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(assembly)){
                type = False.portType = Value.portType = True.portType = Assembly.Load(assembly).GetType(Type);
            }
#else
            Type type = default;
            if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(assembly)){
                type =  Assembly.Load(assembly).GetType(Type);
            }
#endif
            GfuOperation.OnPostInput += (x) => {
                x.InputData[0].Type = type;
                x.InputData[1].Type = type;
                x.OutPutData[0].Type = type;
            };
        }
    }
}