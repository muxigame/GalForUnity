//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DraggableBlock.cs
//
//        Created by 半世癫(Roc) at 2022-10-09 22:44:36
//
//======================================================================
//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DragableBlock.cs Created at 2022-10-09 22:36:40
//
//======================================================================

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public class DraggableBlock : VisualElement, IGalBlock{
        public VisualElement content;
        public Button drag;
        private bool _mouseDown = false;

        public DraggableBlock(){
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/GalForUnity/Graph/Block/DraggableBlock.uxml");
            var templateContainer = visualTree.Instantiate();
            content = templateContainer.Q<VisualElement>("content");
            drag = templateContainer.Q<Button>("dragButton");
            drag.RegisterCallback<MouseMoveEvent>(Callback,TrickleDown.TrickleDown); 
            drag.RegisterCallback<MouseUpEvent>(Up,TrickleDown.TrickleDown);
            drag.RegisterCallback<MouseDownEvent>(Down,TrickleDown.TrickleDown);
            Add(templateContainer);
        }

        private void Up(MouseUpEvent x){ _mouseDown = false; }
        private void Down(MouseDownEvent x){ _mouseDown = true; }

        private void Callback(MouseMoveEvent x){
            Debug.Log(_mouseDown);
            if (_mouseDown){
                // style.left = style.left.value.value + x.mouseDelta.x;
                style.top = style.top.value.value + x.mouseDelta.y;
            }
        }

        ~DraggableBlock(){
            drag.UnregisterCallback<MouseUpEvent>(Up);
            drag.UnregisterCallback<MouseDownEvent>(Down);
            drag.UnregisterCallback<MouseMoveEvent>(Callback);
        }
    }
}
