using System;
using System.Collections.Generic;
using GalForUnity.Core;
using GalForUnity.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GalForUnity.Graph.Editor.Builder
{
    public class PoseAdditionSearchTypeProvider : ScriptableObject, ISearchWindowProvider
    {
        public delegate bool SearchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry,
            SearchWindowContext context);

        public SearchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;

        private RoleAssets roleAssets;

        private PoseAdditionSearchTypeProvider()
        {
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            try
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent(GfuLanguage.GfuLanguageInstance.CHANGETYPE.Value))); //添加了一个一级菜单
                //从程序集中找到GfuNode的所有子类，并且遍历显示到目录当中
                foreach (var pose in roleAssets.pose)
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(pose.name))
                    {
                        level = 1
                    });
                    if (pose is SpritePose spritePose)
                        foreach (var spritePoseBindingPoint in spritePose.bindingPoints)
                        {
                            entries.Add(new SearchTreeGroupEntry(new GUIContent(spritePoseBindingPoint.name))
                            {
                                level = 2
                            });
                            foreach (var spritePoseItem in spritePoseBindingPoint.spritePoseItems)
                                entries.Add(new SearchTreeEntry(new GUIContent(spritePoseItem.name))
                                {
                                    level = 3, userData = string.Concat(pose.name, "/", spritePoseBindingPoint.name, "/", spritePoseItem.name)
                                });
                        }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntryHandler == null) return false;
            return OnSelectEntryHandler(searchTreeEntry, context);
        }

        public static PoseAdditionSearchTypeProvider Create(RoleAssets roleAssets)
        {
            var audioConfigSearchTypeProvider = CreateInstance<PoseAdditionSearchTypeProvider>();
            audioConfigSearchTypeProvider.roleAssets = roleAssets;
            return audioConfigSearchTypeProvider;
        }
    }
}