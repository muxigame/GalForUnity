//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuOperationNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-27
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation;
using GalForUnity.Graph.Tool;
using GalForUnity.System;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GalForUnity.Graph.GFUNode.Operation{
    /// <summary>
    /// 链接的默认值
    /// </summary>
    public class GfuOperationNode : GfuNode{
        [NonSerialized] public GfuOperation GfuOperation;
        [NonSerialized] public VisualElement PortDefaultValueContainer;
        [NonSerialized] public List<GfuInputView> GfuInputViews;
        public bool storageExpanded=true;

        public Type ContainerType(int i){ return ContainerType()[i]; }

        public List<Type> ContainerType(){
            List<Type> types = new List<Type>();
            var fieldInfo = GetType().GetFields();
            foreach (var info in fieldInfo){
                if (info.IsNotSerialized) continue;
                if (info.IsStatic) continue;
                if (info.IsInitOnly) continue;
                if (IsSubClassOf(info, typeof(VisualElement))) continue;
#if UNITY_EDITOR
                if (IsSubClassOf(info, typeof(Port))) continue;
#else
                if(IsSubClassOf(info,typeof(GfuPort)))continue;
#endif
                types.Add(info.FieldType);
            }

            return types;
        }

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            GfuInputViews = new List<GfuInputView>();
#if UNITY_EDITOR
            PortDefaultValueContainer = new VisualElement() {
                name = "PortDefaultValueContainer",
                pickingMode = PickingMode.Ignore,
                style = {
                    marginTop = 5,
                    paddingTop = 4,
                    paddingBottom = 4,
                    position = Position.Absolute,
                    top = 36
                }
            };
            RegisterCallback<GeometryChangedEvent>((evt => {
                PortDefaultValueContainer.style.marginLeft = -PortDefaultValueContainer.layout.width;
            }));
            PortDefaultValueContainer.generateVisualContent += (x) => {
                if(Mathf.Abs((PortDefaultValueContainer.style.marginLeft.value.value+PortDefaultValueContainer.layout.width))<1) return;
                PortDefaultValueContainer.style.marginLeft = -PortDefaultValueContainer.contentContainer.layout.width;
            };
#endif
        }
        
#if UNITY_EDITOR
        protected override void ExecuteDefaultAction(EventBase evt){
            if (evt.target == PortDefaultValueContainer && evt.eventTypeId != 10){
                evt.StopImmediatePropagation();
            }
            base.ExecuteDefaultAction(evt);
        }
#endif

        public void InitDefaultValuePort<T>(NodeData nodeData) where T : GfuOperation, new(){
            Type[] types = new Type[InputPortCount];
            for (var i = 0; i < types.Length; i++){
                if (nodeData&&nodeData.InputPort.Count==InputPortCount) types[i] = nodeData.InputPortType(i);
                else types[i] = InputPortType(i);
            }
            GfuOperation = new T() {
                Input = GfuOperationData.CreateInstance(types), Container = GfuOperationData.CreateInstance(ContainerType())
            };
            InitDefaultValuePort(nodeData);
        }
        
        /// <summary>
        /// 初始化默认值视图，传入节点数据则从节点数据解析，null则自动创建
        /// </summary>
        /// <param name="nodeData"></param>
        public void InitDefaultValuePort(NodeData nodeData){
            
#if UNITY_EDITOR
            if(PortDefaultValueContainer != null && PortDefaultValueContainer.childCount >0) PortDefaultValueContainer.Clear();
            if(GfuInputViews != null && GfuInputViews.Count >0) GfuInputViews.Clear();
            for (var i = 0; i < GfuOperation.Input.Data.Count; i++){
                var port = inputContainer[i] as GfuPort;
                if (port != null){
                    if (nodeData){
                        var inputPortType = nodeData.InputPortType(i);
                        if (inputPortType != null) port.portType = inputPortType;//优先将接口类型解析为保存的类型
                    }
                    if(port.portType==null)//如果解析不出来会赋值默认的类型
                        port.portType = GfuOperation.Input.Data[i].Type;
                }
            }
#endif
            var forEachMainPort =
#if UNITY_EDITOR
                inputContainer.Children();
#else
                InputPortData;
#endif
            foreach (var visualElement in forEachMainPort){
#if UNITY_EDITOR
                GfuPort element = visualElement as GfuPort;
                var gfuPort = GfuInputView.Create(element);
                PortDefaultValueContainer.Add(gfuPort.Connect(element));

#else
                var gfuPort = new GfuInputView();
                InitNodeTool.SetDefaultPortValue(nodeData.InputPort[GfuInputViews.Count],gfuPort);
#endif
                GfuInputViews.Add(gfuPort);
#if UNITY_EDITOR
                AutoValueField(gfuPort);
                if (nodeData != null && nodeData.InputPort != null && inputContainer.IndexOf(visualElement) < nodeData.InputPort.Count){ //后面这个比较避免用户修改代码导致端口数量变多而去旧的节点数据中查找新增的端口数据
                    //当节点数值存在的时候，说明这个节点是由初始化系统创建的，从节点数值中获得数值赋值给端口默认值
                    InitNodeTool.SetDefaultPortValue(nodeData.InputPort[inputContainer.IndexOf(visualElement)], gfuPort);
                } else{
                    var portInfo = GetFieldsWithFieldInfo<GfuPort>()[inputContainer.IndexOf(element)];
                    var defaultValueAttribute = portInfo.GetCustomAttribute<DefaultValueAttribute>();
                    var nodeRenameAttribute = portInfo.GetCustomAttribute<NodeRenameAttribute>();
                    //获得端口的默认值和默认类型，如果端口的值为基元类型获得为string，直接赋值给端口默认值
                    //否在则反射获得目标值的构造方法，调用构造方法并赋值给端口默认值
                    //                    Debug.Log(1);
                    if (defaultValueAttribute?.value != null && nodeRenameAttribute?.Type != null){
                        if (defaultValueAttribute.value.Length == 1 && (defaultValueAttribute.value[0].GetType().IsPrimitive || defaultValueAttribute.value[0] is string)){
                            gfuPort.SetDefaultValue(defaultValueAttribute.value[0]);
                        } else if (defaultValueAttribute.value.Length > 1){
                            var constructorInfos = nodeRenameAttribute.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            foreach (var constructorInfo in constructorInfos){
                                if (constructorInfo.GetParameters().Length == defaultValueAttribute.value.Length){
                                    var invoke = constructorInfo.Invoke(defaultValueAttribute.value);
                                    gfuPort.SetDefaultValue(invoke);
                                }
                            }
                        }
                    }
                }
#endif
            }
#if UNITY_EDITOR
            Add(PortDefaultValueContainer);
            CheckToggleCollapse();
#endif
        }
#if UNITY_EDITOR
        protected override void ToggleCollapse(){
            base.ToggleCollapse();
            storageExpanded = expanded;
            CheckToggleCollapse();
        }

        /// <summary>
        /// 检查storageExpanded是否允许展开默认值输入字段
        /// </summary>
        private void CheckToggleCollapse(){
            expanded = storageExpanded;
            if (storageExpanded){
                PortDefaultValueContainer.SetEnabled(true);
                PortDefaultValueContainer.style.opacity = 1;
                for (int i = 0; i < PortDefaultValueContainer.childCount; i++){
                    var gfuInputView = (GfuInputView) PortDefaultValueContainer[i][1];
                    gfuInputView.edge.style.opacity = gfuInputView.port.connected?0:1;
                }
            } else{
                PortDefaultValueContainer.SetEnabled(false);
                PortDefaultValueContainer.style.opacity = 0;
                for (int i = 0; i < PortDefaultValueContainer.childCount; i++){
                    var gfuInputView = (GfuInputView) PortDefaultValueContainer[i][1];
                    gfuInputView.edge.style.opacity = 0;
                }
            }
        }

        /// <summary>
        /// Editor Method
        /// 该算法将默认值视图显示到gfuInputView当中，显示的视图取决于默认视图的端口类型
        /// TODO 期待更好的解决方法，我说的并不是算法模式，我宁愿使用if else展示模式视图也不愿使用算法模式
        /// </summary>
        /// <param name="gfuInputView"></param>
        private void AutoValueField(GfuInputView gfuInputView){
            gfuInputView.portName = "";
            if (gfuInputView?.fieldContainer == null || gfuInputView.portType == null) return;
            VisualElement visualElement = null;
            var fieldContainer = gfuInputView.fieldContainer;
            if (gfuInputView.portType == typeof(float)){
                fieldContainer.Add(visualElement = new FloatField());
                visualElement.style.width = 50;
            } else if (gfuInputView.portType == typeof(int)){
                fieldContainer.Add(visualElement = new IntegerField());
                visualElement.style.width = 50;
            } else if (gfuInputView.portType == typeof(Vector2)){
                fieldContainer.Add(visualElement = new Vector2Field());
                visualElement.style.width = 50 * 2;
                visualElement[0][0][0].style.marginLeft = 3;
            } else if (gfuInputView.portType == typeof(Vector2Int)){
                fieldContainer.Add(visualElement = new Vector2IntField());
                visualElement.style.width = 50 * 2;
                visualElement[0][0][0].style.marginLeft = 3;
            } else if (gfuInputView.portType == typeof(Vector3)){
                fieldContainer.Add(visualElement = new Vector3Field());
                visualElement.style.width = 50 * 3;
                visualElement[0][0][0].style.marginLeft = 3;
            } else if (gfuInputView.portType == typeof(Vector3Int)){
                fieldContainer.Add(visualElement = new Vector3IntField());
                visualElement.style.width = 50 * 3;
                visualElement[0][0][0].style.marginLeft = 3;
            } else if (gfuInputView.portType == typeof(Vector4) || gfuInputView.portType == typeof(Quaternion)){
                fieldContainer.Add(visualElement = new Vector4Field());
                visualElement.style.width = 50 * 4;
                visualElement[0][0][0].style.marginLeft = 3;
            } else if (gfuInputView.portType == typeof(Color)){
                fieldContainer.Add(visualElement = new ColorField() {
                    style = {
                        width = 100
                    }
                });
            } else if (gfuInputView.portType == typeof(bool)){
                fieldContainer.Add(visualElement = new Toggle());
            } else if (gfuInputView.portType.IsSubclassOf(typeof(Object)) || gfuInputView.portType == typeof(Object)){
                fieldContainer.Add(visualElement = new ObjectField() {
                    objectType = gfuInputView.portType,
                    allowSceneObjects = true,
                    style = {
                        width = 130
                    }
                });
                ((ObjectField) visualElement).RegisterValueChangedCallback((evt => { gfuInputView.portCapLit = true; }));
            } else{
                gfuInputView.portName = "Unknown";
            }

            if (visualElement != null){
                visualElement.style.marginRight = visualElement.style.marginLeft = 0;
                visualElement.pickingMode = PickingMode.Position;
                visualElement.SetEnabled(true);
                var action = new Action<ChangeEvent<object>>((evt => {
                    gfuInputView.Value = evt.newValue; //这个方法并没有在实质性的发挥作用，因为获取目标值的功能以及被GetDefaultValue()方法替代，
                }));
                visualElement.GetType().GetMethod("RegisterValueChangedCallback")?.Invoke(visualElement, new object[] {
                    action
                });
            }
        }
#endif
        //
        // public override void HandleEvent(EventBase evt){
        //     Debug.Log(evt.eventTypeId);
        // }
        [Obsolete]
        public void SetDefaultValue<T>(BaseField<T> baseField, T value){ baseField.value = value; }
#if UNITY_EDITOR
        public void SetDefaultLabel(int index, string label){
            var elementAt = PortDefaultValueContainer.ElementAt(index) as Port;
            if (elementAt != null) elementAt.portName = label;
        }

        public void SetDefaultValue<T>(int index, T value){
            if (((GfuInputView) PortDefaultValueContainer[index]).fieldContainer[0] is BaseField<T> baseField) baseField.value = value;
            throw new NullReferenceException($"BaseField<{typeof(T)}> don't exit!");
        }
#endif
        public T GetDefaultValue<T>(GfuInputView portIndex){
#if UNITY_EDITOR
            if (portIndex.fieldContainer[0] is BaseField<T> baseField) return baseField.value;
            throw new NullReferenceException($"BaseField<{typeof(T)}> don't exit!");
#else
            throw new NullReferenceException("this is EditorMethod");
#endif
        }

        /// <summary>
        /// 一个通过自定义委托获得值的方法
        /// </summary>
        /// <param name="portIndex"></param>
        /// <param name="match">委托</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDefaultValue<T>(int portIndex, Func<GfuInputView, T> match){ return match.Invoke(GfuInputViews[portIndex]); }

        /// <summary>
        /// 一个通过自定义委托获得值的方法
        /// </summary>
        /// <param name="portIndex"></param>
        /// <param name="match">委托</param>
        /// <returns></returns>
        public object GetDefaultValue(int portIndex, Func<GfuPort, Type> match){ return match.Invoke(GfuInputViews[portIndex]); }

        /// <summary>
        /// ImportantMethod 从端口的默认视图获得默认值
        /// </summary>
        /// <param name="portIndex"></param>
        /// <returns></returns>
        public virtual object GetDefaultValue(int portIndex){ return GfuInputViews[portIndex].Value; }

        public List<Graph.Operation.Data> GetDefaultValue(){
            List<Graph.Operation.Data> datas = new List<Graph.Operation.Data>();
            for (int i = 0; i < GfuInputViews.Count; i++){
                datas.Add(new Graph.Operation.Data(GetDefaultValue(i)));
            }
            return datas;
        }

        public void PortTypeSync(List<GfuPort> gfuPorts, Type defaultType){
#if UNITY_EDITOR
            foreach (var gfuPort in gfuPorts){
                gfuPort.OnConnected += (x) => {
                    var trueForAll = gfuPorts.TrueForAll((element) => gfuPort == element || !element.connected);
                    foreach (var port in gfuPorts){
                        if (x.portType != defaultType && trueForAll){
                            port.portType = x.portType;
                            if (port.InputView?.portType != null){
                                AutoType(port);
                            }
                        }
                    }
                };
            }
            foreach (var gfuPort in gfuPorts){
                gfuPort.OnDisConnected += () => {
                    var trueForAll = gfuPorts.TrueForAll((element) => gfuPort == element || !element.connected);
                    foreach (var port in gfuPorts){
                        if (trueForAll){
                            port.portType = defaultType;
                            AutoType(port);
                        }
                    }
                };
            }
            foreach (var gfuPort in GetGfuOutPut()){
                gfuPort.portType = defaultType;
            }
#endif
        }

        public void AutoType(GfuPort gfuPort){
#if UNITY_EDITOR
            if(gfuPort?.InputView?.edge == null) return;
            gfuPort.InputView.edge.RemoveFromHierarchy();
            var indexOf = PortDefaultValueContainer.IndexOf(gfuPort.InputView.portContainer);
            gfuPort.InputView.Clear();
            var gfuInputView = GfuInputView.Create(gfuPort);
            gfuInputView.portType=gfuPort.portType;
            PortDefaultValueContainer.RemoveAt(indexOf);
            GfuInputViews.RemoveAt(indexOf);
            PortDefaultValueContainer.Insert(indexOf,gfuInputView.Connect(gfuPort));
            GfuInputViews.Insert(indexOf,gfuInputView);
            AutoValueField(gfuInputView);
            gfuPort.InputView = gfuInputView;
#endif
        }
    }
}