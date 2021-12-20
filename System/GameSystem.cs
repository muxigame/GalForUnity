//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        Filename :  GameSystem.cs 
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using GalForUnity.Attributes;
using GalForUnity.Controller;
using GalForUnity.Graph.Windows;
using GalForUnity.InstanceID;
using GalForUnity.Model;
using GalForUnity.Model.Scene;
using GalForUnity.System.Archive;
using GalForUnity.System.Archive.Behavior;
using GalForUnity.System.Archive.Data;
using GalForUnity.View;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using EventCenter = GalForUnity.System.Event.EventCenter;

namespace GalForUnity.System{
	/// <summary>
	/// 游戏的控制系统，即GalForUnity的控制系统
	/// </summary>
	// [RequireComponent(typeof(GraphSystem))]
	// [RequireComponent(typeof(GfuInstance))]
	[ExecuteAlways]
	public class GameSystem : GfuSavableMonoInstanceManager<GameSystem>{
		// ReSharper disable all MemberCanBePrivate.Global
		[SerializeField]
		private static SystemData _systemData;
		[SerializeField]
		private static GraphSystem.GraphSystemData _graphSystem;
		
		/// <summary>
		/// 访问系统数据
		/// </summary>
		[Rename(nameof(systemData))]
		[SerializeField]
		public SystemData systemData;
		/// <summary>
		/// 访问图系统
		/// </summary>
		[Rename(nameof(graphSystem))]
		[SerializeField]
		public GraphSystem graphSystem;
		
		/// <summary>
		/// 点击时间
		/// </summary>
		private float clickTime;


