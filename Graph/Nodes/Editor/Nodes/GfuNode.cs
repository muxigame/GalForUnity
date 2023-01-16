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
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using GalForUnity.Attributes;
using GalForUnity.Graph.Build;
using GalForUnity.Graph.SceneGraph;
using GalForUnity.Model;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Base{
    /// <summary>
    ///     节点系统的父类，在编辑器状态下户继承自VisualElement的节点，但是在运行时转为继承节点数据的NodeData
    ///     GfuNode是GalForUnity中所有节点的父类，提供了基础的反射，保存，和节点执行方法，使用和继承此类应注意和Editor的解耦
    /// </summary>
    [Serializable]
    public class GfuNode : EditorNode{
        
        [NonSerialized] private INodeRuntimeProviderBase _nodeFindProvider;

        [NonSerialized] public long instanceID;
        /// <summary>
        /// 节点数据
        /// </summary>
        public GfuNodeAsset nodeAsset;
        /// <summary>
        ///     当节点执行完毕回调
        /// </summary>
        [NonSerialized] public Action<GfuNode> OnExecuted;
        public GfuNode(){}

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected GfuNode(SerializationInfo info, StreamingContext context){
            var type = GetType();
            var serializationInfoEnumerator = info.GetEnumerator();
            while (serializationInfoEnumerator.MoveNext()) type.GetField(serializationInfoEnumerator.Name).SetValue(this, serializationInfoEnumerator.Value);
        }



        /// <summary>
        ///     获得Input和Output总端口的数量
        /// </summary>
        /// <returns></returns>
        public int PortCount => nodeAsset.inputPort.Count + nodeAsset.outputPort.Count;


        /// <summary>
        ///     获得Input端口数量
        /// </summary>
        /// <returns></returns>
        public int InputPortCount => nodeAsset.inputPort.Count;

        /// <summary>
        ///     获得Output端口数量
        /// </summary>
        /// <returns></returns>
        public int OutputPortCount => nodeAsset.outputPort.Count;


        /// <summary>
        ///     初始化节点系统的方法，应该从此方法初始化节点系统，并调用父类的方法
        /// </summary>
        /// <param name="otherRuntimeNode"></param>
        public virtual void OnInit(RuntimeNode otherRuntimeNode){
            RuntimeNode = otherRuntimeNode;
        }
        
        /// <summary>
        ///     需要在此方法中定义节点的操作，并且在节点执行完毕中调用Executed来跳转到下一个端口(不可以在此方法中使用Editor，这会导致节点系统无法使用)
        /// </summary>
        /// <param name="roleData">传入的节点参数</param>
        /// <returns>返回节点参数</returns>
        public virtual RoleData Execute(RoleData roleData){ return Executed(0, roleData); }

        /// <summary>
        ///     需要在此方法中给字段对象赋值，值来自节点的视图系统,如果不实现此方法，则节点可能无法保存
        /// </summary>
        public virtual void Save(){ }

        /// <summary>
        ///     跳转到下一个默认节点，当端口不存在时，会触发异常
        /// </summary>
        public virtual void Executed(){ Executed(0); }

        /// <summary>
        ///     跳转到下一个指定的端口，当端口不存在时，会触发异常
        /// </summary>
        /// <param name="index">下一个端口的索引</param>
        public virtual void Executed(int index){
            // if (_nodeFindProvider != null) OnExecuted?.Invoke(_nodeFindProvider.GetOutputNode(index, 0));
        }

        /// <summary>
        ///     跳转到下一个指定的端口，当端口不存在时，会触发异常
        /// </summary>
        /// <param name="index">下一个端口的索引</param>
        /// <param name="roleData">要传递的角色数据</param>
        /// <returns></returns>
        public virtual RoleData Executed(int index, RoleData roleData){
            Executed(index);
            return roleData;
        }

        /// <summary>
        ///     NormalMethod 判断当前节点是否还有可能存在的下一个节点
        /// </summary>
        public virtual bool HasNext(){ return nodeAsset.HasOutputPort && nodeAsset.outputPort.HasConnection(); }

        /// <summary>
        ///     NormalMethod 判断当前节点是否还有可能存在的下一个节点
        /// </summary>
        // public virtual GfuNode Next(int portIndex){ return GfuGraph.GetNode(nodeData.OutputPort?[portIndex]?.connections?.FirstOrDefault()?.Input.instanceID ?? 0); }
        [Obsolete]
        public GfuNode ToGfuNode(RuntimeNode otherRuntimeNode){
            RuntimeNode = otherRuntimeNode;
            return this;
        }


        /// <summary>
        ///     判断Input端指定的端口是否存在链接
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">节点数据为空时触发异常</exception>
        public bool IsInputConnected(int portIndex){
            if (_nodeFindProvider != null) return _nodeFindProvider.IsInputPortConnected(portIndex);
            if (RuntimeNode is null) throw new NullReferenceException("Node data does not exist");
            return nodeAsset.HasInputPort&&nodeAsset.inputPort.HasConnection();
        }

        /// <summary>
        ///     判断Output端指定的端口是否存在链接
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">节点数据为空时触发异常</exception>
        public bool IsOutputConnected(int portIndex){
            if (_nodeFindProvider != null) return _nodeFindProvider.IsOutputPortConnected(portIndex);
            if (RuntimeNode is null) throw new NullReferenceException("Node data does not exist");
            return nodeAsset.HasOutputPort &&nodeAsset.outputPort.HasConnection();
        }

        public int GetOutputConnectionCount(int index = 0){ return _nodeFindProvider.GetOutputPortConnectionCount(index); }

        public int GetInputConnectionCount(int index = 0){ return _nodeFindProvider.GetInputPortConnectionCount(index); }

        /// <summary>
        ///     获得所有端口指定的端口号(依据声明顺序)的输入数据的类型，当端口不存在时返回null
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        public Type PortType(int portIndex){
            var i = 0;
            foreach (var fieldInfo in GetType().GetFields())
                if (fieldInfo.FieldType == typeof(GfuPort)){
                    if (i == portIndex) return fieldInfo.GetCustomAttribute<NodeRenameAttribute>().Type;
                    i++;
                }

            return null;
        }

        /// <summary>
        ///     获得Input端口指定的端口号的输入数据的类型，当端口不存在时返回null
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        public Type InputPortType(int portIndex){
            var i = 0;
            foreach (var fieldInfo in GetType().GetFields())
                if (fieldInfo.FieldType == typeof(GfuPort) && fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Input){
                    if (i == portIndex) return fieldInfo.GetCustomAttribute<NodeRenameAttribute>().Type;
                    i++;
                }
            return null;
        }

        /// <summary>
        ///     获得Output端口指定的端口号的输入数据的类型，当端口不存在时返回null
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        public Type OutputPortType(int portIndex){
            var i = 0;
            foreach (var fieldInfo in GetType().GetFields())
                if (fieldInfo.FieldType == typeof(GfuPort) && fieldInfo.GetCustomAttribute<NodeRenameAttribute>().PortType == NodeDirection.Output){
                    if (i == portIndex) return fieldInfo.GetCustomAttribute<NodeRenameAttribute>().Type;
                    i++;
                }

            return null;
        }
    }
}