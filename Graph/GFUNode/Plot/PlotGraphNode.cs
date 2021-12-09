//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotGraphNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-16 20:56:27
//
//======================================================================

using GalForUnity.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Model;
using GalForUnity.System;
using GalForUnity.System.Archive.Data;
using GalForUnity.System.Event;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using NotImplementedException = System.NotImplementedException;


namespace GalForUnity.Graph.GFUNode.Plot{
    [NodeRename("Node/" + nameof(PlotGraphNode))]
    [NodeFieldType(typeof(Data.GraphData), "PlotGraph")]
    [NodeAttributeUsage(NodeAttributeTargets.FlowGraph)]
    public class PlotGraphNode : ObjectFieldNode<Data.GraphData>,INodeSaveable{
        [NodeRename(nameof(Exit), typeof(RoleData), NodeDirection.Output, NodeCapacity.Single)]
        public GfuPort Exit;

        public enum GraphSource{
            Assets,
            Scene
        }

        public GfuSceneGraph SceneGraph;
        public GraphSource graphSource;

        private Data.GraphData _graphData;

        public override RoleData Execute(RoleData roleData){
            switch (graphSource){
                case GraphSource.Assets:
                    _graphData = objectReference;
                    if (objectReference is Data.Property.PlotFlowGraphData plotFlowGraphData){
                        new PlotFlowGraph(plotFlowGraphData).Play(this);
                    } else if (objectReference is Data.Property.PlotItemGraphData plotItemGraphData){
                        new PlotItemGraph(plotItemGraphData).Play(this);
                    }

                    break;
                case GraphSource.Scene:
                    _graphData = SceneGraph.graphData;
                    if (SceneGraph.graphData is Data.Property.PlotFlowGraphData plotFlowGraphData1){
                        new PlotFlowGraph(plotFlowGraphData1).Play(this);
                    } else if (SceneGraph.graphData is Data.Property.PlotItemGraphData plotItemGraphData1){
                        new PlotItemGraph(plotItemGraphData1).Play(this);
                    }

                    break;
            }

            EventCenter.GetInstance().OnGraphExecutedEvent += OnSubGraphEnd;


            if (_graphData && !_graphData.isPlay){
                return base.Execute(roleData);
            }

            return roleData;
        }


        private void OnSubGraphEnd(GfuGraph arg0){
            if (_graphData == arg0.graphData&&arg0.IsCaller(this)){
                base.Executed();
            }
        }

#if UNITY_EDITOR
        public ObjectField SceneGraphField;
        public EnumField EnumField;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            EnumField = new EnumField() {
                label = GfuLanguage.Parse("PlotGraphSource"),
                style = {
                    marginTop = 5
                },
                labelElement = {
                    style = {
                        flexBasis = 0,
                        minWidth = 65,
                        width = 65,
                        fontSize = 12,
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                }
            };
            EnumField.Init(graphSource);
            mainContainer.Add(EnumField);
            InitObject(out SceneGraphField, SceneGraph);
            mainContainer.Remove(SceneGraphField);
            mainContainer.Remove(ObjectField);
            EnumField.RegisterValueChangedCallback(evt => { ShowField((GraphSource) evt.newValue); });
            style.width = 200f;
            ShowField(graphSource);
            RegisterValueChangedCallback(this);
        }

        public void ShowField(GraphSource graphSource){
            switch (graphSource){
                case GraphSource.Assets:
                    if (!mainContainer.Contains(ObjectField)) mainContainer.Add(ObjectField);
                    if (mainContainer.Contains(SceneGraphField)) mainContainer.Remove(SceneGraphField);
                    break;
                case GraphSource.Scene:
                    if (!mainContainer.Contains(SceneGraphField)) mainContainer.Add(SceneGraphField);
                    if (mainContainer.Contains(ObjectField)) mainContainer.Remove(ObjectField);
                    break;
            }
        }

        public override void Save(){
            objectReference = (Data.GraphData) ObjectField.value;
            SceneGraph = (GfuSceneGraph) SceneGraphField.value;
            graphSource = (GraphSource) EnumField.value;
        }
#endif
        public void Recover(GfuGraph gfuGraph){
            _graphData = gfuGraph.graphData;
            EventCenter.GetInstance().OnGraphExecutedEvent += OnSubGraphEnd;
        }
    }
}