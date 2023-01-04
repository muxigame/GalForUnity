//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuPort.cs
//
//        Created by 半世癫(Roc) at 2021-01-12 21:34:16
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

namespace GalForUnity.Graph.AssetGraph.GFUNode.Base{
    public class GfuPort
#if UNITY_EDITOR
        : Port
#endif
    {
        public GfuPort InputView;

        public Action<GfuPort> OnConnected;
        public Action OnDisConnected;

#if UNITY_EDITOR
        public GfuPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type, string name) : base(portOrientation, portDirection, portCapacity, type){
            var connectorListener = new GfuEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(m_EdgeConnector);
            this.name = name;
        }
#endif

#if UNITY_EDITOR
        /// <summary>
        ///     EditorMethod 设置默认值端口的标签
        /// </summary>
        /// <param name="label">视图标签</param>
        /// <exception cref="NullReferenceException"></exception>
        public virtual void SetDefaultLabel(string label){
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            InputView.portName = label;
        }
#endif
        public virtual object GetDefaultValue(){
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            return InputView.GetDefaultValue();
        }

        public virtual void SetDefaultValue(object value){
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            InputView.SetDefaultValue(value);
        }

#if UNITY_EDITOR
        public virtual void SetDefaultValue<T>(T value){
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            // if (InputView.fieldContainer[0] is BaseField<T> baseField) baseField.value = value;
            throw new NullReferenceException($"BaseField<{typeof(T)}> don't exit!");
        }

        public virtual T GetDefaultValue<T>(){
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            // if (InputView.fieldContainer[0] is BaseField<T> baseField) return baseField.value;
            throw new NullReferenceException($"BaseField<{typeof(T)}> don't exit!");
        }

        public virtual object GetDefaultValue(Type type){
            var type1 = Assembly.GetAssembly(typeof(GameObject)).GetType($"UnityEngine.UIElements.BaseField`1[{type.FullName}]");
            if (type1     == null) throw new NullReferenceException($"BaseField<{type.Name}> don't exit!");
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            // var typeFieldDefault = InputView.fieldContainer[0].GetType();
            // if (typeFieldDefault.IsSubclassOf(type1)) // var changeType = Convert.ChangeType(InputView.fieldContainer[0], typeFieldDefault);
            //     return typeFieldDefault.GetProperty("value")?.GetValue(InputView.fieldContainer[0]);
            return null;
        }

#endif
#if UNITY_EDITOR
        private void ManageTypeClassList(Type type, Action<string> classListAction){
            if (type == null || !string.IsNullOrEmpty(visualClass)) return;
            if (type.IsSubclassOf(typeof(GameObject)))
                classListAction("typeGameObject");
            else
                classListAction(nameof(type) + type.Name);
        }

        public void CheckDefaultValue(GfuPort gfuPort){
            if (!gfuPort.node.expanded) return;
            if (gfuPort.connected || gfuPort.connections.Count() != 0)
                Hide(gfuPort);
            else
                Show(gfuPort);
        }

        public void Hide(GfuPort gfuPort){
            if (gfuPort.InputView != null){
                // gfuPort.InputView.portContainer.style.opacity = 0;
                // gfuPort.InputView.edge.style.opacity = 0;
                // gfuPort.InputView.fieldContainer.SetEnabled(false);
            }
        }

        public void Show(GfuPort gfuPort){
            if (gfuPort.InputView != null){
                // gfuPort.InputView.portContainer.style.opacity = 1;
                // gfuPort.InputView.edge.style.opacity = 1;
                // gfuPort.InputView.fieldContainer.SetEnabled(true);
                // OnDisConnected?.Invoke();
            }
        }

        public override void OnStartEdgeDragging(){
            base.OnStartEdgeDragging();
            CheckDefaultValue(this);
        }


        public new static GfuPort Create<TEdge>(
            Orientation orientation,
            Direction direction,
            Capacity capacity,
            Type type)
            where TEdge : Edge, new(){
            var connectorListener = new GfuEdgeConnectorListener();
            var ele = new GfuPort(orientation, direction, capacity, type,type.Name) {
                m_EdgeConnector = new EdgeConnector<TEdge>(connectorListener)
            };
            ele.AddManipulator(ele.m_EdgeConnector);
            return ele;
        }

        private class GfuEdgeConnectorListener : IEdgeConnectorListener{
            private readonly List<Edge> m_EdgesToCreate;
            private readonly List<GraphElement> m_EdgesToDelete;
            private readonly GraphViewChange m_GraphViewChange;

            public GfuEdgeConnectorListener(){
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }


            public void OnDropOutsidePort(Edge edge, Vector2 position){
                VisualElement node = null;
                if (edge.input != null){
                    node = edge.input.node;
                    if (edge.output?.connected != null){
                        var edgeInput = (GfuPort) edge.input;
                        edgeInput.Show(edgeInput);
                    }
                }

                if (edge.input != null && edge.output != null){
                    var edgeInput = (GfuPort) edge.input;
                    var edgeOutput = (GfuPort) edge.output;
                    edgeInput.Disconnect(edge);
                    edgeInput.OnDisConnected?.Invoke();
                    edgeOutput.Disconnect(edge);
                    edgeOutput.OnDisConnected?.Invoke();
                    return;
                }

                if (edge.output != null) node = edge.output.node;

                // if (node != null){
                //     while (!(node.parent is GfuGraph||node.parent is GfuSceneGraphView)){
                //         node = node.parent;
                //     }
                //     SearchMenuWindowProvider menuWindowProvider = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
                //     if (node.parent is GfuGraph gfuGraph){
                //         if (gfuGraph is PlotFlowGraph){
                //             menuWindowProvider.attributeTargets = NodeAttributeTargets.FlowGraph;
                //         } else if (gfuGraph is PlotItemGraph){
                //             menuWindowProvider.attributeTargets = NodeAttributeTargets.ItemGraph;
                //         }
                //         menuWindowProvider.OnSelectEntryHandler = (gfuGraph).OnMenuSelectEntry;
                //         
                //     }else if (node.parent is GfuSceneGraphView gfuSceneGraphView){
                //         menuWindowProvider.attributeTargets = NodeAttributeTargets.All;
                //         menuWindowProvider.OnSelectEntryHandler =
                //             // gfuSceneGraphView.SceneGraphEditorWindow
                //             new SearchProvider(gfuSceneGraphView, EditorWindow.focusedWindow , new NodeCreationContext(){screenMousePosition = position})
                //                 .OnMenuSelectEntry ;
                //     }
                //     SearchWindow.Open(new SearchWindowContext(position), menuWindowProvider);
                // }
            }


            public void OnDrop(GraphView graphView, Edge edge){
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                    foreach (var connection in edge.input.connections)
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);

                if (edge.output.capacity == Capacity.Single)
                    foreach (var connection in edge.output.connections)
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);

                if (m_EdgesToDelete.Count > 0) graphView.DeleteElements(m_EdgesToDelete);
                var edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null) edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                foreach (var edge1 in edgesToCreate){
                    graphView.AddElement(edge1);
                    edge.input.Connect(edge1);
                    edge.output.Connect(edge1);
                }

                var input = (GfuPort) edge.input;
                var output = (GfuPort) edge.output;
                input.OnConnected?.Invoke(output);
                output.OnConnected?.Invoke(input);
                input.CheckDefaultValue(input);
                output.CheckDefaultValue(output);
            }
        }
#endif
    }
}