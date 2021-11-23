//======================================================================
//
//       CopyRight 2019-2021 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  NodeFieldTypeAttribute.cs
//
//        Created by 半世癫(Roc)
//
//======================================================================

using System;
using GalForUnity.System;
using UnityEngine;

namespace GalForUnity.Graph.Attributes{
    /// <summary>
    /// 为Node字段提供命名功能和类型支持，记录着一个名称和类型，改类型会在后期由用户自行使用，如给ObjectField提供一个类型，如果不指定类型，默认类型为MonoBehaviour
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Field)]
    public class NodeFieldTypeAttribute : PropertyAttribute{
        //NodeFieldTypeAttribute
        public Type Type;
        public string Name;
        
        public NodeFieldTypeAttribute(Type type, string name){
            Type = type;
            Name = GfuLanguage.Parse(name);
        }

        public NodeFieldTypeAttribute(Type type) : this(type, type.Name){ }
        public NodeFieldTypeAttribute(string name) : this(typeof(MonoBehaviour), name){ }

        public NodeFieldTypeAttribute() : this(typeof(MonoBehaviour), "对象"){ }
    }
    
    [Flags]
    public enum NodeAttributeTargets{
        FlowGraph = 2,
        ItemGraph = 4
    }

    /// <summary>
    /// 允许Node向哪个图系统开放使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeAttributeUsage : PropertyAttribute{
        public NodeAttributeTargets nodeAttributeTargets;

        public NodeAttributeUsage(NodeAttributeTargets nodeAttributeTargets = NodeAttributeTargets.FlowGraph){
            this.nodeAttributeTargets = nodeAttributeTargets;
        }
    }
}