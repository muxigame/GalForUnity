//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  DraggableBlock.cs Created at 2022-10-09 22:36:40
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GalForUnity.External;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Block.Config;
using GalForUnity.Graph.Nodes.Editor;
using GalForUnity.Graph.SceneGraph;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block
{
    public class DraggableBlockEditor : GraphElement, IGalBlockEditor, ISelectable
    {
        private readonly VisualElement _dragContainer;

        private readonly GfuSceneGraphView _graphView;
        private bool _mouseDown;
        public VisualElement additionalContent;
        public VisualElement content;
        public IGalBlock GalBlock;
        protected Button operationButton;
        public PlotNode plotNode;
        private readonly ClickSelector m_ClickSelector;

        public DraggableBlockEditor(PlotNode plotNode, IGalBlock galBlock)
        {
            var templateContainer = UxmlHandler.instance.draggableBlockUxml.Instantiate();
            _graphView = plotNode.GraphView;
            if (_graphView != null) _graphView.OnBlockFocus += OnOtherFocus;
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
            // RegisterCallback<MouseMoveEvent>(Callback);
            // RegisterCallback<MouseUpEvent>(Up);
            // RegisterCallback<MouseDownEvent>(Down);
            pickingMode = PickingMode.Position;
            // m_ClickSelector = new ClickSelector();
            // this.AddManipulator(m_ClickSelector);
            capabilities |= Capabilities.Selectable;
            Add(_dragContainer);
            _dragContainer.AddToClassList("drag-block");
        }

        public virtual IEnumerable<(GfuPort, GfuPortAsset)> OnSavePort(GfuNodeAsset gfuNodeAsset)
        {
            return Array.Empty<(GfuPort, GfuPortAsset)>();
        }

        public virtual IEnumerable<(GfuPortAsset, GfuPort)> OnLoadPort(GfuNodeAsset gfuNodeAsset)
        {
            return Array.Empty<(GfuPortAsset, GfuPort)>();
        }

        public override bool IsSelectable()
        {
            return true;
        }

        public override bool HitTest(Vector2 localPoint)
        {
            return ContainsPoint(localPoint);
        }

        public override void Select(VisualElement selectionContainer, bool additive)
        {
            _dragContainer.RemoveFromClassList("drag-block");
            _dragContainer.AddToClassList("drag-block-selection");
        }

        public override void Unselect(VisualElement selectionContainer)
        {
            _dragContainer.AddToClassList("drag-block");
            _dragContainer.RemoveFromClassList("drag-block-selection");
        }

        public override bool IsSelected(VisualElement selectionContainer)
        {
            return _dragContainer.ClassListContains("drag-block-selection");
        }

        public virtual void OnOtherFocus()
        {
            // _dragContainer.AddToClassList("drag-block");
            // _dragContainer.RemoveFromClassList("drag-block-selection");
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
            if (!x.ctrlKey && !x.shiftKey)
                if (_graphView != null)
                    _graphView.OnBlockFocus?.Invoke();

            // _dragContainer.RemoveFromClassList("drag-block");
            // _dragContainer.AddToClassList("drag-block-selection");
            this.CaptureMouse();
            x.StopImmediatePropagation();
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
            _graphView.OnBlockFocus -= OnOtherFocus;
        }
    }
}