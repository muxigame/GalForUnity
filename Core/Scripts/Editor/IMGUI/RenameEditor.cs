using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace GalForUnity.Core.Editor{
   
    /// <summary>
/// 重命名 编辑器
/// <para>ZhangYu 2018-06-21</para>
/// </summary>
/// 
[CanEditMultipleObjects]
// [CustomEditor(typeof(PlotRequire))]
    public class RenameEditor : BaseEditor {

    // 绘制GUI
    public override void OnInspectorGUI() {
        var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<RenameEditorData>("Assets/NxGIpJnafYxjOfgaY.asset");
        if(!EditorData&&!loadAssetAtPath) EditorData = ScriptableObject.CreateInstance<RenameEditorData>();
        else if (loadAssetAtPath) EditorData = loadAssetAtPath;
        if (!EditorData.Foldout.ContainsKey(target)&&EditorPrefs.HasKey(GetType() +":" +target.GetType())) EditorData.Foldout[target]=EditorPrefs.GetBool(GetType() +":" +target.GetType().ToString());
        EditorGUI.BeginChangeCheck();
        // Debug.Log("in0");
        DrawMonoScript();
        drawProperties(target.GetType());
        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
    }
    public static RenameEditorData EditorData;
    private void OnDestroy(){
        if (!(EditorData is null) && EditorData.Foldout != null){
            foreach (var keyValuePair in EditorData.Foldout){
                EditorPrefs.SetBool(GetType() +":" +keyValuePair.Key.GetType().ToString(),keyValuePair.Value);
            }
        }
    }

    // 绘制属性
    private void drawProperty(string property, string label) {
        SerializedProperty pro = serializedObject.FindProperty(property);
        bool enterChildren = true;
        if (pro != null){
            if (pro.hasVisibleChildren&&pro.propertyType == SerializedPropertyType.Generic &&pro.GetValue<UnityEventBase>()==null){
#region Foldout
                if (!EditorData.Foldout.ContainsKey(target)) EditorData.Foldout[target] = false;
                EditorData.Foldout[target] = EditorGUILayout.Foldout(EditorData.Foldout[target], GfuLanguage.Parse(pro.name),true);
                if (EditorData.Foldout[target]){
                    EditorGUI.indentLevel++;
                    var endProperty = pro.GetEndProperty();
                    while (pro.NextVisible(enterChildren) &&!SerializedProperty.EqualContents(pro, endProperty)){
                        RenameInEditorAttribute[] atts = pro.GetAttributes<RenameInEditorAttribute>() as RenameInEditorAttribute[];
                        if (atts!=null&&atts.Length > 0){
                            enterChildren=EditorGUILayout.PropertyField(pro, new GUIContent(atts[0]?.name != null ? atts[0].name:GfuLanguage.Parse(pro.name)), true);
                        } else{
                            enterChildren=EditorGUILayout.PropertyField(pro, new GUIContent(GfuLanguage.Parse(pro.name)), false);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
#endregion
            } else{
                EditorGUILayout.PropertyField(pro, new GUIContent(GfuLanguage.Parse(label)), false);
            }
            // Debug.Log("in2");
        }
        
    }

    // 绘制所有属性
    private void drawProperties(Type typeo) {
        // 获取类型和可序列化属性
        Type type = typeo;
        List<FieldInfo> fields = new List<FieldInfo>();
        FieldInfo[] array = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        fields.AddRange(array);

        // 获取父类的可序列化属性
        while (IsTypeCompatible(type.BaseType) && type != type.BaseType) {
            type = type.BaseType;
            array = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            fields.InsertRange(0, array);
        }
        
        // 绘制所有属性
        for (int i = 0; i < fields.Count; i++) {
            FieldInfo field = fields[i];

            // 非公有但是添加了[SerializeField]特性的属性
            if (!field.IsPublic) {
                object[] serials = field.GetCustomAttributes(typeof(SerializeField), true);
                if (serials.Length == 0) continue;
            }
            
            // 公有但是添加了[HideInInspector]特性的属性
            object[] hides = field.GetCustomAttributes(typeof(HideInInspector), true);
            if (hides.Length != 0) continue;

            // 绘制符合条件的属性

            
            drawProperty(field.Name, field.Name );
        }

    }

    // 脚本类型是否符合序列化条件
    private bool IsTypeCompatible(Type type) {
        if (type == null || !(type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject))))
            return false;
        return true;
    }

}

}