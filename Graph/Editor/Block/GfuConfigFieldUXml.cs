

using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor.Block{
    public sealed class GfuConfigFieldUXml<T> : VisualElement{
        private readonly FieldInfo _fieldInfo;
        private readonly object _instance;

        public GfuConfigFieldUXml(FieldInfo fieldInfo = null, object instance = null,Action onDelete = null){
            _fieldInfo = fieldInfo;
            _instance = instance;
            name = fieldInfo?.Name;
            BindableElement field = null;
            if (typeof(T) == typeof(bool))
                field = new Toggle {
                    label = fieldInfo?.Name
                };
            if (typeof(T) == typeof(float))
                field = new FloatField {
                    label = fieldInfo?.Name
                };
            if (typeof(T) == typeof(double))
                field = new DoubleField {
                    label = fieldInfo?.Name
                };
            if (typeof(T) == typeof(int))
                field = new IntegerField {
                    label = fieldInfo?.Name
                };
            if (typeof(T) == typeof(long))
                field = new LongField {
                    label = fieldInfo?.Name
                };

            if (field == null) Debug.LogError("can not add field");
            field?.CreateBinder<T>(fieldInfo, instance);
            var button = new Button {
                name = "deleteConfigButton",
                text = "X",
                style = {
                    minWidth = 0, minHeight = 0
                },
                clickable = new Clickable(() => {
                    contentContainer.parent.Remove(contentContainer);
                    onDelete?.Invoke();
                })
            };
            contentContainer.Add(field);
            contentContainer.Add(button);
            styleSheets.Add(UxmlHandler.instance.gfuConfigFieldUss);
        }

        public T Value => (T) _fieldInfo?.GetValue(_instance);
    }

    public sealed class GfuConfigFieldUXml : VisualElement{
        private object _value;

        public GfuConfigFieldUXml(FieldInfo fieldInfo = null, object instance = null,Action onDelete = null){
            _value = fieldInfo;
            VisualElement field = null;
            if (fieldInfo == null){
                Debug.LogError("can not add field");
                return;
            } 

            name = fieldInfo.Name;
            if (fieldInfo.FieldType == typeof(bool) || fieldInfo.FieldType == typeof(bool?)){
                var toggle = new Toggle {
                    label = fieldInfo.Name
                };
                toggle.CreateBinder(fieldInfo, instance);
                field = toggle;
            }

            if (fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(float?)){
                var floatField = new FloatField {
                    label = fieldInfo.Name
                };
                floatField.CreateBinder(fieldInfo, instance);
                field = floatField;
            }

            if (fieldInfo.FieldType == typeof(double) || fieldInfo.FieldType == typeof(double?)){
                var doubleField = new DoubleField {
                    label = fieldInfo.Name
                };
                doubleField.CreateBinder(fieldInfo, instance);
                field = doubleField;
            }

            if (fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(int?)){
                var integerField = new IntegerField {
                    label = fieldInfo.Name
                };
                integerField.CreateBinder(fieldInfo, instance);
                field = integerField;
            }

            if (fieldInfo.FieldType == typeof(long) || fieldInfo.FieldType == typeof(long?)){
                var longField = new LongField {
                    label = fieldInfo.Name
                };
                longField.CreateBinder(fieldInfo, instance);
                field = longField;
            }

            var underlyingType = Nullable.GetUnderlyingType(fieldInfo.FieldType);
            if (underlyingType != null && underlyingType.IsEnum){
                var enumField = new EnumField {
                    label = fieldInfo.Name
                };
                enumField.CreateBinder(fieldInfo, instance);
                field = enumField;
            }

            var button = new Button {
                name = "deleteConfigButton",
                text = "X",
                style = {
                    minWidth = 0, minHeight = 0
                },
                clickable = new Clickable(() => {
                    contentContainer.parent.Remove(contentContainer);
                    onDelete?.Invoke();
                })
            };
            contentContainer.Add(field);
            contentContainer.Add(button);
            styleSheets.Add(UxmlHandler.instance.gfuConfigFieldUss);
        }
    }
}