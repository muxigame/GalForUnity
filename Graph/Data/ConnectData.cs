//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ConnectData.cs
//
//        Created by 半世癫(Roc) at 2021-01-12 00:06:00
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Graph.GFUNode.Base;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine.Serialization;

namespace GalForUnity.Graph.Data{
    /// <summary>
    /// 连接数据，连接保存着连接两端节点的引用
    /// </summary>
    [Serializable]
    public class ConnectData{
        /// <summary>
        /// 该链接连接的被输入节点
        /// </summary>
        [FormerlySerializedAs("_input")] public NodeData Input;

        /// <summary>
        /// 该链接连接的输出节点
        /// </summary>
        [FormerlySerializedAs("_output")] public NodeData Output;

        /// <summary>
        /// 相对于该链接被输入节点的端口号
        /// </summary>
        [FormerlySerializedAs("input_index")] public int inputIndex;

        /// <summary>
        /// 相对于该链接输出节点的端口号
        /// </summary>
        [FormerlySerializedAs("output_index")] public int outputIndex;


        /// <summary>
        /// EditorMethod
        /// 根据Edge解析链接,其中Edge属于EditorMethod,所以该方法在运行中无法调用
        /// </summary>
        /// <param name="edge">Node的链接</param>
        /// <param name="direction"></param>
        /// <param name="datas">数据源</param>
        /// <returns></returns>
#if UNITY_EDITOR
        public ConnectData Parse(Edge edge, Direction direction, Dictionary<GfuNode, NodeData> datas){
            // if (direction == Direction.Input){
            Input = datas[(GfuNode) edge.input.node];
            Output = datas[(GfuNode) edge.output.node];
            inputIndex = edge.input.node.inputContainer.IndexOf(edge.input);
            outputIndex = edge.output.node.outputContainer.IndexOf(edge.output);
            return this;
        }
#endif
    }
}