//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  SearchTypeProvider.cs
//
//        Created by 半世癫(Roc) at 2021-12-06 22:10:41
//
//======================================================================

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GalForUnity.System.Address{
    public class SearchTypeProvider : ScriptableObject, ISearchWindowProvider{
        
        /// <summary>
        /// 方法会解析菜单项，其的解析功能依赖于NodeRenameAttribute和NodeAttributeUsage。
        /// </summary>
        /// <param name="context">鬼知道哪里传过来的参数</param>
        /// <returns>一看就返回了一个供菜单系统使用的列表，而且应该是一个多级嵌套的</returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var entries = new List<SearchTreeEntry>();
            try{
                entries.Add(new SearchTreeGroupEntry(new GUIContent(GfuLanguage.GfuLanguageInstance.CHANGETYPE.Value))); //添加了一个一级菜单
                var childTypes = GetChildTypes(typeof(MonoBehaviour));
                //从程序集中找到GfuNode的所有子类，并且遍历显示到目录当中
                foreach (var childType in childTypes){
                    entries.Add(new SearchTreeEntry(new GUIContent(childType.Name)) {
                        level = 1, userData = childType
                    });
                }
            } catch (global::System.Exception e){
                Debug.Log(e);
            }
            return entries;
        }

        public delegate bool SearchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry, //声明一个delegate类
                                                                   SearchWindowContext context);

        public SearchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler; //delegate回调方法

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context){
            if (OnSelectEntryHandler == null){
                return false;
            }
            return OnSelectEntryHandler(searchTreeEntry, context);
        }
        private Type[] GetChildTypes(Type parentType){
            List<Type> lstType = new List<Type>();
            Assembly assem = Assembly.GetCallingAssembly();
            var executingAssembly = Assembly.GetExecutingAssembly();
            var types = executingAssembly.GetTypes();
            var types1 = assem.GetTypes();
            foreach (Type tChild in types){
                if(tChild.IsSubclassOf(parentType))lstType.Add(tChild);
            }
            foreach (Type tChild in types1){
                if(tChild.IsSubclassOf(parentType))
                    if(!lstType.Contains(tChild))
                        lstType.Add(tChild);
            }
            return lstType.ToArray();
        }
    }
}
#endif