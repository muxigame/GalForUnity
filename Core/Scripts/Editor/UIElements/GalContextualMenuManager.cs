// using UnityEngine;
// using UnityEngine.UIElements;
// using NotImplementedException = System.NotImplementedException;
//
// namespace GalForUnity.Core.Scripts.Editor
// {
//     public class GalContextualMenuManager:ContextualMenuManager
//     {
//         public override void DisplayMenuIfEventMatches(EventBase evt, IEventHandler eventHandler)
//         {
//             if (evt == null)
//                 return;
//             if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
//             {
//                 if (evt.eventTypeId == EventBase<MouseDownEvent>.TypeId())
//                 {
//                     MouseDownEvent mouseDownEvent = evt as MouseDownEvent;
//                     if (mouseDownEvent.button == 1 || mouseDownEvent.button == 0 && mouseDownEvent.modifiers == EventModifiers.Control)
//                     {
//                         this.DisplayMenu(evt, eventHandler);
//                         evt.StopPropagation();
//                         return;
//                     }
//                 }
//             }
//             else if (evt.eventTypeId == EventBase<MouseUpEvent>.TypeId() && (evt as MouseUpEvent).button == 1)
//             {
//                 this.DisplayMenu(evt, eventHandler);
//                 evt.StopPropagation();
//                 return;
//             }
//             if (evt.eventTypeId != EventBase<KeyUpEvent>.TypeId() || (evt as KeyUpEvent).keyCode != KeyCode.Menu)
//                 return;
//             this.DisplayMenu(evt, eventHandler);
//             evt.StopPropagation();
//         }
//
//         protected override void DoDisplayMenu(DropdownMenu menu, EventBase triggerEvent)
//         {
//             menu.DoDisplayEditorMenu(triggerEvent);
//         }
//     }
// }