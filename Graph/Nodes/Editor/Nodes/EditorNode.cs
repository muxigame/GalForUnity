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
using GalForUnity.External;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Build;
using GalForUnity.Graph.Nodes;
using GalForUnity.Graph.SceneGraph;
using GalForUnity.System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Direction = GalForUnity.Graph.SceneGraph.Direction;

#if UNITY_EDITOR
#endif

namespace GalForUnity.Graph.AssetGraph.GFUNode.Base{
    public class EditorNode
#if UNITY_EDITOR
        : Node
#endif
    {
        // /// <summary>
        // /// 如果当前节点数据被创建为图,则能访问到GfuGraph的数据
        // /// </summary>
        // [NonSerialized]
        // public GfuGraph GfuGraph;

        protected EditorNode(){
#if UNITY_EDITOR
            var renameAttribute = GetType().GetCustomAttribute<NodeRenameAttribute>();
            if (renameAttribute == null) return;
            title = GfuLanguage.Parse(renameAttribute.name);
            tooltip = GfuLanguage.Parse(renameAttribute.tooltip);
            // var fields = this.GetType().GetFields().ToList();
            // foreach (var field in fields){
            //     // if (field.GetCustomAttribute(typeof(NodeRenameAttribute)) == null) continue;
            //     // if (!(field.GetCustomAttribute(typeof(NodeRenameAttribute)) is NodeRenameAttribute nodeRenameAttribute)) continue;
            //
            //     if(field.FieldType!=typeof(List<GfuPort>)) continue;
            //     if (field.IsNotSerialized){
            //         continue;
            //     }
            //
            //     // Port port = GfuPort.Create<Edge>(Orientation.Horizontal, (Direction) (int) nodeRenameAttribute.PortType, (Port.Capacity) (int) nodeRenameAttribute.Capacity, nodeRenameAttribute.Type);
            //     // if (nodeRenameAttribute.Type == typeof(RoleData)){
            //     //     port.portColor = new Color(1f, 0.4f, 0.78f);
            //     // }
            //     // if (nodeRenameAttribute.Type == typeof(Transform)){
            //     //     port.portColor = new Color(1f, 0.55f, 0f);
            //     // }
            //
            //     List<GfuPort> ports = field.GetValue(this) as List<GfuPort>;
            //     foreach (var gfuPort in ports){
            //         gfuPort.portName = GfuLanguage.Parse(gfuPort.name);
            //         gfuPort.tooltip = GfuLanguage.Parse(gfuPort.tooltip);
            //         if (gfuPort.direction == Direction.Input){
            //             inputContainer.Add(gfuPort);
            //         } else{
            //             outputContainer.Add(gfuPort);
            //         }
            //     }
            // }
#endif
        }

        public virtual List<GfuPort> Enter{ get; } = new List<GfuPort>();

        public virtual List<GfuPort> Exit{ get; } = new List<GfuPort>();

        public RuntimeNode RuntimeNode{ get; internal set; }
#if UNITY_EDITOR
        public sealed override string title{
            get => base.title;
            set => base.title = value;
        }
#endif
        /// <summary>
        ///     EditorMethod
        ///     获取该节点的所有端口
        /// </summary>
        /// <returns>端口列表</returns>
        internal List<GfuPort> GfuPorts(){ return GetGfuInput().AddAll(GetGfuOutPut()); }


        internal virtual IEnumerable<(GfuPort port, GfuPortAsset gfuPortAsset)> OnSavePort(GfuNodeAsset gfuNodeAsset){
            gfuNodeAsset.position = GetPosition().position;
            gfuNodeAsset.gfuNodeTypeCode = this.GetTypeByCode();
            gfuNodeAsset.inputPort = new List<GfuPortAsset>();
            gfuNodeAsset.outputPort = new List<GfuPortAsset>();
            var gfuPorts = GetGfuInput();
            for (var i = 0; i < gfuPorts.Count; i++){
                var gfuPortAsset = new GfuPortAsset();
                gfuPortAsset.Save(gfuPorts[i], gfuNodeAsset);
                gfuNodeAsset.inputPort.Add(gfuPortAsset);
                yield return (gfuPorts[i], gfuNodeAsset.inputPort[i]);
            }

            gfuPorts = GetGfuOutPut();
            for (var i = 0; i < gfuPorts.Count; i++){
                var gfuPortAsset = new GfuPortAsset();
                gfuPortAsset.Save(gfuPorts[i], gfuNodeAsset);
                gfuNodeAsset.outputPort.Add(gfuPortAsset);
                yield return (gfuPorts[i], gfuNodeAsset.outputPort[i]);
            }
        }

        internal virtual IEnumerable<(GfuPortAsset gfuPortAsset, GfuPort port)> OnLoadPort(GfuNodeAsset gfuNodeAsset){
            if (gfuNodeAsset.inputPort.Count != 0){
                Enter.Clear();
                Enter.AddRange(gfuNodeAsset.inputPort.Select(x => GfuPort.Create<Edge>(x.Orientation, x.Direction, x.Capacity, x.GetType(), x.portName)));
            }

            if (gfuNodeAsset.outputPort.Count != 0){
                Exit.Clear();
                Exit.AddRange(gfuNodeAsset.outputPort.Select(x => GfuPort.Create<Edge>(x.Orientation, x.Direction, x.Capacity, x.GetType(), x.portName)));
            }
            PortList<GfuPort> input = null;
            PortList<GfuPort> output = null;
            GetPortOverride();
            if (Enter.Count != 0){
                input = new PortList<GfuPort>(Enter, Direction.Input);
                var gfuPorts = input.value;
                for (var i = 0; i < gfuPorts.Count; i++) yield return (gfuNodeAsset.inputPort[i], gfuPorts[i]);
                inputContainer.Add(input);
                input.CreateBinder(GetType().GetProperty(nameof(Enter)),this);
            }
            if (Exit.Count != 0){
                output = new PortList<GfuPort>(Exit, Direction.Output);
                var gfuPorts = output.value;
                for (var i = 0; i < gfuPorts.Count; i++) yield return (gfuNodeAsset.outputPort[i], gfuPorts[i]);
                outputContainer.Add(output);
                output.CreateBinder(GetType().GetProperty(nameof(Exit)),this);
            }
            RefreshExpandedState();
        }

        private void GetPortOverride(){
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (RuntimeNode is IPortOverride portOverride){
                Enter.Clear();
                Enter.AddRange(portOverride.InputPortOverride.Select(x => GfuPort.Create<Edge>(x.Orientation,Direction.Input, x.Capacity, x.PortType, x.PortName)));
                Exit.Clear();
                Exit.AddRange(portOverride.InputPortOverride.Select(x => GfuPort.Create<Edge>(x.Orientation,Direction.Output, x.Capacity, x.PortType, x.PortName)));
            }
        }
        internal virtual void OnInitPort(){
            GetPortOverride();
            PortList<GfuPort> input = null;
            PortList<GfuPort> output = null;
            if (Enter.Count != 0){
                input = new PortList<GfuPort>(Enter, Direction.Input);
                inputContainer.Add(input);
                input.CreateBinder(GetType().GetProperty(nameof(Enter)),this);
            }
            if (Exit.Count != 0){
                output = new PortList<GfuPort>(Exit, Direction.Output);
                outputContainer.Add(output);
                output.CreateBinder(GetType().GetProperty(nameof(Exit)),this);
            }
            RefreshExpandedState();
        }
        /// <summary>
        ///     EditorMethod
        ///     获取输出端口列表
        /// </summary>
        /// <returns>端口列表</returns>
        internal virtual List<GfuPort> GetGfuOutPut(){
            var output = new List<GfuPort>();
#if UNITY_EDITOR
            for (var i = 0; i < outputContainer[0].childCount; i++) output.Add((GfuPort) outputContainer[0].ElementAt(i));
#endif
            return output;
        }

        /// <summary>
        ///     EditorMethod
        ///     获取输入端口列表
        /// </summary>
        /// <returns>端口列表</returns>
        internal virtual List<GfuPort> GetGfuInput(){
            var input = new List<GfuPort>();
#if UNITY_EDITOR
            for (var i = 0; i < inputContainer[0].childCount; i++) input.Add((GfuPort) inputContainer[0].ElementAt(i));
#endif
            return input;
        }


        /// <summary>
        ///     EditorMethod 游戏中始终返回0
        /// </summary>
        /// <returns></returns>
        protected int GetInputPortCount(){
#if UNITY_EDITOR
            return inputContainer.childCount;

#else
            return 0;
#endif
        }


        /// <summary>
        ///     EditorMethod 游戏中始终返回0
        /// </summary>
        /// <returns></returns>
        protected int GetOutputPortCount(){
#if UNITY_EDITOR
            return outputContainer.childCount;
#else
            return 0;
#endif
        }

        public GfuNode SetPosition(RuntimeNode otherRuntimeNode){
#if UNITY_EDITOR
            var rect = GetPosition();
            var vector4 = new Vector4();
            float scale = 1;
            // if (nodeData && nodeData.GraphData) scale = nodeData.GraphData.scale;
            rect.position = new Vector2(vector4.x, vector4.y - 19) * (1f / scale);
            SetPosition(rect);
#endif
            return (GfuNode) this;
        }

        // internal GfuNode SetPosition(){ return SetPosition(runtimeNode); }
        public bool IsSubClassOf(FieldInfo fieldInfo, Type parentType){ return IsSubClassOf(fieldInfo.FieldType, parentType); }

        /// <summary>
        ///     Normal Method
        /// </summary>
        /// <param name="otherType"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public bool IsSubClassOf(Type otherType, Type parentType){
            var type = otherType;
            while (type != null){
                if (type == parentType) return true;

                type = type.BaseType;
            }

            return false;
        }