		protected override string OnRename(){
			return "GalForUnity";
		}
		
		
		private void OnEnable(){
#if UNITY_EDITOR
			BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
			var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
			Type type=Type.GetType("Live2D.Cubism.Core.CubismModel");
			// Debug.Log(type);
			if (type != null){
				if (!symbols.Contains("LIVE2D")){
					string str = "";
					str = string.IsNullOrEmpty(symbols) ? "LIVE2D;" : symbols+";" + "LIVE2D;";
					PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, str);
				}
			}else{
				if (symbols.Contains("LIVE2D")){
					PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols.Replace("LIVE2D",""));
				}
			}
			AutoAddGfuCompoenent(transform);
			TraverseTheSubclass(transform);
#endif
		}
		
		private void Update(){
			if (Input.GetMouseButtonDown(0)){
				clickTime = Time.time;
			}
			if (Input.GetMouseButtonUp(0)&&(Time.time-clickTime<=1) &&!EventSystem.current.IsPointerOverGameObject()){
				EventCenter.GetInstance().OnMouseDown.Invoke(Input.mousePosition);
			}
		}

		/// <summary>
		/// 记录着系统信息(当前时间，场景，角色，以及各种控制器的引用等等)，虽然继承自MonoBehavior但是您依旧可以通过静态方法访问数据，但这并不代表静态数据中的值会有效
		/// </summary>
		public static SystemData Data{
			get{
				if (_systemData == null){
					if (FindObjectOfType<GameSystem>()?.systemData != null){
						_systemData = FindObjectOfType<GameSystem>().systemData;
					} else{
						_systemData = new SystemData();
					}
				}
				return _systemData;
			}

			set => _systemData = value;
		}

		/// <summary>
		/// 记录着图系统信息(当前在播放和编辑的图系统等等)，虽然继承自MonoBehavior但是您依旧可以通过静态方法访问数据，但这并不代表静态数据中的值会有效
		/// </summary>
		public static GraphSystem.GraphSystemData GraphData{
			get{
				if (_graphSystem == null){
					if (FindObjectOfType<GraphSystem>()?.GraphData != null){
						_graphSystem = FindObjectOfType<GraphSystem>().GraphData;
					} else{
						_graphSystem = new GraphSystem.GraphSystemData();
					}
				}
				CheckWindow();
				return _graphSystem;
			}

			set{
				Debug.Log(value);
				GameObject.FindObjectOfType<GraphSystem>().GraphData = _graphSystem = value;
			}
		}
		
		private void OnValidate(){
// #if UNITY_EDITOR
// 			if (currentSavableConfig) StaticData.currentSavableConfigPath = AssetDatabase.GetAssetPath(currentSavableConfig);
// #endif
// 			
		}
		
		private void Awake(){
			Data = systemData;
			InitialGameSystem(true);
			CheckWindow();
		}

		private void Start(){
			if (
#if UNITY_EDITOR
				EditorApplication.isPlaying &&
#endif
				true){
				if(systemData?.CurrentSceneModel) systemData.SceneController.GoToScene(systemData.CurrentSceneModel);
				
			}
		}

		private static void CheckWindow(){
#if UNITY_EDITOR
			if (EditorWindow.HasOpenInstances<PlotItemEditorWindow>()){
				GameObject.FindObjectOfType<GraphSystem>().GraphData.currentGfuPlotItemEditorWindow = EditorWindow.GetWindow<PlotItemEditorWindow>();
			}else if (EditorWindow.HasOpenInstances<PlotFlowEditorWindow>()){
				GameObject.FindObjectOfType<GraphSystem>().GraphData.currentGfuPlotFlowEditorWindow = EditorWindow.GetWindow<PlotFlowEditorWindow>();
			}
#endif
		}
		

		public void InitialGameSystem(bool chird){
			systemData.SceneController = InitialSystemComponent<SceneController>();
			systemData.RoleController = InitialSystemComponent<RoleController>();
			systemData.PlotFlowController = InitialSystemComponent<PlotFlowController>();
			systemData.ShowPlotView = InitialSystemComponent<ShowPlotView>();
			systemData.OptionController = InitialSystemComponent<OptionController>();
			if (FindObjectOfType<GfuRunOnMono>() == null) systemData.currentMonoProxy = gameObject.AddComponent<GfuRunOnMono>();
			else systemData.currentMonoProxy = gameObject.GetComponent<GfuRunOnMono>();
			if(gameObject.GetComponent<GraphSystem>()==null) graphSystem = gameObject.AddComponent<GraphSystem>();
			if (!currentInstanceIDStorage){
				var findObjectsOfInstanceIDStorage = Resources.FindObjectsOfTypeAll<InstanceIDStorage>();
				if (findObjectsOfInstanceIDStorage != null && findObjectsOfInstanceIDStorage.Length > 0){
					currentInstanceIDStorage = findObjectsOfInstanceIDStorage[0];
				} else{
					currentInstanceIDStorage = ScriptableObject.CreateInstance<InstanceIDStorage>();
#if UNITY_EDITOR
					AssetDatabase.CreateAsset(currentInstanceIDStorage,"Assets/" +nameof(InstanceIDStorage) +(-currentInstanceIDStorage.GetInstanceID()) +".asset");
					AssetDatabase.SaveAssets();
#endif
				}
			}
			if (chird){
				systemData.SceneController.InitialSceneController();
				systemData.RoleController.InitialRoleController();
				systemData.PlotFlowController.InitialPlowFlowController();
				systemData.ShowPlotView.InitialView();
				systemData.OptionController.InitialView();
			}
		}

		/// <summary>
		/// 初始化系统组件，对场景内的全部组件遍历，将需要的组件移动当当前gameObject下(层级而非游戏中)
		/// </summary>
		/// <typeparam name="T">需要寻找的类型</typeparam>
		/// <returns>返回找到的第一个组件</returns>
		public T InitialSystemComponent<T>() where T : Component{
			T typeObj;
			if ((typeObj = GameObject.FindObjectOfType<T>()) == null){
				GameObject obj = new GameObject();
				obj.transform.parent = transform;
				obj.name = typeof(T).Name;
				return obj.AddComponent<T>();
			}
			if(typeObj is OptionController) return typeObj;
			else typeObj.transform.SetParent(transform);
			return typeObj;
		}

		// public InstanceIDStorage InstanceIDStorage{
		// 	get{
		// 		if (!_instanceIDStorage) _instanceIDStorage = ScriptableObject.Instantiate();
		// 		return _instanceIDStorage;
		// 	}
		// }

		// private InstanceIDStorage _instanceIDStorage;
			
		[Rename(nameof(currentInstanceIDStorage))]
		[SerializeField]
		public InstanceIDStorage currentInstanceIDStorage;
		[Rename(nameof(currentSavableConfig))]
		public SavableConfig currentSavableConfig;
		[Serializable]
		public class SystemData{

			[Rename(nameof(Language))]
			public GfuLanguage.LanguageEnum Language;

			public GameTime CurrentTime{
				get => currentTime;
				set => currentTime = value;
			}

			[Rename(nameof(currentTime))] [SerializeField]
			private GameTime currentTime = new GameTime();
			

			/// <summary>
			/// 一般情况下记录着主角的角色模型，当有程序修该值，触发事件
			/// </summary>
			public RoleModel CurrentRoleModel{
				get => currentRoleModel;
				set{
					currentRoleModel = value;
					EventCenter.GetInstance().RoleStateChangeEvent(currentRoleModel); //当有程序修改当前角色模型是，触发事件
				}
			}

			[Rename(nameof(currentRoleModel))] [SerializeField]
			private RoleModel currentRoleModel;

			/// <summary>
			/// 对此参数的修改会触发场景变换数据
			/// </summary>
			public SceneModel CurrentSceneModel{
				get{ return currentSceneModel ?? SceneModel.NULL; }
				set{
					currentSceneModel = value;
					EventCenter.GetInstance().SceneStateChangeEvent.Invoke(currentSceneModel); //当有程序修改当前场景模型是，触发事件
				}
			}
			
			[Rename(nameof(currentSceneModel))]
			[SerializeField]
			private SceneModel currentSceneModel;

			[Rename(nameof(SceneController))]
			public SceneController SceneController;

			[Rename(nameof(RoleController))]
			public RoleController RoleController;

			[Rename(nameof(PlotFlowController))]
			public PlotFlowController PlotFlowController;

			[Rename(nameof(ShowPlotView))]
			public ShowPlotView ShowPlotView;
			
			[Rename(nameof(OptionController))]
			public OptionController OptionController;
			
			[Rename(nameof(currentMonoProxy))]
			[SerializeField]
			public GfuRunOnMono currentMonoProxy;
			
			// [HideInInspector]
			// [Rename(nameof(currentMonoProxy))]
			// public string currentSavableConfigPath;

		}

		//TODO 恢复内存中的图
		public override void GetObjectData(){
			
		}
		public override void Recover(){
			
		}
