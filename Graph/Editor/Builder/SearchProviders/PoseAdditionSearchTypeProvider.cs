using System;
using System.Collections.Generic;
using GalForUnity.Core;
using GalForUnity.Core.Block;
using GalForUnity.Framework;
using GalForUnity.Graph.Editor.Builder.SearchProviders;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Builder
{
    public class PoseAdditionSearchTypeProvider : ScriptableObject, IPreviewSearchWindowProvider
    {
        public delegate bool SearchMenuWindowOnSelectEntryDelegate(SearchTreeEntry searchTreeEntry,
            SearchWindowContext context);

        public SearchMenuWindowOnSelectEntryDelegate OnSelectEntryHandler;

        private GalObject _galObject;
        private Texture2D _combinedTexture;
        private Sprite _combinedSprite;

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
                foreach (var pose in _galObject.pose)
                {
                    entries.Add(new SearchTreeGroupEntry(new GUIContent(pose.name))
                    {
                        level = 1
                    });
                    if (pose is SpritePose spritePose)
                        foreach (var spritePoseBindingPoint in spritePose.anchors)
                        {
                            entries.Add(new SearchTreeGroupEntry(new GUIContent(spritePoseBindingPoint.name))
                            {
                                level = 2
                            });
                            foreach (var spritePoseItem in spritePoseBindingPoint.sprites)
                                entries.Add(new SearchTreeEntry(new GUIContent(spritePoseItem.name))
                                {
                                    level = 3, userData = new PreviewData()
                                    {
                                        Anchor = spritePoseBindingPoint,
                                        pose = pose,
                                        AnchorSprite = spritePoseItem,
                                        poseLocation = new PoseLocation
                                        {
                                            roleName = _galObject.objectName,
                                            poseName = pose.name,
                                            anchorName = spritePoseItem.name,
                                            faceName = spritePoseBindingPoint.name
                                        }
                                    }
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

        public void OnMouseEnter(SearchTreeEntry enter, Rect windowPosition, MouseEnterEvent mouseEnterEvent)
        {
            if (enter.userData == null) return;
            var previewData = (PreviewData)enter.userData;
            var poseSprite = ((SpritePose)previewData.pose).sprite;
            
            if (PreviewWindow.IsOpen()) PreviewWindow.SetPreview(poseSprite);
            else PreviewWindow.Show(poseSprite, windowPosition.position + new Vector2(windowPosition.width, 0));
            PreviewWindow.AddBindPointImage(previewData);
        }

        public void OnMouseLeave(SearchTreeEntry enter, Rect windowPosition, MouseLeaveEvent mouseEnterEvent)
        {
            if(_combinedSprite) Destroy(_combinedSprite);
            if(_combinedTexture) Destroy(_combinedSprite);
        }

        public static PoseAdditionSearchTypeProvider Create(GalObject galObject)
        {
            var audioConfigSearchTypeProvider = CreateInstance<PoseAdditionSearchTypeProvider>();
            audioConfigSearchTypeProvider._galObject = galObject;
            return audioConfigSearchTypeProvider;
        }
    }
}