using System;
using System.Collections;
using System.Collections.Generic;
using GalForUnity.Graph.Editor;
using GalForUnity.Graph.Editor.Block;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Core.Editor.UIElements
{
    public class PoseList : ListView
    {
        public PoseList()
        {
            
        }
        public PoseList(
            IList itemsSource,
            float itemHeight = -1f,
            Func<VisualElement> makeItem = null,
            Action<VisualElement, int> bindItem = null)
            : base(itemsSource, itemHeight, makeItem, bindItem)
        {

        }
        
        public PoseList(List<Pose> list,PoseView poseView,Action<VisualElement> makeItem = null): this(list,22f,
            () =>
            {
                VisualElement visualElement = UxmlHandler.instance.poseBindingItem.Instantiate();
                TextField textField = visualElement.Q<TextField>();
                ObjectField objectField = visualElement.Q<ObjectField>();

                textField.CreateBinder(() =>
                {
                    if (visualElement.userData is SpritePose spritePose)
                        textField.value = spritePose.name;
                }, (x) =>
                {
                    if (visualElement.userData is SpritePose spritePose)
                        spritePose.name = x;
                });
                objectField.CreateBinder(() =>
                {
                    if (visualElement.userData is SpritePose spritePose)
                        objectField.value = spritePose.sprite;
                }, (x) =>
                {
                    if (visualElement.userData is SpritePose spritePose)
                        spritePose.sprite = (Sprite)x;
                });
                visualElement.RegisterCallback<MouseEnterEvent>((_) =>
                {
                    if (visualElement.userData is SpritePose spritePose)
                        poseView.ShowPose(spritePose.sprite);
                });
                visualElement.RegisterCallback<MouseLeaveEvent>((_) =>
                {
                    poseView.RemovePose();
                });
                visualElement.contentContainer[0].Insert(4,new Button(() =>
                {
                    if (visualElement.userData is SpritePose spritePose)
                        CreatePoseWizard.Show(spritePose);
                })
                {
                    text = "edit"
                });
                objectField.objectType = typeof(Sprite);
                makeItem?.Invoke(visualElement);
                return visualElement;
            }, 
            (x,y) =>
            {
                if (list[y] is SpritePose spritePose)
                {

                    x.userData = spritePose;
                    // if (spritePose.sprite) objectField.value = spritePose.sprite;
                    // if (string.IsNullOrEmpty(spritePose.name)&&spritePose.sprite)
                    //     list[y].name = spritePose.sprite.name;
                    // textField.value = list[y].name;
                }
               
            })
        {
        }
        public class PoseListUxmlFactory : UxmlFactory<PoseList, UxmlTraits>
        {
            public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
            {
                var visualElement = base.Create(bag, cc);
                return visualElement;
            }
        }
    }
}
