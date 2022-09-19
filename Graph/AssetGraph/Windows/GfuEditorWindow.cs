//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuEditorWindow.cs
//
//        Created by 半世癫(Roc) at 2021-01-09 20:59:21
//
//======================================================================


using GalForUnity.System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
#endif


namespace GalForUnity.Graph.AssetGraph.Windows{

    public class GfuEditorWindow
#if UNITY_EDITOR
        : EditorWindow
#endif
    {
#if UNITY_EDITOR
        public GfuGraph GraphView;
        public string path;
        public bool onSceneChanged=false;
        
        public virtual void Init(string path){
            this.path = path;
            GraphView?.Clear();
            rootVisualElement?.Clear();
            autoRepaintOnSceneChange = false;
            onSceneChanged = false;
        }
        

        protected void AddButton(GfuGraph gfuGraph){
            rootVisualElement.Add(new Button(() => {
                gfuGraph.Execute(gfuGraph.RootNode);
            }){
                text = GfuLanguage.GfuLanguageInstance.EXECUTE.Value+"(experimental)",
                style = {
                    width = 200,
                    height = 20,
                    // unityTextAlign = TextAnchor.MiddleCenter,
                    right = 0,
                    position = Position.Absolute
                }
            });
            rootVisualElement.Add(new Button(gfuGraph.Save){
                text =GfuLanguage.GfuLanguageInstance.SAVE.Value,
                style = {
                    width = 200,
                    height = 20,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    right = 0,
                    position = Position.Absolute,
                    top = 20
                }
            });
        }
        
        
        private void OnDidOpenScene(){
            // EditorSceneManager.sceneOpened += (s, y) => { };
            GraphView.Clear();
        }

        private void OnDisable(){
            GraphView = null;
        }
        
        public void OnAddedAsTab(){
            Init(GraphView?.path??path);
        }
        
        public void Open(string path){
            //GfuEditorWindow window = GetWindow<GfuEditorWindow>(ObjectNames.NicifyVariableName(nameof(GfuEditorWindow)));
            this.Init(path);
        }
        
#endif
    }

}
