//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  EditorNode.cs
//
//        Created by 半世癫(Roc) at 2021-11-17 13:17:30
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalForUnity.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.Operation;
using GalForUnity.Model;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

namespace GalForUnity.Graph.GFUNode.Base{
    public class EditorNode
#if UNITY_EDITOR
        : Node
#endif
    {
        /// <summary>
        /// 节点数据
        /// </summary>
        [NonSerialized] public NodeData nodeData;

        /// <summary>
        /// 如果当前节点数据被创建为图,则能访问到GfuGraph的数据
        /// </summary>
        public GfuGraph GfuGraph;

        protected EditorNode(){
#if UNITY_EDITOR
            NodeRenameAttribute renameAttribute = GetType().GetCustomAttribute<NodeRenameAttribute>();
            // title = renameAttribute.name;
            if (renameAttribute == null) return;
            title = GfuLanguage.Parse(renameAttribute.name);
            var fields = this.GetType().GetFields().ToList();
            // for (var i = 0; i < fields.Count; i++){
            //     //所有GfuNode都会继承出口，非但是GfuOperationNode会正常显示出口，但是GfuOperationNode不会，会被过滤
            //     if (fields[i].Name == "Exit" && fields[i].DeclaringType == typeof(GfuNode)){
            //         if (GetType().IsSubclassOf(typeof(GfuOperationNode))){
            //             fields.Remove(fields[i]); //如果子类重写本类字段，过滤本类字段
            //             break; 
            //         }
            //     }
            // }
            foreach (var field in fields){
                if (field.GetCustomAttribute(typeof(NodeRenameAttribute)) == null) continue;
                if (!(field.GetCustomAttribute(typeof(NodeRenameAttribute)) is NodeRenameAttribute nodeRenameAttribute)) continue;
                if (field.IsNotSerialized){
                    continue;
                }

                Port port = GfuPort.Create<Edge>(Orientation.Horizontal, (Direction) (int) nodeRenameAttribute.PortType, (Port.Capacity) (int) nodeRenameAttribute.Capacity, nodeRenameAttribute.Type);
                if (nodeRenameAttribute.Type == typeof(RoleData)){
                    port.portColor = new Color(1f, 0.4f, 0.78f);
                } else if (IsSubClassOf(nodeRenameAttribute.Type, typeof(GfuOperation))){
                    if (nodeRenameAttribute.Type == typeof(GfuOperation)){
                        port.portColor = new Color(0f, 0.88f, 0.89f);
                    }
                } else{
                    //port.portColor = new Color(0.89f, 0.89f, 0.89f);
                }

                if (nodeRenameAttribute.Type == typeof(Transform)){
                    port.portColor = new Color(1f, 0.55f, 0f);
                }

                field.SetValue(this, port);
                //设置port显示的名称
                port.portName = GfuLanguage.Parse(nodeRenameAttribute.name);
                field.SetValue(this, port); //为端口赋值
                if (nodeRenameAttribute.PortType == NodeDirection.Input){
                    inputContainer.Add(port);
                } else{
                    outputContainer.Add(port);
                }
            }

            RefreshExpandedState();
#endif
        }
#if UNITY_EDITOR
        public sealed override string title{
            get => base.title;
            set => base.title = value;
        }

        /// <summary>
        /// EditorMethod
        /// 获取该节点的所有端口
        /// </summary>
        /// <returns>端口列表</returns>
        internal List<Port> Ports(Direction direction){
            switch (direction){
                case Direction.Input:
                    return GetInput();
                case Direction.Output:
                    return GetOutPut();
                default:
                    return GetInput().AddAll(GetOutPut());
            }
        }

        /// <summary>
        /// EditorMethod
        /// 获取该节点的所有端口
        /// </summary>
        /// <returns>端口列表</returns>
        internal List<Port> Ports(){ return GetInput().AddAll(GetOutPut()); }

        /// <summary>
        /// EditorMethod
        /// 获取输出端口列表
        /// </summary>
        /// <returns>端口列表</returns>
        protected virtual List<Port> GetOutPut(){
            var output = new List<Port>();
            for (int i = 0; i < outputContainer.childCount; i++){
                output.Add((Port) outputContainer.ElementAt(i));
            }

            return output;
        }

        /// <summary>
        /// EditorMethod
        /// 获取输入端口列表
        /// </summary>
        /// <returns>端口列表</returns>
        protected virtual List<Port> GetInput(){
            var input = new List<Port>();
            for (int i = 0; i < inputContainer.childCount; i++){
                input.Add((Port) inputContainer.ElementAt(i));
            }

            return input;
        }
#endif
        public GfuNode SetPosition(NodeData otherNodeData){
#if UNITY_EDITOR
            Rect rect = this.GetPosition();
            Vector4 vector4 = new Vector4();
            if (otherNodeData) vector4 = otherNodeData.vector4;
            float scale = 1;
            if (nodeData && nodeData.GraphData) scale = nodeData.GraphData.scale;
            rect.position = new Vector2(vector4.x, vector4.y - 19) * (1f / scale);
            this.SetPosition(rect);
#endif
            return (GfuNode) this;
        }

        internal GfuNode SetPosition(){ return SetPosition(nodeData); }
        public bool IsSubClassOf(FieldInfo fieldInfo, Type parentType){ return IsSubClassOf(fieldInfo.FieldType, parentType); }

        /// <summary>
        /// Normal Method 
        /// </summary>
        /// <param name="otherType"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public bool IsSubClassOf(Type otherType, Type parentType){
            Type type = otherType;
            while (type != null){
                if (type == parentType){
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        public virtual T[] GetFieldsArr<T>(){ return GetFields<T>().ToArray(); }

        public virtual List<T> GetFields<T>(){
            var fieldInfos = GetType().GetFields();
            List<T> t = new List<T>();
            foreach (var fieldInfo in fieldInfos){
                if (fieldInfo.FieldType == typeof(T)){
                    t.Add((T) fieldInfo.GetValue(this));
                }
            }

            return t;
        }

        public virtual List<FieldInfo> GetFieldsWithFieldInfo<T>(){
            var fieldInfos = GetType().GetFields();
            List<FieldInfo> t = new List<FieldInfo>();
            foreach (var fieldInfo in fieldInfos){
                if (fieldInfo.FieldType == typeof(T)){
                    t.Add(fieldInfo);
                }
            }

            return t;
        }

        public virtual Dictionary<T, string> GetFieldsName<T>(){
            var fieldInfos = GetType().GetFields();
            Dictionary<T, string> t = new Dictionary<T, string>();
            foreach (var fieldInfo in fieldInfos){
                if (fieldInfo.GetValue(this) is T){
                    t.Add((T) fieldInfo.GetValue(this), fieldInfo.Name);
                }
            }

            return t;
        }

        public List<FieldInfo> GfuInputPortFieldInfos(){
            List<FieldInfo> gfuInput = new List<FieldInfo>();
            var gfuPorts = GetFieldsWithFieldInfo<GfuPort>();
            foreach (var fieldInfo in gfuPorts){
                if (fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Input){
                    gfuInput.Add(fieldInfo);
                }
            }

            return gfuInput;
        }

        public List<FieldInfo> GfuOutputPortFieldInfos(){
            List<FieldInfo> gfuInput = new List<FieldInfo>();
            var gfuPorts = GetFieldsWithFieldInfo<GfuPort>();
            foreach (var fieldInfo in gfuPorts){
                if (fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Output){
                    gfuInput.Add(fieldInfo);
                }
            }

            return gfuInput;
        }
    }
}