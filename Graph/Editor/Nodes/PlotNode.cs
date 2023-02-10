

using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Core.Block;
using GalForUnity.Core.Editor.Attributes;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Editor.Block;
using GalForUnity.Graph.Editor.Builder;
using GalForUnity.Graph.Nodes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Direction = GalForUnity.Graph.Direction;
using Orientation = GalForUnity.Graph.Orientation;

namespace GalForUnity.Graph.Editor.Nodes{
    [NodeRename(nameof(PlotNode), "角色检查节点，该节点负责检查在图中流转的角色是否满足要求，如果要求达到，则会跳转满足出口，否则则会跳转不满足出口")]
    [NodeType(NodeCode.PlotNode)]
    [NodeEditor(typeof(Graph.Nodes.PlotNode))]
    public sealed class PlotNode : GfuNode,ISelection{
        public readonly VisualElement content;

        public override List<GalPort> Enter{ get; }= new List<GalPort>{
            new GalPort(Orientation.Horizontal, Direction.Input, Capacity.Multi, typeof(GalNodeAsset), nameof(Enter))
        };

        public override List<GalPort> Exit { get; }= new List<GalPort>{
            new GalPort(Orientation.Horizontal, Direction.Output, Capacity.Multi, typeof(GalNodeAsset), nameof(Exit))
        };
        
        public Graph.Nodes.PlotNode runtimeNode;

        public PlotNode(){ Add(content = new VisualElement()); }

        internal override IEnumerable<(GalPort port, GalPortAsset gfuPortAsset)> OnSavePort(GalNodeAsset galNodeAsset){
            foreach (var port in base.OnSavePort(galNodeAsset)) yield return port;
            foreach (var draggableBlockEditor in content.Query<DraggableBlockEditor>().ToList()){
                foreach (var blockPort in draggableBlockEditor.OnSavePort(galNodeAsset)) yield return blockPort;
            }
        }

        internal override IEnumerable<(GalPortAsset gfuPortAsset, GalPort port)> OnLoadPort(GalNodeAsset galNodeAsset){
            foreach (var port in base.OnLoadPort(galNodeAsset)) yield return port;
            foreach (var draggableBlockEditor in content.Query<DraggableBlockEditor>().ToList()){
                foreach (var blockPort in draggableBlockEditor.OnLoadPort(galNodeAsset)) yield return blockPort;
            }
        }

        public override void OnInit(RuntimeNode otherRuntimeNode, GalGraphView graphView){
            base.OnInit(otherRuntimeNode, graphView);
            runtimeNode = (Graph.Nodes.PlotNode) otherRuntimeNode;
            styleSheets.Add(UxmlHandler.instance.plotNodeUss);
            Add(new Button{
                name = "AddBlockButton",
                text = "AddBlock",
                clickable = new Clickable(() => {
                    var searchWindowContext = new SearchWindowContext(EditorWindow.focusedWindow.position.position + this.LocalToWorld(transform.position));
                    var searchTypeProvider = ScriptableObject.CreateInstance<PlotBlockSearchProvider>();
                    searchTypeProvider.OnSelectEntryHandler += (x, y) => {
                        if (!(x.userData is Type blockType)) return false;
                        // 下面是反射工厂的方法
                        // var assembly = Assembly.Load("com.muxigame.galforunity");
                        // var blockFactory =  assembly.GetTypes().First(x=>x.IsSubclassOf(typeof(UxmlFactory<,>).MakeGenericType(xUserData, typeof(UxmlTraits))));
                        // if (!(Activator.CreateInstance(blockFactory) is IUxmlFactory uxmlFactoryInstance)) return false;
                        var nodeEditor = blockType.GetCustomAttribute<NodeEditor>();
                        if (nodeEditor == null) return false;
                        if (!(Activator.CreateInstance(nodeEditor.Type) is IGalBlock block)) return false;
                        if (!(Activator.CreateInstance(blockType, this, block) is DraggableBlockEditor galBlock)) return false;
                        
                        content.Add(galBlock);
                        runtimeNode.config.Add(galBlock.GalBlock);
                        return true;
                    };
                    SearchWindow.Open(searchWindowContext, searchTypeProvider);
                })
            });
            Debug.Assert(runtimeNode?.config != null, "runtimeNode?.config == null");
            runtimeNode?.config?.ForEach(x => {
                var type = x?.GetType();
                if (!(Activator.CreateInstance(NodeEditor.GetEditor(type), this, x) is DraggableBlockEditor galBlock)) return;
                content.Add(galBlock);
            });
            RegisterCallback<ExecuteCommandEvent>(new EventCallback<ExecuteCommandEvent>(this.OnExecuteCommand));
        }

