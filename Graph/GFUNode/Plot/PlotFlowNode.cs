//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotFlowNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-09 21:06:23
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Model;
using GalForUnity.Model.Plot;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.GFUNode.Plot{
    [NodeRename("Node/" + nameof(PlotFlowNode))]
    [NodeAttributeUsage(NodeAttributeTargets.FlowGraph)]
    [Obsolete]
    public class PlotFlowNode : EnterNode{
        [NodeRename(nameof(Exit), typeof(RoleData), NodeDirection.Output, NodeCapacity.Multi)]
        public GfuPort Exit;
#if UNITY_EDITOR
        public ObjectField ObjectField;
#endif
        [NonSerialized]
        public PlotFlow PlotFlow;

        public override RoleData Execute(RoleData roleData){
            // Debug.Log(PlotFlow);
            // Debug.Log(PlotFlow.GetInstanceID());
            if (PlotFlow){
                // PlotFlow.ActionModel.actionModelType = ActionModel.ActionModelType.Custom;
                // PlotFlow.ActionModel.customEventNoParam.AddListener(Executed);
                if (PlotFlow.PlotFlowType == PlotFlowType.Graph){
                    PlotFlow.Init();
                }

                // GameSystem.Data.PlotFlowController.ExecutePlotFlow(PlotFlow);
            } else{
                Executed();
            }

            return roleData;
        }

        public override void Executed(){
            if (PlotFlow){
                // PlotFlow.ActionModel.customEventNoParam.RemoveListener(Executed);
                if (!PlotFlow.HasNext()){
                    PlotFlow.startIndex = 0;
                }
            }

            base.Executed();
        }

#if UNITY_EDITOR

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            ObjectField = new ObjectField(GfuLanguage.Parse(nameof(PlotFlow))) {
                value = PlotFlow,
                objectType = typeof(PlotFlow),
                style = {
                    width = 200
                },
                labelElement = {
                    style = {
                        minWidth = 0, fontSize = 12, unityTextAlign = TextAnchor.MiddleLeft
                    }
                }
            };
            ObjectField.RegisterValueChangedCallback((plowFlow) => { PlotFlow = (PlotFlow) plowFlow.newValue; });
            mainContainer.Add(ObjectField);
        }
#endif
        [NodeRename("Node/" + nameof(PlotJumpNode))]
        [NodeAttributeUsage(NodeAttributeTargets.ItemGraph)]
        public sealed class PlotJumpNode : PlotFlowNode{
            public override RoleData Execute(RoleData roleData){
                Executed();
                if (PlotFlow){
                    // PlotFlow.ActionModel.actionModelType = ActionModel.ActionModelType.Custom;
                    // PlotFlow.ActionModel.customEventNoParam.AddListener(Executed);
                    if (PlotFlow.PlotFlowType == PlotFlowType.Graph){
                        PlotFlow.Init();
                    }

                    // GameSystem.Data.PlotFlowController._currentPlotFlow=PlotFlow;
                }

                return roleData;
            }

            // public override void Executed(){
            //     Executed(0);
            // }
        }
    }
}