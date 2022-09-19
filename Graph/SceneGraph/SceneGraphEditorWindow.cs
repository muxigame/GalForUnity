//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuSceneGraphView.cs
//
//        Created by 半世癫(Roc) at 2022-04-14 23:51:22
//
//======================================================================

using System;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.AssetGraph.Tool;
using GalForUnity.InstanceID;
using GalForUnity.System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.SceneGraph{
    public class SceneGraphEditorWindow : EditorWindow{
        public static SceneGraphEditorWindow instance;
        public GfuGraphAsset gfuGraphAsset;
        public SceneGraph sceneGraph;
        public string guid = "123";
        public GfuSceneGraphView GraphView;

        private void OnEnable(){
            // gfuGraphAsset = EditorWindowHandler.GetGfuGraphAsset(this);
            if (gfuGraphAsset != null){
                var assetPathToGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(gfuGraphAsset));
                if (!GfuSceneGraphHandler.HasRegister(assetPathToGuid)){
                    Debug.LogError("scene graph don't exist in this scene");
                    return;
                }

                sceneGraph = GfuSceneGraphHandler.GetGfuGraphAsset(assetPathToGuid);
                gfuGraphAsset = sceneGraph.graph;
                InitGraph(GraphView, gfuGraphAsset);
                Debug.Log("InitGraph");
            }
        }

        private void OnDisable(){
            // Destroy(this);
            rootVisualElement.Clear();
        }

        // private void OnDestroy(){
        //     DestroyImmediate(this);
        // }


        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line){
            var path = AssetDatabase.GetAssetPath(instanceID);
            if (path.EndsWith(".asset")){
                var loadAssetAtPath = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (loadAssetAtPath == null || loadAssetAtPath != typeof(GfuGraphAsset)) return false;
                var assetAtPath = AssetDatabase.LoadAssetAtPath<GfuGraphAsset>(path);
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (!GfuSceneGraphHandler.HasRegister(guid)){
                    Debug.LogError("scene graph don't exist in this scene");
                    return false;
                }

                var sceneGraphEditorWindow = CreateWindow(GfuSceneGraphHandler.GetGfuGraphAsset(guid));
                return true;
            }

            return false;
        }

        public void Show(GfuGraphAsset paramGraphAsset){
            gfuGraphAsset = paramGraphAsset;
            guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(gfuGraphAsset));
            var gfuSceneGraphView = GraphView = new GfuSceneGraphView(this);
            if (GraphView != null) InitGraph(gfuSceneGraphView, paramGraphAsset);
            Show(true);
        }

        public static SceneGraphEditorWindow CreateWindow(SceneGraph sceneGraph){
            var instance = CreateInstance<SceneGraphEditorWindow>();
            if (instance.titleContent != null) instance.titleContent = new GUIContent(instance.titleContent);
            instance.sceneGraph = sceneGraph;
            instance.Show(sceneGraph.graph);
            Debug.Log(instance.guid);
            return instance;
        }

        public new void Show(){ Show(null); }


        private void InitGraph(GfuSceneGraphView gfuSceneGraphView, GfuGraphAsset otherGraphAsset){
            if (otherGraphAsset == null) gfuGraphAsset = otherGraphAsset = GfuGraphCreator.Create();
            gfuGraphAsset = otherGraphAsset;
            if (gfuSceneGraphView == null) GraphView = gfuSceneGraphView = new GfuSceneGraphView(this);
            GraphView = gfuSceneGraphView;
            rootVisualElement.Add(gfuSceneGraphView);
            var menuWindowProvider = CreateInstance<SearchMenuWindowProvider>();
            menuWindowProvider.attributeTargets = NodeAttributeTargets.All;
            GraphView.nodeCreationRequest += context => {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
                menuWindowProvider.OnSelectEntryHandler = new OnSearchProvider(GraphView, this, context).OnMenuSelectEntry;
            }; //添加右键添加节点的回调  
            rootVisualElement.Add(new Button(() => { new GalGraph(sceneGraph).Play(); }) {
                text = GfuLanguage.GfuLanguageInstance.EXECUTE.Value + "(experimental)",
                style = {
                    width = 200,
                    height = 20,
                    // unityTextAlign = TextAnchor.MiddleCenter,
                    right = 0,
                    position = Position.Absolute
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
        }

        // [MenuItem("GalForUnity/SceneGraph")]
        // public static void Open(){
        //     var window = CreateWindow(GfuGraphCreator.Create());
        // }

        public void Save(){
            if (string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(gfuGraphAsset))) gfuGraphAsset = GfuGraphCreator.Create();
            SceneGraphEditor.Save(this);
            gfuGraphAsset.Save(GraphView);
        }
    }

    public class OnSearchProvider{
        private readonly NodeCreationContext _context;
        private readonly SceneGraphEditorWindow _editorWindow;
        private readonly GfuSceneGraphView _graphView;

        public OnSearchProvider(GfuSceneGraphView graphView, SceneGraphEditorWindow editorWindow, NodeCreationContext context){
            _graphView = graphView;
            _context = context;
            _editorWindow = editorWindow;
        }

        protected internal bool OnMenuSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context){
            var type = searchTreeEntry.userData as Type;
            var node = Activator.CreateInstance(type ?? typeof(GfuNode)) as GfuNode;
            _graphView.AddElement(node);
            var windowRoot = _editorWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, _context.screenMousePosition - _editorWindow.position.position);
            var graphMousePosition = _graphView.contentViewContainer.WorldToLocal(windowMousePosition);
            node.SetPosition(new Rect(graphMousePosition, Vector2.zero)); //将节点移动到鼠标位置
            node.Init(null);
            _graphView.Nodes.Add(GfuInstance.CreateInstanceID(), node);
            // _editorWindow.gfuGraphAsset
            return true;
        }
    }
    public class NodeInspector : VisualElement{ }
}