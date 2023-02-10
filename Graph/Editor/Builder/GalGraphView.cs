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
using GalForUnity.Graph.Editor.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Builder{
    public class GalGraphView : GraphView{

        public Dictionary<long, GfuNode> Nodes = new Dictionary<long, GfuNode>();

        public GalGraphWindow GalGraphWindow;
        public GalGraphView():this(null){}
        public GalGraphView(GalGraphWindow galGraphWindow) : this(null,galGraphWindow){ }

        public override void ClearSelection(){
            base.ClearSelection();
            foreach (var plotNode in nodes.Where(x=>x is PlotNode).Cast<PlotNode>()){
                plotNode.ClearSelection();
            }
        }

        public GalGraphView(GalGraphAsset galGraphAsset,GalGraphWindow galGraphWindow)
        {
            GalGraphWindow = galGraphWindow;
            // _sceneGraphEditorWindow = sceneGraphEditorWindow;
            InitEditorView();
            if (galGraphAsset == null || galGraphAsset.nodes == null || galGraphAsset.nodes.Count == 0){
                var node = Activator.CreateInstance(typeof(MainNode)) as GfuNode;
                if (node == null) return;
                node.OnInit(new Graph.Nodes.MainNode(), this);
                foreach (var _ in node.OnLoadPort(null)) {}
                AddElement(node);
                Nodes.Add(new Guid().GetHashCode(), node);
                return;
            }

            var connection = new HashSet<GfuConnectionAsset>();
            Dictionary<GalPortAsset,GalPort> portMap=new Dictionary<GalPortAsset, GalPort>();
            for (var i = 0; i < galGraphAsset.nodes.Count; ++i){
                GalNodeAsset galNodeAsset = galGraphAsset.nodes[i];
                foreach (var port in InitNode(galNodeAsset)){
                    if(!portMap.ContainsKey(port.Item1))portMap.Add(port.Item1,port.Item2);
                }
                
                if (galNodeAsset.HasInputPort)
                    foreach (var gfuPortAsset in galNodeAsset.inputPort){
                        if (!gfuPortAsset.HasConnection) continue;
                        gfuPortAsset.connections.ForEach(asset => connection.Add(asset));
                    }

                if (galNodeAsset.HasOutputPort)
                    foreach (var gfuPortAsset in galNodeAsset.outputPort){
                        if (!gfuPortAsset.HasConnection) continue;
                        gfuPortAsset.connections.ForEach(asset => connection.Add(asset));
                    }
            }
            foreach (var gfuConnectionAsset in connection) InitConnection(gfuConnectionAsset,portMap);
            foreach (var keyValuePair in Nodes){
                var nodeByInstanceID = galGraphAsset.GetNodeByInstanceID(keyValuePair.Value.instanceID);
                // keyValuePair.Value.InitWithGfuNodeData(nodeByInstanceID, graphData.GetNodeData(keyValuePair.Value.instanceID), null);
            }
        }

        public void InitEditorView(){
            GridBackground gridBackground = new GfuBackground();
            gridBackground.name = "GridBackground";
            Insert(0, gridBackground);
            SetupZoom(0.25f, 2.0f);
            deleteSelection += (x, y) =>
            {
                foreach (var selectable in selection)
                {
                    if (selectable is GfuNode gfuNode)
                    {
                        Nodes.Remove(gfuNode.instanceID);
                    }
                }
                DeleteSelection();
              
            };
            this.StretchToParentSize();
            this.StretchToParentSize();
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public IEnumerable<(GalPortAsset, GalPort)> InitNode(GalNodeAsset galNodeAsset){
            var node = Activator.CreateInstance(galNodeAsset.Type ?? typeof(GfuNode)) as GfuNode;
            if (node == null){
                Debug.LogError("node is null");
                return default;
            }

            node.OnInit(galNodeAsset.runtimeNode, this);

            AddElement(node);
            // if(node is GfuOperationNode gfuOperationNode)gfuOperationNode.GfuInputViews.ForEach(x=>x.);.Init(null);
            Nodes.Add(galNodeAsset.instanceID, node);
            node.instanceID = galNodeAsset.instanceID;
            var rect = node.GetPosition();
            rect.position = galNodeAsset.position;
            node.SetPosition(rect);
            return node.OnLoadPort(galNodeAsset);
        }

        public void InitConnection(GfuConnectionAsset gfuConnectionAsset,Dictionary<GalPortAsset,GalPort> portMap){
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
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
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
                    if (startAnchor.direction == UnityEditor.Experimental.GraphView.Direction.Input  && !HasImplicitConversion(port.portType, startAnchor.portType)) continue;
                    if (startAnchor.direction == UnityEditor.Experimental.GraphView.Direction.Output && !HasImplicitConversion(startAnchor.portType, port.portType)) continue;
                }

                compatiblePorts.Add(port);
            }

            return compatiblePorts;
        }

        public string Save()
        {
            return  this.SerializeGraphElements(nodes);
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

        public class GfuSceneGraphViewFactory : UxmlFactory<GalGraphView, UxmlTraits>{ }

        public class GfuBackground : GridBackground{ }
    }
}