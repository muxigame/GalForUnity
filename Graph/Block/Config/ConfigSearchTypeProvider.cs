using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalForUnity.System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GalForUnity.Graph.Block.Config{
    public class ConfigSearchTypeProvider : ScriptableObject, ISearchWindowProvider{
        private ConfigSearchTypeProvider(){ }

        private IEnumerable<FieldInfo> _type;
        public static ConfigSearchTypeProvider Create<T>(Func<FieldInfo,bool> match){
            var audioConfigSearchTypeProvider = ScriptableObject.CreateInstance<ConfigSearchTypeProvider>();
            audioConfigSearchTypeProvider._type = GetChildTypes(typeof(T)).Where(match.Invoke);
            return audioConfigSearchTypeProvider;
        }
 
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var entries = new List<SearchTreeEntry>();
            try{
                entries.Add(new SearchTreeGroupEntry(new GUIContent(GfuLanguage.GfuLanguageInstance.CHANGETYPE.Value))); //添加了一个一级菜单
                //从程序集中找到GfuNode的所有子类，并且遍历显示到目录当中
                foreach (var childType in _type){
                    entries.Add(new SearchTreeEntry(new GUIContent(childType.Name)) {
                        level = 1, userData = childType
                    });
                }
            } catch (Exception e){
                Debug.LogError(e);
            }
            return entries;
        }

        public delegate bool SearchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

        public SearchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context){
            if (OnSelectEntryHandler == null){
                return false;
            }
            return OnSelectEntryHandler(searchTreeEntry, context);
        }
        private static FieldInfo[] GetChildTypes(Type parentType){
            return parentType.GetFields();
        }
    }
}