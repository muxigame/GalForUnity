//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ProbabilityNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-16 19:27:00
//
//======================================================================


using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using GalForUnity.Graph.AssetGraph.Data;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Model;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace GalForUnity.Graph.AssetGraph.GFUNode.Logic{
    [NodeRename("Logic/" + nameof(ProbabilityNode), "通过进度条来控制对应剧情的概率")]
    [Serializable]
    [NodeType(NodeCode.ProbabilityNode)]
    [NodeAttributeUsage(NodeAttributeTargets.FlowGraph | NodeAttributeTargets.ItemGraph)]
    public class ProbabilityNode : DoubleExitNode{
        public float probability = 100;

#if UNITY_EDITOR
        public BaseSlider<float> Slider;
        public FloatField ExitOneProbability;
        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            Slider = new Slider() {
                value = probability, highValue = 100, lowValue = 0, tooltip = "当概率满时必定走分支一，当概率为零是必定走分支二，否则走一个对应概率的随机端口",
            };
            ExitOneProbability = new FloatField() {
                label = GfuLanguage.Parse(nameof(ExitOneProbability)),
                value = probability,
                tooltip = "当概率满时必定走分支一，当概率为零是必定走分支二，否则走一个对应概率的随机端口",
                style = { },
                labelElement = {
                    style = {
                        minWidth = 0, fontSize = 12, unityTextAlign = TextAnchor.MiddleLeft
                    }
                },
            };
            Slider.RegisterValueChangedCallback((x) => {
                if (probability != x.newValue){
                    probability = ExitOneProbability.value = x.newValue;
                }
            });
            Slider.SetValueWithoutNotify(probability);
            ExitOneProbability.RegisterValueChangedCallback((x) => {
                if (probability != x.newValue){
                    probability = Slider.value = x.newValue;
                }
            });
            mainContainer.Add(Slider);
            mainContainer.Add(ExitOneProbability);
        }
#endif
        /// <summary>
        /// 如果获得的随机概率小于条件概率，去第二条端口
        /// </summary>
        /// <param name="roleData"></param>
        /// <returns></returns>
        public override RoleData Execute(RoleData roleData){
            float value = Random.Range(0f, 100f);
            if (value <= probability){
                return base.Execute(roleData);
            }

            return Executed(1, roleData);
        }


#if UNITY_EDITOR
        public override void Save(){ probability = ExitOneProbability.value; }
#endif
    }
}