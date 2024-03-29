

using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Graph.Editor.Nodes;
using UnityEngine;

namespace GalForUnity.Graph.Attributes{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class NodeType : Attribute{
        // ReSharper disable once CollectionNeverQueried.Global
        public static readonly Dictionary<int, Type> NodeByType = new Dictionary<int, Type>();
        public static readonly Dictionary<Type, int> TypeByNode = new Dictionary<Type, int>();
        public readonly int TypeCode;

        public NodeType(int typeCode){ TypeCode = typeCode; }

        public static void Init(){
            var types = Assembly.GetAssembly(typeof(NodeType)).GetTypes();
            foreach (var type in types){
                if (!type.IsSubclassOf(typeof(GfuNode))) continue;
                var customAttribute = type.GetCustomAttribute<NodeType>();
                if (customAttribute == null) continue;
                NodeByType[customAttribute.TypeCode] = type;
                TypeByNode[type] = customAttribute.TypeCode;
            }
        }

        public static Type GetTypeByCode(int typeCode){
            if (NodeByType == null || NodeByType.Count == 0 || TypeByNode == null || TypeByNode.Count == 0) Init();

            // if (NodeByType == null) return typeof(PlotItemNode);
            // if(!NodeByType.ContainsKey(typeCode)) return typeof(PlotItemNode);
            return NodeByType?[typeCode];
        }

        public static int GetCodeByType(Type type){
            if (!type.IsSubclassOf(typeof(GfuNode))) return -1;
            if (NodeByType == null || NodeByType.Count == 0 || TypeByNode == null || TypeByNode.Count == 0) Init();

            if (TypeByNode == null){
                Debug.LogError("type dictionary is null");
                return -1;
            }

            if (!TypeByNode.ContainsKey(type)) return -1;
            return TypeByNode[type];
        }
    }

    public static class NodeTypeStatic{
        public static int GetTypeByCode(this EditorNode gfuNode){ return NodeType.GetCodeByType(gfuNode.GetType()); }
    }

    public struct NodeCode{
#region Main

        public const ushort MainNode = 0;
        public const ushort EndNode = 1;
        public const ushort SceneNode = 2;
        public const ushort PlotItemNode = 3;
        public const ushort PlotFlowNode = 4;
        public const ushort PlotGraphNode = 5;
        public const ushort PlotNode = 6;

#endregion

#region Logic

        public const ushort TimeCheckNode = 5;
        public const ushort SceneCheckNode = 6;
        public const ushort RoleDataCheckNode = 7;
        public const ushort RoleCheckNode = 8;
        public const ushort ProbabilityNode = 9;
        public const ushort OptionNode = 10;

#endregion

#region Operation

        public const ushort Vector4Node = 100;
        public const ushort TransformNode = 101;
        public const ushort TimeNode = 102;
        public const ushort RoleNode = 103;
        public const ushort LinearNode = 104;
        public const ushort GfuOperationNode = 105;
        public const ushort ChangeRoleDataNode = 106;
        public const ushort AnimationNode = 107;

#region Math

        public const ushort SubtractNode = 158;
        public const ushort SquareRootNode = 159;
        public const ushort PowerNode = 160;
        public const ushort MultiplyNode = 161;
        public const ushort DivisionNode = 162;
        public const ushort AddNode = 163;

#endregion

#region Logic

        public const ushort BooleanNode = 204;
        public const ushort CompareNode = 205;

#endregion

#region Geometry

        public const ushort ArccosineNode = 256;
        public const ushort ArcsineNode = 257;
        public const ushort ArctangentNode = 258;
        public const ushort CosineNode = 259;
        public const ushort SineNode = 260;
        public const ushort TangentNode = 261;

#endregion

#region Channel

        public const ushort CombineNode = 300;
        public const ushort SplitNode = 301;

#endregion
        
        public const ushort CustomNode = 301;
        
#endregion
    }
}