//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  PlotItemEditorWindow.cs
//
//        Created by 半世癫(Roc) at 2021-01-26 13:56:22
//
//======================================================================

using System;
using GalForUnity.System;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace GalForUnity.Graph.AssetGraph.Windows{
    [Serializable]
    public class PlotItemEditorWindow
#if UNITY_EDITOR
        : GfuEditorWindow
#endif
    {
#if UNITY_EDITOR
        public override void Init(string path){
            base.Init(path);
            GameSystem.GraphData.currentGfuPlotItemEditorWindow = this;
            GraphView = new PlotItemGraph(this, path);
            rootVisualElement.Add(GraphView);
            AddButton(GraphView);
            titleContent = new GUIContent(GfuLanguage.GfuLanguageInstance.PLOTITEMEDITORWINDOW.Value);
        }


        private void OnDidOpenScene(){
            onSceneChanged = true;
            GraphView?.Clear();
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

        private void OnDisable(){
            // GraphView?.Clear();
            // GraphView = null;
        }

        PlotItemEditorWindow(){
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        public void OnPlayModeChanged(PlayModeStateChange playModeStateChange){
            if (onSceneChanged) return;
            if (playModeStateChange == PlayModeStateChange.ExitingPlayMode){
                EditorApplication.delayCall += () => {
                    GraphView?.Clear();
                    Init(GraphView?.path ?? path);
                };
            }
        }

        [MenuItem("GalForUnity/PlotItem")]
        public static void Open(){
            PlotItemEditorWindow window =
                GameSystem.GraphData.currentGfuPlotItemEditorWindow =
                    GetWindow<PlotItemEditorWindow>(
                        ObjectNames.NicifyVariableName(nameof(PlotItemEditorWindow)));
            window.Init(null);
        }

        private void OnEnable(){
            EditorApplication.delayCall += (() => {
                if (onSceneChanged) return;
                GraphView?.Clear();
                Init(GraphView?.path ?? path);
            });
        }
#endif
    }
}