//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuSceneGraphView.cs
//
//        Created by 半世癫(Roc) at 2022-04-14 22:56:54
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Nodes.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using PlotNode = GalForUnity.Graph.Nodes.Runtime.PlotNode;


namespace GalForUnity.Graph.SceneGraph{
    public class GfuSceneGraphView : GraphView{
        public Dictionary<long, GfuNode> Nodes = new Dictionary<long, GfuNode>();

        public GfuSceneGraphView() : this(null){ }

        public GfuSceneGraphView(GfuGraphAsset gfuGraphAsset){
            // _sceneGraphEditorWindow = sceneGraphEditorWindow;
            InitEditorView();
            if (gfuGraphAsset == null || gfuGraphAsset.nodes == null || gfuGraphAsset.nodes.Count == 0){
                var node = Activator.CreateInstance(typeof(MainNode)) as GfuNode;
                AddElement(node);
                Nodes.Add(new Guid().GetHashCode(), node);
                return;
            }

            var connection = new HashSet<GfuConnectionAsset>();
            Dictionary<GfuPortAsset,GfuPort> portMap=new Dictionary<GfuPortAsset, GfuPort>();
            for (var i = 0; i < gfuGraphAsset.nodes.Count; ++i){
                GfuNodeAsset gfuNodeAsset = gfuGraphAsset.nodes[i];
                foreach (var port in InitNode(gfuNodeAsset)){
                    if(!portMap.ContainsKey(port.Item1))portMap.Add(port.Item1,port.Item2);
                }
                if (gfuNodeAsset.HasInputPort)
                    foreach (var gfuPortAsset in gfuNodeAsset.inputPort){
                        if (!gfuPortAsset.HasConnection) continue;
                        gfuPortAsset.connections.ForEach(asset => connection.Add(asset));
                    }

                if (gfuNodeAsset.HasOutputPort)
                    foreach (var gfuPortAsset in gfuNodeAsset.outputPort){
                        if (!gfuPortAsset.HasConnection) continue;
                        gfuPortAsset.connections.ForEach(asset => connection.Add(asset));
                    }
            }
            foreach (var gfuConnectionAsset in connection) InitConnection(gfuConnectionAsset,portMap);
            foreach (var keyValuePair in Nodes){
                var nodeByInstanceID = gfuGraphAsset.GetNodeByInstanceID(keyValuePair.Value.instanceID);
                // keyValuePair.Value.InitWithGfuNodeData(nodeByInstanceID, graphData.GetNodeData(keyValuePair.Value.instanceID), null);
            }
        }

        public void InitEditorView(){
            GridBackground gridBackground = new GfuBackground();
            gridBackground.name = "GridBackground";
            Insert(0, gridBackground);
            SetupZoom(0.25f, 2.0f);
            this.StretchToParentSize();
            this.StretchToParentSize();
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public IEnumerable<(GfuPortAsset, GfuPort)> InitNode(GfuNodeAsset gfuNodeAsset){
            var node = Activator.CreateInstance(gfuNodeAsset.Type ?? typeof(GfuNode)) as GfuNode;
            if (node == null){
                Debug.LogError("node is null");
                return default;
            }

            node.Init(gfuNodeAsset.runtimeNode);

            AddElement(node);
            // if(node is GfuOperationNode gfuOperationNode)gfuOperationNode.GfuInputViews.ForEach(x=>x.);.Init(null);
            Nodes.Add(gfuNodeAsset.instanceID, node);
            node.instanceID = gfuNodeAsset.instanceID;
            var rect = node.GetPosition();
            rect.position = gfuNodeAsset.position;
            node.SetPosition(rect);
            return node.OnLoadPort(gfuNodeAsset);
        }

        public void InitConnection(GfuConnectionAsset gfuConnectionAsset,Dictionary<GfuPortAsset,GfuPort> portMap){
            var inputPort = gfuConnectionAsset.input;
            var outputPort = gfuConnectionAsset.output;
            if (inputPort == null || outputPort == null){
                Debug.LogError("链接没有节点");
                return;
            }
            var visualElementInput = portMap[inputPort] as Port;
            var visualElementOutput = portMap[outputPort] as Port;
            var connectTo = visualElementInput?.ConnectTo(visualElementOutput);
            AddElement(connectTo);
        }


        /// <summary>
        ///     节点间连接判断的方法，不允许节点自身相连，同时也不允许不同类型的节点相连
        /// </summary>
        /// <param name="startAnchor"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter){
            var compatiblePorts = new List<Port>();
            foreach (var port in ports.ToList()){
                if (!port.enabledSelf) continue;
                if (startAnchor.node      == port.node      ||
                    startAnchor.direction == port.direction ||
                    // port is GfuInputView                    ||
                    startAnchor.portType != port.portType  &&
                    port.portType        != typeof(object) &&
                    startAnchor.portType != typeof(object)
                ){
                    if (startAnchor.direction == Direction.Input  && !HasImplicitConversion(port.portType, startAnchor.portType)) continue;
                    if (startAnchor.direction == Direction.Output && !HasImplicitConversion(startAnchor.portType, port.portType)) continue;
                }

                compatiblePorts.Add(port);
            }

            return compatiblePorts;
        }

        /// <summary>
        ///     判断某类型是否可以隐式转换为某类型
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static bool HasImplicitConversion(Type baseType, Type targetType){
            if (targetType == typeof(float) && (baseType == typeof(Vector2) || baseType == typeof(Vector3) || baseType == typeof(Vector4))) return true;
            return baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                           .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                           .Any(mi => {
                               var pi = mi.GetParameters().FirstOrDefault();
                               return pi != null && pi.ParameterType == baseType;
                           }) || targetType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                           .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                                           .Any(mi => {
                                               var pi = mi.GetParameters().FirstOrDefault();
                                               return pi != null && pi.ParameterType == baseType;
                                           });
        }

        public class GfuSceneGraphViewFactory : UxmlFactory<GfuSceneGraphView, UxmlTraits>{ }

        public class GfuBackground : GridBackground{ }
    }
}