//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotFlowGraph.cs
//
//        Created by 半世癫(Roc) at 2021-01-09 20:57:55
//
//======================================================================


using GalForUnity.Graph.Data;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GalForUnity.Graph{
    public class PlotFlowGraph : GfuGraph {
        public PlotFlowGraph(){
            GameSystem.GraphData.CurrentPlotFlowGraph = this;
        }
        public PlotFlowGraph(Data.GraphData graphData) : base(graphData){
#if UNITY_EDITOR
            GameSystem.GraphData.CurrentPlotFlowGraph = this;
            EditorWindow = GameSystem.GraphData.currentGfuPlotFlowEditorWindow;
#endif
        }
        
#if UNITY_EDITOR
        public PlotFlowGraph(EditorWindow editorWindow, string path) : base(path){
            GameSystem.GraphData.CurrentPlotFlowGraph = this;
            EditorWindow = editorWindow;
            Init();
        }
#endif
    }

}
