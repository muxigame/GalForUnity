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
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Tool;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UIElements;


namespace GalForUnity.Graph.GFUNode.Base{
    public class GfuPort
#if UNITY_EDITOR
        : Port
#endif
    {
        public GfuInputView InputView;


#if UNITY_EDITOR
        protected GfuPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type){ }
#endif

        public Action<GfuPort> OnConnected;
        public Action OnDisConnected;

#if UNITY_EDITOR
        /// <summary>
        /// EditorMethod 设置默认值端口的标签
        /// </summary>
        /// <param name="label">视图标签</param>
        /// <exception cref="NullReferenceException"></exception>
        public virtual void SetDefaultLabel(string label){
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            InputView.portName = label;
        }
#endif

#if UNITY_EDITOR
        public virtual void SetDefaultValue<T>(T value){
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            if (InputView.fieldContainer[0] is BaseField<T> baseField) baseField.value = value;
            throw new NullReferenceException($"BaseField<{typeof(T)}> don't exit!");
        }

        public virtual T GetDefaultValue<T>(){
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            if (InputView.fieldContainer[0] is BaseField<T> baseField) return baseField.value;
            throw new NullReferenceException($"BaseField<{typeof(T)}> don't exit!");
        }

        public virtual object GetDefaultValue(Type type){
            var type1 = Assembly.GetAssembly(typeof(GameObject)).GetType($"UnityEngine.UIElements.BaseField`1[{type.FullName}]");
            if (type1     == null) throw new NullReferenceException($"BaseField<{type.Name}> don't exit!");
            if (InputView == null) throw new NullReferenceException("DefaultInputValue don't exit!");
            var typeFieldDefault = InputView.fieldContainer[0].GetType();
            if (typeFieldDefault.IsSubclassOf(type1)){
                // var changeType = Convert.ChangeType(InputView.fieldContainer[0], typeFieldDefault);
                return typeFieldDefault.GetProperty("value")?.GetValue(InputView.fieldContainer[0]);
            }
            return null;
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
        private void ManageTypeClassList(Type type, Action<string> classListAction){
            if (type == null || !string.IsNullOrEmpty(visualClass)) return;
            if (type.IsSubclassOf(typeof(GameObject)))
                classListAction("typeGameObject");
            else
                classListAction(nameof(type) + type.Name);
        }

        public void CheckDefaultValue(GfuPort gfuPort){
            if(!gfuPort.node.expanded) return;
            if (gfuPort.connected || gfuPort.connections.Count() != 0){
                Hide(gfuPort);
            } else{
                Show(gfuPort);
            }
        }
        
        public void Hide(GfuPort gfuPort){
            if (gfuPort.InputView != null){
                gfuPort.InputView.portContainer.style.opacity = 0;
                gfuPort.InputView.edge.style.opacity = 0;
                gfuPort.InputView.fieldContainer.SetEnabled(false);
            }
        }

        public void Show(GfuPort gfuPort){
            if (gfuPort.InputView != null){
                gfuPort.InputView.portContainer.style.opacity = 1;
                gfuPort.InputView.edge.style.opacity = 1;
                gfuPort.InputView.fieldContainer.SetEnabled(true);
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
            GfuEdgeConnectorListener connectorListener = new GfuEdgeConnectorListener();
            GfuPort ele = new GfuPort(orientation, direction, capacity, type) {
                m_EdgeConnector = (EdgeConnector) new EdgeConnector<TEdge>((IEdgeConnectorListener) connectorListener)
            };
            ele.AddManipulator((IManipulator) ele.m_EdgeConnector);
            return ele;
        }

        private class GfuEdgeConnectorListener : IEdgeConnectorListener{
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public GfuEdgeConnectorListener(){
                this.m_EdgesToCreate = new List<Edge>();
                this.m_EdgesToDelete = new List<GraphElement>();
                this.m_GraphViewChange.edgesToCreate = this.m_EdgesToCreate;
            }


            public void OnDropOutsidePort(Edge edge, UnityEngine.Vector2 position){
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

                if (edge.output != null){
                    node = edge.output.node;
                }

                if (node != null){
                    while (!(node.parent is GfuGraph)){
                        node = node.parent;
                    }

                    var menuWindowProvider = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
                    GfuGraph gfuGraph = (GfuGraph) node.parent;
                    if (gfuGraph is PlotFlowGraph){
                        menuWindowProvider.attributeTargets = NodeAttributeTargets.FlowGraph;
                    } else if (gfuGraph is PlotItemGraph){
                        menuWindowProvider.attributeTargets = NodeAttributeTargets.ItemGraph;
                    }

                    menuWindowProvider.OnSelectEntryHandler = (gfuGraph).OnMenuSelectEntry;
                    SearchWindow.Open(new SearchWindowContext(position), menuWindowProvider);
                }
            }


            public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge){
                this.m_EdgesToCreate.Clear();
                this.m_EdgesToCreate.Add(edge);
                this.m_EdgesToDelete.Clear();
                if (edge.input.capacity == Port.Capacity.Single){
                    foreach (Edge connection in edge.input.connections){
                        if (connection != edge) this.m_EdgesToDelete.Add((GraphElement) connection);
                    }
                }

                if (edge.output.capacity == Port.Capacity.Single){
                    foreach (Edge connection in edge.output.connections){
                        if (connection != edge) this.m_EdgesToDelete.Add((GraphElement) connection);
                    }
                }

                if (this.m_EdgesToDelete.Count > 0) graphView.DeleteElements((IEnumerable<GraphElement>) this.m_EdgesToDelete);
                List<Edge> edgesToCreate = this.m_EdgesToCreate;
                if (graphView.graphViewChanged != null) edgesToCreate = graphView.graphViewChanged(this.m_GraphViewChange).edgesToCreate;
                foreach (Edge edge1 in edgesToCreate){
                    graphView.AddElement((GraphElement) edge1);
                    edge.input.Connect(edge1);
                    edge.output.Connect(edge1);
                }

                GfuPort input = (GfuPort) edge.input;
                GfuPort output = (GfuPort) edge.output;
                input.OnConnected?.Invoke(output);
                output.OnConnected?.Invoke(input);
                input.CheckDefaultValue(input);
                output.CheckDefaultValue(output);
            }
        }
#endif
    }
}