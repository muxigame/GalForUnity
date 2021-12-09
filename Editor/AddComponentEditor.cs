// using System;
// using System.Reflection;
// using System;
// using System.Reflection;
// using UnityEditor;
// using UnityEngine;
//
// namespace GalForUnity.Editor{
//
//     public class AddComponentEditor{
//         
//         Type window;
//         Type componentItem;
//         EventInfo selectionChanged;
//         Delegate selectDelegate;
//
//         EventInfo windowClosed;
//         Delegate closedDelegate;
//
//         PropertyInfo displayName;
//
//         public AddComponentEditor(){
//             window = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.AddComponent.AddComponentWindow", true);
//             componentItem = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.AddComponent.ComponentDropdownItem", true);
//             selectionChanged = window.GetEvent("selectionChanged");
//             windowClosed = window.GetEvent("windowClosed");
//             closedDelegate = CreateDelegate(windowClosed.EventHandlerType, "WindowClosed");
//             selectDelegate = CreateDelegate(selectionChanged.EventHandlerType, "ItemSelect");
//             displayName = componentItem.GetProperty("name");
//         }
//
//         public void Show(Rect rect){
//             if ((bool) window.InvokeMember("Show", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, null, new object[] {
//                 rect, null
//             })){
//                 var windows = Resources.FindObjectsOfTypeAll(window);
//                 foreach (var w in windows){
//                     windowClosed.AddEventHandler(w, closedDelegate);
//                     selectionChanged.AddEventHandler(w, selectDelegate);
//                 }
//             }
//         }
//
//         void WindowClosed(object arg){
//             var windows = Resources.FindObjectsOfTypeAll(window);
//             foreach (var w in windows){
//                 windowClosed.RemoveEventHandler(w, closedDelegate);
//                 selectionChanged.RemoveEventHandler(w, selectDelegate);
//             }
//
//         }
//
//         void ItemSelect(object arg){
//             var component = (string) displayName.GetValue(arg);
//             Debug.Log(component);      //名字
//             Debug.Log(arg.ToString()); //菜单路径
//         }
//
//         Delegate CreateDelegate(Type type, string name){
//             Type t = this.GetType();
//             var method = t.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
//             return method.CreateDelegate(type, this);
//         }
//     }
// }