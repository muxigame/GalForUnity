using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block{
    public class BlockClickSelector : MouseManipulator{
        public BlockClickSelector(){
            activators.Add(new ManipulatorActivationFilter{
                button = MouseButton.LeftMouse
            });
            activators.Add(new ManipulatorActivationFilter{
                button = MouseButton.LeftMouse, modifiers = EventModifiers.Shift
            });
            activators.Add(new ManipulatorActivationFilter{
                button = MouseButton.LeftMouse, modifiers = EventModifiers.Alt
            });
            activators.Add(new ManipulatorActivationFilter{
                button = MouseButton.RightMouse
            });
            activators.Add(new ManipulatorActivationFilter{
                button = MouseButton.RightMouse, modifiers = EventModifiers.Shift
            });
            activators.Add(new ManipulatorActivationFilter{
                button = MouseButton.RightMouse, modifiers = EventModifiers.Alt
            });
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                activators.Add(new ManipulatorActivationFilter{
                    button = MouseButton.LeftMouse, modifiers = EventModifiers.Command
                });
            else
                activators.Add(new ManipulatorActivationFilter{
                    button = MouseButton.LeftMouse, modifiers = EventModifiers.Control
                });
        }

        private static bool WasSelectableDescendantHitByMouse(
            VisualElement currentTarget,
            MouseDownEvent evt){
            if (!(evt.target is VisualElement target) || currentTarget == target) return false;
            for (var dest = target; dest != null && currentTarget != dest; dest = dest.parent)
                if (dest is DraggableBlockEditor graphElement && graphElement.enabledInHierarchy && graphElement.pickingMode != PickingMode.Ignore && graphElement.IsSelectable()){
                    var localPoint = currentTarget.ChangeCoordinatesTo(dest, evt.localMousePosition);
                    if (graphElement.HitTest(localPoint)) return true;
                }

            return false;
        }

        protected override void RegisterCallbacksOnTarget(){
            target.RegisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown));
        }

        private void OnMouseDown(MouseDownEvent evt){
            if (!(evt.currentTarget is DraggableBlockEditor draggableBlockEditor) || !CanStartManipulation(evt) || !draggableBlockEditor.IsSelectable() || !draggableBlockEditor.HitTest(evt.localMousePosition) || WasSelectableDescendantHitByMouse((VisualElement) evt.currentTarget, evt)) return;
            ISelection firstAncestorOfType = draggableBlockEditor.GetFirstAncestorOfType<ISelection>();
            if (draggableBlockEditor.IsSelected((VisualElement) firstAncestorOfType))
            {
                if (evt.actionKey)
                    draggableBlockEditor.Unselect((VisualElement) firstAncestorOfType);
            }
            else
                draggableBlockEditor.Select((VisualElement) firstAncestorOfType, evt.actionKey);
        }

        /// <summary>
        ///     <para>Called to unregister event callbacks from the target element.</para>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget(){ target.UnregisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown)); }
    }
}