        public virtual T[] GetFieldsArr<T>(){ return GetFields<T>().ToArray(); }

        public virtual List<T> GetFields<T>(){
            var fieldInfos = GetType().GetFields();
            var t = new List<T>();
            foreach (var fieldInfo in fieldInfos)
                if (fieldInfo.FieldType == typeof(T))
                    t.Add((T) fieldInfo.GetValue(this));

            return t;
        }

        public virtual List<FieldInfo> GetFieldsWithFieldInfo<T>(){
            var fieldInfos = GetType().GetFields();
            var t = new List<FieldInfo>();
            foreach (var fieldInfo in fieldInfos)
                if (fieldInfo.FieldType == typeof(T))
                    t.Add(fieldInfo);

            return t;
        }

        public virtual Dictionary<T, string> GetFieldsName<T>(){
            var fieldInfos = GetType().GetFields();
            var t = new Dictionary<T, string>();
            foreach (var fieldInfo in fieldInfos)
                if (fieldInfo.GetValue(this) is T)
                    t.Add((T) fieldInfo.GetValue(this), fieldInfo.Name);

            return t;
        }

        public List<FieldInfo> GfuInputPortFieldInfos(){
            var gfuInput = new List<FieldInfo>();
            var gfuPorts = GetFieldsWithFieldInfo<GfuPort>();
            foreach (var fieldInfo in gfuPorts)
                if (fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Input)
                    gfuInput.Add(fieldInfo);

            return gfuInput;
        }

