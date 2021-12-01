//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  LinearNode.cs
//
//        Created by 半世癫(Roc) at 2021-02-24 13:57:29
//
//======================================================================

using System;
using System.Collections.Generic;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Operation;
using GalForUnity.System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.GFUNode.Operation{
    [NodeRename("Operation/" + nameof(LinearNode), "提供时间维度的线性操作")]
    [Serializable]
    [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
    public class LinearNode : GfuOperationNode{
        [NodeRename(nameof(From), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort From;

        [NodeRename(nameof(To), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort To;

        [NodeRename(nameof(Time), typeof(float), NodeDirection.Input, NodeCapacity.Single)]
        public GfuPort Time;

        [NodeRename("Vector1", typeof(float), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;

        public bool loop;
        public float durationTime;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitDefaultValuePort<LinearOperation>(otherNodeData);
#if UNITY_EDITOR
            Toggle toggle=new Toggle() {
                label = GfuLanguage.GfuLanguageInstance.LOOP.Value,
                tooltip = "是否在From和To之间循环",
                value = loop,
                labelElement = {
                    style= {
                        minWidth = 0,
                    }
                }
            };
            FloatField floatField=new FloatField() {
                label = GfuLanguage.GfuLanguageInstance.DURATIONTIME.Value,
                tooltip = GfuLanguage.GfuLanguageInstance.DURATIONTIME.Value,
                value = durationTime,
                labelElement = {
                    style= {
                        minWidth = 0,
                    }
                }
            };
            floatField.RegisterValueChangedCallback((evt) => {
                durationTime = evt.newValue;
            });
            toggle.RegisterValueChangedCallback((evt) => {
                loop = evt.newValue;
                if (loop){
                    extensionContainer.Add(floatField);
                } else{
                    extensionContainer.Remove(floatField);
                }
            });
            extensionContainer.Add(toggle);
            if(loop)extensionContainer.Add(floatField);
            RefreshExpandedState();
#endif
            GfuOperation.OnStart += (x) => {
                x.ContainerData = new List<Graph.Operation.Data>() {
                    new Graph.Operation.Data(loop),
                    new Graph.Operation.Data(durationTime)
                };
            };
        }
    }
}