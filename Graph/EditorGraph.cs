//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  EditorGraph.cs
//
//        Created by 半世癫(Roc) at 2021-11-16 21:46:36
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Tool;
using GalForUnity.InstanceID;
using System.Runtime.Serialization;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace GalForUnity.Graph{
    public class EditorGraph
#if UNITY_EDITOR
        : GraphView,ISerializable
#endif
    {
        public readonly string path;
        [FormerlySerializedAs("GraphData")] public Data.GraphData graphData;
        public GfuNode RootNode => Root;
        
        protected MainNode Root;
#if UNITY_EDITOR
        [NonSerialized]
        public EditorWindow EditorWindow;
        private class GfuBackground : GridBackground{ }
        protected class PasteFlagClass{
            public Node Node;
            public Node pasteFlag;
        }
        protected readonly PasteFlagClass PasteFlag = new PasteFlagClass();
#endif
        
        protected Dictionary<long, GfuNode> CreateNodeDataset;
        protected List<NodeData> NodeDataset;
        
        
        protected EditorGraph(){ }

        /// <summary>
        /// EditorMethod 这个方法里对Editor的判断主要只是为了在编辑模式下显示UI
        /// </summary>
        /// <param name="path"></param>
        protected EditorGraph(string path){ this.path = path; }

        /// <summary>
        /// 需要注意的是，在编辑器模式中，初始化方法会解析所有节点，但是
        /// </summary>
        public void Init(){
            graphData = null;
            //如果路径为空的话，创建一个含有主节点的空图，否则从资源文件中加载Gfu图进行初始化
#if UNITY_EDITOR
            if (path != null){
                graphData = AssetDatabase.LoadAssetAtPath<Data.GraphData>(path);
            } else{
                // throw new NullReferenceException("An attempt was made to initialize the graph using no-parameter initialization, but path was not provided!");
            }
#endif
            Init(graphData);
        }
        /// <summary>
        /// 通过GraphData创建图,如果传入为空则创建默认空图
        /// </summary>
        /// <param name="graphData"></param>
        public virtual void Init(Data.GraphData graphData){
#if UNITY_EDITOR
            if (nodes.First() != null){
                this.nodes.ForEach(RemoveElement);
                this.edges.ForEach(RemoveElement);
            }
            Pastable();
            Menuable();
            AddGridBackGround();
            //按照父级的宽高全屏填充
            this.StretchToParentSize();
            //滚轮缩放
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            //graphview窗口内容的拖动
            this.AddManipulator(new ContentDragger());
            //选中Node移动功能
            this.AddManipulator(new SelectionDragger());
            //多个node框选功能
            this.AddManipulator(new RectangleSelector());
            // this.AddToSelection();//估计可用于显示进度
            if (graphData != null && graphData.Nodes != null && graphData.Nodes.Count != 0){
                viewTransform.scale = new Vector3(graphData.scale, graphData.scale, 1);
                foreach (var nodeData in NodeDataset){
                    if (nodeData.type == typeof(MainNode).ToString()){
                        CreateRoot(nodeData);
                        break;
                    }
                }
                CreateNotConnect(NodeDataset.FindAll((x) => {
                        if (!x) return true;
                        return !CreateNodeDataset.ContainsKey(x.instanceID);
                    }
                ) );
            } else{
                Root = new MainNode();
                AddElement(Root);
            }
#endif
        }
        /// <summary>
        /// 供图内部执行下一个节点的虚方法
        /// </summary>
        /// <param name="nodeObj"></param>
        public virtual void Execute(GfuNode nodeObj){
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying){
                Debug.LogError(GfuLanguage.ParseLog("It is not allowed to execute the diagram without playing, please run the scene first"));
                return;
            }
#endif

#if UNITY_EDITOR
            LoadGraph();
            // AddToSelection(nodeObj); //在编辑模式是，显示剧情进度
#endif
        }

        /// <summary>
        /// EditorMethod 向剧情流编辑窗口添加剧情图
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void LoadGraph(){
#if UNITY_EDITOR
            if (this is PlotItemGraph){
                // if(GameSystem.GraphData.currentGfuPlotItemEditorWindow)
                // if (GameSystem.GraphData.currentGfuPlotItemEditorWindow.rootVisualElement[0] != this){
                //     GameSystem.GraphData.currentGfuPlotItemEditorWindow.rootVisualElement.Clear();
                //     GameSystem.GraphData.currentGfuPlotItemEditorWindow.rootVisualElement.Add(this);
                // }
            } else if (this is PlotFlowGraph){
                // if(GameSystem.GraphData.currentGfuPlotFlowEditorWindow)
                // if (GameSystem.GraphData.currentGfuPlotFlowEditorWindow.rootVisualElement[0] != this){
                //     GameSystem.GraphData.currentGfuPlotFlowEditorWindow.rootVisualElement.Clear();
                //     GameSystem.GraphData.currentGfuPlotFlowEditorWindow.rootVisualElement.Add(this);
                // }
            } else{
                throw new ArgumentException("Unknown Graph!");
            }
#endif
        }

        /// <summary>
        /// 添加背景网格
        /// </summary>
        protected void AddGridBackGround(){
#if UNITY_EDITOR
            //添加网格背景
            GridBackground gridBackground = new GfuBackground();
            //直接使用GridBackground 不会出现网格 
            //GridBackground gridBackground = new GridBackground();
            gridBackground.name = "GridBackground";
            Insert(0, gridBackground);
            //设置背景缩放范围
            this.SetupZoom(0.25f, 2.0f);
            //扩展大小与父对象相同
            this.StretchToParentSize();
#endif
        }

        protected void Menuable(){
#if UNITY_EDITOR
            if (EditorWindow) EditorWindow.minSize = new Vector2(600, 300);
            var menuWindowProvider = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
            if (this is PlotFlowGraph){
                menuWindowProvider.attributeTargets = NodeAttributeTargets.FlowGraph;
            } else if (this is PlotItemGraph){
                menuWindowProvider.attributeTargets = NodeAttributeTargets.ItemGraph;
            }

            menuWindowProvider.OnSelectEntryHandler = OnMenuSelectEntry;
            nodeCreationRequest += context => { SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider); }; //添加右键添加节点的回调  
#endif
        }
