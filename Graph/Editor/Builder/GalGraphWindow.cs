

using System;
using System.Linq;
using System.Threading.Tasks;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Framework;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Editor.Block;
using GalForUnity.Graph.Editor.Nodes;
using GalForUnity.Graph.Nodes;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Builder{
    public class GalGraphWindow : EditorWindow
    {

        [SerializeField] private int instanceID = -1;

        public IGalGraph galGraph;
        public GalGraphView GraphView;

        private void OnEnable(){
            if (instanceID != -1){
                galGraph = EditorUtility.InstanceIDToObject(instanceID) as IGalGraph;
                GetWindow(false);
            } else{
                InitGraph(galGraph?.GraphNode);
            }
        }

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line){
            var path = AssetDatabase.GetAssetPath(instanceID);
            if (path.EndsWith(".asset")){
                var loadAssetAtPath = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (loadAssetAtPath == null || loadAssetAtPath != typeof(AssetGraph)) return false;
                Open(AssetDatabase.LoadAssetAtPath<AssetGraph>(path));
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
            sceneGraphEditorWindow.InitGraph(galGraph.GraphNode);
            sceneGraphEditorWindow.Show(true);
        }

        private void GetWindow(bool focus = true){
            instanceID = galGraph.GetInstanceID();
            titleContent = new GUIContent(name = galGraph.name);
            InitGraph(galGraph.GraphNode);
            if (focus) Focus();
        }

        private void InitGraph(GalGraphAsset graphAsset){
            rootVisualElement.Clear();
            try{
                GraphView = new GalGraphView(graphAsset,this);
            } catch (Exception e){
                Debug.LogError(e);
            } finally{
                VisualElement labelFromUxml = UxmlHandler.instance.galGraphWindowUxml.Instantiate();
                labelFromUxml.styleSheets.Add(UxmlHandler.instance.galGraphWindowUss);
                if (GraphView != null){
                    rootVisualElement.Add(GraphView);
                    var menuWindowProvider = CreateInstance<SearchMenuWindowProvider>();
                    menuWindowProvider.attributeTargets = NodeAttributeTargets.All;
                    GraphView.nodeCreationRequest += context => {
                        PreviewSearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
                        menuWindowProvider.OnSelectEntryHandler = new SearchProvider(GraphView, this, context).OnMenuSelectEntry;
                    };
                    rootVisualElement.Add(new Button(async () =>
                    {
                        EditorApplication.isPlaying = true;
                        while (!Application.isPlaying){
                            await Task.Yield();
                        }
                        new GalGraph(galGraph).Play();
                    }) {
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
                
            }

        }

        [MenuItem("Test/OpenGalWindows")]
        private static void CreateGUI(){
            var galGraphWindow = GetWindow<GalGraphWindow>();
            galGraphWindow.InitGraph(null);
        }


        public void Save(){
            if (galGraph is SceneGraph sceneGraph){
                Undo.RecordObject(sceneGraph,"");
                sceneGraph.Save(GraphView);
            }

            if (galGraph is AssetGraph assetGraph){
                Undo.RecordObject(assetGraph,"");
                assetGraph.Save(GraphView);
            }
               
        }
    }


    /// <summary>
    ///     SearchProvider 是一个用来显示创建节点选项弹窗的辅助类
    /// </summary>
    public class SearchProvider{
        private readonly NodeCreationContext _context;
        private readonly EditorWindow _editorWindow;
        private readonly GalGraphView _graphView;

        public SearchProvider(GalGraphView graphView, EditorWindow editorWindow, NodeCreationContext context){
            _graphView = graphView;
            _context = context;
            _editorWindow = editorWindow;
        }

        protected internal bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context){
            var type = searchTreeEntry.userData as Type;
            var runtimeNode = Activator.CreateInstance(type ?? typeof(RuntimeNode)) as RuntimeNode;
            var editorNode = Activator.CreateInstance( NodeEditor.GetEditor(type) ?? typeof(CustomNode)) as GfuNode;
            if (editorNode == null) return false;
            _graphView.AddElement(editorNode);
            var windowRoot = _editorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, _context.screenMousePosition - _editorWindow.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(windowMousePosition);
            editorNode.SetPosition(new Rect(graphMousePosition, Vector2.zero)); //将节点移动到鼠标位置
            editorNode.OnInit(runtimeNode, _graphView);
            editorNode.OnInitPort();
            _graphView.Nodes.Add(GetHashCode(), editorNode);
            return true;
        }
    }
}