//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-09 21:59:17
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using GalForUnity.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.Tool;
using GalForUnity.Model;
using GalForUnity.System;
using GalForUnity.System.Event;
using JetBrains.Annotations;
using UnityEngine;


namespace GalForUnity.Graph.GFUNode.Base{
    /// <summary>
    /// 节点系统的父类，在编辑器状态下户继承自VisualElement的节点，但是在运行时转为继承节点数据的NodeData
    /// GfuNode是GalForUnity中所有节点的父类，提供了基础的反射，保存，和节点执行方法，使用和继承此类应注意和Editor的解耦
    /// </summary>
    [Serializable]
    public class GfuNode : EditorNode,ISerializable{
        /// <summary>
        /// 当节点执行完毕回调
        /// </summary>
        public Action<GfuNode> OnExecuted;

        public GfuNode(NodeData nodeData = null){ this.nodeData = nodeData; }

        /// <summary>
        /// 初始化节点系统的方法，应该从此方法初始化节点系统，并调用父类的方法
        /// </summary>
        /// <param name="otherNodeData"></param>
        public virtual void Init(NodeData otherNodeData){
            if (!nodeData) nodeData = otherNodeData;
            SetPosition(otherNodeData);
            if (otherNodeData){
                try{
                    InitNodeTool.SetContainerValue(otherNodeData, this);
                } catch (Exception e){
                    Debug.LogError(GfuLanguage.Parse("Failed to initialize node data") + e);
                }
            }
        }

        /// <summary>
        /// 需要在此方法中定义节点的操作，并且在节点执行完毕中调用Executed来跳转到下一个端口(不可以在此方法中使用Editor，这会导致节点系统无法使用)
        /// </summary>
        /// <param name="roleData">传入的节点参数</param>
        /// <returns>返回节点参数</returns>
        public virtual RoleData Execute(RoleData roleData){ return Executed(0, roleData); }

        /// <summary>
        /// 需要在此方法中给字段对象赋值，值来自节点的视图系统,如果不实现此方法，则节点可能无法保存
        /// </summary>
        public virtual void Save(){ }

        /// <summary>
        /// 跳转到下一个默认节点，当端口不存在时，会触发异常
        /// </summary>
        public virtual void Executed(){ Executed(0); }

        /// <summary>
        /// 跳转到下一个指定的端口，当端口不存在时，会触发异常
        /// </summary>
        /// <param name="index">下一个端口的索引</param>
        public virtual void Executed(int index){
            var invocationList = EventCenter.GetInstance().OnNodeExecutedEvent.GetInvocationList();
            GfuNode inputNode = null;
            if (nodeData.OutputPort != null && nodeData.OutputPort.Count > 0){
                var output = nodeData.OutputPort[index];
                if (output == null) throw new NullReferenceException(GfuLanguage.ParseLog("The current node has no output port") + this);
                if (output.connections == null || output.connections.Count == 0) throw new NullReferenceException(GfuLanguage.ParseLog("The current port is not connected") + this);
                var input = output.connections.FirstOrDefault()?.Input;
                if (input == null) throw new NullReferenceException(GfuLanguage.ParseLog("The current connection has no input node") + this);
                inputNode = GfuGraph.GetNode(input.instanceID);
            }

            for (var i = invocationList.Length - 1; i >= 0; i--){
                invocationList[i].Method.Invoke(invocationList[i].Target, new object[] {
                    this
                });
            }

            OnExecuted?.Invoke(inputNode);
        }

        /// <summary>
        /// 跳转到下一个指定的端口，当端口不存在时，会触发异常
        /// </summary>
        /// <param name="index">下一个端口的索引</param>
        /// <param name="roleData">要传递的角色数据</param>
        /// <returns></returns>
        public virtual RoleData Executed(int index, RoleData roleData){
            Executed(index);
            return roleData;
        }

        /// <summary>
        /// NormalMethod 判断当前节点是否还有可能存在的下一个节点
        /// </summary>
        public virtual bool HasNext(){
            bool hasNext = false;
            foreach (var portData in nodeData.OutputPort){
                if (portData.connections != null && portData.connections.Count > 0) hasNext = true;
            }

            return hasNext;
        }

        /// <summary>
        /// NormalMethod 判断当前节点是否还有可能存在的下一个节点
        /// </summary>
        public virtual GfuNode Next(int portIndex){ return GfuGraph.GetNode(nodeData.OutputPort?[portIndex]?.connections?.FirstOrDefault()?.Input.instanceID ?? 0); }

        [Obsolete]
        public GfuNode ToGfuNode(NodeData otherNodeData){
            this.nodeData = otherNodeData;
            return this;
        }

        /// <summary>
        /// 获得Input端口指定端口索引，链接索引的指定节点，如果错误的创建了图和节点，可能返回null值
        /// </summary>
        /// <param name="portIndex"></param>
        /// <param name="connectionIndex"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">当获取失败的时触发</exception>
        public virtual GfuNode GetInputNode(int portIndex, int connectionIndex){
            var portData = nodeData.InputPort[portIndex];
            if (GfuGraph == null) throw new NullReferenceException("Graph does not exist, node may have been incorrectly initialized");
            if (portData == null) throw new NullReferenceException("Get input portData failed,portIndex:" + portIndex);
            var portDataConnection = portData.connections[connectionIndex];
            if (portDataConnection == null) throw new NullReferenceException("Get connect failed,portIndex:" + portIndex + "connectIndex:" + connectionIndex);
            return GfuGraph.GetNode(portDataConnection.Output.instanceID);
        }

