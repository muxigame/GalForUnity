

using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace GalForUnity.Graph.Editor{
    public class FBinder<TValue> : IBinding{
        private readonly VisualElement _visualElement;
        private Action _get;
        private Action _onUIPreUpdate;
        private Action<TValue> _set;

        public FBinder(VisualElement visualElement, Action<TValue> set, Action get, Action onUIPreUpdate){
            visualElement.RegisterCallback<ChangeEvent<TValue>>(Callback);
            _visualElement = visualElement;
            _get = get;
            _set = set;
            _onUIPreUpdate = onUIPreUpdate;
        }

        public void PreUpdate(){ _onUIPreUpdate?.Invoke(); }
        public void Update(){ _get?.Invoke(); }

        public void Release(){
            _visualElement.UnregisterCallback<ChangeEvent<TValue>>(Callback);
            _get = null;
            _set = null;
            _onUIPreUpdate = null;
        }

        private void Callback(ChangeEvent<TValue> evt){ _set?.Invoke(evt.newValue); }
    }

    public static class Expansion{
        public static IBinding CreateBinder<TValue>(this INotifyValueChanged<TValue> notifyValueChanged, FieldInfo fieldInfo, object instance,Func<TValue, TValue> filter = null,  Action onUIPreUpdate = null, Action<TValue> onValueChanged = null){
            if (notifyValueChanged is BindableElement bindableElement)
                return bindableElement.binding = new FBinder<TValue>(bindableElement,
                    value => {
                        if (filter != null) value = filter.Invoke(value);
                        fieldInfo.SetValue(instance, value);
                        onValueChanged?.Invoke(value);
                    },
                    () => { notifyValueChanged.value = (TValue) fieldInfo.GetValue(instance); },
                    onUIPreUpdate);
            return null;
        }
        public static IBinding CreateBinder<TValue>(this INotifyValueChanged<TValue> notifyValueChanged, Action onUIUpdate = null, Action<TValue> onValueChanged = null,Action onUIPreUpdate = null){
            if (notifyValueChanged is BindableElement bindableElement)
                return bindableElement.binding = new FBinder<TValue>(bindableElement,
                    value => {
                        onValueChanged?.Invoke(value);
                    },
                    () =>
                    {
                        onUIUpdate?.Invoke();
                    },
                    onUIPreUpdate);
            return null;
        }
        public static IBinding CreateBinder<TValue>(this INotifyValueChanged<TValue> notifyValueChanged, FieldInfo fieldInfo, Func<object> instance, Action onUIPreUpdate = null, Action onValueChanged = null){
            if (notifyValueChanged is BindableElement bindableElement)
                return bindableElement.binding = new FBinder<TValue>(bindableElement,
                    value => {
                        fieldInfo.SetValue(instance.Invoke(), value);
                        onValueChanged?.Invoke();
                    },
                    () => { notifyValueChanged.value = (TValue) fieldInfo.GetValue(instance.Invoke()); },
                    onUIPreUpdate);
            return null;
        }

        public static IBinding CreateBinder<TValue>(this INotifyValueChanged<TValue> notifyValueChanged,
            PropertyInfo propertyInfo, object instance, Func<TValue, TValue> filter = null, Action onUIPreUpdate = null, Action onValueChanged = null)
        {
            if (notifyValueChanged is BindableElement bindableElement)
                return bindableElement.binding = new FBinder<TValue>(bindableElement,
                    value =>
                    {
                        if (filter != null) value = filter.Invoke(value);
                        propertyInfo.SetValue(instance, value);
                        onValueChanged?.Invoke();
                    },
                    () => { notifyValueChanged.value = (TValue)propertyInfo.GetValue(instance); },
                    onUIPreUpdate);
            return null;
        }

        public static IBinding CreateBinder<TValue>(this BindableElement bindableElement, FieldInfo fieldInfo, object instance, Action onUIPreUpdate = null, Action onValueChanged = null){
            if (bindableElement is INotifyValueChanged<TValue> notifyValueChanged)
                return bindableElement.binding = new FBinder<TValue>(bindableElement,
                    value => {
                        fieldInfo.SetValue(instance, value);
                        onValueChanged?.Invoke();
                    },
                    () => { notifyValueChanged.value = (TValue) fieldInfo.GetValue(instance); },
                    onUIPreUpdate);
            return null;
        }
    }
}