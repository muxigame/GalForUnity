//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GraphData.cs
//
//        Created by 半世癫(Roc) at 2021-01-10 22:12:01
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.InstanceID;
#if UNITY_EDITOR
using GalForUnity.System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;


namespace GalForUnity.Graph.Data{
    /// <summary>
    /// 图的数据，保存着所有节点及其连接信息，这个类在UNITY_EDITOR环境下，负责给节点系统提供底层数据
    /// 关于数据保存的方法，如果有更好的建议欢迎通过https://github.com/muxigame/GalForUnity提交。
    /// </summary>
    [Serializable]
    public class GraphData : GfuInstanceID{
        public List<NodeData> Nodes;
        public float scale;
        public bool isPlay = false;
        [NonSerialized] private Dictionary<GfuNode, NodeData> _dictionary = new Dictionary<GfuNode, NodeData>();
#if UNITY_EDITOR
        /// <summary>
        /// EditorMethod
        /// 解析节点数据并返回GraphData,这个接口是不会保存私有字段的。
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="graphScale"></param>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public virtual GraphData Parse(List<Node> nodes, float graphScale, long instanceID){
            try{
                this.scale = graphScale;
                this.InstanceID = instanceID == 0 ? BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0) : instanceID;
                Nodes = new List<NodeData>();
                _dictionary = new Dictionary<GfuNode, NodeData>();
                if (nodes == null || nodes.Count == 0) return this;
                foreach (var node1 in nodes){
                    if (node1 is GfuNode gfuNode){
                        NodeData nodeData = CreateInstance<NodeData>();
                        try{
                            gfuNode.Save();
                        } catch (Exception e){
                            Debug.LogError(GfuLanguage.ParseLog("The node failed to save itself, which means the data may not have been saved") + e);
                        }

                        try{
                            nodeData.Parse(gfuNode); //解析节点该方法会将保存于VisualElement的输出转移到节点当中
                        } catch (Exception e){
                            Debug.LogError(GfuLanguage.ParseLog("The node failed to be resolved, which means there may be an unsupported type in the node") + e);
                        }

                        nodeData.GraphData = this;
                        _dictionary.Add(gfuNode, nodeData);
                        Nodes.Add(nodeData); //将数据在字典和列表中暂存一份提供引用
                    } else{
                        Debug.LogError(GfuLanguage.ParseLog("This is not a GfuNode object"));
                    }
                }

                for (var i = 0; i < nodes.Count; i++){
                    NodeData nodeData = Nodes[i];
                    if (nodes[i] is GfuNode gfuNode){
                        var ports = gfuNode.Ports();
                        nodeData.type = gfuNode.GetType().FullName;
                        nodeData.OutputPort = new List<PortData>();
                        nodeData.InputPort = new List<PortData>();
                        for (var i1 = 0; i1 < ports.Count; i1++){ //遍历端口
                            PortData portData = new PortData();
                            try{
                                portData = portData.Parse(ports[i1], _dictionary);
                            } catch (Exception e){
                                Debug.LogError(GfuLanguage.ParseLog("Resolving the port failed, which means that there may be an unsupported type in the default data for the port") + e);
                            }

                            if (ports[i1].direction == Direction.Output){
                                nodeData.OutputPort.Add(portData); //将端口上的链接赋予引用
                            } else if (ports[i1].direction == Direction.Input){
                                nodeData.InputPort.Add(portData);
                            }
                        }
                    } else{
                        Debug.LogError(GfuLanguage.ParseLog("This is not a GfuNode object"));
                    }
                }
            } catch (Exception e){
                Debug.LogError(GfuLanguage.ParseLog("There is an exception when saving the file, please contact the developer as soon as possible to avoid loss") + e);
                throw;
            }

            return this;
        }

        public virtual void Save(string path){
            for (var i = 0; i < Nodes.Count; i++){
                AssetDatabase.AddObjectToAsset(Nodes[i], path);
                AssetDatabase.SetMainObject(this,path);
                Nodes[i].Save(path);
            }
        }

        public virtual void Delete(string path){
            if (Nodes != null){
                for (var i = 0; i < Nodes.Count; i++){
                    if (Nodes[i] == null) continue;
                    AssetDatabase.RemoveObjectFromAsset(Nodes[i]);
                }
            }
        }

#endif
    }
}