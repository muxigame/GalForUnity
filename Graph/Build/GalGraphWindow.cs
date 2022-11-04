//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GalGraphWindow.cs Created at 2022-09-26 21:47:59
//
//======================================================================

using System;
using System.Linq;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.Tool;
using GalForUnity.Graph.Block;
using GalForUnity.Graph.SceneGraph;
using GalForUnity.InstanceID;
using GalForUnity.System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Build{
    public class GalGraphWindow : EditorWindow{
        [SerializeField] 
        private int instanceID = -1;
        public IGalGraph galGraph;
        public GfuSceneGraphView GraphView;

        private void OnEnable(){
            if (instanceID != -1){
                galGraph = EditorUtility.InstanceIDToObject(instanceID) as IGalGraph;
                GetWindow(false);
            } else{
                GraphView = new GfuSceneGraphView(null, null);
                InitGraph();
            }
        }

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line){
            var path = AssetDatabase.GetAssetPath(instanceID);
            if (path.EndsWith(".asset")){
                var loadAssetAtPath = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (loadAssetAtPath == null || loadAssetAtPath != typeof(SceneGraph.AssetGraph)) return false;
                Open(AssetDatabase.LoadAssetAtPath<SceneGraph.AssetGraph>(path));
                return true;
            }

            return false;
        }

        public static void Open(IGalGraph graph){
            var graphEditorWindows = Resources.FindObjectsOfTypeAll<GalGraphWindow>().ToList().Find(x => x.instanceID == graph.GetInstanceID());
            if (graphEditorWindows != null)
                graphEditorWindows.GetWindow();
            else
                CreateWindow(graph);
        }

        private static void CreateWindow(IGalGraph galGraph){
            var sceneGraphEditorWindow = CreateInstance<GalGraphWindow>();
            sceneGraphEditorWindow.galGraph = galGraph;
            sceneGraphEditorWindow.instanceID = galGraph.GetInstanceID();
            sceneGraphEditorWindow.titleContent = new GUIContent(sceneGraphEditorWindow.name = galGraph.name);
            sceneGraphEditorWindow.GraphView = new GfuSceneGraphView(galGraph.GraphNode, galGraph.GraphData);
            sceneGraphEditorWindow.InitGraph();
            sceneGraphEditorWindow.Show(true);
        }

        private void GetWindow(bool focus = true){
            instanceID = galGraph.GetInstanceID();
            titleContent = new GUIContent(name = galGraph.name);
            GraphView = new GfuSceneGraphView(galGraph.GraphNode, galGraph.GraphData);
            InitGraph();
            if (focus) Focus();
        }

        private void InitGraph(){
            rootVisualElement.Clear();
            
            VisualElement labelFromUxml = UxmlHandler.instance.galGraphWindowUxml.Instantiate();
            labelFromUxml.styleSheets.Add(UxmlHandler.instance.galGraphWindowUss);

            if (GraphView == null) return;

            rootVisualElement.Add(GraphView);
            var menuWindowProvider = CreateInstance<SearchMenuWindowProvider>();
            menuWindowProvider.attributeTargets = NodeAttributeTargets.All;
            GraphView.nodeCreationRequest += context => {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
                menuWindowProvider.OnSelectEntryHandler = new SearchProvider(GraphView, this, context).OnMenuSelectEntry;
            };
            rootVisualElement.Add(new Button(() => { new GalGraph(galGraph).Play(); }) {
                text = GfuLanguage.GfuLanguageInstance.EXECUTE.Value + "(experimental)",
                style = {
                    width = 200, height = 20, right = 0, position = Position.Absolute
                }
            });
            rootVisualElement.Add(new Button(Save) {
                text = GfuLanguage.GfuLanguageInstance.SAVE.Value,
                style = {
                    width = 200,
                    height = 20,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    right = 0,
                    position = Position.Absolute,
                    top = 20
                }
            });
            rootVisualElement.Add(labelFromUxml);
        }

        [MenuItem("Test/OpenGalWindows")]
        private static void CreateGUI(){
            var galGraphWindow = GetWindow<GalGraphWindow>();
            galGraphWindow.GraphView = new GfuSceneGraphView(null, null);
            galGraphWindow.InitGraph();
        }
        

        public void Save(){
            if (galGraph is SceneGraph.SceneGraph sceneGraph) sceneGraph.Save(GraphView);
            if (galGraph is SceneGraph.AssetGraph assetGraph) assetGraph.Save(GraphView);
        }
    }


    /// <summary>
    ///     SearchProvider 是一个用来显示创建节点选项弹窗的辅助类
    /// </summary>
    public class SearchProvider{
        private readonly NodeCreationContext _context;
        private readonly EditorWindow _editorWindow;
        private readonly GfuSceneGraphView _graphView;

        public SearchProvider(GfuSceneGraphView graphView, EditorWindow editorWindow, NodeCreationContext context){
            _graphView = graphView;
            _context = context;
            _editorWindow = editorWindow;
        }

        protected internal bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context){
            var type = searchTreeEntry.userData as Type;
            var node = Activator.CreateInstance(type ?? typeof(GfuNode)) as GfuNode;
            if (node == null) return false;
            _graphView.AddElement(node);
            var windowRoot = _editorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, _context.screenMousePosition - _editorWindow.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(windowMousePosition);
            node.SetPosition(new Rect(graphMousePosition, Vector2.zero)); //将节点移动到鼠标位置
            node.Init(null);
            _graphView.Nodes.Add(GfuInstance.CreateInstanceID(), node);
            return true;
        }
    }
}