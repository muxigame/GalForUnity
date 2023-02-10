using System;
using System.Collections;
using System.Collections.Generic;
using GalForUnity.Graph.Editor.Block;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor.UIElements
{

    public class PoseBindingList : ListView
    {
        private PoseBindingPoint _poseBindingPoint;
        public PoseBindingList(){}
        public PoseBindingList(List<SpritePoseItem> list,PoseBindingPoint poseBindingPoint): this(list,22f,
            () =>
            {
                VisualElement visualElement = UxmlHandler.instance.poseBindingItem.Instantiate();
                TextField textField = visualElement.Q<TextField>();
                ObjectField objectField = visualElement.Q<ObjectField>();
                objectField.objectType = typeof(Sprite);
                textField.RegisterValueChangedCallback(changeEvent=>
                {
                    if(visualElement.userData is SpritePoseItem spritePoseItem) 
                        spritePoseItem.name = changeEvent.newValue;
                });
                objectField.RegisterValueChangedCallback(changeEvent=>
                {
                    if(visualElement.userData is SpritePoseItem spritePoseItem) 
                        spritePoseItem.sprite = (Sprite)changeEvent.newValue;
                });
                visualElement.RegisterCallback<MouseEnterEvent>((_) =>
                {
                    if(visualElement.userData is SpritePoseItem spritePoseItem) 
                        poseBindingPoint.PoseView.ShowAnchor(spritePoseItem.sprite,poseBindingPoint.PoseBindingAnchor);
                });
                visualElement.RegisterCallback<MouseLeaveEvent>((_) =>
                {
                    poseBindingPoint.PoseView.HideAnchor(poseBindingPoint.PoseBindingAnchor);
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
            _poseBindingPoint = poseBindingPoint;
        }
        public PoseBindingList(
            IList itemsSource,
            float itemHeight = -1f,
            Func<VisualElement> makeItem = null,
            Action<VisualElement, int> bindItem = null)
            : base(itemsSource, itemHeight, makeItem, bindItem)
        {
            
        }
        
        public class PoseBindingListUxmlFactory : UxmlFactory<PoseBindingList, UxmlTraits>{
        }
    }
}
