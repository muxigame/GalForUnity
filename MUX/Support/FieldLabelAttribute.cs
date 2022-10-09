using System;
using UnityEngine;

namespace MUX.Support{
    /// <summary>
    /// 能让字段在inspect面板显示中文字符
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FieldLabelAttribute : PropertyAttribute {
        public string label;//要显示的字符
        public FieldLabelAttribute(string label) {
            this.label = label;
            //获取你想要绘制的字段（比如"技能"）
        }
    }
}