#if UNITY_EDITOR
		//
		// [PostProcessScene(2)]
		// public static void OnBuild(){
		// 	// var gameSystem = GameSystem.GetInstance();
		// 	// gameSystem.AutoAddGfuCompoenent(gameSystem.transform);
		// 	// gameSystem.TraverseTheSubclass(gameSystem.transform);
		// }
		
		void TraverseTheSubclass(Transform transform){
			foreach (var subCalss in transform){
				var calss = (Transform) subCalss;
				AutoAddGfuCompoenent(calss);
				// TraverseTheSubclass(calss);
			}
		}
		/// <summary>
		/// 为可保存的组件的添加GfuInstance,
		/// 即该组件在SavableConfig中添加或者继承了Savable
		/// </summary>
		/// <param name="transform"></param>
		void AutoAddGfuCompoenent(Transform transform){
			var components = transform.GetComponents<MonoBehaviour>();
			foreach (var component in components){
				var type = component.GetType();
				var gfuInstance = component.GetComponent<GfuInstance>();
				if (type.IsSubclassOf(typeof(SavableBehaviour)) ||(currentSavableConfig&&currentSavableConfig.Contains(type))){
					if(!gfuInstance) component.gameObject.AddComponent<GfuInstance>();
					if(gfuInstance)AutoParent(component.transform);
					break;
				}
				if(gfuInstance) AutoParent(component.transform);
			}
		}
		//为含有GfuInstance组件的所有父组件添加GfuInstance
		void AutoParent(Transform transform){
			var transformParent = transform;
			while (transformParent.parent!=null){
				transformParent = transformParent.parent;
				if(transformParent.GetComponent<GfuInstance>() ==null) transformParent.gameObject.AddComponent<GfuInstance>();
			}
			
		}
		
#endif
		
		//存档系统支持
		public void Save(int index) => ArchiveSystem.GetInstance().SaveAsync(index);
		public void Save() => ArchiveSystem.GetInstance().SaveAsync(0);
		public void Load(int index) => ArchiveSystem.GetInstance().LoadAsync(index);
		public void Load() => ArchiveSystem.GetInstance().LoadAsync(0);
	}

}
