//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  Node.cs
//
//        Created by 半世癫(Roc) at 2021-01-10 19:15:44
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


namespace GalForUnity.Graph.Data{
    /// <summary>
    /// 节点数据，保存着输入输出已经字段和引用信息，这个类在UNITY_EDITOR环境下，负责给节点系统记录节点信息 ，在运行时，直接被节点系统继承，利用节点信息跑剧情流
    /// </summary>
    [Serializable]
    public class NodeData : ScriptableObject, DataInfo{
        public GraphData GraphData;
        public string type;
        public Vector4 vector4;
        [FormerlySerializedAs("Input")] public List<PortData> InputPort;
        [FormerlySerializedAs("Output")] public List<PortData> OutputPort;
        public long instanceID;

        [Serializable]
        public class NodeFieldInfo{
            [SerializeField] public string name;

            [FormerlySerializedAs("json")] [SerializeField]
            public string data;

            [SerializeField] public long instanceID;
            [SerializeField] public string assembly;
            [SerializeField] public string type;
            [SerializeField] public Object scriptableObject;
        }

        [Serializable]
        public class ListData{
            [SerializeField] public string name;
            [SerializeField] public string assembly;
            [SerializeField] public string type;
            [SerializeField] public List<NodeData.NodeFieldInfo> jsonField;
            [SerializeField] public List<NodeData.NodeFieldInfo> idField;
        }
#if UNITY_EDITOR
        /// <summary>
        /// 节点的解析为编辑器状态下的方法，不允许运行时解析，原因是节点系统不可避免的调用了Editor
        /// 这个方法会通过序列化和保存InstanceID两种方式保存节点信息，在运行时有Graph系统和InitNodeTool负责解析节点和赋值
        /// </summary>
        /// <param name="node">VisualElement节点</param>
        /// <returns>解析得到的纯节点数据</returns>
        public NodeData Parse(GfuNode node){
            instanceID = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0); //保存节点的InstanceID此ID供节点系统查找唯一节点
            jsonField = new List<NodeFieldInfo>();
            idField = new List<NodeFieldInfo>();
            listField = new List<ListData>();
            if (node == null){
                type = "";
                vector4 = new Vector4();
                return this;
            }

            vector4 = new Vector4(node.worldBound.x, node.worldBound.y, node.worldBound.width, node.worldBound.height);
            type = node.GetType().FullName;
            var fieldInfos = node.GetType().GetFields();
            if (fieldInfos.Length == 0) return this;
            foreach (var fieldInfo in fieldInfos){
                object fieldValue = fieldInfo.GetValue(node);
                if (fieldValue is Port || fieldValue == null) continue; //不保存接口和空对象
                this.ParseField(fieldValue, fieldInfo);
            }

            return this;
        }


        // public List<Node> Nodes;
        public void Save(string path){
            name = nameof(NodeData);
            for (var i = 0; i < InputPort.Count; i++){
                // AssetDatabase.AddObjectToAsset(Input[i], path);
                InputPort[i].Save(path);
            }
            for (var i = 0; i < OutputPort.Count; i++){
                // AssetDatabase.AddObjectToAsset(Output[i], path);
                OutputPort[i].Save(path);
            }
        }
#endif

        [FormerlySerializedAs("JsInfos")] public List<NodeFieldInfo> JsonInfos;
        public List<NodeFieldInfo> IdInfos;

        [FormerlySerializedAs("ListJsonInfos")] [SerializeField]
        public List<ListData> listInfos;

        public List<NodeFieldInfo> jsonField{
            get => JsonInfos;
            set => JsonInfos = value;
        }

        public List<NodeFieldInfo> idField{
            get => IdInfos;
            set => IdInfos = value;
        }

        public List<ListData> listField{
            get => listInfos;
            set => listInfos = value;
        }

        //         public NodeData ToGfuNode(){
        // #if UNITY_EDITOR
        //             return this;
        // #else
        //             var gfuNode = ScriptableObject.CreateInstance(Type.GetType(type));
        //             gfuNode.ToGfuNode(this);
        //             return gfuNode;
        // #endif
        //         }
    }
}