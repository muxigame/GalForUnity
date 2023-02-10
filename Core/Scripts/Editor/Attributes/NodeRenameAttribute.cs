//======================================================================
//
//       CopyRight 2019-2020 © MUXI Game Studio 
//       . All Rights Reserved 
//
//        FileName :  NodeRenameAttribute.cs
//
//        Created by 半世癫(Roc) at 2021-01-08 21:23:10
//
//======================================================================

using System;
using GalForUnity.Graph.Editor.Builder;
using UnityEngine;

namespace GalForUnity.Core.Editor.Attributes{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class NodeRenameAttribute : PropertyAttribute{
        
        public string name="";
        // public readonly LanguageItem languageItem;
        public readonly string tooltip;
        public Type Type;
        public NodeDirection PortType;
        public NodeCapacity Capacity;

        public NodeRenameAttribute(string name,string tooltip,Type type,NodeDirection portType = NodeDirection.Output,NodeCapacity capacity = NodeCapacity.Single){
            this.name = name;
            this.Type = type;
            this.PortType = portType;
            this.Capacity = capacity;
            this.tooltip = tooltip;
        }
        public NodeRenameAttribute(string name,Type type,NodeDirection portType = NodeDirection.Output,NodeCapacity capacity = NodeCapacity.Single){
//#if UNITY_EDITOR
            // if (new Regex("(^[a-zA-Z]+/?)+").IsMatch(name)){
            //     var strings = name.Split('/');
            //     foreach (var s in strings){
            //         this.name += (typeof(GfuLanguage).GetField(s.ToLower().Trim()).GetValue(GfuLanguage.GfuLanguageInstance) as LanguageItem)?.Value+"/";
            //     }
            //     this.name = this.name.Substring(0, this.name.Length - 1);
            // }
            // else{
                this.name = name;
            // }
            // Debug.Log(4);
            this.Type = type;
            this.PortType = portType;
            this.Capacity = capacity;
//#endif
        }

        public NodeRenameAttribute(string name,string tooltip):this(name,typeof(GalPort)){
#if UNITY_EDITOR
            this.tooltip = tooltip;
#endif
        }
        public NodeRenameAttribute(string name):this(name,typeof(GalPort)){ }

        public NodeRenameAttribute():this("None",typeof(GalPort)){ }

    }
    public enum NodeDirection{
        /// <summary>
        ///   <para>端口类型为输入</para>
        /// </summary>
        Input,
        /// <summary>
        ///   <para>端口类型为输出</para>
        /// </summary>
        Output,
    } 
    public enum NodeCapacity{
        /// <summary>
        ///   <para>只允许单个连接</para>
        /// </summary>
        Single,
        /// <summary>
        ///   <para>允许多个连接</para>
        /// </summary>
        Multi,
    }
}
