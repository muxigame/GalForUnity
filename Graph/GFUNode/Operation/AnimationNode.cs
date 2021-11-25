//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  AnimationNode.cs
//
//        Created by 半世癫(Roc) at 2021-02-08 13:37:31
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;


namespace GalForUnity.Graph.GFUNode.Operation{
    [NodeRename("Operation/" + nameof(AnimationNode), "动画节点保存着动画剪辑")]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class AnimationNode : GfuOperationNode{
        [NodeRename(nameof(Animation), typeof(AnimationClip), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;

        public Animation Animation;
        public AnimationClip AnimationClip;


        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<AnimationOperation>(otherNodeData);
            GfuOperation.OnInit = (x) => {
                x.Input.AutoOver = true;
                x.ContainerData[0].value = Animation;
                x.ContainerData[1].value = AnimationClip;
            };

#if UNITY_EDITOR
            ObjectField objectField = new ObjectField() {
                label = GfuLanguage.Parse(nameof(Animation)),
                objectType = typeof(AnimationClip),
                allowSceneObjects = true,
                value = AnimationClip ? (Object) AnimationClip : Animation,
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft
                    }
                }
            };
            objectField.RegisterValueChangedCallback(evt => {
                if (evt.newValue is AnimationClip animationClip){
                    AnimationClip = animationClip;
                    Animation = null;
                } else{
                    AnimationClip = null;
                    Animation = (Animation) evt.newValue;
                }
            });
            style.width = 150;
            mainContainer.Add(objectField);
#endif
            // mainContainer.Add(toggle);
        }
    }
}