        /// <summary>
        /// 获得Input端口指定端口索引的所有节点，如果错误的创建了图和节点，可能返回null值
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">当获取失败的时触发</exception>
        public virtual List<GfuNode> GetInputNodes(int portIndex){
            List<GfuNode> listNode = new List<GfuNode>();
            if (GfuGraph == null) throw new NullReferenceException("Graph does not exist, node may have been incorrectly initialized");
            var connections = nodeData.InputPort[portIndex].connections;
            if (connections == null) throw new NullReferenceException("The link to the node input segment was not obtained");
            foreach (var connection in connections){
                listNode.Add(GfuGraph.GetNode(connection.Output.instanceID));
            }

            return listNode;
        }

        /// <summary>
        /// 通过instanceID获得节点，如果错误的初始化了图或节点则会返回null
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public GfuNode GetNode(long instanceID){ return GfuGraph?.GetNode(instanceID); }

        /// <summary>
        /// 判断Input端指定的端口是否存在链接
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">节点数据为空时触发异常</exception>
        public bool IsInputConnected(int portIndex){
            if (nodeData is null) throw new NullReferenceException("Node data does not exist");
            if (nodeData.InputPort?[portIndex]                     == null) return false;
            return nodeData.InputPort[portIndex].connections.Count != 0;
        }

        /// <summary>
        /// 判断Output端指定的端口是否存在链接
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">节点数据为空时触发异常</exception>
        public bool IsOutputConnected(int portIndex){
            if (nodeData is null) throw new NullReferenceException("Node data does not exist");
            if (nodeData.OutputPort?[portIndex] == null) return false;
            return nodeData.OutputPort != null && nodeData.OutputPort[portIndex].connections.Count != 0;
        }

        /// <summary>
        /// 获得所有端口指定的端口号(依据声明顺序)的输入数据的类型，当端口不存在时返回null
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        public Type PortType(int portIndex){
            int i = 0;
            foreach (var fieldInfo in GetType().GetFields()){
                if (fieldInfo.FieldType == typeof(GfuPort)){
                    if (i == portIndex) return fieldInfo.GetCustomAttribute<NodeRenameAttribute>().Type;
                    i++;
                }
            }

            return null;
        }

        /// <summary>
        /// 获得Input端口指定的端口号的输入数据的类型，当端口不存在时返回null
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        public Type InputPortType(int portIndex){
            int i = 0;
            foreach (var fieldInfo in GetType().GetFields()){
                if (fieldInfo.FieldType == typeof(GfuPort) && fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Input){
                    if (i == portIndex) return fieldInfo.GetCustomAttribute<NodeRenameAttribute>().Type;
                    i++;
                }
            }

            return null;
        }

        /// <summary>
        /// 获得Output端口指定的端口号的输入数据的类型，当端口不存在时返回null
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        public Type OutputPortType(int portIndex){
            int i = 0;
            foreach (var fieldInfo in GetType().GetFields()){
                if (fieldInfo.FieldType == typeof(GfuPort) && fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Output){
                    if (i == portIndex) return fieldInfo.GetCustomAttribute<NodeRenameAttribute>().Type;
                    i++;
                }
            }

            return null;
        }

        /// <summary>
        /// 获得Input和Output总端口的数量
        /// </summary>
        /// <returns></returns>
        public int PortCount{
            get{
                if (nodeData) return nodeData.InputPort?.Count ?? 0 + nodeData.OutputPort?.Count ?? 0;
                Debug.LogError("Node data does not exist");
                return 0;
            }
        }

        // [ItemCanBeNull]
        // public List<GfuPort> GfuPorts{
        //     get{
        //         List<GfuPort> gfuPorts=new List<GfuPort>();
        //         foreach (var fieldInfo in GetType().GetFields()){
        //             if (fieldInfo.FieldType == typeof(GfuPort) && fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Output){
        //                 gfuPorts.Add((GfuPort) fieldInfo.GetValue(this));
        //             }
        //         }
        //     }
        // }

        /// <summary>
        /// 获得Input端口数量
        /// </summary>
        /// <returns></returns>
        public int InputPortCount{
            get{
                if (nodeData) return nodeData.InputPort?.Count ?? 0;
                // Debug.LogError("Node data does not exist");
                return GetInputPortCount();
            }
        }

        /// <summary>
        /// 获得Output端口数量
        /// </summary>
        /// <returns></returns>
        public int OutputPortCount{
            get{
                if (nodeData) return nodeData.OutputPort?.Count ?? 0;
                // Debug.LogError("Node data does not exist");
                return GetOutputPortCount();
            }
        }

        /// <summary>
        /// 获得Input端口的所有端口数据
        /// </summary>
        /// <returns></returns>
        public List<PortData> InputPortData{
            get{
                List<PortData> inputPort = new List<PortData>();
                if (!nodeData){
                    Debug.LogError("Node data does not exist");
                    return inputPort;
                }
                nodeData.InputPort?.ForEach((element => { inputPort.Add(element); }));
                return inputPort;
            }
        }

        /// <summary>
        /// 获得Output端口的所有端口数据
        /// </summary>
        /// <returns></returns>
        public List<PortData> OutputPortData{
            get{
                List<PortData> outputPort = new List<PortData>();
                if (!nodeData){
                    Debug.LogError("Node data does not exist");
                    return outputPort;
                }

                nodeData.OutputPort?.ForEach((element => outputPort.Add(element)));
                return outputPort;
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context){
            var fieldInfos = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var fieldInfo in fieldInfos){
                if (fieldInfo.GetValue(this) !=null){
                    fieldInfo.SetValue(this,fieldInfo.GetValue(this));
                }
            }
        }
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected GfuNode(SerializationInfo info, StreamingContext context){
            Type type = GetType();
            var serializationInfoEnumerator = info.GetEnumerator();
            while (serializationInfoEnumerator.MoveNext()){
                type.GetField(serializationInfoEnumerator.Name).SetValue(this,serializationInfoEnumerator.Value);
            }
        }
    }
}