#if UNITY_EDITOR
        /// <summary>
        /// 菜单项选中时的回调，创建一个新节点
        /// </summary>
        /// <param name="searchTreeEntry"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected internal bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context){
            var type = searchTreeEntry.userData as Type;
            GfuNode node = Activator.CreateInstance(type ?? typeof(GfuNode)) as GfuNode;
            AddElement(node);
            var position1 = node.GetPosition();
            position1.position = context.screenMousePosition;
            var mousePosition = EditorWindow.rootVisualElement.ChangeCoordinatesTo(EditorWindow.rootVisualElement.parent,
                context.screenMousePosition - EditorWindow.position.position);
            var graphMousePosition = contentViewContainer.WorldToLocal(mousePosition);
            node.GfuGraph = (GfuGraph) this;
            var nodeData = ScriptableObject.CreateInstance<NodeData>();
            nodeData.GraphData = graphData;
            node.Init(nodeData);
            position1.position = graphMousePosition;
            node.SetPosition(position1); //将节点移动到鼠标位置
            return true;
        }
#endif
        protected void Pastable(){
#if UNITY_EDITOR
            canPasteSerializedData = (x) => {
                return true;
            };
            unserializeAndPaste = (x, y) => {
                var strings = y.Split('|');
                foreach (var s in strings){
                    var type = Type.GetType(s);
                    if (type != null && type.IsSubclassOf(typeof(Node))){
                        var addNode = AddNode(type);
                        if (selection.Count > 0){
                            var visualElement = (Node) selection[0];
                            Rect position1;
                            if (PasteFlag.pasteFlag != null && PasteFlag.pasteFlag == visualElement){
                                position1 = PasteFlag.Node.GetPosition();
                            } else{
                                position1 = visualElement.GetPosition();
                            }
                            PasteFlag.pasteFlag = visualElement;
                            PasteFlag.Node = addNode;
                            position1.x += 20;
                            position1.y += 20;
                            addNode.SetPosition(position1);
                        }
                    }
                }
            };
            serializeGraphElements = (x) => {
                string ele = "";
                foreach (var element in x){
                    ele += element.GetType().ToString() + "|";
                }

                var substring = ele.Substring(0, ele.Length - 1);
                return substring;
            };
#endif
        }

        public GfuNode AddNode(NodeData nodeData){
            GfuNode gfuNode = (GfuNode) Activator.CreateInstance(Type.GetType(nodeData.type) ?? typeof(GfuNode));
            CreateNodeDataset.Add(nodeData.instanceID, gfuNode);
            gfuNode.GfuGraph = (GfuGraph) this; //所以单靠EditorGraph是创建不了图的,EditorGraph只是为了和Editor解耦.
            gfuNode.nodeData = nodeData;
#if UNITY_EDITOR
            AddElement(gfuNode);
#endif
            return gfuNode;
        }

        public GfuNode AddNode(Type type){
            GfuNode gfuNode = (GfuNode) Activator.CreateInstance(type ?? typeof(GfuNode));
            gfuNode.Init(null);
            CreateNodeDataset.Add(GfuInstance.CreateInstanceID(), gfuNode);
            gfuNode.GfuGraph = (GfuGraph) this;
#if UNITY_EDITOR
            AddElement(gfuNode);
#endif
            return gfuNode;
        }
