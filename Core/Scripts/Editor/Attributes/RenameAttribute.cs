using System;
using System.Reflection;
using System.Text.RegularExpressions;
using GalForUnity.Core.External;
using GalForUnity.Framework;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace GalForUnity.Core.Editor{
#if UNITY_EDITOR
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Class)]
#endif
    public class RenameAttribute : PropertyAttribute{
        /// <summary> 枚举名称 </summary>
        public readonly string Name = "";

        /// <summary>
        /// 语言项
        /// </summary>
        public readonly LanguageItem LanguageItem;

        /// <summary>
        /// 字符串提示
        /// </summary>
        public readonly string ToolTip = "";

        /// <summary> 文本颜色 </summary>
        public readonly string HtmlColor = "#ffffff";

        /// <summary> 重命名属性 </summary>
        /// <param name="name">新名称</param>
        public RenameAttribute(string name){
            if (new Regex(@"[a-zA-Z]+").IsMatch(name)){
                try{
                    LanguageItem = (LanguageItem) typeof(GfuLanguage).GetField(name.ToUpper()).GetValue(GfuLanguage.GfuLanguageInstance);
                } catch (Exception){
                    Name = name;
                }
            } else{
                Name = name;
            }
        }

        public RenameAttribute(Type languageItem, string name){ LanguageItem = (LanguageItem) languageItem.GetField(name.ToUpper()).GetValue(GfuLanguage.GfuLanguageInstance); }

        public RenameAttribute(string name, string toolTipOrColor){
            if (new Regex(@"[a-zA-Z]+").IsMatch(name)){
                LanguageItem = (LanguageItem) typeof(GfuLanguage).GetField(name.ToUpper()).GetValue(GfuLanguage.GfuLanguageInstance);
            } else{
                Name = name;
            }

            if (toolTipOrColor.IndexOf("#", StringComparison.Ordinal) == 0){
                HtmlColor = toolTipOrColor;
            } else{
                ToolTip = toolTipOrColor;
            }
        }

        public RenameAttribute(string name, string toolTip, string htmlColor){
            if (new Regex(@"[a-zA-Z]+").IsMatch(name)){
                LanguageItem = (LanguageItem) typeof(GfuLanguage).GetField(name.ToUpper()).GetValue(GfuLanguage.GfuLanguageInstance);
            } else{
                Name = name;
            }

            ToolTip = toolTip;
            HtmlColor = htmlColor;
        }

        /// <summary> 重命名属性 </summary>
        /// <param name="name">新名称</param>
        /// <param name="htmlColor">文本颜色 例如："#FFFFFF" 或 "black"</param>
        public RenameAttribute(string name, Color htmlColor){
            Name = name;
            HtmlColor = htmlColor.ToString();
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RenameAttribute))]
    public class RenameDrawer : PropertyDrawer{
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            if (!LogicDrawer.CanShow(property)) return 0;
            float baseHeight = base.GetPropertyHeight(property, label)+2;
            if (property.isExpanded){
                if (property.propertyType == SerializedPropertyType.Generic){
                    if (property.hasChildren){
                        if(property.depth!=0) return (EditorGUIUtility.singleLineHeight +2) * (property.CountInProperty());
                        return baseHeight + (EditorGUIUtility.singleLineHeight +2) * (property.CountInProperty());
                    }
                    // - property.depth
                    return baseHeight;
                }
            }
            return baseHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            // Debug.Log(property.name);
            // Debug.Log(label.text);
            using (new EditorGUI.PropertyScope(position, label, property)){
                if (!LogicDrawer.CanShow(property)) return;

                RenameAttribute rename = (RenameAttribute) attribute;
                if (rename.LanguageItem != null){
                    label.text = rename.LanguageItem.Value;
                } else{
                    label.text = rename.Name;
                }

                if (String.IsNullOrEmpty(rename.ToolTip)){
                    label.tooltip = rename.ToolTip;
                }
                // 重绘GUI
                Color defaultColor = EditorStyles.label.normal.textColor;
                EditorStyles.label.normal.textColor = htmlToColor(rename.HtmlColor);
                if (property.propertyType == SerializedPropertyType.Enum){
                    DrawEnum(position, property, label);
                } else if (property.propertyType == SerializedPropertyType.Generic){
                    EditorGUI.PropertyField(position, property, label, true);
                } else if (property.propertyType == SerializedPropertyType.Float){
                    var attributes = (RangeAttribute)property.GetAttribute<RangeAttribute>();
                    if (attributes!=null){
                        EditorGUI.Slider(position, property, attributes.min, attributes.max, label);
                    } else{
                        property.floatValue = EditorGUI.FloatField(position, label,property.floatValue);
                    }
                } else{
                    EditorGUI.PropertyField(position, property, label);
                }
                
                EditorStyles.label.normal.textColor = defaultColor;
            }
            // 替换属性名称
        }

        // 绘制枚举类型
        private void DrawEnum(Rect position, SerializedProperty property, GUIContent label){
            EditorGUI.BeginChangeCheck();
            // 获取枚举相关属性
            Type type = fieldInfo.FieldType;
            string[] names = property.enumNames;
            string[] values = new string[names.Length];
            Array.Copy(names, values, names.Length);
            while (type.IsArray) type = type.GetElementType();
            // 获取枚举所对应的RenameAttribute
            for (int i = 0; i < names.Length; i++){
                FieldInfo info = type.GetField(names[i]);
                RenameAttribute[] atts = (RenameAttribute[]) info.GetCustomAttributes(typeof(RenameAttribute), true);
                if (atts.Length != 0){
                    for (int j = 0; j < atts.Length; j++){
                        if (atts[0].LanguageItem != null){
                            values[i] = atts[0].LanguageItem.Value;
                        } else{
                            values[i] = !string.IsNullOrEmpty(atts[0].Name)?atts[0].Name:names[i];
                        }
                    }
                }
            }

            // 重绘GUI
            int index = EditorGUI.Popup(position, label.text, property.enumValueIndex, values);
            if (EditorGUI.EndChangeCheck() && index != -1) property.enumValueIndex = index;
        }

        /// <summary> Html颜色转换为Color </summary>
        /// <param name="hex">字符串颜色</param>
        /// <returns>返回Unity的Color类</returns>
        public static Color htmlToColor(string hex){
            hex = hex.ToLower();
            if (!String.IsNullOrEmpty(hex)){
                ColorUtility.TryParseHtmlString(hex, out Color color);
                return color;
            }

            return new Color(0.705f, 0.705f, 0.705f);
        }
    }

