//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotFlowEditorWindow.cs
//
//        Created by 半世癫(Roc) at 2021-01-17 13:53:15
//
//======================================================================


using MUX.Mono;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using GalForUnity.System;
using UnityEditor;
using UnityEngine;
#endif

namespace GalForUnity.Graph.Windows{
    
    public class PlotFlowEditorWindow:GfuEditorWindow{
        
#if UNITY_EDITOR
        public override void Init(string path){
            base.Init(path);
            GameSystem.GraphData.currentGfuPlotFlowEditorWindow = this;
            GraphView = new PlotFlowGraph(this,path);
            rootVisualElement.Add(GraphView);
            AddButton(GraphView);
            
            titleContent=new GUIContent(GfuLanguage.GfuLanguageInstance.PLOTFLOWEDITORWINDOW.Value);
            // EditorWindow.FocusWindowIfItsOpen(GetType());
            
        }
        private void OnDidOpenScene(){
            GraphView?.Clear();
            onSceneChanged = true;
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }
        private void OnDisable(){
            // GraphView?.Clear();
            // GraphView = null;
        }

        PlotFlowEditorWindow(){
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }
        public void OnPlayModeChanged(PlayModeStateChange playModeStateChange){
            if (playModeStateChange == PlayModeStateChange.ExitingPlayMode){
                EditorApplication.delayCall += () => {
                    GraphView?.Clear();
                    Init(GraphView?.path ?? path);
                };
            }
        }
        [MenuItem("GalForUnity/PlotFlow")]
        public static void Open()
        {
            PlotFlowEditorWindow window = GameSystem.GraphData.currentGfuPlotFlowEditorWindow = GetWindow<PlotFlowEditorWindow>(
                ObjectNames.NicifyVariableName(nameof(PlotFlowEditorWindow)));
            window.Init(null);
        }

        private void OnEnable(){
            EditorApplication.delayCall+=(()=>{
                if (onSceneChanged) return;
                GraphView?.Clear();
                Init(GraphView?.path ?? path);
            }); 
        }
#endif
    }
}
