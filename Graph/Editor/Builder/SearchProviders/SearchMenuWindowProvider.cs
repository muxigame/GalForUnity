


using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Framework;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using MainNode = GalForUnity.Graph.Nodes.MainNode;

#if UNITY_EDITOR
#endif

namespace GalForUnity.Graph.Editor.Builder{
#if UNITY_EDITOR
    public class SearchMenuWindowProvider : ScriptableObject, ISearchWindowProvider{
        /// <summary>
        /// 用以确定当前创建应该支持哪些节点
        /// </summary>
        public NodeAttributeTargets attributeTargets = default;

        /// <summary>
        /// 方法会解析菜单项，其的解析功能依赖于NodeRenameAttribute和NodeAttributeUsage。
        /// </summary>
        /// <param name="context">鬼知道哪里传过来的参数</param>
        /// <returns>一看就返回了一个供菜单系统使用的列表，而且应该是一个多级嵌套的</returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var entries = new List<SearchTreeEntry>();
            try{
                entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node"))); //添加了一个一级菜单
                var childTypes = GetChildTypes(typeof(RuntimeNode));
                foreach (var childType in childTypes){
                    if (childType == typeof(MainNode)) continue;
                    if (childType == typeof(OperationNode)) continue;
                    var nodeRenameAttribute = childType.GetCustomAttribute<NodeRenameAttribute>();
                    if (nodeRenameAttribute == null){
                        entries.Add(new SearchTreeEntry(new GUIContent(GfuLanguage.Parse(childType.Name).Trim())) {
                            level = 1,userData = childType
                        });
                        continue;
                    } //没有重命名节点名称的节点直接添加

                    // if ((nodeAttributeUsage.nodeAttributeTargets & attributeTargets) == 0) continue; //过滤没有节点作用目标的不是本窗口的节点
                    if (nodeRenameAttribute.name.Contains("/")){                                     //利用”/“拆分节点名称,然后遍历解析
                        var strings = nodeRenameAttribute.name.Split('/');
                        for (var i = 0; i < strings.Length - 1; i++){
                            if (entries.TrueForAll((x) => x.name != GfuLanguage.Parse(strings[i]))){
                                entries.Add(new SearchTreeGroupEntry(new GUIContent(GfuLanguage.Parse(strings[i]).Trim())) {
                                    level = i + 1,
                                });
                            }
                        }
                        entries.Add(new SearchTreeEntry(new GUIContent(GfuLanguage.Parse(strings[strings.Length - 1]).Trim())) {
                            level = strings.Length, userData = childType
                        });
                    } else{
                        entries.Add(new SearchTreeEntry(new GUIContent(GfuLanguage.Parse(nodeRenameAttribute.name).Trim())){
                            level = 1, userData = childType
                        });
                    }
                }
            } catch (Exception e){
                Debug.LogError(e);
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

            Assembly assem = Assembly.GetAssembly(parentType);

            foreach (Type tChild in assem.GetTypes()){
                Type type = tChild.BaseType;
                while (type != null){
                    if (type == parentType){
                        lstType.Add(tChild);
                    }

                    type = type.BaseType;
                }
            }

            return lstType.ToArray();
        }
    }
#endif
}