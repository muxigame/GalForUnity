//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  AttributeDrawer.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using System.Reflection;
using GalForUnity.Attributes;
using GalForUnity.Controller;
using GalForUnity.System;
using GalForUnity.View;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GalForUnity.Editor{
	
	public class ButtonEditor : BaseEditor {
		protected void DrawButton<T>(string text,Action<T> action) where T:Object{
			DrawDefaultInspector();
			GUILayout.Space(5);
			GUILayout.BeginHorizontal();
			GUILayout.Space(EditorGUIUtility.currentViewWidth /4f -20);
			if(GUILayout.Button(text,GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth / 2f))) {
				action.Invoke((T)target);
			}
			GUILayout.EndHorizontal();
		}
		[CustomEditor(typeof(PlotFlowController))]
		public class PlotFlowControllerEditor:ButtonEditor{
			public override void OnInspectorGUI() {
				DrawButton<PlotFlowController>("初始化Hierarchy中所有剧情",(x)=>{
					x.InitialPlowFlowController();
				});
			}
		}
		[CustomEditor(typeof(SceneController))]
		public class SceneControllerEditor:ButtonEditor{
			public override void OnInspectorGUI() {
				DrawButton<SceneController>("添加Hierarchy中所有场景",(x)=>{			
					x.InitialSceneController();
				});
			}
		}
		[CustomEditor(typeof(ShowPlotView))]
		public class ShowPlotViewEditor:ButtonEditor{
			public override void OnInspectorGUI() {
				DrawButton<ShowPlotView>("初始化游戏视图系统",(x)=>{
					x.InitialView();
				});
			}
		}
		[CustomEditor(typeof(RoleController))]
		public class RoleControllerEditor:ButtonEditor{
			public override void OnInspectorGUI() {
				DrawButton<RoleController>("初始化Hierarchy中所有角色",(x)=>{
					x.InitialRoleController();
				});
			}
		}
		[CustomEditor(typeof(GameSystem))]
		public class GameSystemEditor:ButtonEditor{
			public override void OnInspectorGUI() {
				DrawButton<GameSystem>("初始化游戏系统",(x)=>{	
					if (EditorUtility.DisplayDialog("提示","需要为您初始化其他依赖项吗","是滴","谢谢，我手动操作")){
						x.InitialGameSystem(true);
					}
					x.InitialGameSystem(false);
				});

			}
		}

	}
	[CustomPropertyDrawer(typeof(AddComponentAttribute))]
	public class AttributeDrawer : PropertyDrawer {
		// Start is called before the first frame update
		bool _first;
		public override VisualElement CreatePropertyGUI(SerializedProperty property) {
			//base.OnGUI(position, property, label);
			object parent = GetParentObjectOfProperty(property.propertyPath, property.serializedObject.targetObject);
			//Debug.Log();
			//Component component = (fieldInfo.GetValue(parent) as Component).gameObject.AddComponent(data.type1);
			Type type = parent.GetType();
			if (!_first) {
			
				_first = true;
				GameObject.Find(property.serializedObject.targetObject.name).AddComponent(type);
				Debug.Log(1);
			}
			return base.CreatePropertyGUI(property);
		}
		//public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		
		
		//}
		private object GetParentObjectOfProperty(string path, object obj) {
			string[] fields = path.Split('.');

			// We've finally arrived at the final object that contains the property
			if (fields.Length == 1) {
				return obj;
			}

			// We may have to walk public or private fields along the chain to finding our container object, so we have to allow for both
			FieldInfo fi = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			// Keep searching for our object that contains the property
			return GetParentObjectOfProperty(string.Join(".", fields, 1, fields.Length - 1), obj);
		}
	}
}
