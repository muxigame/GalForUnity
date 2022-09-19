//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuInputView.cs
//
//        Created by 半世癫(Roc) at 2021-01-29
//
//======================================================================

using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
#endif

namespace GalForUnity.Graph.AssetGraph.GFUNode.Base{
    public class GfuInputView : GfuPort{
        // public class Factory : UxmlFactory<GfuInputView> {}

#if UNITY_EDITOR
        public Edge edge;
        public GfuPort port;
        public VisualElement portContainer;
        public VisualElement fieldContainer;
#endif

        private object _value;

        public object Value{
            set{
                Type = value?.GetType();
#if UNITY_EDITOR
                fieldContainer?[0]?.GetType().GetProperty("value")?.SetValue(fieldContainer?[0], value);
#endif
                _value = value;
            }
            get{
#if UNITY_EDITOR
                _value= fieldContainer?[0]?.GetType().GetProperty("value")?.GetValue(fieldContainer?[0]);
#endif
                return _value;
            }
        }

        public Type Type;

#if UNITY_EDITOR
        public override void OnStartEdgeDragging(){ highlight = false; }
        public override void OnStopEdgeDragging(){ highlight = false; }
#endif

#if UNITY_EDITOR
        protected GfuInputView(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) :
            base(portOrientation, portDirection, portCapacity, type){
            // SetEnabled(false);
            style.height = 22;
            focusable = false;
            pickingMode = PickingMode.Ignore;
            RegisterCallback<GeometryChangedEvent>((evt => {
                // portContainer.style.alignSelf = Align.FlexEnd;//取消该段注释能使端口默认值字段自适应宽度，而非对其
            }));
        }
        
#else
        public GfuInputView(object value){
            Value = value;
            Type = value?.GetType();
        }
        public GfuInputView(){
        }
#endif

// #if UNITY_EDITOR
//         /// <summary>
//         /// 这段对于事件系统的响应，响应了当节点被删除时，同时删除虚连接的子Edge
//         /// 同时响应了当节点收拢时的预览视图消失的功能
//         /// </summary>
//         /// <param name="evt"></param>
//         protected override void ExecuteDefaultAction(EventBase evt){
//             if (evt.eventTypeId == 3){
//                 edge.parent.Remove(edge);
//             }
//
//             Debug.Log(evt.eventTypeId);
//             edge.visible = portContainer.visible = port.visible;
//             base.ExecuteDefaultAction(evt);
//         }
// #endif


        /// <summary>
        /// 使端口默认值视图连接端口，并返回视图
        /// </summary>
        /// <param name="port">要连接的端口</param>
        /// <returns>返回处理得到的视图</returns>
        public VisualElement Connect(GfuPort port){
#if UNITY_EDITOR
            port.InputView = this;
            this.port = port;
            portType = port.portType;
            edge = new Edge {
                // title="edge",
                output = this,
                input = port,
                pickingMode = PickingMode.Ignore,
                focusable = false,
                // capabilities = Capabilities.Deletable,
            };
            Add(edge);
            //修正原先port即被绑定的port出现显示鼠标进入端口小圆点不显示异常的问题
            //TODO 可以适当查明原因，并用更好的方式修复
            port.RegisterCallback<MouseEnterEvent>((evt => {
                if (!port.HitTest(evt.mousePosition)){
                    port.portCapLit = true;
                }
            }));
            port.RegisterCallback<MouseOutEvent>((evt => {
                if (!port.HitTest(evt.mousePosition)){
                    port.portCapLit = false;
                }
            }));
            port.node.RegisterCallback<DetachFromPanelEvent>(evt => {
                edge.RemoveFromHierarchy();//TODO 从层级面板删除Edge
            });
            //初始化容器结构并返回portContainer
            portContainer = new VisualElement() {
                name = "portContainer",
                focusable = false,
                pickingMode = PickingMode.Ignore,
                style = {
                    backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.8f),
                    borderBottomLeftRadius = 2,
                    borderBottomRightRadius = 2,
                    borderTopLeftRadius = 2,
                    borderTopRightRadius = 2,
                    marginTop = 1,
                    marginBottom = 1,
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.FlexEnd,
                    alignItems = Align.Center
                }
            };
            portContainer.Add(fieldContainer = new VisualElement() {
                style = {
                    paddingLeft = 10
                }
            });
            RefreshEdgeVisible();
            SetEnabled(false);
            portContainer.Add(this);
            highlight = false;
            portCapLit = true;
            return portContainer;
#else
            return null;
#endif
        }

        public void RefreshEdgeVisible(){
#if UNITY_EDITOR
            if (port.connected){
                portContainer.style.opacity = 0f;
                edge.style.opacity = 0;
                SetEnabled(false);
                fieldContainer.SetEnabled(false);
            } else{
                portContainer.style.opacity = 1f;
                edge.style.opacity = 1;
            }
#endif
        }

        public static GfuInputView Create(object port){
#if UNITY_EDITOR
            GfuInputViewListener connectorListener = new GfuInputViewListener();
            GfuInputView ele = new GfuInputView(((GfuPort) port).orientation, Direction.Output, Capacity.Single, ((GfuPort) port).portType) {
                m_EdgeConnector = (EdgeConnector) new EdgeConnector<Edge>((IEdgeConnectorListener) connectorListener)
            };
            ele.AddManipulator((IManipulator) ele.m_EdgeConnector);
            return ele;

#else
            return new GfuInputView(port);
#endif
        }


#if UNITY_EDITOR
        private class GfuInputViewListener : IEdgeConnectorListener{
            private GraphViewChange m_GraphViewChange;
            private List<Edge> m_EdgesToCreate;
            private List<GraphElement> m_EdgesToDelete;

            public GfuInputViewListener(){
                this.m_EdgesToCreate = new List<Edge>();
                this.m_EdgesToDelete = new List<GraphElement>();
                this.m_GraphViewChange.edgesToCreate = this.m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, UnityEngine.Vector2 position){ }
            public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge){ }
        }
#endif
        public void SetDefaultValue<T>(BaseField<T> baseField, T value){ baseField.value = value; }

#if UNITY_EDITOR
        /// <summary>
        /// EditorMethod 设置默认值端口的表情
        /// </summary>
        /// <param name="label"></param>
        public override void SetDefaultLabel(string label){ portName = label; }

        /// <summary>
        /// EditorMethod 设置默认值端口的值
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="NullReferenceException"></exception>
        public override void SetDefaultValue<T>(T value){
            if (fieldContainer[0] is BaseField<T> baseField) baseField.value = value;
            throw new NullReferenceException($"BaseField<{typeof(T)}> don't exit!");
        }

        /// <summary>
        /// EditorMethod 设置默认值端口的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public override T GetDefaultValue<T>(){
            if (fieldContainer[0] is BaseField<T> baseField) return baseField.value;
            throw new NullReferenceException($"BaseField<{typeof(T)}> don't exit!");
        }

#endif

        public override object GetDefaultValue(){ return Value; }
        public override void SetDefaultValue(object value){ Value = value; }
    }
}