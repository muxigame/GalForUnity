using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Framework;
using GalForUnity.Graph.Editor.Block;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GalForUnity.Graph.Editor.Nodes{
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
            return assembly.GetTypes().Where(parentType.IsAssignableFrom).Where(x=>x.GetCustomAttribute<NodeEditor>()!=null);
        }
    }
}