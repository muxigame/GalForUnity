//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotItemGraph.cs
//
//        Created by 半世癫(Roc) at 2021-01-26 13:57:13
//
//======================================================================

using GalForUnity.Graph.Data;
using GalForUnity.System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GalForUnity.Graph{
    public class PlotItemGraph : GfuGraph{
        public PlotItemGraph(){
            GameSystem.GraphData.CurrentPlotItemGraph = this;
        }

        public PlotItemGraph(Data.GraphData graphData) : base(graphData){
#if UNITY_EDITOR
            GameSystem.GraphData.CurrentPlotItemGraph = this;
            EditorWindow = GameSystem.GraphData.currentGfuPlotItemEditorWindow;
#endif
        }

#if UNITY_EDITOR
        public PlotItemGraph(EditorWindow editorWindow, string path) : base(path){
            GameSystem.GraphData.CurrentPlotItemGraph = this;
            EditorWindow = editorWindow;
            Init();
        }
#endif
    
    }
}
