//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotNode.cs Created at 2022-09-26 22:16:45
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Block;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Build;
using GalForUnity.Graph.SceneGraph;
using GalForUnity.System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Direction = GalForUnity.Graph.SceneGraph.Direction;
using Orientation = GalForUnity.Graph.SceneGraph.Orientation;

namespace GalForUnity.Graph.Nodes.Editor{
    [NodeRename(nameof(PlotNode), "角色检查节点，该节点负责检查在图中流转的角色是否满足要求，如果要求达到，则会跳转满足出口，否则则会跳转不满足出口")]
    [NodeType(NodeCode.PlotNode)]
    [NodeEditor(typeof(Runtime.PlotNode))]
    public sealed class PlotNode : GfuNode{
        public readonly VisualElement content;

        public override List<GfuPort> Enter{ get; }= new List<GfuPort>{
            new GfuPort(Orientation.Horizontal, Direction.Input, Capacity.Multi, typeof(GfuNodeAsset), nameof(Enter))
        };

        public override List<GfuPort> Exit { get; }= new List<GfuPort>{
            new GfuPort(Orientation.Horizontal, Direction.Output, Capacity.Multi, typeof(GfuNodeAsset), nameof(Exit))
        };


        public Runtime.PlotNode runtimeNode;

        public PlotNode(){ Add(content = new VisualElement()); }

        internal override IEnumerable<(GfuPort port, GfuPortAsset gfuPortAsset)> OnSavePort(GfuNodeAsset gfuNodeAsset){
            foreach (var port in base.OnSavePort(gfuNodeAsset)) yield return port;
            foreach (var draggableBlockEditor in content.Query<DraggableBlockEditor>().ToList()){
                foreach (var blockPort in draggableBlockEditor.OnSavePort(gfuNodeAsset)) yield return blockPort;
            }
        }

        internal override IEnumerable<(GfuPortAsset gfuPortAsset, GfuPort port)> OnLoadPort(GfuNodeAsset gfuNodeAsset){
            foreach (var port in base.OnLoadPort(gfuNodeAsset)) yield return port;
            foreach (var draggableBlockEditor in content.Query<DraggableBlockEditor>().ToList()){
                foreach (var blockPort in draggableBlockEditor.OnLoadPort(gfuNodeAsset)) yield return blockPort;
            }
        }

        public override void OnInit(RuntimeNode otherRuntimeNode){
            base.OnInit(otherRuntimeNode);
            runtimeNode = (Runtime.PlotNode) otherRuntimeNode;
            styleSheets.Add(UxmlHandler.instance.plotNodeUss);
            Add(new Button{
                name = "AddBlockButton",
                text = "AddBlock",
                clickable = new Clickable(() => {
                    var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + this.LocalToWorld(transform.position));
                    var searchTypeProvider = ScriptableObject.CreateInstance<PlotBlockSearchProvider>();
                    searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                        if (!(x.userData is Type blockType)) return false;
                        // 下面是反射工厂的方法
                        // var assembly = Assembly.Load("com.muxigame.galforunity");
                        // var blockFactory =  assembly.GetTypes().First(x=>x.IsSubclassOf(typeof(UxmlFactory<,>).MakeGenericType(xUserData, typeof(UxmlTraits))));
                        // if (!(Activator.CreateInstance(blockFactory) is IUxmlFactory uxmlFactoryInstance)) return false;
                        var nodeEditor = blockType.GetCustomAttribute<NodeEditor>();
                        if (nodeEditor == null) return false;
                        if (!(Activator.CreateInstance(nodeEditor.Type) is IGalBlock block)) return false;
                        if (!(Activator.CreateInstance(blockType, this, block) is DraggableBlockEditor galBlock)) return false;
                        content.Add(galBlock);
                        runtimeNode.config.Add(galBlock.GalBlock);
                        return true;
                    };
                    SearchWindow.Open(searchWindowContext, searchTypeProvider);
                })
            });
            Debug.Assert(runtimeNode?.config != null, "runtimeNode?.config == null");
            runtimeNode?.config?.ForEach(x => {
                var type = x?.GetType();
                if (!(Activator.CreateInstance(NodeEditor.GetEditor(type), this, x) is DraggableBlockEditor galBlock)) return;
                content.Add(galBlock);
            });
        }

        public class PlotNodeUxmlFactory : UxmlFactory<PlotNode, UxmlTraits>{ }
    }

    public class PlotBlockSearchProvider : ScriptableObject, ISearchWindowProvider{
        public delegate bool SearchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

        public SearchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var entries = new List<SearchTreeEntry>();
            try{
                entries.Add(new SearchTreeGroupEntry(new GUIContent(GfuLanguage.GfuLanguageInstance.CHANGETYPE.Value))); //添加了一个一级菜单
                var childTypes = GetChildTypes(typeof(DraggableBlockEditor));
                //从程序集中找到GfuNode的所有子类，并且遍历显示到目录当中
                foreach (var childType in childTypes)
                    entries.Add(new SearchTreeEntry(new GUIContent(childType.Name)){
                        level = 1, userData = childType
                    });
            } catch (Exception e){
                Debug.LogError(e);
            }

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context){
            if (OnSelectEntryHandler == null) return false;
            return OnSelectEntryHandler(searchTreeEntry, context);
        }

        private IEnumerable<Type> GetChildTypes(Type parentType){
            var assembly = Assembly.Load("com.muxigame.galforunity");
            return assembly.GetTypes().Where(parentType.IsAssignableFrom);
        }
    }
}