        private void OnExecuteCommand(ExecuteCommandEvent evt)
        {
            Debug.LogError(evt);
            if (this.panel.GetCapturingElement(PointerId.mousePointerId) != null)
                return;
            if (evt.commandName == "Copy")
            {
                this.CopySelectionCallback();
                evt.StopPropagation();
            }
            else if (evt.commandName == "Paste")
            {
                this.PasteCallback();
                evt.StopPropagation();
            }
            else if (evt.commandName == "Duplicate")
            {
                this.DuplicateSelectionCallback();
                evt.StopPropagation();
            }
            else if (evt.commandName == "Cut")
            {
                this.CutSelectionCallback();
                evt.StopPropagation();
            }
            else if (evt.commandName == "Delete")
            {
                this.DeleteSelectionCallback(UnityEditor.Experimental.GraphView.GraphView.AskUser.DontAskUser);
                evt.StopPropagation();
            }
        }

        private void DeleteSelectionCallback(GraphView.AskUser dontAskUser)
        {
            throw new NotImplementedException();
        }

        private void CutSelectionCallback()
        {
            throw new NotImplementedException();
        }

        private void DuplicateSelectionCallback()
        {
            throw new NotImplementedException();
        }

        private void PasteCallback()
        {
            throw new NotImplementedException();
        }

        private void CopySelectionCallback()
        {
            throw new NotImplementedException();
        }

        public class PlotNodeUxmlFactory : UxmlFactory<PlotNode, UxmlTraits>{ }

        private void AddToSelectionNoUndoRecord(DraggableBlockEditor draggableBlock){
            draggableBlock.selected = true; 
            selection.Add(draggableBlock);
            draggableBlock.OnSelected();
            // draggableBlock.UnregisterCallback<DetachFromPanelEvent>((this.OnSelectedElementDetachedFromPanel));
            draggableBlock.MarkDirtyRepaint();
        }
        private void RemoveFromSelectionNoUndoRecord(DraggableBlockEditor draggableBlock){
            draggableBlock.selected = false; 
            selection.Remove(draggableBlock);
            draggableBlock.OnUnselected();
            // draggableBlock.UnregisterCallback<DetachFromPanelEvent>((this.OnSelectedElementDetachedFromPanel));
            draggableBlock.MarkDirtyRepaint();
        }

        public override void OnUnselected(){
            ClearSelection();
            base.OnUnselected();
        }

        public void AddToSelection(ISelectable selectable){
            if (!(selectable is DraggableBlockEditor graphElement) || this.selection.Contains(selectable))
                return;
            AddToSelectionNoUndoRecord(graphElement);
        }

        public void RemoveFromSelection(ISelectable selectable){
            if (!(selectable is DraggableBlockEditor graphElement) || this.selection.Contains(selectable))
                return;
            RemoveFromSelectionNoUndoRecord(graphElement);
        }

        public void ClearSelection(){
            selection.ForEach(x=> {
                x.Unselect(this);
                if(x is DraggableBlockEditor draggableBlockEditor)
                    draggableBlockEditor.OnUnselected();
            });
            selection.Clear();
        }
        public List<ISelectable> selection{ get; }=new List<ISelectable>();
    }
}