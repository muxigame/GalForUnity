using System.Collections.Generic;
using GalForUnity.Model;
using GalForUnity.Model.Plot;
using GalForUnity.System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalForUnity.Editor
{
    public class BaseEditor: UnityEditor.Editor{
        // ReSharper disable all MemberCanBePrivate.Global
        private Object monoScript;

        protected virtual void OnEnable(){
            monoScript = MonoScript.FromMonoBehaviour(this.target as MonoBehaviour);
        }

        protected bool CheckRemove(ReorderableList reorderableList){
            if (reorderableList.count > 0){
                return reorderableList.displayRemove = true;
            }
            return reorderableList.displayRemove = false;
        }

        public T GetFirstAttribute<T>(SerializedProperty serializedProperty) where T : class{
            if (serializedProperty.HasAttribute<T>()){
                return (T) serializedProperty.GetAttributes<T>()[0];
            }

            return null;
        }
        protected void DrawMonoScript()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", this.monoScript, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
        }
    }
    public static class Extern{
        public static T GetFirstAttribute<T>(this SerializedProperty serializedProperty) where T : class{
            if (serializedProperty.HasAttribute<T>()){
                return (T) serializedProperty.GetAttributes<T>()[0];
            }
            return null;
        }
    }
    [CustomEditor(typeof(RoleData))]
    public class ReorderableListEditor : BaseEditor
    {
        private ReorderableList _roleDataItem;
        private readonly string[] _defaultNameList=new string[]{"力量","灵感","节奏","运动","爱心","魅力","心情","潜能","好感度"};
        protected override void OnEnable(){
            base.OnEnable();
            _roleDataItem = new ReorderableList(serializedObject, serializedObject.FindProperty("dataArray")
                , true, true, true, true);
            //自定义列表名称
            _roleDataItem.drawHeaderCallback = (Rect rect) => {
                //rect.y += 20;
                GUI.Label(rect, GfuLanguage.Parse("roleData"));
            };
            //自定义绘制列表元素
            _roleDataItem.drawElementCallback = (Rect rect, int index, bool selected, bool focused) => {
                //根据index获取对应元素
                SerializedProperty item = _roleDataItem.serializedProperty.GetArrayElementAtIndex(index);
                rect.height = EditorGUIUtility.singleLineHeight;
                // rect.y += 1;
                EditorGUI.PropertyField(rect, item, new GUIContent("Element " + index));
            };
            //当添加新元素时的回调函数，自定义新元素的值
            _roleDataItem.onAddCallback = (ReorderableList list) => {
                if (list.serializedProperty != null){
                    list.serializedProperty.arraySize++;
                    list.index = list.serializedProperty.arraySize - 1;
                    SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(list.index);
                    //item.stringValue = "Default Value";
                    foreach (SerializedProperty serialized in item){
                        // Debug.Log(serialized.name);
                        // Debug.Log(serialized.displayName);
                        if (serialized.name == "name"){
                            serialized.stringValue = list.count <= _defaultNameList.Length ? _defaultNameList[list.count - 1] : "default";
                        }
                        if (serialized.name == "value"){
                            serialized.intValue = 1;
                        }
                    }

                    serializedObject.ApplyModifiedProperties();
                } else{
                    ReorderableList.defaultBehaviours.DoAddButton(list);
                }
            };

            //当删除元素时候的回调函数，实现删除元素时，有提示框跳出
            _roleDataItem.onRemoveCallback = (ReorderableList list) => {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                // if (EditorUtility.DisplayDialog("Warnning","Do you want to remove this element?","Remove","Cancel"))
                // {
                //     
                // }
            };
            _roleDataItem.onCanRemoveCallback = (list) => {
                // Debug.Log(list.count);
                if (list.count > 0){
                    return true;
                }

                return false;
            };
            // _roleDataItem.elementHeight = EditorGUIUtility.singleLineHeight+1.5f;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawMonoScript();
            //自动布局绘制列表
            _roleDataItem.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }

#pragma warning disable 612
    [CustomEditor(typeof(PlotFlow))]
#pragma warning restore 612
    public class PoltItemReorderableListEditor : BaseEditor{
        private ReorderableList _reorderableList;
        private SerializedObject _plotAnimation;
        private ReorderableList _role;
        private SerializedProperty _startIndex;
        private SerializedProperty _plotFlowType;
        private SerializedProperty _plotItemGraph;

        protected override void OnEnable(){
            base.OnEnable();
            _startIndex = serializedObject.FindProperty("startIndex");
            _plotFlowType = serializedObject.FindProperty("PlotFlowType");
            _plotItemGraph = serializedObject.FindProperty("PlotItemGraph");
            _role = new ReorderableList(serializedObject, serializedObject.FindPropsOfType<List<RoleModel>>()[0]){
                elementHeight = UnityEditor.EditorGUIUtility.singleLineHeight,
                drawElementCallback = (rect, index, selected, focused) => {
                    SerializedProperty item = _role.serializedProperty.GetArrayElementAtIndex(index);
                    // rect.height =EditorGUIUtility.singleLineHeight;
                    // rect.y += 1;
                    EditorGUI.PropertyField(rect, item, label: new GUIContent("登场角色" + index));
                },drawHeaderCallback = (rect) => {
                    GUI.Label(rect,"登场角色列表");
                },onRemoveCallback = (list)=>{
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                },onAddCallback = (list) => {
                    if (list.serializedProperty != null){
                        list.serializedProperty.arraySize++;
                        list.index = list.serializedProperty.arraySize - 1;
                        SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(list.index);
                        item.objectReferenceValue = null;
                        serializedObject.ApplyModifiedProperties();
                    } else{
                        ReorderableList.defaultBehaviours.DoAddButton(list);
                    }
                }
            };
            _reorderableList = new ReorderableList(serializedObject, serializedObject.FindPropsOfType<List<PlotItem>>()[0]){
                elementHeight = 108,
                drawElementCallback = (rect, index, selected, focused) => {
                    SerializedProperty item = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, item, label: new GUIContent("剧情项" + index));
                    // var subrect = new Rect(rect){
                    //     y = rect.y + rect.height,height = 40
                    // };
                    // rects.Add(subrect);
                },
                onAddCallback = (list) => {
                    if (list.serializedProperty != null){
                        list.serializedProperty.arraySize++;
                        list.index = list.serializedProperty.arraySize - 1;
                        SerializedProperty item = list.serializedProperty.GetArrayElementAtIndex(list.index);
                        // item = null;
                        foreach (SerializedProperty o in item){ 
                            if(o.name.Contains("anim"))o.objectReferenceValue=null;
                        }
                        serializedObject.ApplyModifiedProperties();
                    } else{
                        ReorderableList.defaultBehaviours.DoAddButton(list);
                    }
                },drawHeaderCallback = (rect) => {
                    GUI.Label(rect,"剧情流");
                },onRemoveCallback = (list) => {
                    // _plotAnimationReorderableList.RemoveAt(list.index);
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
            
            // _reorderableList.elementHeight += 50;
        }

        public override void OnInspectorGUI(){
            serializedObject.Update();
            DrawMonoScript();
            EditorGUILayout.PropertyField(_plotFlowType, new GUIContent(GfuLanguage.Parse(nameof(_plotFlowType))));
            // EditorGUILayout.PropertyField(_role, new GUIContent("出场角色"));
            if (_plotFlowType.enumValueIndex == 0){
                _role.DoLayoutList();
                EditorGUILayout.PropertyField(_startIndex, new GUIContent(GfuLanguage.Parse(nameof(_startIndex))));
            }
            else EditorGUILayout.ObjectField(_plotItemGraph, new GUIContent(GfuLanguage.Parse("PlotItemGraph")));
            
           
            // EditorGUILayout.PropertyField(_plotsItem, new GUIContent("剧情流"));
            if (_plotFlowType.enumValueIndex == 0) _reorderableList.DoLayoutList();
            
            // for (var i = 0; i < _plotAnimationReorderableList.Count; i++){
            //     _plotAnimationReorderableList[i].DoList(rects[i]);
            // }
            
            serializedObject.ApplyModifiedProperties();
            // base.OnInspectorGUI();
        }
    }

    
    
}