#if UNITY_EDITOR
        /// <summary>
        /// 节点间连接判断的方法，不允许节点自身相连，同时也不允许不同类型的节点相连
        /// </summary>
        /// <param name="startAnchor"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter){
            var compatiblePorts = new List<Port>();
            foreach (var port in ports.ToList()){
                if(!port.enabledSelf) continue;
                if (startAnchor.node      == port.node      ||
                    startAnchor.direction == port.direction ||
                    port is GfuInputView                    ||
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
#endif
        /// <summary>
        /// 判断某类型是否可以隐式转换为某类型
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        private static bool HasImplicitConversion(Type baseType, Type targetType){
            if (targetType == typeof(float) && (baseType == typeof(Vector2) || baseType == typeof(Vector3) || baseType == typeof(Vector4))) return true;
            // if (baseType == typeof(float) &&(targetType == typeof(Vector2) || targetType == typeof(Vector3) || targetType == typeof(Vector4))) return true;
            // if (targetType == typeof(Color) &&(baseType == typeof(Vector4))) return true;
            return baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                           .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                           .Any(mi => {
                               ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                               return pi != null && pi.ParameterType == baseType;
                           }) || targetType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                           .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                                           .Any(mi => {
                                               ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                                               return pi != null && pi.ParameterType == baseType;
                                           });
        }

        /// <summary>
        /// 在编辑器模式中，会递归渲染所有的节点，但是在游戏中，仅给root节点赋值，然后通过根节点执行剧情流
        /// </summary>
        /// <param name="nodeData"></param>
        protected void CreateRoot(NodeData nodeData){
#if UNITY_EDITOR
            GfuNode gfuNode = (GfuNode) Activator.CreateInstance(Type.GetType(nodeData.type) ?? typeof(GfuNode));
            AddElement(gfuNode);
            CreateNodeDataset.Add(nodeData.instanceID, gfuNode);
            CreateSub(nodeData.OutputPort, gfuNode);
#endif
        }

        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <param name="portData"></param>
        /// <param name="gfuNode"></param>
        protected void CreateSub(List<PortData> portData, GfuNode gfuNode){
#if UNITY_EDITOR
            for (var i = 0; i < portData.Count; i++){
                if (portData[i]?.connections != null){
                    foreach (var portDataConnection in portData[i].connections){
                        var nodeData = portDataConnection.Input;
                        if(!nodeData) continue;
                        if (CreateNodeDataset.ContainsKey(nodeData.instanceID)){
                            if (gfuNode.outputContainer.ElementAt(i) is Port elementAt){
                                var connectTo = elementAt.ConnectTo(CreateNodeDataset[nodeData.instanceID].inputContainer.ElementAt(portDataConnection.inputIndex) as Port);
                                AddElement(connectTo);
                            }
                        } else{
                            GfuNode gfuNodesub = (GfuNode) Activator.CreateInstance(Type.GetType(nodeData.type) ?? typeof(GfuNode));
                            AddElement(gfuNodesub);
                            CreateNodeDataset.Add(nodeData.instanceID, gfuNodesub);
                            if (gfuNode.outputContainer.ElementAt(i) is Port elementAt){
                                var connectTo = elementAt.ConnectTo(gfuNodesub.inputContainer.ElementAt(portDataConnection.inputIndex) as Port);
                                AddElement(connectTo);
                            }

                            CreateSub(nodeData.OutputPort, gfuNodesub);
                        }
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 绘制没有和根节点连接的节点，实际上这些节点不会运行
        /// </summary>
        /// <param name="portData"></param>
        protected void CreateNotConnect(List<NodeData> portData){
#if UNITY_EDITOR
            foreach (var nodeData in portData){
                CreateNodeWithNodeData(nodeData);
            }
#endif
        }

        /// <summary>
        /// 使用节点数据创建节点
        /// </summary>
        /// <param name="nodeData"></param>
        protected void CreateNodeWithNodeData(NodeData nodeData){
#if UNITY_EDITOR
            if(!nodeData) return;
            if (CreateNodeDataset.ContainsKey(nodeData.instanceID)){
                return;
            }
            GfuNode gfuNode = (GfuNode) Activator.CreateInstance(Type.GetType(nodeData.type) ?? typeof(GfuNode));
            CreateNodeDataset.Add(nodeData.instanceID, gfuNode);
            AddElement(gfuNode);
            CreateSub(nodeData.OutputPort, gfuNode); //绘制不和主节点相连的节点
#endif
        }
        /// <summary>
        /// 仅在编辑器中能够保存剧情图
        /// </summary>
        public virtual void Save(){
#if UNITY_EDITOR
            string path = string.Format(this.path ?? "Assets/" + Random.Range(10000, 99999) + ".asset");
            List<Node> listNode = nodes.ToList();
            Data.GraphData graphData = null;
            if (this is PlotFlowGraph){
                graphData = ScriptableObject.CreateInstance<Data.Property.PlotFlowGraphData>();
            } else if (this is PlotItemGraph){
                graphData = ScriptableObject.CreateInstance<Data.Property.PlotItemGraphData>();
            }

            if (graphData == null) throw new ArgumentException("A graph type that is not allowed!");
            // graphData.Nodes = listNode;
            if (!this.graphData || !AssetDatabase.Contains(this.graphData)){
                graphData.Parse(listNode, scale, graphData.InstanceID);
                AssetDatabase.CreateAsset(graphData, path);
                graphData.Save(path);
                AssetDatabase.ImportAsset(path);
            } else{
                this.graphData.Delete(path);
                this.graphData.Parse(listNode, scale, this.graphData.InstanceID);
                this.graphData.Save(path);
                AssetDatabase.Refresh();
            }
            EditorSceneManager.SaveScene(SceneManager.GetSceneByName(SceneManager.GetActiveScene().name));
#endif
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context){
            var fieldInfos = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            foreach (var fieldInfo in fieldInfos){
                if (fieldInfo.GetValue(this)!=null){
                    fieldInfo.SetValue(this,fieldInfo.GetValue(this));
                }
            }
        }
        
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected EditorGraph(SerializationInfo info, StreamingContext context){

            Type type = GetType();
            var serializationInfoEnumerator = info.GetEnumerator();
            while (serializationInfoEnumerator.MoveNext()){
                type.GetField(serializationInfoEnumerator.Name).SetValue(this,serializationInfoEnumerator.Value);
            }
            Type basetype = this.GetType().BaseType;
            MemberInfo[] mi = FormatterServices.GetSerializableMembers(basetype, context);
            for (int i = 0; i < mi.Length; i++){
                //由于AddValue不能添加重名值，为了避免子类变量名与基类变量名相同，将基类序列化的变量名加上基类类名
                info.AddValue(basetype.FullName + "+" + mi[i].Name, ((FieldInfo)mi[i]).GetValue(this));
            }
        }
    }
}