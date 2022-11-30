// //======================================================================
// //
// //       CopyRight 2019-2021 © MUXI Game Studio 
// //       . All Rights Reserved 
// //
// //        FileName :  SavableConfigEditor.cs
// //
// //        Created by 半世癫(Roc)
// //
// //======================================================================
//
// using System;
// using System.Reflection;
// using GalForUnity.System;
// using GalForUnity.System.Address;
// using GalForUnity.System.Archive.Data;
// using MUX.Type;
// using UnityEditor;
// using UnityEditor.Experimental.GraphView;
// using UnityEditor.UIElements;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace GalForUnity.Editor{
//     
//     [CustomEditor(typeof(SavableConfig))]
//     
//     public class SavableConfigEditor : ButtonEditor{
//         Type _inspectorType;
//         VisualElement _container;
//         VisualElement _list;
//         public override VisualElement CreateInspectorGUI(){
//             var o = new SerializedObject(target);
//             var savableConfig = (SavableConfig) target;
//             _inspectorType = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow", true);
//             var editorWindow = EditorWindow.GetWindow(_inspectorType);
//             _list = new PropertyField(o.FindProperty("types"));
//             var castDictionaryCountSer = o.FindProperty("castDictionaryCount");
//             var saveHierarchySer = o.FindProperty("saveHierarchy");
//             _container = new VisualElement();
//             var saveHierarchy = new PropertyField(saveHierarchySer) { };
//             var castDictionaryCount = new PropertyField(castDictionaryCountSer) { };
//             var button = new Button() {
//                 text= GfuLanguage.GfuLanguageInstance.ADDTYPE.Value
//             };
//             button.clicked += () => {
//                 var propertyHeight = _container.contentRect.height+_container.worldBound.y;
//                 var positionCenter = editorWindow.position.center;
//                 positionCenter.y = 90;
//                 positionCenter.y += propertyHeight;
//                 var searchWindowContext = new SearchWindowContext(positionCenter);
//                 var searchTypeProvider = ScriptableObject.CreateInstance<SearchTypeProvider>();
//                 searchTypeProvider.OnSelectEntryHandler += (x, y) => {
//                     var serializableType = new SerializableType((Type) x.userData);
//                     if(!savableConfig.types.Contains(serializableType))savableConfig.types.Add(serializableType);
//                     else if (EditorUtility.DisplayDialog(GfuLanguage.GfuLanguageInstance.HINT.Value,
//                         GfuLanguage.GfuLanguageInstance.HASEXISTS.Value,GfuLanguage.GfuLanguageInstance.OK.Value)){
//                     }
//                     AssetDatabase.SaveAssets();
//                     AssetDatabase.Refresh();
//                     return true;
//                 };
//                 SearchWindow.Open(searchWindowContext, searchTypeProvider);
//             };
//             var removeButton = new Button() {
//                 text= GfuLanguage.GfuLanguageInstance.REMOVETYPE.Value
//             };
//             removeButton.clicked += () => {
//                 var propertyHeight = _container.contentRect.height +_container.worldBound.y;
//                 var positionCenter = editorWindow.position.center;
//                 positionCenter.y = 90;
//                 positionCenter.y += propertyHeight;
//                 var searchWindowContext = new SearchWindowContext(positionCenter);
//                 var searchTypeProvider = ScriptableObject.CreateInstance<SearchTypeProvider>();
//                 searchTypeProvider.OnSelectEntryHandler += (x, y) => {
//                     var serializableType = new SerializableType((Type) x.userData);
//                     if(savableConfig.types.Contains(serializableType))savableConfig.types.Remove(serializableType);
//                     AssetDatabase.SaveAssets();
//                     AssetDatabase.Refresh();
//                     return true;
//                 };
//                 SearchWindow.Open(searchWindowContext, searchTypeProvider);
//             };
//             var obj = new ObjectField {
//                 value = MonoScript.FromScriptableObject((ScriptableObject) target), objectType = typeof(MonoScript), allowSceneObjects = false
//             };
//             // obj.s
//             _container.Add(obj);
//             _container.Add(castDictionaryCount);
//             _container.Add(saveHierarchy);
//             _container.Add(_list);
//             _container.Add(button);
//             _container.Add(removeButton);
//             return _container;
//         }
//
//     
//     }
// }
// //
// // EventInfo selectionChanged;
// //         Delegate selectDelegate;
// //         public override void OnInspectorGUI(){
// //             DrawButton<SavableConfig>(GfuLanguage.GfuLanguageInstance.ADDTYPE.Value,(x)=>{
// //                 // window = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow", true);
// //                 // selectionChanged = window.GetEvent("selectionChanged");
// //                 var o = new SerializedObject(target);
// //                 // var savableConfig = (SavableConfig) target;
// //                 
// //                 var serializedProperty = o.FindProperty("types");
// //                 var propertyHeight = EditorGUI.GetPropertyHeight(serializedProperty);
// //                 var rect1 = new Rect(EditorGUIUtility.currentViewWidth /2f -240 /2f,0 -145 +propertyHeight,240,200);
// //                 // window.InvokeMember("Show", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, null, new object[] {
// //                 //     rect1, null
// //                 // });
// //                 // CreateDelegate(EditorWindow.GetWindow(window));
// //                 Debug.Log(list.contentRect);
// //                 // window.position;
// //                 var searchWindowContext = new SearchWindowContext(container.transform.position);
// //                 SearchWindow.Open(searchWindowContext, ScriptableObject.CreateInstance<SearchTypeProvider>());
// //             });
// //             // Debug.Log(visualElement.contentRect);
// //         }
// //         void CreateDelegate(object obj){
// //             selectionChanged.AddEventHandler(obj,new Action<AdvancedDropdownItem>((x) => {
// //                 // var type = Type.GetType();
// //                 var savableConfig = (SavableConfig)target;
// //                 Debug.Log(x.name);
// //                 // SpriteResolver
// //                 var value = x.GetType().GetProperty("localizedName").GetValue(x);
// //                 Debug.Log(value);
// //                 var component = GetConfigTypeByAssembly(x.name);
// //                 var serializableType = new SerializableType(component.GetType());
// //                 if(!savableConfig.types.Contains(serializableType)) savableConfig.types.Add(serializableType);
// //             }) );
// //         }
// //         void ItemSelect(object arg){
// //             Debug.Log(arg.ToString());
// //         }
// // public Type GetConfigTypeByAssembly(string className){
// // Type type = null;
// // Assembly assembly = Assembly.GetExecutingAssembly();
// //     if(assembly.FullName.Contains("UnityEngine"))  type= assembly.GetType("UnityEngine."+className);
// // else type=Type.GetType(className);
// // return type;
// // }