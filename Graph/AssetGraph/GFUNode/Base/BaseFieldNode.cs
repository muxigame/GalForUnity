//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  BaseFieldNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-15 15:37:52
//
//======================================================================


using System;
using System.Collections.Generic;
using System.Reflection;
using GalForUnity.Attributes;
using GalForUnity.Graph.AssetGraph.Attributes;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

// #if UNITY_EDITOR
//
// #endif

namespace GalForUnity.Graph.AssetGraph.GFUNode.Base{
    [Serializable]
    public class BaseFieldNode : EnterNode{
        [NonSerialized] 
        private readonly Dictionary<string, NodeFieldTypeAttribute> _nodeFieldTypeAttributes;

        /// <summary>
        /// 将所有的NodeFieldTypeAttribute放入字典当中
        /// </summary>
        protected BaseFieldNode(){
            _nodeFieldTypeAttributes = new Dictionary<string, NodeFieldTypeAttribute>();
            foreach (var fieldInfo in GetType().GetFields()){
                if (IsSubClassOf(fieldInfo, typeof(VisualElement))){
                    _nodeFieldTypeAttributes.Add(fieldInfo.Name, fieldInfo.GetCustomAttribute<NodeFieldTypeAttribute>());
                }
            }
        }
        

        /// <summary>
        /// 通过反射来将字段添加到节点容器当中，需要注意的是，需要在func中初始化一个UIElement返回，并且如果没有将字段放置到全局，一定要注册回调，否则字段无法保存
        /// </summary>
        /// <param name="fieldName">字段名，可以是UIElement或者是普通字段</param>
        /// <param name="func">初始化方法</param>
        /// <typeparam name="T">初始化方法需要返回一个UIElement</typeparam>
        protected virtual T InitObject<T>(string fieldName, Func<NodeFieldTypeAttribute, T> func) where T : VisualElement{
#if UNITY_EDITOR
            NodeFieldTypeAttribute customAttribute = new NodeFieldTypeAttribute();
            T typElement;
            // NodeRenameAttribute nodeRename = new NodeRenameAttribute();
            var field = GetType().GetField(fieldName);
            if (_nodeFieldTypeAttributes.ContainsKey(fieldName)){ //如果NodeFieldType对象是一个VisualElement的话，反射该字段赋值，否在则是一个字段，去反射字段的属性获得名称值，并用用户的方法来初始化字段
                var fieldInfo = field;
                // customAttribute.Name = GfuLanguage.Parse(fieldInfo.GetCustomAttribute<NodeFieldTypeAttribute>().Name);
                customAttribute = _nodeFieldTypeAttributes[fieldName];
                typElement = func(customAttribute);
                fieldInfo.SetValue(this, typElement);
                extensionContainer.Add((VisualElement) fieldInfo.GetValue(this));
            } else{
                if (field.GetCustomAttribute<RenameAttribute>() != null){
                    customAttribute.Name = field.GetCustomAttribute<RenameAttribute>().LanguageItem.Value;
                    typElement = func(customAttribute);
                } else if (field.GetCustomAttribute<NodeFieldTypeAttribute>() != null){
                    customAttribute.Name = field.GetCustomAttribute<NodeFieldTypeAttribute>().Name;
                    typElement = func(customAttribute);
                } else{
                    typElement = null;
                }
                extensionContainer.Add(typElement);
            }

            return typElement;
#else
            return null;
#endif
        }

        protected virtual void RegisterValueChangedCallback<T, T2>(BaseFieldNode objectFieldNode) where T : BaseField<T2>{
            foreach (var objectField in objectFieldNode.GetFieldsName<T>()){
                objectField.Key.RegisterValueChangedCallback((changeEvent) => { OnValueChangedCallback(objectField.Key, changeEvent); });
            }
        }


        protected virtual void OnValueChangedCallback<T, T2>(T field, ChangeEvent<T2> changeEvent) where T : BaseField<T2>{ }


        /// <summary>
        /// 方法通过反射获得所有(Object和object字段)的UI字段，并且将值赋值给所对应的字段
        /// </summary>
        public override void Save(){
#if UNITY_EDITOR
            base.Save();
            var bindableElements = GetFields<BaseField<Object>>();
            var bindableElements2 = GetFields<BaseField<object>>();
            foreach (var bindableElement in bindableElements){
                var fields = this.GetType().GetFields();
                var type = bindableElement.value != null ? bindableElement.value.GetType() : null;
                foreach (var fieldInfo in fields){
                    if (fieldInfo.FieldType == type){
                        fieldInfo.SetValue(this, bindableElement.value);
                    }
                }
            }

            foreach (var bindableElement in bindableElements2){
                var fields = this.GetType().GetFields();
                var type = bindableElement.value != null ? bindableElement.value.GetType() : null;
                foreach (var fieldInfo in fields){
                    if (fieldInfo.FieldType == type){
                        fieldInfo.SetValue(this, bindableElement.value);
                    }
                }
            }
#endif
        }
    }
}