        public List<FieldInfo> GfuOutputPortFieldInfos(){
            var gfuInput = new List<FieldInfo>();
            var gfuPorts = GetFieldsWithFieldInfo<GfuPort>();
            foreach (var fieldInfo in gfuPorts)
                if (fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Output)
                    gfuInput.Add(fieldInfo);

            return gfuInput;
        }

#if UNITY_EDITOR
        /// <summary>
        ///     EditorMethod
        ///     获取该节点的所有端口
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
        ///     EditorMethod
        ///     获取该节点的所有端口
        /// </summary>
        /// <returns>端口列表</returns>
        internal List<Port> Ports(){ return GetInput().AddAll(GetOutPut()); }

        /// <summary>
        ///     EditorMethod
        ///     获取输出端口列表
        /// </summary>
        /// <returns>端口列表</returns>
        internal virtual List<Port> GetOutPut(){
            var output = new List<Port>();
            for (var i = 0; i < outputContainer.childCount; i++) output.Add((Port) outputContainer.ElementAt(i));
            return output;
        }

        /// <summary>
        ///     EditorMethod
        ///     获取输入端口列表
        /// </summary>
        /// <returns>端口列表</returns>
        internal virtual List<Port> GetInput(){
            var input = new List<Port>();
            for (var i = 0; i < inputContainer.childCount; i++) input.Add((Port) inputContainer.ElementAt(i));
            return input;
        }
#endif
    }
}