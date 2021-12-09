//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GraphSystem.cs
//
//        Created by 半世癫(Roc) at 2021-01-16 23:34:24
//
//======================================================================

using System;
using System.IO;
using GalForUnity.Attributes;
using GalForUnity.Graph;
using GalForUnity.Graph.Data;
using GalForUnity.Graph.Data.Property;
using GalForUnity.Graph.GFUNode;
using GalForUnity.Graph.GFUNode.Base;
using GalForUnity.Graph.Windows;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using GraphData = GalForUnity.Graph.Data.GraphData;
using PlotFlowGraphData = GalForUnity.Graph.Data.Property.PlotFlowGraphData;
using PlotItemGraphData = GalForUnity.Graph.Data.Property.PlotItemGraphData;

namespace GalForUnity.System{
    [Serializable]
    [ExecuteInEditMode]
    public class GraphSystem : MonoBehaviour
    {
        private void OnValidate(){
            // hideFlags = HideFlags.NotEditable;
        }

        [Rename(nameof(GraphData))]
        [SerializeField]
        public GraphSystemData GraphData=new GraphSystemData();
        // public static GraphSystemData staticGraphData =new GraphSystemData();


        /// <summary>
        /// 图系统的数据
        /// </summary>
        [Serializable]
        public class GraphSystemData{
            
            [NonSerialized] public GfuNode currentExecutingPlotFlowNode;
            [NonSerialized] public GfuNode currentExecutingPlotItemNode;

            [FormerlySerializedAs("CurrentPlotFlowGraphData")] [Rename(nameof(currentPlotFlowGraphData))]
            public GraphData currentPlotFlowGraphData;

            [FormerlySerializedAs("CurrentPlotItemGraphData")] [Rename(nameof(currentPlotItemGraphData))]
            public GraphData currentPlotItemGraphData;
            
            [Rename(nameof(currentGraph))]
            public GfuGraph currentGraph;
            
#if UNITY_EDITOR
            [Rename(nameof(currentGfuPlotFlowEditorWindow))]
            public PlotFlowEditorWindow currentGfuPlotFlowEditorWindow;

            [Rename(nameof(currentGfuPlotItemEditorWindow))]
            public PlotItemEditorWindow currentGfuPlotItemEditorWindow;
#endif
        }
#if UNITY_EDITOR
        [UnityEditor.Callbacks.OnOpenAssetAttribute(2)]
        public static bool OpenGraph(int instanceID, int line){
            var instanceIDToObject = EditorUtility.InstanceIDToObject(instanceID);
            string strFilePath = AssetDatabase.GetAssetPath(instanceIDToObject);
            string strFileName = Directory.GetParent(Application.dataPath) + "/" + strFilePath;
            if (strFileName.EndsWith(".asset")) //文件扩展名类型
            {
                // GfuEditorWindow gfuEditorWindow=null;
                if (instanceIDToObject is PlotFlowGraphData plotFlowGraph){
                    PlotFlowEditorWindow gfuEditorWindow=null;
                    if(!GameSystem.GraphData.currentPlotFlowGraphData){
                        gfuEditorWindow = GameSystem.GraphData.currentGfuPlotFlowEditorWindow = EditorWindow.GetWindow<PlotFlowEditorWindow>(ObjectNames.NicifyVariableName(nameof(PlotFlowEditorWindow)));
                    }

                    if (gfuEditorWindow != null){
                        gfuEditorWindow.Open(strFilePath);
                        GameSystem.GraphData.currentGraph = gfuEditorWindow.GraphView;
                    }
                    
                    return true;
                }
                if (instanceIDToObject is PlotItemGraphData plotItemGraphData){
                    PlotItemEditorWindow gfuEditorWindow=null;
                    if(!GameSystem.GraphData.currentPlotItemGraphData){
                        gfuEditorWindow = GameSystem.GraphData.currentGfuPlotItemEditorWindow = EditorWindow.GetWindow<PlotItemEditorWindow>(ObjectNames.NicifyVariableName(nameof(PlotItemEditorWindow)));
                    }
                    if (gfuEditorWindow != null){
                        gfuEditorWindow.Open(strFilePath);
                        GameSystem.GraphData.currentGraph = gfuEditorWindow.GraphView;
                    }
                    return true;
                }
            }
            return false;
        }
#endif
    }
}
