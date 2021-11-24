//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  ObjectFieldNode.cs
//
//        Created by 半世癫(Roc) at 2021-01-13 22:01:19
//
//======================================================================

using System.Reflection;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Data;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif
using UnityEngine;
using UnityEngine.UIElements;


namespace GalForUnity.Graph.GFUNode.Base{
    /// <summary>
    /// 引用对象的节点，通过InitObject初始化该对象，并且在数值改变的时候触发OnValueChangedCallback回调。在保存时，将所有字段为对象引用赋值
    /// </summary>
    /// <typeparam name="T">对顶对象可以接受的值</typeparam>
    public class ObjectFieldNode<T> : EnterNode where T : Object{
        public T objectReference;

#if UNITY_EDITOR
        public ObjectField ObjectField;

        public override void Init(NodeData otherNodeData){
            base.Init(otherNodeData);
            InitObject(objectReference);
        }

        protected virtual void InitObject(T obj){
            obj = obj ? obj : null;
            NodeFieldTypeAttribute customAttribute = (NodeFieldTypeAttribute) GetType().GetCustomAttribute(typeof(NodeFieldTypeAttribute));
            if (customAttribute == null) customAttribute = new NodeFieldTypeAttribute();
            ObjectField = new ObjectField() {
                value = obj,
                objectType = typeof(T),
                label = customAttribute.Name,
                style = {
                    minWidth = 0,
                },
                labelElement = {
                    style = {
                        // flexBasis = 0, minWidth = customAttribute.Name.Length * 13, width = customAttribute.Name.Length * 13, fontSize = 12,unityTextAlign = TextAnchor.MiddleLeft
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft,
                    }
                }
            };
            ObjectField.RegisterValueChangedCallback((evt) => { objectReference = (T) evt.newValue; });
            mainContainer.Add(ObjectField);
        }

        protected virtual void InitObject<T2>(out ObjectField objectField, T2 obj) where T2 : Object{
            obj = obj ? obj : default;
            NodeFieldTypeAttribute customAttribute = (NodeFieldTypeAttribute) GetType().GetCustomAttribute(typeof(NodeFieldTypeAttribute));
            if (customAttribute == null) customAttribute = new NodeFieldTypeAttribute();
            objectField = new ObjectField() {
                value = obj,
                objectType = typeof(T2),
                label = customAttribute.Name,
                style = {
                    minWidth = 0,
                },
                labelElement = {
                    style = {
                        minWidth = 0, unityTextAlign = TextAnchor.MiddleLeft,
                    }
                }
            };
            objectField.RegisterValueChangedCallback((evt) => { obj = (T2) evt.newValue; });
            mainContainer.Add(objectField);
        }

        protected virtual void RegisterValueChangedCallback(ObjectFieldNode<T> objectFieldNode){
            foreach (var objectField in objectFieldNode.GetFieldsName<ObjectField>()){
                objectField.Key.RegisterValueChangedCallback((changeEvent) => { OnValueChangedCallback(objectField.Key, changeEvent); });
            }
        }


        protected virtual void OnValueChangedCallback(ObjectField objectField, ChangeEvent<Object> changeEvent){ }


        public override void Save(){
            var bindableElements = GetFields<ObjectField>();
            foreach (var bindableElement in bindableElements){
                var fields = this.GetType().GetFields();
                var type = bindableElement?.value != null ? bindableElement.value.GetType() : null;
                foreach (var fieldInfo in fields){
                    if (fieldInfo.FieldType == type){
                        fieldInfo.SetValue(this, bindableElement.value);
                    }
                }
            }
        }
#endif
    }
}