#endif

    /// <summary>
    /// 添加标题属性
    /// </summary>
#if UNITY_EDITOR
    [AttributeUsage(AttributeTargets.Field)]
#endif
    public class TitleAttribute : PropertyAttribute{
        /// <summary> 标题名称 </summary>
        public string title = "";

        /// <summary> 文本颜色 </summary>
        public string htmlColor = "#B3B3B3";

        public LanguageItem LanguageItem;

        /// <summary> 在属性上方添加一个标题 </summary>
        /// <param name="title">标题名称</param>
        public TitleAttribute(string title){
            if (new Regex(@"[a-zA-Z]+").IsMatch(title)){
                LanguageItem = (LanguageItem) typeof(GfuLanguage).GetField(title.ToUpper()).GetValue(GfuLanguage.GfuLanguageInstance);
            } else{
                this.title = title;
            }
        }

        /// <summary> 在属性上方添加一个标题 </summary>
        /// <param name="title">标题名称</param>
        /// <param name="htmlColor">文本颜色 例如："#FFFFFF" 或 "black"</param>
        public TitleAttribute(string title, string htmlColor){
            this.title = title;
            this.htmlColor = htmlColor;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleAttributeDrawer : DecoratorDrawer{
        // 文本样式
        private GUIStyle style = new GUIStyle(EditorStyles.label);

        public override void OnGUI(Rect position){
            // 获取Attribute
            TitleAttribute rename = (TitleAttribute) attribute;

            style.fixedHeight = 18;
            style.normal.textColor = RenameDrawer.htmlToColor(rename.htmlColor);

            // 重绘GUI
            position = EditorGUI.IndentedRect(position);
            if (rename.LanguageItem != null){
                GUI.Label(position, rename.LanguageItem.Value, style);
            } else{
                GUI.Label(position, rename.title, style);
            }
        }

        public override float GetHeight(){ return base.GetHeight() - 3; }
    }
#endif

    /// <summary>
    /// 重命名脚本编辑器中的属性名称
    /// </summary>
#if UNITY_EDITOR
    [AttributeUsage(AttributeTargets.Field)]
#endif
    public class RenameInEditorAttribute : PropertyAttribute{
        /// <summary> 新名称 </summary>
        public string name = "";
        
        /// <summary>
        /// 语言项
        /// </summary>
        public readonly LanguageItem LanguageItem;

        /// <summary> 重命名属性 </summary>
        /// <param name="name">新名称</param>
        public RenameInEditorAttribute(string name){
            if (new Regex(@"[a-zA-Z]+").IsMatch(name)){
                LanguageItem = (LanguageItem) typeof(GfuLanguage).GetField(name.ToUpper()).GetValue(GfuLanguage.GfuLanguageInstance);
                this.name = LanguageItem.Value;
            } else{
                this.name = name;
            }
        }
    }
}