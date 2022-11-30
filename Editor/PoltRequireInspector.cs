using System;
using GalForUnity.Attributes;
using GalForUnity.Model;
using GalForUnity.Model.Plot;
using GalForUnity.View;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GalForUnity.Editor {
    // [CustomEditor(typeof(PlotRequire))]
    public sealed class PoltRequireInspector : BaseEditor {

        private SerializedObject _obj;
        // private PlotRequire _testA;
        private SerializedProperty _replacePlot;
        private SerializedProperty _triggerPlot;
        private SerializedProperty _startTime;
        private SerializedProperty _overTime;
        private SerializedProperty _roleData;
        private ReorderableList _sceneModels;
        private SerializedProperty _sceneModel;
        private SerializedProperty _priority;
        private SerializedProperty _replaceCount;
        private SerializedProperty _overDay;
        private SerializedProperty _probability;
        protected override void OnEnable() {
            base.OnEnable();
            // _testA = (PlotRequire)target;
            _obj = new SerializedObject(target);
            _startTime = _obj.FindProperty("startTime");
            _overTime = _obj.FindProperty("overTime");
            _roleData = _obj.FindProperty("roleData");
            _sceneModel = _obj.FindProperty("sceneModels");
            _probability = _obj.FindProperty("plotProbability");
            _sceneModels=new ReorderableList(serializedObject,_sceneModel,true,true,true,true); 
            //自定义列表名称
            //_sceneModels.drawFooterCallback
            _sceneModels.drawHeaderCallback = (Rect rect) =>
            {
                
                EditorGUI.LabelField(rect, "场景要求列表");
                GUI.tooltip = _priority.tooltip;
            };
            //自定义绘制列表元素
            _sceneModels.drawElementCallback = (Rect rect,int index,bool selected,bool focused) =>
            {
                //根据index获取对应元素
                SerializedProperty item = _sceneModels.serializedProperty.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.y += 1;
                // rect.y += 10;
                EditorGUI.PropertyField(rect, item, new GUIContent("场景要求 " +index));
            };
            _sceneModels.onRemoveCallback = (list) => {
                if (list.serializedProperty != null){
                    list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
                    list.serializedProperty.arraySize--;
                    list.index = list.serializedProperty.arraySize - 1;
                    
                } else{
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
            _sceneModels.onAddCallback = (list) => {
                if (list.serializedProperty != null){
                    list.serializedProperty.arraySize++;
                    list.index=list.serializedProperty.arraySize-1;
                    SerializedProperty serialized= list.serializedProperty.GetArrayElementAtIndex(list.index);
                    serialized.objectReferenceValue = null;
                } else{
                    ReorderableList.defaultBehaviours.DoAddButton(list);
                }
            };
            _priority = _obj.FindProperty("priority");
            _replacePlot = _obj.FindProperty("replacePlot");
            _triggerPlot = _obj.FindProperty("triggerPlot");
            _replaceCount = _obj.FindProperty("replaceCount");
            _overDay = _obj.FindProperty("overDay");

        }
        /// <summary>
        /// 用于显示的枚举，不参与程序过程
        /// </summary>
        // public enum TypeInInspector {
        //
        //     常规 = PlotType.date,
        //
        //     固定 = PlotType.Fixed,
        //
        //     特殊 = PlotType.Special,
        //
        //     替代 = PlotType.plot
        // }

   //      public TypeInInspector type;
   //      public override void OnInspectorGUI() {
   //          _obj.Update();
   //          CheckRemove(_sceneModels);
   //          DrawMonoScript();
   //          type = (TypeInInspector)_testA.plotType;
   //          type = (TypeInInspector)EditorGUILayout.EnumPopup("剧情类型", type);
			// EditorGUILayout.PropertyField(_startTime, new GUIContent("剧情开始时间"));
   //          if ((int)type != (int)PlotType.Fixed) {
   //              EditorGUILayout.PropertyField(_overTime, new GUIContent("剧情结束时间"));
   //          }
   //          if ((int)type == (int)PlotType.plot) {
   //              //EditorGUILayout.PropertyField(_roleData, new GUIContent("角色数据要求"));
   //              // EditorGUILayout.PropertyField(_sceneModel, new GUIContent("场景要求"));
   //              _sceneModels.DoLayoutList();
   //              EditorGUILayout.PropertyField(_triggerPlot, new GUIContent("触发条件剧情"));
   //              EditorGUILayout.PropertyField(_replacePlot, new GUIContent("被替换的剧情"));
   //              EditorGUILayout.PropertyField(_replaceCount, new GUIContent("重复的次数"));
   //              //EditorGUILayout.PropertyField(height);
   //          } else if ((int)type == (int)PlotType.Fixed) {
   //
   //              //EditorGUILayout.PropertyField(height);
   //          } else if ((int)type == (int)PlotType.date) {
   //              EditorGUILayout.PropertyField(_roleData, new GUIContent("角色数据要求"));
   //              // EditorGUILayout.PropertyField(_sceneModel, new GUIContent("场景要求"));
   //              _sceneModels.DoLayoutList();
   //              
   //          } else {
   //              EditorGUILayout.PropertyField(_roleData, new GUIContent("角色数据要求"));
   //              // EditorGUILayout.PropertyField(_sceneModel, new GUIContent("场景要求"));
   //              _sceneModels.DoLayoutList();
   //          }
   //          //EditorGUILayout.PropertyField(polt);
   //          EditorGUILayout.PropertyField(_priority, new GUIContent("当前剧情的优先级"));
   //          EditorGUILayout.PropertyField(_overDay, new GUIContent("是否结束本日"));
   //          EditorGUILayout.PropertyField(_probability, new GUIContent("剧情触发率(百分之)"));
   //          _obj.ApplyModifiedProperties();
   //          // serializedObject.ApplyModifiedProperties();
   //          _testA.plotType = (PlotType)type;
   //      }
        
    }
    // [CustomEditor(typeof(PlotModel))]
    public class PlotModelInspector : UnityEditor.Editor {
        private void OnEnable() {
            // if (!_firstFlag) {
            //     _firstFlag = true;
            //     try {
            //         PlotModel plot = (PlotModel)target;
            //         if (plot.TryGetComponent(out PlotRequire poltRequire)) {
            //             plot.plotRequire = poltRequire;//自动让PoltModel和PoltRequire绑定
            //         }
            //         if (plot.TryGetComponent(out PlotFlow poltFlow)) {
            //             plot.plotFlow = poltFlow;//自动让PoltModel和PoltRequire绑定 = poltRequire;//自动让PoltModel和PoltRequire绑定
            //         }
            //     } catch (Exception e) {
            //         Debug.LogError(e);
            //     }
            // }
        }
    }
    [CustomEditor(typeof(BackgroundAutoSize))]
    public class BackgroundAutoSizeInspector : BaseEditor {
        private SerializedProperty _customMethod;
        private SerializedProperty _renderingMode;
        private SerializedProperty _startIndex;
        private SerializedObject _obj;

        protected override void OnEnable() {
            base.OnEnable();
            _obj = new SerializedObject(target);
            _customMethod = serializedObject.FindProperty("customMethod");
            _renderingMode = serializedObject.FindProperty("renderingMode");
        }
        public override void OnInspectorGUI(){
            serializedObject.Update();
            DrawMonoScript();
            var renameAttribute = _renderingMode.GetFirstAttribute<RenameAttribute>();
            var guiContent = new GUIContent(){
                tooltip = renameAttribute.ToolTip, text = renameAttribute.Name
            };
            EditorGUILayout.PropertyField(_renderingMode, guiContent);
            if(_renderingMode.enumValueIndex==(byte)RenderingMode.Custom)
            {
                GUILayout.Space(20);
                guiContent.tooltip = "指定图片应当做何种处理，默认执行等比缩放";
                guiContent.text = "自定义方法";
                EditorGUILayout.PropertyField(_customMethod, guiContent);
                
            }
            serializedObject.ApplyModifiedProperties();
        }
    
    }
#pragma warning disable 612
    // [CustomEditor(typeof(ActionModel))]
#pragma warning restore 612
    public class ActionModelInspector : BaseEditor {
        private SerializedProperty _actionModelType;
        private SerializedProperty _directionSceneModel;
        private SerializedProperty _plotModel;
        private SerializedProperty _roleData;
        private SerializedProperty _customEvent;
        private SerializedProperty _customEventNoParam;
        private SerializedProperty _probability;

        protected override void OnEnable() {
            base.OnEnable();
            _actionModelType = serializedObject.FindProperty("actionModelType");
            _directionSceneModel = serializedObject.FindProperty("directionSceneModel");
            _plotModel = serializedObject.FindProperty("directionPlotModel");
            _roleData = serializedObject.FindProperty("roleData");
            _customEvent = serializedObject.FindProperty("customEvent");
            _customEventNoParam = serializedObject.FindProperty("customEventNoParam");
            _probability = serializedObject.FindProperty("changeProbability");
        }
        
        public override void OnInspectorGUI(){
#pragma warning disable 612
            // GameObject gameObj = ((ActionModel)target).gameObject;
            // serializedObject.Update();
            // DrawMonoScript();
            // EditorGUILayout.PropertyField(_actionModelType, new GUIContent(new GUIContent(_actionModelType.GetFirstAttribute<RenameAttribute>().Name)));
            // if (_actionModelType.enumValueIndex == (byte) ActionModel.ActionModelType.Custom){
            //     EditorGUILayout.PropertyField(_customEvent, new GUIContent(_customEvent.GetFirstAttribute<RenameInEditorAttribute>().LanguageItem.Value));
            //     EditorGUILayout.PropertyField(_customEventNoParam, new GUIContent(_customEventNoParam.GetFirstAttribute<RenameInEditorAttribute>().LanguageItem.Value));
            // } else if(_actionModelType.enumValueIndex == (byte) ActionModel.ActionModelType.JumpPlot){
            //     EditorGUILayout.PropertyField(_plotModel, new GUIContent(_plotModel.GetFirstAttribute<RenameInEditorAttribute>().LanguageItem.Value));
            // } else if(_actionModelType.enumValueIndex == (byte) ActionModel.ActionModelType.ChangePlotProbability){
            //     EditorGUILayout.PropertyField(_plotModel, new GUIContent(_plotModel.GetFirstAttribute<RenameInEditorAttribute>().LanguageItem.Value));
            //     EditorGUILayout.PropertyField(_probability, new GUIContent(_probability.GetFirstAttribute<RenameInEditorAttribute>().LanguageItem.Value));
            // } else if(_actionModelType.enumValueIndex == (byte) ActionModel.ActionModelType.GotoScene){
            //     EditorGUILayout.PropertyField(_directionSceneModel, new GUIContent(_directionSceneModel.GetFirstAttribute<RenameInEditorAttribute>().LanguageItem.Value));
            // } else if(_actionModelType.enumValueIndex == (byte) ActionModel.ActionModelType.GrowUp){
            //     EditorGUILayout.PropertyField(_roleData, new GUIContent(_roleData.GetFirstAttribute<RenameInEditorAttribute>().LanguageItem.Value));
            //     if (_roleData.objectReferenceValue == null){
            //         if (!gameObj.TryGetComponent<RoleData>(out RoleData roleData)){
            //             _roleData.objectReferenceValue = gameObj.AddComponent<RoleData>();
            //         } else{
            //             _roleData.objectReferenceValue = roleData;
            //         }
            //     }
            // }
            // if (_actionModelType.enumValueIndex != (byte) ActionModel.ActionModelType.GrowUp){
            //     if (gameObj.TryGetComponent<RoleData>(out RoleData roleData)){
            //         Component.DestroyImmediate(roleData,true);
            //     }
            // }
            // serializedObject.ApplyModifiedProperties();
#pragma warning restore 612
        }
    }

}