using System;
using System.Collections;
using System.Collections.Generic;
using GalForUnity.Graph.Block;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Scripts.Editor
{
    public class PoseSpriteItem
    {
        public string Name;
        public Sprite Sprite;
    }
    public class PoseBindingList : ListView
    {
        private PoseBindingPoint _poseBindingPoint;
        public PoseBindingList(){}
        public PoseBindingList(List<PoseSpriteItem> list,PoseBindingPoint poseBindingPoint): this(list,22f,
            () => UxmlHandler.instance.poseBindingItem.Instantiate(), 
            (x,y) =>
            {
                TextField textField = x.Q<TextField>();
                ObjectField objectField = x.Q<ObjectField>();
                objectField.objectType = typeof(Sprite);
                textField.RegisterValueChangedCallback(changeEvent=>
                {
                    list[y].Name = changeEvent.newValue;
                });
                objectField.RegisterValueChangedCallback(changeEvent=>
                {
                    list[y].Sprite = (Sprite)changeEvent.newValue;
                });
                x.RegisterCallback<MouseEnterEvent>((_) =>
                {
                    poseBindingPoint.PoseView.ShowAnchor(list[y].Sprite,poseBindingPoint.PoseBindingAnchor);
                });
                x.RegisterCallback<MouseLeaveEvent>((_) =>
                {
                    poseBindingPoint.PoseView.HideAnchor(poseBindingPoint.PoseBindingAnchor);
                });
                if (list[y].Sprite) objectField.value = list[y].Sprite;
                if (string.IsNullOrEmpty(list[y].Name)&&list[y].Sprite)
                    list[y].Name = list[y].Sprite.name;
                textField.value = list[y].Name;
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
