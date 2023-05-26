using System;
using System.Collections;
using System.Collections.Generic;
using GalForUnity.Graph.Editor.Block;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor
{

    public class AnchorListView : ListView
    {
        private AnchorFoldout m_AnchorFoldout;
        public AnchorListView(){}
        public AnchorListView(List<AnchorSprite> list,AnchorFoldout anchorFoldout): this(list,22f,
            () =>
            {
                VisualElement visualElement = UxmlHandler.instance.poseBindingItem.Instantiate();
                TextField textField = visualElement.Q<TextField>();
                ObjectField objectField = visualElement.Q<ObjectField>();
                objectField.objectType = typeof(Sprite);
                textField.RegisterValueChangedCallback(changeEvent=>
                {
                    if(visualElement.userData is AnchorSprite spritePoseItem) 
                        spritePoseItem.name = changeEvent.newValue;
                });
                objectField.RegisterValueChangedCallback(changeEvent=>
                {
                    if(visualElement.userData is AnchorSprite spritePoseItem) 
                        spritePoseItem.sprite = (Sprite)changeEvent.newValue;
                });
                visualElement.RegisterCallback<MouseEnterEvent>((_) =>
                {
                    if(visualElement.userData is AnchorSprite spritePoseItem) 
                        anchorFoldout.PoseView.ShowAnchor(spritePoseItem.sprite,anchorFoldout.AnchorElement);
                });
                visualElement.RegisterCallback<MouseLeaveEvent>((_) =>
                {
                    anchorFoldout.PoseView.HideAnchor(anchorFoldout.AnchorElement);
                });
                return visualElement;
            }, 
            (x,y) =>
            {
                TextField textField = x.Q<TextField>();
                ObjectField objectField = x.Q<ObjectField>();
                x.userData = list[y];
                if (list[y].sprite) objectField.value = list[y].sprite;
                if (string.IsNullOrEmpty(list[y].name)&&list[y].sprite)
                    list[y].name = list[y].sprite.name;
                textField.value = list[y].name;
            })
        {
            m_AnchorFoldout = anchorFoldout;
        }
        public AnchorListView(
            IList itemsSource,
            float itemHeight = -1f,
            Func<VisualElement> makeItem = null,
            Action<VisualElement, int> bindItem = null)
            : base(itemsSource, itemHeight, makeItem, bindItem)
        {
            
        }
        
        public class PoseBindingListUxmlFactory : UxmlFactory<AnchorListView, UxmlTraits>{
        }
    }
}
