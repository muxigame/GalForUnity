//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PortData.cs
//
//        Created by 半世癫(Roc) at 2021-01-10 22:57:21
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.External;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

namespace GalForUnity.Graph.Data{
    /// <summary>
    /// 端口数据，端口记录着端口的所有连接，以及此端口的接受和输出的数据类型
    /// </summary>
    [Serializable]
    public class PortData : DataInfo{
        public List<ConnectData> connections;
        public string sourceType;
        public string type;
        public long instanceID;
#if UNITY_EDITOR
        public PortData Parse(Port ports, Dictionary<GfuNode, NodeData> datas){
            connections = new List<ConnectData>();
            if (ports?.source == null) return this;
            sourceType = ports.source.GetType().ToString();
            type = ports.portType?.ToString();
            foreach (var portsConnection in ports.connections){
                ConnectData connectData = new ConnectData();
                if (portsConnection != null){
                    try{
                        connectData.Parse(portsConnection, ports.direction, datas);
                        connections.Add(connectData);
                    } catch (Exception e){
                        Debug.Log(GfuLanguage.ParseLog("Parse connection failure" + e));
                    }
                }
            }

            if (ports is GfuPort gfuPort){
                if (gfuPort.InputView != null){
                    idField = new List<NodeData.NodeFieldInfo>();
                    jsonField = new List<NodeData.NodeFieldInfo>();
                    var type = gfuPort.InputView?.fieldContainer?.Children()?.FirstOrDefault()?.GetType();
                    if (type != null){
                        var inputViewValue = type.BaseType?.GetProperty("value");
                        if (inputViewValue != null){
                            try{
                                this.ParseField(inputViewValue.GetValue(gfuPort.InputView.fieldContainer[0]), inputViewValue);
                            } catch (Exception e){
                                Debug.Log(GfuLanguage.ParseLog("Parsing the port failed, which means that the port data was not saved" + e));
                            }
                        }
                    }
                }
            } else{
                Debug.Log(GfuLanguage.ParseLog("This is not a GfuPort"));
            }

            return this;
        }

        public void Save(string path){
            for (var i = 0; i < connections.Count; i++){
                // AssetDatabase.AddObjectToAsset(connections[i], path);
            }
        }
#endif
        public List<NodeData.NodeFieldInfo> JsInfos;
        public List<NodeData.NodeFieldInfo> IdInfos;
        public List<NodeData.ListData> ListJsonInfos;

        public List<NodeData.NodeFieldInfo> jsonField{
            get => JsInfos;
            set => JsInfos = value;
        }

        public List<NodeData.NodeFieldInfo> idField{
            get => IdInfos;
            set => IdInfos = value;
        }

        public List<NodeData.ListData> listField{
            get => ListJsonInfos;
            set => ListJsonInfos = value;
        }
    }
}