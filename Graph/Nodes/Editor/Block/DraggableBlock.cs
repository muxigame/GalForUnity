//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DraggableBlock.cs Created at 2022-10-09 22:36:40
//
//======================================================================

using System.Linq;
using GalForUnity.External;
using GalForUnity.Graph.AssetGraph.GFUNode;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Nodes.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public class DraggableBlock : VisualElement, IGalBlock{
        private readonly VisualElement _dragContainer;
        private bool _mouseDown;
        public VisualElement content;
        public Button delete;
        public Button drag;
        public IGalConfig galConfig;
        public PlotNode plotNode;

        public DraggableBlock(){
            var templateContainer = UxmlHandler.instance.draggableBlockUxml.Instantiate();
            _dragContainer = new VisualElement {
                name = "DragContainer"
            };
            _dragContainer.Add(templateContainer);
            content = templateContainer.Q<VisualElement>("content");
            drag = templateContainer.Q<Button>("dragButton");
            delete = templateContainer.Q<Button>("deleteDragButton");
            delete.clickable = new Clickable(() => { parent.Remove(this); });
            drag.RegisterCallback<MouseMoveEvent>(Callback, TrickleDown.TrickleDown);
            drag.RegisterCallback<MouseUpEvent>(Up, TrickleDown.TrickleDown);
            drag.RegisterCallback<MouseDownEvent>(Down, TrickleDown.TrickleDown);
            Add(_dragContainer);
        }

        private void Up(MouseUpEvent x){
            _mouseDown = false;
            if (_dragContainer.style.top != 0) _dragContainer.style.top = 0;
            var configValue = plotNode.runtimeNode.config.value;
            configValue.Remove(galConfig);
            configValue.Insert(parent.IndexOf(this), galConfig);
            PortProcess(galConfig);
        }

        private void Down(MouseDownEvent x){ _mouseDown = true; }

        private void Callback(MouseMoveEvent x){
            if (_mouseDown){
                _dragContainer.style.top = _dragContainer.style.top.value.value + CreateLast();
                _dragContainer.style.top = _dragContainer.style.top.value.value + x.mouseDelta.y;
            }
        }

        private float CreateLast(){
            var node = parent;
            var dragPositionY = _dragContainer.worldBound.center.y;
            var min = node.Children().Min(x => new SelectCap<float, VisualElement>(Mathf.Abs(x.worldBound.center.y - dragPositionY), x));
            if (min.Obj == this) return 0;
            var index = node.IndexOf(min.Obj);
            var mIndex = node.IndexOf(this);
            if (mIndex < index){
                if (_dragContainer.worldBound.center.y < worldBound.y + min.Obj.worldBound.size.y) return 0;
            } else{
                if (_dragContainer.worldBound.center.y > min.Obj.worldBound.y + _dragContainer.worldBound.size.y) return 0;
            }

            node.Remove(this);
            node.Insert(index, this);
            return mIndex < index ? -min.Obj.worldBound.size.y : min.Obj.worldBound.size.y;
        }
        private void PortProcess(IGalConfig changedGalConfig){
            var index = plotNode.runtimeNode.config.value.IndexOf(changedGalConfig);
            var hashSet = plotNode.runtimeNode.portalizedData.value[index];
            hashSet.Clear();
            content.Q<BlockPortUxml>().Query<GfuTogglePort>().ForEach(x => hashSet.Add(x.name));
        }
        
        ~DraggableBlock(){
            drag.UnregisterCallback<MouseUpEvent>(Up);
            drag.UnregisterCallback<MouseDownEvent>(Down);
            drag.UnregisterCallback<MouseMoveEvent>(Callback);
        }
    }
}