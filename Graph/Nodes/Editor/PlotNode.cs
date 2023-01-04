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
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Block;
using GalForUnity.Model;
using GalForUnity.System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Nodes.Editor{
    [NodeRename(nameof(PlotNode), "角色检查节点，该节点负责检查在图中流转的角色是否满足要求，如果要求达到，则会跳转满足出口，否则则会跳转不满足出口")]
    // [NodeFieldType(typeof(RoleModel), "roleModel")]
    [NodeAttributeUsage()]
    [NodeType(NodeCode.PlotNode)]
    public sealed class PlotNode : GfuNode{
        public readonly VisualElement content;


        public Runtime.PlotNode runtimeNode;
        public List<GfuPort> Enter=new List<GfuPort>() {
            new GfuPort(Orientation.Horizontal,Direction.Input, Port.Capacity.Multi,typeof(object),nameof(Enter))
        };
        
        public List<GfuPort> Exit=new List<GfuPort>() {
            new GfuPort(Orientation.Horizontal,Direction.Output, Port.Capacity.Multi,typeof(object),nameof(Exit))
        };

        public PlotNode(){
            runtimeNode=new Runtime.PlotNode();
            styleSheets.Add(UxmlHandler.instance.plotNodeUss);
            Add(new Button {
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
                        if (!(Activator.CreateInstance(blockType) is DraggableBlock galBlock)) return false;
                        content.Add(galBlock);
                        runtimeNode.config.value.Add(galBlock.galConfig);
                        return true;
                    };
                    SearchWindow.Open(searchWindowContext, searchTypeProvider);
                })
            });
            Add(content = new VisualElement());
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
                var childTypes = GetChildTypes(typeof(IGalBlock));
                //从程序集中找到GfuNode的所有子类，并且遍历显示到目录当中
                foreach (var childType in childTypes)
                    entries.Add(new SearchTreeEntry(new GUIContent(childType.Name)) {
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
            return assembly.GetTypes().Where(x => parentType.IsAssignableFrom(x));
        }
    }
}