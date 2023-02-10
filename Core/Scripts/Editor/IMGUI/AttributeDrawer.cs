
//
// using System;
// using System.Reflection;
// using GalForUnity.Attributes;
// using GalForUnity.Controller;
// using GalForUnity.System;
// using GalForUnity.View;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UIElements;
// using Object = UnityEngine.Object;
//
// namespace GalForUnity.Editor{
// 	
// 	public class ButtonEditor : BaseEditor {
// 		public override void OnInspectorGUI(){
// 			DrawDefaultInspector();
// 		}
// 		protected void DrawButton<T>(string text,Action<T> action) where T:Object{
// 			GUILayout.Space(5);
// 			GUILayout.BeginHorizontal();
// 			GUILayout.Space(EditorGUIUtility.currentViewWidth /4f -20);
// 			if(GUILayout.Button(text,GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / 2f))) {
// 				action.Invoke((T)target);
// 			}
// 			GUILayout.EndHorizontal();
// 		}
// 		// [CustomEditor(typeof(PlotFlowController))]
// 		public class PlotFlowControllerEditor:ButtonEditor{
// 			public override void OnInspectorGUI() {
// 				base.OnInspectorGUI();
// 				// DrawButton<PlotFlowController>(GfuLanguage.GfuLanguageInstance.INITIALIZEALLPLOTMODEL.Value,(x)=>{
// 				// 	x.InitialPlowFlowController();
// 				// });
// 			}
// 		}
// 		[CustomEditor(typeof(SceneController))]
// 		public class SceneControllerEditor:ButtonEditor{
// 			public override void OnInspectorGUI() {
// 				base.OnInspectorGUI();
// 				DrawButton<SceneController>(GfuLanguage.GfuLanguageInstance.ADDALLSCENEMODEL.Value,(x)=>{			
// 					x.InitialSceneController();
// 				});
// 			}
// 		}
// 		[CustomEditor(typeof(ShowPlotView))]
// 		public class ShowPlotViewEditor:ButtonEditor{
// 			public override void OnInspectorGUI() {
// 				base.OnInspectorGUI();
// 				DrawButton<ShowPlotView>(GfuLanguage.GfuLanguageInstance.INITIALIZEGAMEVIEW.Value,(x)=>{
// 					x.InitialView();
// 				});
// 			}
// 		}
// 		[CustomEditor(typeof(RoleController))]
// 		public class RoleControllerEditor:ButtonEditor{
// 			public override void OnInspectorGUI() {
// 				base.OnInspectorGUI();
// 				DrawButton<RoleController>(GfuLanguage.GfuLanguageInstance.INITIALIZEHIERARCHY.Value,(x)=>{
// 					x.InitialRoleController();
// 				});
// 			}
// 		}
// 		[CustomEditor(typeof(GameSystem))]
// 		public class GameSystemEditor:ButtonEditor{
// 			public override void OnInspectorGUI() {
// 				base.OnInspectorGUI();
// 				DrawButton<GameSystem>(GfuLanguage.GfuLanguageInstance.INITIALIZETHEGAMESYSTEM.Value,(x)=>{	
// 					if (EditorUtility.DisplayDialog(GfuLanguage.GfuLanguageInstance.HINT.Value,
// 						GfuLanguage.GfuLanguageInstance.INITIALIZEAOTHER.Value,GfuLanguage.GfuLanguageInstance.YES.Value,
// 						GfuLanguage.GfuLanguageInstance.NO.Value)){
// 						x.InitialGameSystem(true);
// 					}
// 					x.InitialGameSystem(false);
// 				});
//
// 			}
// 		}
//
// 	}
// 	[CustomPropertyDrawer(typeof(AddComponentAttribute))]
// 	public class AttributeDrawer : PropertyDrawer {
// 		// Start is called before the first frame update
// 		bool _first;
// 		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
// 			//base.OnGUI(position, property, label);
// 			object parent = GetParentObjectOfProperty(property.propertyPath, property.serializedObject.targetObject);
// 			//Debug.Log();
// 			//Component component = (fieldInfo.GetValue(parent) as Component).gameObject.AddComponent(data.type1);
// 			Type type = parent.GetType();
// 			if (!_first) {
// 			
// 				_first = true;
// 				GameObject.Find(property.serializedObject.targetObject.name).AddComponent(type);
// 				Debug.Log(1);
// 			}
// 			return base.CreatePropertyGUI(property);
// 		}
// 		//public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
// 		
// 		
// 		//}
// 		private object GetParentObjectOfProperty(string path, object obj) {
// 			string[] fields = path.Split('.');
//
// 			// We've finally arrived at the final object that contains the property
// 			if (fields.Length == 1) {
// 				return obj;
// 			}
//
// 			// We may have to walk public or private fields along the chain to finding our container object, so we have to allow for both
// 			FieldInfo fi = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//
// 			// Keep searching for our object that contains the property
// 			return GetParentObjectOfProperty(string.Join(".", fields, 1, fields.Length - 1), obj);
// 		}
// 	}
// }
