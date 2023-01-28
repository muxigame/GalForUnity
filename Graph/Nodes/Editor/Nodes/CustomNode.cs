using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GalForUnity.Graph.AssetGraph.GFUNode.Base;
using GalForUnity.Graph.Attributes;
using GalForUnity.Graph.Build;
using GalForUnity.Graph.SceneGraph;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GalForUnity.Graph.Nodes.Editor.Nodes{
    [NodeType(NodeCode.CustomNode)]
    public class CustomNode : GfuNode{
        public BindableElement Bind<T,T2>(FieldInfo fieldInfo,object[] param) where T2:BindableElement{
            T2 bindableElement = Activator.CreateInstance(typeof(T2), param) as T2;
            // bindableElement.CreateBinder<T>(fieldInfo, RuntimeNode);
            return bindableElement;
        }

        public Func<FieldInfo, object[], BindableElement> CreateBindableElement(FieldInfo fieldInfo){
            if (fieldInfo.FieldType == typeof(bool)) return Bind<bool, Toggle>;
            if (fieldInfo.FieldType == typeof(float)) return Bind<float, FloatField>;
            if (fieldInfo.FieldType == typeof(double)) return Bind<double, DoubleField>;
            if (fieldInfo.FieldType == typeof(int)) return Bind<int, IntegerField>;
            if (fieldInfo.FieldType == typeof(long)) return Bind<long, LongField>;
            if (fieldInfo.FieldType == typeof(Color)) return Bind<Color, ColorField>;
            if (fieldInfo.FieldType == typeof(string)) return Bind<string, TextField>;
            if (fieldInfo.FieldType == typeof(Vector2)) return Bind<Vector2, Vector2Field>;
            if (fieldInfo.FieldType == typeof(Vector3)) return Bind<Vector3, Vector3Field>;
            if (fieldInfo.FieldType == typeof(Vector4)) return Bind<Vector4, Vector4Field>;
            if (fieldInfo.FieldType == typeof(Object) || fieldInfo.FieldType.IsSubclassOf(typeof(Object)))
                return (x, y) => {
                    var objectField = Bind<Object, ObjectField>(x, y) as ObjectField;
                    if (objectField != null) objectField.objectType = fieldInfo.FieldType;
                    return objectField;
                };
            if (fieldInfo.FieldType == typeof(Enum)) return Bind<Enum, EnumField>;
            if (fieldInfo.FieldType == typeof(AnimationCurve)) return Bind<AnimationCurve, CurveField>;
            if (fieldInfo.FieldType == typeof(Bounds)) return Bind<Bounds, BoundsField>;
            if (fieldInfo.FieldType == typeof(Rect)) return Bind<Rect, RectField>;
            if (fieldInfo.FieldType == typeof(RectInt)) return Bind<RectInt, RectIntField>;
            if (fieldInfo.FieldType == typeof(Gradient)) return Bind<Gradient, GradientField>;
            return null;
        }
        public override List<GfuPort> Enter{ get; } = new List<GfuPort>{
            new GfuPort(Orientation.Horizontal, Direction.Input, Capacity.Multi, typeof(GfuNodeAsset), nameof(Enter))
        };

        public override List<GfuPort> Exit{ get; } = new List<GfuPort>{
            new GfuPort(Orientation.Horizontal, Direction.Output, Capacity.Single, typeof(GfuNodeAsset), nameof(Exit))
        };

        public override void OnInit(RuntimeNode otherRuntimeNode, GfuSceneGraphView graphView){
            base.OnInit(otherRuntimeNode, graphView);
            var type = RuntimeNode.GetType();
            var publicField = type.GetFields(BindingFlags.Instance |BindingFlags.Public);
            var nonPublicField = type.GetFields(BindingFlags.Instance |BindingFlags.NonPublic);
            foreach (var fieldInfo in publicField.Where(x=>(x.Attributes & FieldAttributes.NotSerialized) == 0).Union(nonPublicField.Where(x=>x.GetCustomAttribute<SerializableAttribute>() !=null))){
                BindableElement bindableElement = null;
                if (fieldInfo.GetCustomAttribute<RangeAttribute>() is { } rangeAttribute){
                    contentContainer.Add(bindableElement=new Slider(rangeAttribute.min,rangeAttribute.max));
                    bindableElement.CreateBinder<float>(fieldInfo, RuntimeNode);
                } else{
                    contentContainer.Add(bindableElement=CreateBindableElement(fieldInfo).Invoke(fieldInfo,null));
                }
                
                var type1 = bindableElement.GetType();
                type.GetProperty("label")?.SetValue(bindableElement,fieldInfo.Name);
            }

        }

  
        internal override IEnumerable<(GfuPort port, GfuPortAsset gfuPortAsset)> OnSavePort(GfuNodeAsset gfuNodeAsset){
            return base.OnSavePort(gfuNodeAsset);
        }

        internal override IEnumerable<(GfuPortAsset gfuPortAsset, GfuPort port)> OnLoadPort(GfuNodeAsset gfuNodeAsset){
            return base.OnLoadPort(gfuNodeAsset);
        }
    }
}