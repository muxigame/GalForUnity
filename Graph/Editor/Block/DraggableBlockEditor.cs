

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.Core.Block;
using GalForUnity.Core.External;
using GalForUnity.Graph.Editor.Builder;
using GalForUnity.Graph.Editor.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block
{
    public class DraggableBlockEditor : VisualElement, IGalBlockEditor, ISelectable
    {
        private readonly VisualElement _dragContainer;

        private readonly GalGraphView _graphView;
        private bool _mouseDown;
        public VisualElement additionalContent;
        public VisualElement content;
        public IGalBlock GalBlock;
        protected Button operationButton;
        public PlotNode plotNode;
        private readonly BlockClickSelector m_ClickSelector;
        public bool selected;

        public DraggableBlockEditor(PlotNode plotNode, IGalBlock galBlock)
        {
            var templateContainer = UxmlHandler.instance.draggableBlockUxml.Instantiate();
            _graphView = plotNode.GraphView;
            _dragContainer = new VisualElement
            {
                name = "DragContainer"
            };
            GalBlock = galBlock;
            this.plotNode = plotNode;
            _dragContainer.Add(templateContainer);
            content = templateContainer.Q<VisualElement>("content");
            additionalContent = templateContainer.Q<VisualElement>("AdditionalContent");
            operationButton = templateContainer.Q<Button>("operationButton");
            var delete = templateContainer.Q<Button>("deleteDragButton");
            delete.clickable = new Clickable(() => { parent.Remove(this); });
            RegisterCallback<MouseMoveEvent>(Callback);
            RegisterCallback<MouseUpEvent>(Up);
            RegisterCallback<MouseDownEvent>(Down);
            Add(_dragContainer);
            _dragContainer.AddToClassList("drag-block");
            m_ClickSelector = new BlockClickSelector();
            this.AddManipulator(m_ClickSelector);
        }

        public virtual IEnumerable<(GalPort, GalPortAsset)> OnSavePort(GalNodeAsset galNodeAsset)
        {
            return Array.Empty<(GalPort, GalPortAsset)>();
        }

        public virtual IEnumerable<(GalPortAsset, GalPort)> OnLoadPort(GalNodeAsset galNodeAsset)
        {
            return Array.Empty<(GalPortAsset, GalPort)>();
        }

        public bool IsSelectable()
        {
            return true;
        }

        public bool HitTest(Vector2 localPoint)
        {
            return ContainsPoint(localPoint);
        }

        public void Select(VisualElement selectionContainer, bool additive)
        {
            if (selectionContainer is PlotNode){
                if(!additive)
                    plotNode.ClearSelection();
                plotNode.AddToSelection(this);
            }
            _dragContainer.RemoveFromClassList("drag-block");
            _dragContainer.AddToClassList("drag-block-selection");
            selected = true;
        }

        public void Unselect(VisualElement selectionContainer)
        {
            if (selectionContainer is PlotNode){
                plotNode.RemoveFromSelection(this);
            }
            _dragContainer.AddToClassList("drag-block");
            _dragContainer.RemoveFromClassList("drag-block-selection");
            selected = false;
        }

        public bool IsSelected(VisualElement selectionContainer)
        {
            return selected;
        }
        
        private void Up(MouseUpEvent x)
        {
            _mouseDown = false;
            if (_dragContainer.style.top != 0) _dragContainer.style.top = 0;
            var configValue = plotNode.runtimeNode.config;
            configValue.Remove(GalBlock);
            configValue.Insert(parent.IndexOf(this), GalBlock);
            PortProcess(GalBlock);
            this.ReleaseMouse();
        }

        private void Down(MouseDownEvent x)
        {
            if (x.button != 0) return;
            _mouseDown = true;
            this.CaptureMouse();
            x.StopPropagation();
        }

        private void Callback(MouseMoveEvent x)
        {
            if (_mouseDown)
            {
                _dragContainer.style.top = _dragContainer.style.top.value.value + CreateLast();
                _dragContainer.style.top = _dragContainer.style.top.value.value + x.mouseDelta.y;
            }
        }

        private float CreateLast()
        {
            var node = parent;
            var dragPositionY = _dragContainer.worldBound.center.y;
            var min = node.Children().Min(x =>
                new SelectCap<float, VisualElement>(Mathf.Abs(x.worldBound.center.y - dragPositionY), x));
            if (min.Obj == this) return 0;
            var index = node.IndexOf(min.Obj);
            var mIndex = node.IndexOf(this);
            if (mIndex < index)
            {
                if (_dragContainer.worldBound.center.y < worldBound.y + min.Obj.worldBound.size.y) return 0;
            }
            else
            {
                if (_dragContainer.worldBound.center.y >
                    min.Obj.worldBound.y + _dragContainer.worldBound.size.y) return 0;
            }

            node.Remove(this);
            node.Insert(index, this);
            return mIndex < index ? -min.Obj.worldBound.size.y : min.Obj.worldBound.size.y;
        }

        private void PortProcess(IGalBlock changedGalBlock)
        {
            if (!(changedGalBlock is IGalConfig config)) return;
            config.Clear();
        }

        ~DraggableBlockEditor()
        {
            UnregisterCallback<MouseUpEvent>(Up);
            UnregisterCallback<MouseDownEvent>(Down);
            UnregisterCallback<MouseMoveEvent>(Callback);
        }

        public virtual void OnUnselected(){
            
        }

        public virtual void OnSelected(){
            
        }
    }
}