//======================================================================
//
//       CopyRight 2019-2022 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  GfuConfigFieldUXml.cs Created at 2022-09-27 22:53:31
//
//======================================================================

using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Block{
    public sealed class GfuConfigFieldUXml<T> : VisualElement{
        private T _value;

        public GfuConfigFieldUXml(Action<T> action,object defaultValue=null){
            _value = (T)defaultValue;
            VisualElement field = null;
            if (typeof(T) == typeof(bool)){
                field = new Toggle(){value = defaultValue != null && (bool) defaultValue};
            }
            if (typeof(T) == typeof(float)){
                field = new FloatField(){value = defaultValue == null ? 0 : (float)defaultValue};
            }
            if (typeof(T) == typeof(double)){
                field = new DoubleField(){value = defaultValue == null ? 0 : (double)defaultValue};
            }
            if (typeof(T) == typeof(int)){
                field = new IntegerField(){value = defaultValue == null ? 0 : (int)defaultValue};
            }
            if (typeof(T) == typeof(long)){
                field = new LongField(){value = defaultValue == null ? 0 : (long)defaultValue};
            }

            if (field == null){
                Debug.LogError("can not add field");
            }
            field?.RegisterCallback<ChangeEvent<T>>(x=> { action.Invoke(_value = x.newValue); });
            var button = new Button() {
                name="deleteConfigButton",
                text = "X",
                style= {
                    minWidth = 0,
                    minHeight = 0
                },
                clickable = new Clickable(() => { contentContainer.parent.Remove(contentContainer); })
            };
            contentContainer.Add(field);
            contentContainer.Add(button);
            styleSheets.Add(UxmlHandler.instance.gfuConfigFieldUss);
        }
    }
    public sealed class GfuConfigFieldUXml : VisualElement{
        private object _value;

        public GfuConfigFieldUXml(Action<object> action,FieldInfo fieldInfo=null,object defaultValue=null){
            _value = fieldInfo;
            VisualElement field = null;
            if (fieldInfo == null){
                Debug.LogError("can not add field");
                return;
            }

            name = fieldInfo.Name;
            if (fieldInfo.FieldType == typeof(bool) ||fieldInfo.FieldType == typeof(bool?)){
                var toggle = new Toggle() {
                    value = defaultValue != null && (bool) defaultValue,
                    label = fieldInfo.Name
                };
                toggle.RegisterValueChangedCallback(x=> { action.Invoke(_value = x.newValue); });
                field = toggle;
            }
            if (fieldInfo.FieldType == typeof(float)||fieldInfo.FieldType == typeof(float?)){
                var floatField =  new FloatField(){value = defaultValue == null ? 0 : (float)defaultValue,
                    label = fieldInfo.Name};
                floatField.RegisterValueChangedCallback(x=> { action.Invoke(_value = x.newValue); });
                field = floatField;
            }
            if (fieldInfo.FieldType == typeof(double) ||fieldInfo.FieldType == typeof(double?)){
                var doubleField = new DoubleField(){value = defaultValue == null ? 0 : (double)defaultValue,
                    label = fieldInfo.Name};
                doubleField.RegisterValueChangedCallback(x=> { action.Invoke(_value = x.newValue); });
                field = doubleField;
            }
            if (fieldInfo.FieldType == typeof(int) ||fieldInfo.FieldType == typeof(int?)){
                var integerField = new IntegerField(){value = defaultValue == null ? 0 : (int)defaultValue,
                    label = fieldInfo.Name};
                integerField.RegisterValueChangedCallback(x=> { action.Invoke(_value = x.newValue); });
                field = integerField;
            }
            if (fieldInfo.FieldType == typeof(long) ||fieldInfo.FieldType == typeof(long?)){
                var longField = new LongField(){value = defaultValue == null ? 0 : (long)defaultValue,
                    label = fieldInfo.Name};
                longField.RegisterValueChangedCallback(x=> { action.Invoke(_value = x.newValue); });
                field = longField;
            }
            
            var button = new Button() {
                name="deleteConfigButton",
                text = "X",
                style= {
                    minWidth = 0,
                    minHeight = 0
                },
                clickable = new Clickable(() => { contentContainer.parent.Remove(contentContainer); })
            };
            contentContainer.Add(field);
            contentContainer.Add(button);
            styleSheets.Add(UxmlHandler.instance.gfuConfigFieldUss);
        }
    }
}